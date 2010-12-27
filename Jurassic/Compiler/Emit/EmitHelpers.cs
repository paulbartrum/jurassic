using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Outputs IL for misc tasks.
    /// </summary>
    internal static class EmitHelpers
    {
        /// <summary>
        /// Emits undefined.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void EmitUndefined(ILGenerator generator)
        {
            generator.LoadField(ReflectionHelpers.Undefined_Value);
        }

        /// <summary>
        /// Emits null.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void EmitNull(ILGenerator generator)
        {
            generator.LoadField(ReflectionHelpers.Null_Value);
        }

        /// <summary>
        /// Emits a dummy value of the given type.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        /// <param name="type"> The type of value to generate. </param>
        public static void EmitDummyValue(ILGenerator generator, PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.Bool:
                    generator.LoadBoolean(false);
                    break;
                case PrimitiveType.UInt32:
                case PrimitiveType.Int32:
                    generator.LoadInt32(0);
                    break;
                case PrimitiveType.Null:
                    EmitNull(generator);
                    break;
                case PrimitiveType.Number:
                    generator.LoadDouble(0);
                    break;
                case PrimitiveType.Undefined:
                    EmitUndefined(generator);
                    break;
                case PrimitiveType.Any:
                case PrimitiveType.Object:
                case PrimitiveType.String:
                    generator.LoadNull();
                    break;
                default:
                    throw new NotImplementedException("Unsupported PrimitiveType value.");
            }
        }

        /// <summary>
        /// Emits a JavaScriptException.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        /// <param name="name"> The type of error to generate. </param>
        /// <param name="message"> The error message. </param>
        public static void EmitThrow(ILGenerator generator, string name, string message)
        {
            EmitHelpers.LoadScriptEngine(generator);
            generator.LoadString(name);
            generator.LoadString(message);
            generator.NewObject(ReflectionHelpers.JavaScriptException_Constructor_Error);
            generator.Throw();
        }



        //     LOAD METHOD PARAMETERS
        //_________________________________________________________________________________________

        /// <summary>
        /// Pushes a reference to the script engine onto the stack.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void LoadScriptEngine(ILGenerator generator)
        {
            generator.LoadArgument(0);
        }

        /// <summary>
        /// Pushes a reference to the current scope onto the stack.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void LoadScope(ILGenerator generator)
        {
            generator.LoadArgument(1);
        }

        /// <summary>
        /// Stores the reference on top of the stack as the new scope.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void StoreScope(ILGenerator generator)
        {
            generator.StoreArgument(1);
        }

        /// <summary>
        /// Pushes the value of the <c>this</c> keyword onto the stack.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void LoadThis(ILGenerator generator)
        {
            generator.LoadArgument(2);
        }

        /// <summary>
        /// Stores the reference on top of the stack as the new value of the <c>this</c> keyword.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void StoreThis(ILGenerator generator)
        {
            generator.StoreArgument(2);
        }

        /// <summary>
        /// Pushes a reference to the current function onto the stack.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void LoadFunction(ILGenerator generator)
        {
            generator.LoadArgument(3);
        }

        /// <summary>
        /// Pushes a reference to the array of argument values for the current function onto the
        /// stack.
        /// </summary>
        /// <param name="generator"> The IL generator. </param>
        public static void LoadArgumentsArray(ILGenerator generator)
        {
            generator.LoadArgument(4);
        }
    }

}
