using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System.Text;

namespace Attribute_Code_Generation
{
    class Program
    {
        static void Main(string[] args)
        {
            var workspace = MSBuildWorkspace.Create();
            var project = workspace.OpenProjectAsync(@"..\..\..\Jurassic\Jurassic.csproj").Result;

            foreach (var document in project.Documents)
            {
                var root = document.GetSyntaxRootAsync().Result;

                // Construct the output file.
                var output = new StringBuilder();
                output.AppendLine("/*");
                output.AppendLine(" * This file is auto-generated, do not modify directly.");
                output.AppendLine(" */");
                output.AppendLine();
                output.AppendLine("using System.Collections.Generic;");
                output.AppendLine("using Jurassic;");
                output.AppendLine();
                output.AppendLine("namespace Jurassic.Library");
                output.AppendLine("{");
                output.AppendLine();

                bool outputFile = false;
                var classCollector = new ClassCollector();
                classCollector.Visit(root);
                foreach (var classSyntax in classCollector.Classes)
                {
                    // Find all the methods with [JSInternalFunction], [JSCallFunction], [JSConstructorFunction], [JSProperty] or [JSField].
                    var memberCollector = new ClassMembersCollector();
                    memberCollector.Visit(classSyntax);
                    if (memberCollector.JSInternalFunctionMethods.Any() == false &&
                        memberCollector.JSCallFunctionMethods.Any() == false &&
                        memberCollector.JSConstructorFunctionMethods.Any() == false &&
                        memberCollector.JSProperties.Any() == false &&
                        memberCollector.JSFields.Any() == false)
                        continue;

                    Console.WriteLine($"Generating stubs for {classSyntax.Identifier.ToString()}");

                    outputFile = true;
                    var methodGroups = JSMethodGroup.FromMethods(memberCollector.JSInternalFunctionMethods);

                    output.AppendLine($"\t{classSyntax.Modifiers} class {classSyntax.Identifier}");
                    output.AppendLine("\t{");

                    // Output the PopulateStubs method.
                    if (memberCollector.JSInternalFunctionMethods.Any() ||
                        memberCollector.JSProperties.Any() ||
                        memberCollector.JSFields.Any())
                    {
                        output.AppendLine("\t\tprivate List<PropertyNameAndValue> GetDeclarativeProperties()");
                        output.AppendLine("\t\t{");
                        output.AppendLine($"\t\t\treturn new List<PropertyNameAndValue>({memberCollector.JSInternalFunctionMethods.Count + memberCollector.JSFields.Count + 4})");
                        output.AppendLine("\t\t\t{");

                        foreach (var field in memberCollector.JSFields)
                        {
                            foreach (var variable in field.Declaration.Variables)
                            {
                                output.AppendLine($"\t\t\t\tnew PropertyNameAndValue(\"{variable.Identifier.ToString()}\", {variable.Identifier.ToString()}, PropertyAttributes.Sealed),");
                            }
                        }
                        foreach (var property in memberCollector.JSProperties.Select(p => new JSProperty(p)))
                        {
                            if (property.SetterStubName == null)
                            {
                                output.AppendLine($"\t\t\t\tnew PropertyNameAndValue(\"{property.JSName}\", new PropertyDescriptor(" +
                                    $"new ClrStubFunction(Engine.FunctionInstancePrototype, \"get {property.JSName}\", 0, {property.GetterStubName}), " +
                                    $"null, {property.JSPropertyAttributes})),");
                            }
                            else
                            {
                                output.AppendLine($"\t\t\t\tnew PropertyNameAndValue(\"{property.JSName}\", new PropertyDescriptor(" +
                                    $"new ClrStubFunction(Engine.FunctionInstancePrototype, \"get {property.JSName}\", 0, {property.GetterStubName}), " +
                                    $"new ClrStubFunction(Engine.FunctionInstancePrototype, \"set {property.JSName}\", 0, {property.GetterStubName}), " +
                                    $"{property.JSPropertyAttributes})),");
                            }
                        }
                        foreach (var methodGroup in methodGroups)
                        {
                            output.AppendLine($"\t\t\t\tnew PropertyNameAndValue(\"{methodGroup.JSName}\", " +
                                $"new ClrStubFunction(Engine.FunctionInstancePrototype, \"{methodGroup.JSName}\", " +
                                $"{methodGroup.JSLength}, {methodGroup.StubName}), {methodGroup.JSPropertyAttributes}),");
                        }
                        output.AppendLine("\t\t\t};");
                        output.AppendLine("\t\t}");
                    }

                    if (memberCollector.JSCallFunctionMethods.Any())
                        GenerateMethodStub(output, classSyntax, new JSMethodGroup(memberCollector.JSCallFunctionMethods.Select(mds => new JSMethod(mds))));
                    if (memberCollector.JSConstructorFunctionMethods.Any())
                        GenerateMethodStub(output, classSyntax, new JSMethodGroup(memberCollector.JSConstructorFunctionMethods.Select(mds => new JSMethod(mds))), "ObjectInstance");
                    foreach (var property in memberCollector.JSProperties.Select(p => new JSProperty(p)))
                    {
                        output.AppendLine();
                        output.AppendLine($"\t\tobject {property.GetterStubName}(ScriptEngine engine, object thisObj, object[] args)");
                        output.AppendLine("\t\t{");
                        output.AppendLine($"\t\t\tthisObj = TypeConverter.ToObject(engine, thisObj);");
                        output.AppendLine($"\t\t\tif (!(thisObj is {classSyntax.Identifier.ToString()}))");
                        output.AppendLine($"\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"The method 'get {property.JSName}' is not generic.\");");
                        output.AppendLine($"\t\t\treturn (({classSyntax.Identifier.ToString()})thisObj).{property.Name};");
                        output.AppendLine("\t\t}");

                        if (property.SetterStubName != null)
                        {
                            output.AppendLine();
                            output.AppendLine($"\t\tobject {property.SetterStubName}(ScriptEngine engine, object thisObj, object[] args)");
                            output.AppendLine("\t\t{");
                            output.AppendLine($"\t\t\tthisObj = TypeConverter.ToObject(engine, thisObj);");
                            output.AppendLine($"\t\t\tif (!(thisObj is {classSyntax.Identifier.ToString()}))");
                            output.AppendLine($"\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"The method 'set {property.JSName}' is not generic.\");");
                            output.AppendLine($"\t\t\t(({classSyntax.Identifier.ToString()})thisObj).{property.Name} = {ConvertTo("args.Length > 0 ? args[0] : Undefined.Value", property.ReturnType, null)};");
                            output.AppendLine("\t\t}");
                        }
                    }
                    foreach (var methodGroup in methodGroups)
                    {
                        GenerateMethodStub(output, classSyntax, methodGroup);
                    }

                    output.AppendLine("\t}");
                }

                output.AppendLine();
                output.AppendLine("}");

                if (outputFile)
                {
                    // Write the output file.
                    File.WriteAllText(Path.Combine(Path.GetDirectoryName(document.FilePath), Path.GetFileNameWithoutExtension(document.FilePath) + ".g.cs"), output.ToString());
                }
            }
        }

