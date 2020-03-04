using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Attribute_Code_Generation
{
    class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(@"..\..\..\..\Jurassic", "*.cs", SearchOption.AllDirectories);
            files = files.Union(Directory.EnumerateFiles(@"..\..\..\..\Jurassic.Extensions", "*.cs", SearchOption.AllDirectories));
            foreach (var csFilePath in files)
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(File.ReadAllText(csFilePath));
                var classCollector = new ClassCollector();
                classCollector.Visit(syntaxTree.GetRoot());

                // Construct the output file.
                var output = new StringBuilder();
                output.AppendLine("/*");
                output.AppendLine(" * This file is auto-generated, do not modify directly.");
                output.AppendLine(" */");
                output.AppendLine();
                output.AppendLine("using System.Collections.Generic;");
                output.AppendLine("using Jurassic;");
                if (classCollector.Classes.Any(classSyntax => ((NamespaceDeclarationSyntax)classSyntax.Parent).Name.ToString() != "Jurassic.Library"))
                    output.AppendLine("using Jurassic.Library;");
                output.AppendLine();

                bool outputFile = false;
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

                    output.AppendLine($"namespace {((NamespaceDeclarationSyntax)classSyntax.Parent).Name}");
                    output.AppendLine("{");
                    output.AppendLine();

                    output.AppendLine($"\t{classSyntax.Modifiers} class {classSyntax.Identifier}");
                    output.AppendLine("\t{");

                    // Output the PopulateStubs method.
                    if (memberCollector.JSInternalFunctionMethods.Any() ||
                        memberCollector.JSProperties.Any() ||
                        memberCollector.JSFields.Any())
                    {
                        output.AppendLine("\t\tprivate static List<PropertyNameAndValue> GetDeclarativeProperties(ScriptEngine engine)");
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
                                output.AppendLine($"\t\t\t\tnew PropertyNameAndValue({property.PropertyKey}, new PropertyDescriptor(" +
                                    $"new ClrStubFunction(engine, \"get {property.FunctionName}\", 0, {property.GetterStubName}), " +
                                    $"null, {property.JSPropertyAttributes})),");
                            }
                            else
                            {
                                output.AppendLine($"\t\t\t\tnew PropertyNameAndValue({property.PropertyKey}, new PropertyDescriptor(" +
                                    $"new ClrStubFunction(engine, \"get {property.FunctionName}\", 0, {property.GetterStubName}), " +
                                    $"new ClrStubFunction(engine, \"set {property.FunctionName}\", 0, {property.GetterStubName}), " +
                                    $"{property.JSPropertyAttributes})),");
                            }
                        }
                        foreach (var methodGroup in methodGroups)
                        {
                            output.AppendLine($"\t\t\t\tnew PropertyNameAndValue({methodGroup.PropertyKey}, " +
                                    $"new ClrStubFunction(engine, \"{methodGroup.FunctionName}\", " +
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
                        output.AppendLine($"\t\tprivate static object {property.GetterStubName}(ScriptEngine engine, object thisObj, object[] args)");
                        output.AppendLine("\t\t{");
                        output.AppendLine($"\t\t\tthisObj = TypeConverter.ToObject(engine, thisObj);");
                        output.AppendLine($"\t\t\tif (!(thisObj is {classSyntax.Identifier.ToString()}))");
                        output.AppendLine($"\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"The method 'get {property.FunctionName}' is not generic.\");");
                        output.AppendLine($"\t\t\treturn (({classSyntax.Identifier.ToString()})thisObj).{property.PropertyName};");
                        output.AppendLine("\t\t}");

                        if (property.SetterStubName != null)
                        {
                            output.AppendLine();
                            output.AppendLine($"\t\tprivate static object {property.SetterStubName}(ScriptEngine engine, object thisObj, object[] args)");
                            output.AppendLine("\t\t{");
                            output.AppendLine($"\t\t\tthisObj = TypeConverter.ToObject(engine, thisObj);");
                            output.AppendLine($"\t\t\tif (!(thisObj is {classSyntax.Identifier.ToString()}))");
                            output.AppendLine($"\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"The method 'set {property.FunctionName}' is not generic.\");");
                            output.AppendLine($"\t\t\t(({classSyntax.Identifier.ToString()})thisObj).{property.PropertyName} = {ConvertTo("args.Length > 0 ? args[0] : Undefined.Value", property.ReturnType, null)};");
                            output.AppendLine("\t\t}");
                        }
                    }
                    foreach (var methodGroup in methodGroups)
                    {
                        GenerateMethodStub(output, classSyntax, methodGroup);
                    }

                    output.AppendLine("\t}");
                    output.AppendLine();
                    output.AppendLine("}");
                }

                if (outputFile)
                {
                    // Write the output file.
                    File.WriteAllText(Path.Combine(Path.GetDirectoryName(csFilePath), Path.GetFileNameWithoutExtension(csFilePath) + ".g.cs"), output.ToString());
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
                return methods.Select(m => new JSMethod(m)).GroupBy(ms => ms.FunctionName).Select(group => new JSMethodGroup(group));
            }

            public JSMethodGroup(IEnumerable<JSMethod> group)
                : base(group)
            {
                MethodName = this.First().MethodName;
                if (!this.All(m => m.MethodName == MethodName))
                    throw new InvalidOperationException("All methods must have the same name.");
                FunctionName = this.First().FunctionName;
                if (!this.All(m => m.FunctionName == FunctionName))
                    throw new InvalidOperationException("All methods must have the same name.");
                PropertyKey = this.First().PropertyKey;
                if (!this.All(m => m.PropertyKey == PropertyKey))
                    throw new InvalidOperationException("All methods must have the same name.");
                JSLength = this.First().JSLength;
                if (!this.All(m => m.JSLength == JSLength))
                    throw new InvalidOperationException($"All {FunctionName} methods must have the same length.");
                if (JSLength == -1)
                    JSLength = this.Max(m => m.Parameters.Count());
                JSPropertyAttributes = this.First().JSPropertyAttributes;
                if (!this.All(m => m.JSPropertyAttributes == JSPropertyAttributes))
                    throw new InvalidOperationException($"All {FunctionName} methods must have the same PropertyAttributes.");
                StubName = "__STUB__" + MethodName;
                IsStatic = this.First().IsStatic;
                if (!this.All(m => m.IsStatic == IsStatic))
                    throw new InvalidOperationException($"All {FunctionName} methods must all be static or instance methods.");
                RequiredArgumentCount = this.First().RequiredArgumentCount;
                if (!this.All(m => m.RequiredArgumentCount == RequiredArgumentCount))
                    throw new InvalidOperationException($"All {FunctionName} methods must all have the same RequiredArgumentCount.");
            }

            public string MethodName { get; private set; }
            public string PropertyKey { get; private set; }
            public string FunctionName { get; private set; }
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
                MethodName = methodSyntax.Identifier.ToString();
                ReturnType = methodSyntax.ReturnType.ToString();
                IsStatic = methodSyntax.Modifiers.Any(m => m.Kind() == SyntaxKind.StaticKeyword);

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
                var jsName = MethodName;
                var nameParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "Name");
                if (nameParameter != null)
                    jsName = ((LiteralExpressionSyntax)nameParameter.Expression).Token.ValueText;

                // Determine the property key and function name.
                if (jsName.StartsWith("@@"))
                {
                    // This is a symbol.
                    var symbolName = new StringBuilder(jsName.Substring(2));
                    symbolName[0] = char.ToUpper(symbolName[0]);
                    PropertyKey = $"engine.Symbol.{symbolName}";
                    FunctionName = $"[Symbol.{jsName.Substring(2)}]";
                }
                else
                {
                    // Not a symbol.
                    PropertyKey = $"\"{jsName}\"";
                    FunctionName = jsName;
                }

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
            public string MethodName { get; private set; }
            public string PropertyKey { get; private set; }
            public string FunctionName { get; private set; }
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
                PropertyName = propertySyntax.Identifier.ToString();
                ReturnType = propertySyntax.Type.ToString();
                if (propertySyntax.Modifiers.Any(m => m.Kind() == SyntaxKind.StaticKeyword))
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
                var jsName = PropertyName;
                var nameParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == "Name");
                if (nameParameter != null)
                    jsName = ((LiteralExpressionSyntax)nameParameter.Expression).Token.ValueText;

                // Determine the property key and function name.
                if (jsName.StartsWith("@@"))
                {
                    // This is a symbol.
                    var symbolName = new StringBuilder(jsName.Substring(2));
                    symbolName[0] = char.ToUpper(symbolName[0]);
                    PropertyKey = $"engine.Symbol.{symbolName}";
                    FunctionName = $"[Symbol.{jsName.Substring(2)}]";
                }
                else
                {
                    // Not a symbol.
                    PropertyKey = $"\"{jsName}\"";
                    FunctionName = jsName;
                }

                // Determine the stub names.
                GetterStubName = "__GETTER__" + PropertyName;
                if (propertySyntax.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration &&
                    !a.Modifiers.Any(m => m.Kind() == SyntaxKind.PrivateKeyword)))
                    SetterStubName = "__SETTER__" + PropertyName;
            }

            private bool GetBooleanAttributeFlag(AttributeSyntax jsAttribute, string name, bool defaultValue)
            {
                var boolParameter = jsAttribute?.ArgumentList?.Arguments.SingleOrDefault(arg =>
                    arg.NameEquals != null && arg.NameEquals.Name.ToString() == name);
                if (boolParameter == null)
                    return defaultValue;
                return (bool)((LiteralExpressionSyntax)boolParameter.Expression).Token.Value;
            }

            public string PropertyName { get; private set; }
            public string PropertyKey { get; private set; }
            public string FunctionName { get; private set; }
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
                output.AppendLine($"\t\t\t\tthrow new JavaScriptException(engine, ErrorType.TypeError, \"The method '{methodGroup.First().FunctionName}' is not generic.\");");
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
                    // If the parameter count is X, then find the method with smallest number of
                    // parameters which has at least X parameters.
                    var method = methodGroup.Where(mds => mds.Parameters.Count() >= i).OrderBy(mds => mds.Parameters.Count()).First();
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
            result.Append(method.MethodName);
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
                            case "SymbolInstance":
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
                case "ArrayBufferInstance":
                case "SymbolInstance":
                    return $"TypeConverter.ToObject<{type}>(engine, {arg})";
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