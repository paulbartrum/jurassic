using System;
using System.Collections.Generic;

namespace Jurassic.Library
{

    /// <summary>
    /// Represents a container for property names and attributes.
    /// </summary>
    [Serializable]
    internal class HiddenClassSchema
    {
        // Properties
        private Dictionary<string, SchemaProperty> properties;

        // Transitions
        [Serializable]
        private struct AddPropertyTransition
        {
            public string Name;
            public PropertyAttributes Attributes;
        }
        private Dictionary<AddPropertyTransition, HiddenClassSchema> addTransitions;
        private Dictionary<string, HiddenClassSchema> deleteTransitions;
        private Dictionary<AddPropertyTransition, HiddenClassSchema> modifyTransitions;

        // The index of the next value.
        private int nextValueIndex;

        /// <summary>
        /// Creates a new HiddenClassSchema instance.
        /// </summary>
        private HiddenClassSchema(Dictionary<string, SchemaProperty> properties, int nextValueIndex)
        {
            this.properties = properties;
            this.addTransitions = new Dictionary<AddPropertyTransition, HiddenClassSchema>();
            this.deleteTransitions = new Dictionary<string, HiddenClassSchema>();
            this.modifyTransitions = new Dictionary<AddPropertyTransition, HiddenClassSchema>();
            this.nextValueIndex = nextValueIndex;
        }

        /// <summary>
        /// Creates a hidden class schema with no properties.
        /// </summary>
        /// <returns> A hidden class schema with no properties. </returns>
        public static HiddenClassSchema CreateEmptySchema()
        {
            return new HiddenClassSchema(new Dictionary<string, SchemaProperty>(), 0);
        }

        /// <summary>
        /// Gets the number of properties defined in this schema.
        /// </summary>
        public int PropertyCount
        {
            get { return this.properties.Count; }
        }

        /// <summary>
        /// Gets the index into the Values array of the next added property.
        /// </summary>
        public int NextValueIndex
        {
            get { return this.nextValueIndex; }
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
            SchemaProperty propertyInfo;
            if (this.properties.TryGetValue(name, out propertyInfo) == false)
                return -1;
            return propertyInfo.Index;
        }

        /// <summary>
        /// Gets the zero-based index of the property with the given name and the attributes
        /// associated with the property.
        /// </summary>
        /// <param name="name"> The name of the property. </param>
        /// <returns> A structure containing the zero-based index of the property, or <c>-1</c> if a property with the
        /// given name does not exist. </returns>
        public SchemaProperty GetPropertyIndexAndAttributes(string name)
        {
            SchemaProperty propertyInfo;
            if (this.properties.TryGetValue(name, out propertyInfo) == false)
                return SchemaProperty.Undefined;
            return propertyInfo;
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
                var properties = new Dictionary<string, SchemaProperty>(this.properties);
                properties.Add(name, new SchemaProperty(this.NextValueIndex, attributes));
                newSchema = new HiddenClassSchema(properties, this.NextValueIndex + 1);

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
                var properties = new Dictionary<string, SchemaProperty>(this.properties);
                if (properties.Remove(name) == false)
                    throw new InvalidOperationException(string.Format("The property '{0}' does not exist.", name));
                newSchema = new HiddenClassSchema(properties, this.NextValueIndex);

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
                SchemaProperty propertyInfo;
                if (this.properties.TryGetValue(name, out propertyInfo) == false)
                    throw new InvalidOperationException(string.Format("The property '{0}' does not exist.", name));
                if (attributes == propertyInfo.Attributes)
                    return this;

                // Create a new schema based on this one.
                var properties = new Dictionary<string, SchemaProperty>(this.properties);
                properties[name] = new SchemaProperty(propertyInfo.Index, attributes);
                newSchema = new HiddenClassSchema(properties, this.NextValueIndex);

                // Add a transition to the new schema.
                this.modifyTransitions.Add(new AddPropertyTransition() { Name = name, Attributes = attributes }, newSchema);
            }

            return newSchema;
        }
    }

}
