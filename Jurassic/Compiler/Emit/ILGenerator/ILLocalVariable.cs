using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a local variable in CIL code.
    /// </summary>
    internal abstract class ILLocalVariable
    {
        /// <summary>
        /// Gets the zero-based index of the local variable within the method body.
        /// </summary>
        public abstract int Index
        {
            get;
        }

        /// <summary>
        /// Gets the type of the local variable.
        /// </summary>
        public abstract Type Type
        {
            get;
        }

        /// <summary>
        /// Gets the local variable name, or <c>null</c> if a name was not provided.
        /// </summary>
        public abstract string Name
        {
            get;
        }
    }

    /// <summary>
    /// Represents a local variable in CIL code.
    /// </summary>
    internal class DynamicILLocalVariable : ILLocalVariable
    {
        private int index;
        private Type type;
        private string name;

        /// <summary>
        /// Creates a new local variable instance.
        /// </summary>
        /// <param name="generator"> The generator that created this variable. </param>
        /// <param name="index"> The index of the local variable within the method body. </param>
        /// <param name="type"> The type of the variable. </param>
        /// <param name="name"> The name of the local variable.  Can be <c>null</c>. </param>
#if NETSTANDARD1_6
        public DynamicILLocalVariable(ReflectionEmitILGenerator generator, int index, Type type, string name)
#else
        public DynamicILLocalVariable(DynamicILGenerator generator, int index, Type type, string name)
#endif
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            this.ILGenerator = generator;
            this.index = index;
            this.type = type;
            this.name = name;
        }

        /// <summary>
        /// Gets the generator that created this variable.
        /// </summary>
        public ILGenerator ILGenerator
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the zero-based index of the local variable within the method body.
        /// </summary>
        public override int Index
        {
            get { return this.index; }
        }

        /// <summary>
        /// Gets the type of the local variable.
        /// </summary>
        public override Type Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the local variable name, or <c>null</c> if a name was not provided.
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }
    }

    /// <summary>
    /// Represents a local variable in CIL code.
    /// </summary>
    internal class ReflectionEmitILLocalVariable : ILLocalVariable
    {
        private string name;

        /// <summary>
        /// Creates a new local variable instance.
        /// </summary>
        /// <param name="local"> The underlying local variable. </param>
        /// <param name="name"> The name of the local variable.  Can be <c>null</c>. </param>
        public ReflectionEmitILLocalVariable(System.Reflection.Emit.LocalBuilder local, string name)
        {
            if (local == null)
                throw new ArgumentNullException(nameof(local));
            this.UnderlyingLocal = local;
            this.name = name;
            //if (name != null)
            //    local.SetLocalSymInfo(name);
        }

        /// <summary>
        /// Gets the underlying local variable.
        /// </summary>
        public System.Reflection.Emit.LocalBuilder UnderlyingLocal
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the zero-based index of the local variable within the method body.
        /// </summary>
        public override int Index
        {
            get { return this.UnderlyingLocal.LocalIndex; }
        }

        /// <summary>
        /// Gets the type of the local variable.
        /// </summary>
        public override Type Type
        {
            get { return this.UnderlyingLocal.LocalType; }
        }

        /// <summary>
        /// Gets the local variable name, or <c>null</c> if a name was not provided.
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }
    }

}
