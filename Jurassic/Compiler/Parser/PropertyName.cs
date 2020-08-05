using System;

namespace Jurassic.Compiler
{
    /// <summary>
    /// Represents the valid prefixes that can appear before a property name.
    /// </summary>
    [Flags]
    internal enum PropertyNameFlags
    {
        None = 0,
        Get = 1,    // Mutually exclusive with Set.
        Set = 2,    // Mutually exclusive with Get.
        Static = 4,
    }

    /// <summary>
    /// Represents the name of a property (member), which can be
    /// </summary>
    internal struct PropertyName
    {
        PropertyNameFlags flags;
        object name;

        /// <summary>
        /// Creates a property name instance with a statically-known name.
        /// </summary>
        /// <param name="name"> The statically-known name. </param>
        public PropertyName(string name)
        {
            this.flags = PropertyNameFlags.None;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Creates a property name instance with a name that will be computed at runtime.
        /// </summary>
        /// <param name="name"> The expression that will compute the name. </param>
        public PropertyName(Expression name)
        {
            this.flags = PropertyNameFlags.None;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Indicates the property was preceeded by 'get', making this property a getter.
        /// </summary>
        public bool IsGetter
        {
            get { return flags.HasFlag(PropertyNameFlags.Get); }
        }

        /// <summary>
        /// Indicates the property was preceeded by 'set', making this property a setter.
        /// </summary>
        public bool IsSetter
        {
            get { return flags.HasFlag(PropertyNameFlags.Set); }
        }

        /// <summary>
        /// Indicates the property was preceeded by 'static', making this a static function
        /// (applies to classes, not object literals).
        /// </summary>
        public bool IsStatic
        {
            get { return flags.HasFlag(PropertyNameFlags.Static); }
        }

        /// <summary>
        /// Indicates whether the name is statically known.
        /// </summary>
        public bool HasStaticName
        {
            get { return name is string; }
        }

        /// <summary>
        /// If HasStaticName is <c>true</c>, contains the name of the property.
        /// </summary>
        public string StaticName
        {
            get { return (string)name; }
        }

        /// <summary>
        /// If HasStaticName is <c>false</c>, contains an expression which computes the name of the
        /// property.
        /// </summary>
        public Expression ComputedName
        {
            get { return (Expression)name; }
        }

        /// <summary>
        /// Modifies this name to include the given flags.
        /// </summary>
        /// <param name="flags"> The flags to add. </param>
        /// <returns> A new property name instance with the existing flags and the provided flags. </returns>
        public PropertyName WithFlags(PropertyNameFlags flags)
        {
            this.flags |= flags;
            return this;
        }
    }
}
