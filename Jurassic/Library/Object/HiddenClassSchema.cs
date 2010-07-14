using System;
using System.Collections.Generic;

namespace Jurassic.Library
{

    /// <summary>
    /// Represents a container for property names and attributes.
    /// </summary>
    internal class HiddenClassSchema
    {
        // Properties
        private struct PropertyInfo
        {
            public int Index;
            public PropertyAttributes Attributes;
        }
        private Dictionary<string, PropertyInfo> properties;

        // Transitions
        private struct AddPropertyTransition
        {
            public string Name;
            public PropertyAttributes Attributes;
        }
        private Dictionary<AddPropertyTransition, HiddenClassSchema> addTransitions;
        private Dictionary<string, HiddenClassSchema> deleteTransitions;
        private Dictionary<AddPropertyTransition, HiddenClassSchema> modifyTransitions;

        // The initial schema
        [ThreadStatic]
        private static HiddenClassSchema empty;

        /// <summary>
        /// Creates a new HiddenClassSchema instance.
        /// </summary>
        private HiddenClassSchema(Dictionary<string, PropertyInfo> properties)
        {
            this.properties = properties;
            this.addTransitions = new Dictionary<AddPropertyTransition, HiddenClassSchema>();
            this.deleteTransitions = new Dictionary<string, HiddenClassSchema>();
            this.modifyTransitions = new Dictionary<AddPropertyTransition, HiddenClassSchema>();
        }

        /// <summary>
        /// Gets an empty schema.
        /// </summary>
        public static HiddenClassSchema Empty
        {
            get
            {
                if (empty == null)
                    empty = new HiddenClassSchema(new Dictionary<string, PropertyInfo>());
                return empty;
            }
        }

        /// <summary>
        /// Enumerates the property names and values for this schema.
        /// </summary>
        /// <param name="values"> The array containing the property values. </param>
        /// <returns> An enumerable collection of property names and values. </returns>
        public IEnumerable<PropertyNameAndValue> EnumeratePropertyNamesAndValues(object[] values)
        {
            foreach (var pair in this.properties)
            {
                yield return new PropertyNameAndValue(pair.Key, new PropertyDescriptor(values[pair.Value.Index], pair.Value.Attributes));
            }
        }

        /// <summary>
        /// Gets the zero-based index of the property with the given name.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <returns> The zero-based index of the property, or <c>-1</c> if a property with the
        /// given name does not exist. </returns>
        public int GetPropertyIndex(string name)
        {
            PropertyInfo propertyInfo;
            if (this.properties.TryGetValue(name, out propertyInfo) == false)
                return -1;
            return propertyInfo.Index;
        }

        /// <summary>
        /// Gets the zero-based index of the property with the given name and the attributes
        /// associated with the property.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <param name="attributes"> A variable that will receive the property attributes. </param>
        /// <returns> The zero-based index of the property, or <c>-1</c> if a property with the
        /// given name does not exist. </returns>
        public int GetPropertyIndexAndAttributes(string name, out PropertyAttributes attributes)
        {
            PropertyInfo propertyInfo;
            if (this.properties.TryGetValue(name, out propertyInfo) == false)
            {
                attributes = PropertyAttributes.Sealed;
                return -1;
            }
            attributes = propertyInfo.Attributes;
            return propertyInfo.Index;
        }

        /// <summary>
        /// Adds a property to the schema.
        /// </summary>
        /// <param name="name"> The name of the property to add. </param>
        /// <param name="attributes"> The property attributes. </param>
        /// <returns> A new schema with the extra property. </returns>
        public HiddenClassSchema AddProperty(string name, PropertyAttributes attributes = PropertyAttributes.FullAccess)
        {
            // Check if there is a transition to the schema already.
            HiddenClassSchema newSchema;
            this.addTransitions.TryGetValue(new AddPropertyTransition() { Name = name, Attributes = attributes }, out newSchema);

            if (newSchema == null)
            {
                // Create a new schema based on this one.
                var properties = new Dictionary<string, PropertyInfo>(this.properties);
                properties.Add(name, new PropertyInfo() { Index = this.properties.Count, Attributes = attributes });
                newSchema = new HiddenClassSchema(properties);

                // Add a transition to the new schema.
                this.addTransitions.Add(new AddPropertyTransition() { Name = name, Attributes = attributes }, newSchema);
            }

            return newSchema;
        }

        /// <summary>
        /// Deletes a property from the schema.
        /// </summary>
        /// <param name="name"> The name of the property to delete. </param>
        /// <returns> A new schema without the property. </returns>
        public HiddenClassSchema DeleteProperty(string name)
        {
            // Check if there is a transition to the schema already.
            HiddenClassSchema newSchema;
            this.deleteTransitions.TryGetValue(name, out newSchema);

            if (newSchema == null)
            {
                // Create a new schema based on this one.
                var properties = new Dictionary<string, PropertyInfo>(this.properties);
                if (properties.Remove(name) == false)
                    throw new InvalidOperationException(string.Format("The property '{0}' does not exist.", name));
                newSchema = new HiddenClassSchema(properties);

                // Add a transition to the new schema.
                this.deleteTransitions.Add(name, newSchema);
            }

            return newSchema;
        }

        /// <summary>
        /// Modifies the attributes for a property in the schema.
        /// </summary>
        /// <param name="name"> The name of the property to modify. </param>
        /// <param name="attributes"> The new attributes. </param>
        /// <returns> A new schema with the modified property. </returns>
        public HiddenClassSchema SetPropertyAttributes(string name, PropertyAttributes attributes)
        {
            // Check if there is a transition to the schema already.
            HiddenClassSchema newSchema;
            this.modifyTransitions.TryGetValue(new AddPropertyTransition() { Name = name, Attributes = attributes }, out newSchema);

            if (newSchema == null)
            {
                // Check the attributes differ from the existing attributes.
                PropertyInfo propertyInfo;
                if (this.properties.TryGetValue(name, out propertyInfo) == false)
                    throw new InvalidOperationException(string.Format("The property '{0}' does not exist.", name));
                if (attributes == propertyInfo.Attributes)
                    return this;

                // Create a new schema based on this one.
                var properties = new Dictionary<string, PropertyInfo>(this.properties);
                properties[name] = new PropertyInfo() { Index = propertyInfo.Index, Attributes = attributes };
                newSchema = new HiddenClassSchema(properties);

                // Add a transition to the new schema.
                this.modifyTransitions.Add(new AddPropertyTransition() { Name = name, Attributes = attributes }, newSchema);
            }

            return newSchema;
        }
    }

}
