using System;
using System.Collections.Generic;
using System.Linq;

namespace Jurassic.Library
{

    /// <summary>
    /// Represents a container for property names and attributes.
    /// </summary>
    internal class HiddenClassSchema
    {
        // Properties
        private Dictionary<object, SchemaProperty> properties;

        // Transitions
        private struct TransitionInfo
        {
            public object Key;
            public PropertyAttributes Attributes;
        }
        private Dictionary<TransitionInfo, HiddenClassSchema> addTransitions;
        private Dictionary<object, HiddenClassSchema> deleteTransitions;
        private Dictionary<TransitionInfo, HiddenClassSchema> modifyTransitions;

        // The index of the next value.
        private int nextValueIndex;

        // Used to recreate the properties dictionary if properties == null.
        private HiddenClassSchema parent;
        private TransitionInfo addPropertyTransitionInfo;

        /// <summary>
        /// Creates a new HiddenClassSchema instance from a modify or delete operation.
        /// </summary>
        internal HiddenClassSchema(Dictionary<object, SchemaProperty> properties, int nextValueIndex)
        {
            this.properties = properties;
            this.addTransitions = null;
            this.deleteTransitions = null;
            this.modifyTransitions = null;
            this.nextValueIndex = nextValueIndex;
        }

        /// <summary>
        /// Creates a new HiddenClassSchema instance from an add operation.
        /// </summary>
        private HiddenClassSchema(Dictionary<object, SchemaProperty> properties, int nextValueIndex, HiddenClassSchema parent, TransitionInfo addPropertyTransitionInfo)
            : this(properties, nextValueIndex)
        {
            this.parent = parent;
            this.addPropertyTransitionInfo = addPropertyTransitionInfo;
        }

        /// <summary>
        /// Creates a hidden class schema with no properties.
        /// </summary>
        /// <returns> A hidden class schema with no properties. </returns>
        public static HiddenClassSchema CreateEmptySchema()
        {
            return new HiddenClassSchema(new Dictionary<object, SchemaProperty>(), 0);
        }

        /// <summary>
        /// Gets the number of properties defined in this schema.
        /// </summary>
        public int PropertyCount
        {
            get
            {
                if (this.properties == null)
                    this.properties = CreatePropertiesDictionary();
                return this.properties.Count;
            }
        }

        /// <summary>
        /// Gets the index into the Values array of the next added property.
        /// </summary>
        public int NextValueIndex
        {
            get { return this.nextValueIndex; }
        }

        /// <summary>
        /// Enumerates the property names for this schema.
        /// </summary>
        /// <returns> An enumerable collection of property names. </returns>
        public IEnumerable<object> EnumeratePropertyNames()
        {
            if (this.properties == null)
                this.properties = CreatePropertiesDictionary();
            this.parent = null;     // Prevents the properties dictionary from being stolen while an enumeration is in progress.
            foreach (var pair in this.properties)
                yield return pair.Key;
        }

        /// <summary>
        /// Enumerates the property names and values for this schema.
        /// </summary>
        /// <param name="values"> The array containing the property values. </param>
        /// <returns> An enumerable collection of property names and values. </returns>
        public IEnumerable<PropertyNameAndValue> EnumeratePropertyNamesAndValues(object[] values)
        {
            if (this.properties == null)
                this.properties = CreatePropertiesDictionary();
            this.parent = null;     // Prevents the properties dictionary from being stolen while an enumeration is in progress.
            foreach (var pair in this.properties)
                yield return new PropertyNameAndValue(pair.Key, values[pair.Value.Index], pair.Value.Attributes);
        }

        /// <summary>
        /// Gets the zero-based index of the property with the given name.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> The zero-based index of the property, or <c>-1</c> if a property with the
        /// given name does not exist. </returns>
        public int GetPropertyIndex(object key)
        {
            return GetPropertyIndexAndAttributes(key).Index;
        }

        /// <summary>
        /// Gets the zero-based index of the property with the given name and the attributes
        /// associated with the property.
        /// </summary>
        /// <param name="key"> The property key (either a string or a Symbol). </param>
        /// <returns> A structure containing the zero-based index of the property, or <c>-1</c> if a property with the
        /// given name does not exist. </returns>
        public SchemaProperty GetPropertyIndexAndAttributes(object key)
        {
            if (this.properties == null)
                this.properties = CreatePropertiesDictionary();
            SchemaProperty propertyInfo;
            if (this.properties.TryGetValue(key, out propertyInfo) == false)
                return SchemaProperty.Undefined;
            return propertyInfo;
        }

        /// <summary>
        /// Adds a property to the schema.
        /// </summary>
        /// <param name="key"> The property key of the property to add. </param>
        /// <param name="attributes"> The property attributes. </param>
        /// <returns> A new schema with the extra property. </returns>
        public HiddenClassSchema AddProperty(object key, PropertyAttributes attributes = PropertyAttributes.FullAccess)
        {
            // Package the name and attributes into a struct.
            var transitionInfo = new TransitionInfo() { Key = key, Attributes = attributes };

            // Check if there is a transition to the schema already.
            HiddenClassSchema newSchema = null;
            if (this.addTransitions != null)
                this.addTransitions.TryGetValue(transitionInfo, out newSchema);

            if (newSchema == null)
            {
                if (this.parent == null)
                {
                    // Create a new schema based on this one.  A complete copy must be made of the properties hashtable.
                    var properties = new Dictionary<object, SchemaProperty>(this.properties);
                    properties.Add(key, new SchemaProperty(this.NextValueIndex, attributes));
                    newSchema = new HiddenClassSchema(properties, this.NextValueIndex + 1, this, transitionInfo);
                }
                else
                {
                    // Create a new schema based on this one.  The properties hashtable is "given
                    // away" so a copy does not have to be made.
                    if (this.properties == null)
                        this.properties = CreatePropertiesDictionary();
                    this.properties.Add(key, new SchemaProperty(this.NextValueIndex, attributes));
                    newSchema = new HiddenClassSchema(this.properties, this.NextValueIndex + 1, this, transitionInfo);
                    this.properties = null;
                }
                

                // Add a transition to the new schema.
                if (this.addTransitions == null)
                    this.addTransitions = new Dictionary<TransitionInfo, HiddenClassSchema>(1);
                this.addTransitions.Add(transitionInfo, newSchema);
            }

            return newSchema;
        }