        class ClassCollector : CSharpSyntaxWalker
        {
            public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

            public override void VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                Classes.Add(node);
            }
        }

        class ClassMembersCollector : CSharpSyntaxWalker
        {
            public List<MethodDeclarationSyntax> JSInternalFunctionMethods { get; } = new List<MethodDeclarationSyntax>();
            public List<MethodDeclarationSyntax> JSCallFunctionMethods { get; } = new List<MethodDeclarationSyntax>();
            public List<MethodDeclarationSyntax> JSConstructorFunctionMethods { get; } = new List<MethodDeclarationSyntax>();
            public List<PropertyDeclarationSyntax> JSProperties { get; } = new List<PropertyDeclarationSyntax>();
            public List<FieldDeclarationSyntax> JSFields { get; } = new List<FieldDeclarationSyntax>();

            public override void VisitAttribute(AttributeSyntax node)
            {
                var name = node.Name.ToString();
                if (name == "JSInternalFunction")
                {
                    JSInternalFunctionMethods.Add((MethodDeclarationSyntax)node.Parent.Parent);
                }
                else if (name == "JSCallFunction")
                {
                    JSCallFunctionMethods.Add((MethodDeclarationSyntax)node.Parent.Parent);
                    if (node.ArgumentList != null)
                        throw new InvalidOperationException("Arguments are not supported for [JSCallFunction].");
                }
                else if (name == "JSConstructorFunction")
                {
                    JSConstructorFunctionMethods.Add((MethodDeclarationSyntax)node.Parent.Parent);
                    if (node.ArgumentList != null)
                        throw new InvalidOperationException("Arguments are not supported for [JSConstructorFunction].");
                }
                else if (name == "JSProperty")
                {
                    JSProperties.Add((PropertyDeclarationSyntax)node.Parent.Parent);
                }
                else if (name == "JSField")
                {
                    JSFields.Add((FieldDeclarationSyntax)node.Parent.Parent);
                }
            }
        }



