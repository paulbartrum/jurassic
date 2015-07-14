using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SilverlightREPL
{

    /// <summary>
    /// Used to display a tree-view type representation of an object.
    /// </summary>
    public class ExpandableObjectControl : StackPanel
    {
        private object root;
        private Func<object, IEnumerable<ExpandableObjectProperty>> propertyEnumerator;

        /// <summary>
        /// Creates a new ExpandableObjectControl instance with the given root object.
        /// </summary>
        /// <param name="text"> The text to display. </param>
        /// <param name="root"> A reference that represents the object being displayed. </param>
        /// <param name="propertyEnumerator"> A callback used to enumerate child properties. </param>
        public ExpandableObjectControl(string text, object root, Func<object, IEnumerable<ExpandableObjectProperty>> propertyEnumerator)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (root == null)
                throw new ArgumentNullException("root");
            if (propertyEnumerator == null)
                throw new ArgumentNullException("propertyEnumerator");
            this.root = root;
            this.propertyEnumerator = propertyEnumerator;
            this.Children.Add(CreateControl(null, null, text, root, Colors.Black));
        }

        /// <summary>
        /// Information attached to each StackPanel item.
        /// </summary>
        private class TagInfo
        {
            public object Key;
            public ExpanderButton Parent;
        }

        /// <summary>
        /// Creates a StackPanel item.
        /// </summary>
        /// <param name="parent"> The control that was expanded. </param>
        /// <param name="name"> The property name. </param>
        /// <param name="value"> The property value (as a string). </param>
        /// <param name="key"> A key that can be used to retrieve further properties. </param>
        /// <param name="valueColor"> The color to use for the value. </param>
        /// <returns> A control that should be inserted into the Items collection. </returns>
        private Control CreateControl(ExpanderButton parent, string name, string value, object key, Color valueColor)
        {
            // Create the content portion.
            var content = new TextBlock();
            if (name == null)
            {
                content.Inlines.Add(new Run() { Text = value });
            }
            else
            {
                content.Inlines.Add(new Run() { Foreground = new SolidColorBrush(Colors.Purple), Text = name });
                content.Inlines.Add(new Run() { Foreground = new SolidColorBrush(valueColor), Text = ": " + value });
            }

            // Create tag information to attach to the control.
            var tagInfo = new TagInfo();
            tagInfo.Key = key;
            tagInfo.Parent = parent;

            // Calculate the left margin.
            var leftMargin = parent == null ? 0 : parent.Margin.Left + 8;

            // Use an expander if the value is an object.
            if (key != null)
            {
                var result = new ExpanderButton();
                result.Margin = new Thickness(leftMargin, 0, 0, 0);
                result.Tag = tagInfo;
                result.Content = content;
                result.Checked += new RoutedEventHandler(OnChecked);
                result.Unchecked += new RoutedEventHandler(OnUnchecked);
                return result;
            }
            else
            {
                var result = new ContentControl();
                result.Content = content;
                result.Margin = new Thickness(leftMargin + 16, 0, 0, 0);
                result.Tag = tagInfo;
                return result;
            }
        }

        /// <summary>
        /// Called when an item is expanded.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            var source = (ExpanderButton)sender;

            // Get the position in the Items list of the source control.
            int index = this.Children.IndexOf(source);

            // Enumerate the properties of the object.
            foreach (var property in this.propertyEnumerator(((TagInfo)source.Tag).Key))
            {
                // Create a new child control.
                var child = CreateControl(source, property.Name, property.Value, property.Key, property.Color);
                
                // Insert the child control.
                index++;
                this.Children.Insert(index, child);
            }
        }

        /// <summary>
        /// Called when an item is collapsed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            var source = (ExpanderButton)sender;

            // Get the position of the children in the Items list of the source control.
            int index = this.Children.IndexOf(source) + 1;

            while (this.Children.Count > index)
            {
                // Check if the item is a child property.
                if (HasAncestor(this.Children[index] as FrameworkElement, source) == false)
                    break; 
                
                // Remove the child property.
                this.Children.RemoveAt(index);
            }
            
        }

        /// <summary>
        /// Determines if a control is a logical child of another control.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="ancestor"></param>
        /// <returns></returns>
        private static bool HasAncestor(FrameworkElement child, ExpanderButton ancestor)
        {
            while (true)
            {
                var parent = ((TagInfo)child.Tag).Parent;
                if (parent == null)
                    return false;
                if (parent == ancestor)
                    return true;
                child = parent;
            }
        }
    }

    /// <summary>
    /// Information about a single property.
    /// </summary>
    public class ExpandableObjectProperty
    {
        public ExpandableObjectProperty(string name, string value, object key)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (value == null)
                throw new ArgumentNullException("value");
            this.Name = name;
            this.Value = value;
            this.Key = key;
            this.Color = Colors.Black;
        }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the property value, as a string.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the color of the value.
        /// </summary>
        public Color Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a key that can be used to retrieve further properties.
        /// </summary>
        public object Key
        {
            get;
            private set;
        }
    }

}