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
                case PrimitiveType.ConcatenatedString:
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
            generator.LoadString(name);
            generator.LoadString(message);
            generator.NewObject(ReflectionHelpers.JavaScriptException_Constructor2);
            generator.Throw();
        }
    }

}