        public class JSMethodGroup : List<JSMethod>
        {
            public static IEnumerable<JSMethodGroup> FromMethods(IEnumerable<MethodDeclarationSyntax> methods)
            {
                return methods.Select(m => new JSMethod(m)).GroupBy(ms => ms.JSName).Select(group => new JSMethodGroup(group));
            }

            public JSMethodGroup(IEnumerable<JSMethod> group)
                : base(group)
            {
                JSName = this.First().JSName;
                if (!this.All(m => m.JSName == JSName))
                    throw new InvalidOperationException("All methods must have the same name.");
                JSLength = this.First().JSLength;
                if (!this.All(m => m.JSLength == JSLength))
                    throw new InvalidOperationException($"All {JSName} methods must have the same length.");
                if (JSLength == -1)
                    JSLength = this.Max(m => m.Parameters.Count());
                JSPropertyAttributes = this.First().JSPropertyAttributes;
                if (!this.All(m => m.JSPropertyAttributes == JSPropertyAttributes))
                    throw new InvalidOperationException($"All {JSName} methods must have the same PropertyAttributes.");
                StubName = "__STUB__" + JSName;
                IsStatic = this.First().IsStatic;
                if (!this.All(m => m.IsStatic == IsStatic))
                    throw new InvalidOperationException($"All {JSName} methods must all be static or instance methods.");
                RequiredArgumentCount = this.First().RequiredArgumentCount;
                if (!this.All(m => m.RequiredArgumentCount == RequiredArgumentCount))
                    throw new InvalidOperationException($"All {JSName} methods must all have the same RequiredArgumentCount.");
            }

            public string JSName { get; private set; }
            public int JSLength { get; private set; }
            public string JSPropertyAttributes { get; private set; }
            public string StubName { get; private set; }
            public bool IsStatic { get; private set; }
            public int RequiredArgumentCount { get; private set; }
        }

        public class JSMethod
        {
            public JSMethod(MethodDeclarationSyntax methodSyntax)
            {
                Name = methodSyntax.Identifier.ToString();
                ReturnType = methodSyntax.ReturnType.ToString();
                IsStatic = methodSyntax.Modifiers.Any(m => m.ToString() == "static");

                int parameterSkipCount = 0;
                var jsAttribute = methodSyntax.AttributeLists.SelectMany(al => al.Attributes).Single(att =>
                    att.Name.ToString() == "JSInternalFunction" ||
                    att.Name.ToString() == "JSCallFunction" ||
                    att.Name.ToString() == "JSConstructorFunction");

                var flagsParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "Flags");
                if (flagsParameter != null)
                {
                    HasEngineParameter = flagsParameter.Expression.ToFullString().Contains("JSFunctionFlags.HasEngineParameter");
                    if (HasEngineParameter)
                    {
                        if (methodSyntax.ParameterList.Parameters.Count == 0)
                            throw new InvalidOperationException("Method has HasEngineParameter but has no parameters.");
                        if (methodSyntax.ParameterList.Parameters[0].Type.ToString() != "ScriptEngine")
                            throw new InvalidOperationException("Method has HasEngineParameter but the type of the first parameter is incorrect.");
                        parameterSkipCount++;
                    }

                    HasThisObject = flagsParameter.Expression.ToFullString().Contains("JSFunctionFlags.HasThisObject");
                    if (HasThisObject)
                    {
                        int thisParamIndex = HasEngineParameter ? 1 : 0;
                        if (methodSyntax.ParameterList.Parameters.Count <= thisParamIndex)
                            throw new InvalidOperationException("Method has HasThisObject but has too few parameters.");
                        ThisObjectParameterType = methodSyntax.ParameterList.Parameters[thisParamIndex].Type.ToString();
                        parameterSkipCount++;
                    }
                }