        /// <summary>
        /// Adds multiple properties to the schema.
        /// </summary>
        /// <param name="properties"> The properties to add. </param>
        /// <returns> A new schema with the extra properties. </returns>
        public HiddenClassSchema AddProperties(IEnumerable<PropertyNameAndValue> properties)
        {
            if (this.properties == null)
            {
                var propertyDictionary = new Dictionary<object, SchemaProperty>(properties.Count());
                int nextValueIndex = 0;
                foreach (var property in properties)
                    propertyDictionary.Add(property.Key, new SchemaProperty(nextValueIndex ++, property.Attributes));
                return new HiddenClassSchema(propertyDictionary, nextValueIndex);
            }
            else
            {
                // There are already properties in the schema.  Just add them one by one.
                HiddenClassSchema newSchema = this;
                foreach (var property in properties)
                    newSchema = AddProperty(property.Key, property.Attributes);
                return newSchema;
            }
        }

        /// <summary>
        /// Deletes a property from the schema.
        /// </summary>
        /// <param name="key"> The property key of the property to delete. </param>
        /// <returns> A new schema without the property. </returns>
        public HiddenClassSchema DeleteProperty(object key)
        {
            // Check if there is a transition to the schema already.
            HiddenClassSchema newSchema = null;
            if (this.deleteTransitions != null)
                this.deleteTransitions.TryGetValue(key, out newSchema);

            if (newSchema == null)
            {
                // Create a new schema based on this one.
                var properties = this.properties == null ? CreatePropertiesDictionary() : new Dictionary<object, SchemaProperty>(this.properties);
                if (properties.Remove(key) == false)
                    throw new InvalidOperationException(string.Format("The property '{0}' does not exist.", key));
                newSchema = new HiddenClassSchema(properties, this.NextValueIndex);

                // Add a transition to the new schema.
                if (this.deleteTransitions == null)
                    this.deleteTransitions = new Dictionary<object, HiddenClassSchema>(1);
                this.deleteTransitions.Add(key, newSchema);
            }

            return newSchema;
        }

        /// <summary>
        /// Modifies the attributes for a property in the schema.
        /// </summary>
        /// <param name="key"> The property key of the property to modify. </param>
        /// <param name="attributes"> The new attributes. </param>
        /// <returns> A new schema with the modified property. </returns>
        public HiddenClassSchema SetPropertyAttributes(object key, PropertyAttributes attributes)
        {
            // Package the name and attributes into a struct.
            var transitionInfo = new TransitionInfo() { Key = key, Attributes = attributes };

            // Check if there is a transition to the schema already.
            HiddenClassSchema newSchema = null;
            if (this.modifyTransitions != null)
                this.modifyTransitions.TryGetValue(transitionInfo, out newSchema);

            if (newSchema == null)
            {
                // Create the properties dictionary if it hasn't already been created.
                if (this.properties == null)
                    this.properties = CreatePropertiesDictionary();

                // Check the attributes differ from the existing attributes.
                SchemaProperty propertyInfo;
                if (this.properties.TryGetValue(key, out propertyInfo) == false)
                    throw new InvalidOperationException(string.Format("The property '{0}' does not exist.", key));
                if (attributes == propertyInfo.Attributes)
                    return this;

                // Create a new schema based on this one.
                var properties = new Dictionary<object, SchemaProperty>(this.properties);
                properties[key] = new SchemaProperty(propertyInfo.Index, attributes);
                newSchema = new HiddenClassSchema(properties, this.NextValueIndex);

                // Add a transition to the new schema.
                if (this.modifyTransitions == null)
                    this.modifyTransitions = new Dictionary<TransitionInfo, HiddenClassSchema>(1);
                this.modifyTransitions.Add(transitionInfo, newSchema);
            }

            return newSchema;
        }

        /// <summary>
        /// Creates the properties dictionary.
        /// </summary>
        private Dictionary<object, SchemaProperty> CreatePropertiesDictionary()
        {
            // Search up the tree until a schema is found with a populated properties hashtable, 
            // while keeping a list of the transitions.

            var addTransitions = new Stack<KeyValuePair<object, SchemaProperty>>();
            var node = this;
            while (node != null)
            {
                if (node.properties == null)
                {
                    // The schema is the same as the parent schema except with the addition of a single
                    // property.
                    addTransitions.Push(new KeyValuePair<object, SchemaProperty>(
                        node.addPropertyTransitionInfo.Key,
                        new SchemaProperty(node.NextValueIndex - 1, node.addPropertyTransitionInfo.Attributes)));
                }
                else
                {
                    // The schema has a populated properties hashtable - we can stop here.
                    break;
                }
                node = node.parent;
            }
            if (node == null)
                throw new InvalidOperationException("Internal error: no route to a populated schema was found.");

            // Add the properties to the hashtable in order.
            var result = new Dictionary<object, SchemaProperty>(node.properties);
            while (addTransitions.Count > 0)
            {
                var keyValuePair = addTransitions.Pop();
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return result;
        }
    }

}