                // Determine the PropertyAttributes.
                bool isWritable = GetBooleanAttributeFlag(jsAttribute, "IsWritable", true);
                bool isEnumerable = GetBooleanAttributeFlag(jsAttribute, "IsEnumerable", false);
                bool isConfigurable = GetBooleanAttributeFlag(jsAttribute, "IsConfigurable", true);
                if (!isWritable && !isEnumerable && !isConfigurable)
                    JSPropertyAttributes = "PropertyAttributes.Sealed";
                else if (isWritable && !isEnumerable && isConfigurable)
                    JSPropertyAttributes = "PropertyAttributes.NonEnumerable";
                else
                {
                    var propertyAttributes = new List<string>();
                    if (isWritable)
                        propertyAttributes.Add("PropertyAttributes.Writable");
                    if (isEnumerable)
                        propertyAttributes.Add("PropertyAttributes.Enumerable");
                    if (isConfigurable)
                        propertyAttributes.Add("PropertyAttributes.Configurable");
                    JSPropertyAttributes = string.Join(" | ", propertyAttributes);
                }

                Parameters = methodSyntax.ParameterList.Parameters.Skip(parameterSkipCount).Select(p => new MethodParameter(p));

                // Determine the name of the javascript function.
                JSName = Name;
                var nameParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "Name");
                if (nameParameter != null)
                    JSName = ((LiteralExpressionSyntax)nameParameter.Expression).Token.ValueText;

                // Determine the "length" property of the javascript function.
                JSLength = -1;
                var lengthParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "Length");
                if (lengthParameter != null)
                {
                    JSLength = int.Parse(lengthParameter.Expression.ToString());
                }

                // Determine the required argument count.
                var requiredArgumentCountParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "RequiredArgumentCount");
                if (requiredArgumentCountParameter != null)
                    RequiredArgumentCount = (int)((LiteralExpressionSyntax)requiredArgumentCountParameter.Expression).Token.Value;
                if (RequiredArgumentCount > Parameters.Count())
                    throw new InvalidOperationException("RequiredArgumentCount must be less than or equal to the number of parameters.");
            }

            private bool GetBooleanAttributeFlag(AttributeSyntax jsAttribute, string name, bool defaultValue)
            {
                var boolParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == name);
                if (boolParameter == null)
                    return defaultValue;
                return (bool)((LiteralExpressionSyntax)boolParameter.Expression).Token.Value;
            }

            public bool HasEngineParameter { get; private set; }
            public bool HasThisObject { get; private set; }
            public string ThisObjectParameterType { get; private set; }
            public bool IsStatic { get; private set; }
            public int RequiredArgumentCount { get; private set; }
            public string Name { get; private set; }
            public string JSName { get; private set; }
            public int JSLength { get; private set; }
            public string JSPropertyAttributes { get; private set; }
            public string ReturnType { get; private set; }
            public IEnumerable<MethodParameter> Parameters { get; private set; }
        }

        public class MethodParameter
        {
            public MethodParameter(ParameterSyntax param)
            {
                Name = param.Identifier.ToString();
                Type = param.Type.ToString();
                DefaultValue = param.Default?.Value?.ToString();
            }

            public string Name { get; private set; }
            public string Type { get; private set; }
            public string DefaultValue { get; private set; }
        }

        public class JSProperty
        {
            public JSProperty(PropertyDeclarationSyntax propertySyntax)
            {
                Name = propertySyntax.Identifier.ToString();
                ReturnType = propertySyntax.Type.ToString();
                if (propertySyntax.Modifiers.Any(m => m.ToString() == "static"))
                    throw new NotImplementedException("Static properties are not supported.");

                var jsAttribute = propertySyntax.AttributeLists.SelectMany(al => al.Attributes).Single(att =>
                    att.Name.ToString() == "JSProperty");

                // Determine the PropertyAttributes.
                bool isEnumerable = GetBooleanAttributeFlag(jsAttribute, "IsEnumerable", false);
                bool isConfigurable = GetBooleanAttributeFlag(jsAttribute, "IsConfigurable", true);
                if (!isEnumerable && !isConfigurable)
                    JSPropertyAttributes = "PropertyAttributes.Sealed";
                else
                {
                    var propertyAttributes = new List<string>();
                    if (isEnumerable)
                        propertyAttributes.Add("PropertyAttributes.Enumerable");
                    if (isConfigurable)
                        propertyAttributes.Add("PropertyAttributes.Configurable");
                    JSPropertyAttributes = string.Join(" | ", propertyAttributes);
                }

                // Determine the name of the javascript function.
                JSName = Name;
                var nameParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "Name");
                if (nameParameter != null)
                    JSName = ((LiteralExpressionSyntax)nameParameter.Expression).Token.ValueText;

                // Determine the stub names.
                GetterStubName = "__GETTER__" + JSName;
                if (propertySyntax.AccessorList.Accessors.Any(a => a.Keyword.ToString() == "set"))
                    SetterStubName = "__SETTER__" + JSName;
            }

            private bool GetBooleanAttributeFlag(AttributeSyntax jsAttribute, string name, bool defaultValue)
            {
                var boolParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == name);
                if (boolParameter == null)
                    return defaultValue;
                return (bool)((LiteralExpressionSyntax)boolParameter.Expression).Token.Value;
            }

            public string Name { get; private set; }
            public string JSName { get; private set; }
            public string ReturnType { get; private set; }
            public string JSPropertyAttributes { get; private set; }
            public string GetterStubName { get; private set; }
            public string SetterStubName { get; private set; }
        }

        private static void GenerateMethodStub(StringBuilder output, ClassDeclarationSyntax classSyntax, JSMethodGroup methodGroup, string returnType = "object")
        {
            output.AppendLine();
            output.AppendLine($"\t\tprivate static {returnType} {methodGroup.StubName}(ScriptEngine engine, object thisObj, object[] args)");
            output.AppendLine("\t\t{");

            if (!methodGroup.IsStatic)
            {
                output.AppendLine($"\t\t\tthisObj = TypeConverter.ToObject(engine, thisObj);");
                output.AppendLine($"\t\t\tif (!(thisObj is {classSyntax.Identifier.ToString()}))");
                output.AppendLine($"\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"The method '{methodGroup.First().JSName}' is not generic.\");");
            }
            else if (methodGroup.Any(m => m.HasThisObject && m.ThisObjectParameterType != "object"))
            {
                output.AppendLine("\t\t\tif (thisObj == null || thisObj == Undefined.Value || thisObj == Null.Value)");
                output.AppendLine("\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"Cannot convert undefined or null to object.\");");
            }

            int maxParameterCount = methodGroup.Max(mds => mds.Parameters.Count());
            if (maxParameterCount == 0)
            {
                output.Append("\t\t\t");
                output.AppendLine(GenerateMethodCall(classSyntax, methodGroup.Single(), 0));
            }
            else
            {
                output.AppendLine("\t\t\tswitch (args.Length)");
                output.AppendLine("\t\t\t{");
                for (int i = 0; i <= maxParameterCount; i++)
                {
                    var minDifference = methodGroup.Min(mds => Math.Abs(i - mds.Parameters.Count()));
                    var method = methodGroup.Where(mds => Math.Abs(i - mds.Parameters.Count()) == minDifference).Single();
                    if (i < maxParameterCount)
                        output.AppendLine($"\t\t\t\tcase {i}:");
                    else
                        output.AppendLine($"\t\t\t\tdefault:");
                    output.Append("\t\t\t\t\t");
                    if (i < methodGroup.RequiredArgumentCount)
                        output.AppendLine($"throw new JavaScriptException(engine, ErrorType.TypeError, \"Required argument '{method.Parameters.Skip(i).First().Name}' was not specified.\");");
                    else
                        output.AppendLine(GenerateMethodCall(classSyntax, method, i));
                }
                output.AppendLine("\t\t\t}");
            }

            output.AppendLine("\t\t}");
        }

        private static string GenerateMethodCall(ClassDeclarationSyntax classSyntax, JSMethod method, int argsLength)
        {
            var result = new StringBuilder();
            if (method.ReturnType != "void")
                result.Append("return ");
            if (!method.IsStatic)
                result.Append($"(({classSyntax.Identifier.ToString()})thisObj).");
            result.Append(method.Name);
            result.Append('(');

            int skipCount = 0;
            bool appendComma = false;
            if (method.HasEngineParameter)
            {
                appendComma = true;
                result.Append("engine");
                skipCount++;
            }
            if (method.HasThisObject)
            {
                if (appendComma)
                    result.Append(", ");
                appendComma = true;

                result.Append(ConvertTo($"thisObj", method.ThisObjectParameterType, null));
                skipCount++;
            }

            int argIndex = 0;
            foreach (var parameter in method.Parameters)
            {
                if (appendComma)
                    result.Append(", ");
                appendComma = true;

                if (argIndex < argsLength)
                {
                    result.Append(ConvertTo($"args[{argIndex}]", parameter.Type, parameter.DefaultValue, argIndex));
                }
                else
                {
                    // Pass through undefined.
                    if (parameter.DefaultValue != null)
                    {
                        result.Append(parameter.DefaultValue);
                    }
                    else
                    {
                        switch (parameter.Type)
                        {
                            case "object":
                                result.Append("Undefined.Value");
                                break;
                            case "bool":
                                result.Append("false");
                                break;
                            case "int":
                                result.Append("0");
                                break;
                            case "string":
                                result.Append("\"undefined\"");
                                break;
                            case "double":
                                result.Append($"double.NaN");
                                break;
                            case "ObjectInstance":
                            case "FunctionInstance":
                            case "ArrayBufferInstance":
                                return "throw new JavaScriptException(engine, ErrorType.TypeError, \"undefined cannot be converted to an object\");";

                            case "object[]":
                                result.Append("new object[0]");
                                break;
                            case "string[]":
                                result.Append("new string[0]");
                                break;
                            case "double[]":
                                result.Append("new double[0]");
                                break;

                            default:
                                throw new InvalidOperationException($"Unsupported parameter type {parameter.Type}");
                        }
                    }
                }
                argIndex++;
            }
            result.Append(");");

            if (method.ReturnType == "void")
                result.Append(" return Undefined.Value;");
            return result.ToString();
        }

        private static string ConvertTo(string arg, string type, string defaultValue, int arrayIndex = -1)
        {
            if (defaultValue != null)
            {
                if (defaultValue == "null" && type.EndsWith("?"))
                    return $"TypeUtilities.IsUndefined({arg}) ? ({type})null : {ConvertTo(arg, type, null)}";
                return $"TypeUtilities.IsUndefined({arg}) ? {defaultValue} : {ConvertTo(arg, type, null)}";
            }

            if (type.EndsWith("?"))
                type = type.Substring(0, type.Length - 1);
            switch (type)
            {
                case "object":
                    return arg;
                case "bool":
                    return $"TypeConverter.ToBoolean({arg})";
                case "int":
                    return $"TypeConverter.ToInteger({arg})";
                case "string":
                    return $"TypeConverter.ToString({arg})";
                case "double":
                    return $"TypeConverter.ToNumber({arg})";
                case "ObjectInstance":
                    return $"TypeConverter.ToObject(engine, {arg})";
                case "FunctionInstance":
                    return $"TypeConverter.ToFunction(engine, {arg})";
                case "ArrayBufferInstance":
                    return $"TypeConverter.ToObject(engine, {arg}) as ArrayBufferInstance";
                case "object[]":
                    if (arrayIndex < 0)
                        throw new InvalidOperationException("Cannot convert to array.");
                    if (arrayIndex == 0)
                        return "args";
                    return $"TypeUtilities.SliceArray(args, {arrayIndex})";
                case "string[]":
                    if (arrayIndex < 0)
                        throw new InvalidOperationException("Cannot convert to array.");
                    return $"TypeConverter.ConvertParameterArrayTo<string>(engine, args, {arrayIndex})";
                case "double[]":
                    if (arrayIndex < 0)
                        throw new InvalidOperationException("Cannot convert to array.");
                    return $"TypeConverter.ConvertParameterArrayTo<double>(engine, args, {arrayIndex})";
                default:
                    throw new InvalidOperationException($"Unsupported parameter type {type}");
            }
        }
    }
}
