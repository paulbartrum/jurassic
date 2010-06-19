using System;
using System.Linq.Expressions;
using Jurassic.Library;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a variable scope or an object instance.
    /// </summary>
    internal interface IEnvironmentRecord
    {
        /// <summary>
        /// Determines if an environment record has a binding for an identifier.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <returns> <c>true</c> the binding exists; <c>false</c> if it does not. </returns>
        bool HasBinding(string name);

        /// <summary>
        /// Creates a new mutable binding in the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="mayBeDeleted"> <c>true</c> if the binding can be deleted; <c>false</c>
        /// otherwise. </param>
        void CreateMutableBinding(string name, bool mayBeDeleted);

        /// <summary>
        /// Sets the value of an already existing mutable binding in the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="value"> The new value of the binding. </param>
        /// <param name="strict"> Indicates whether to use strict mode semantics. </param>
        /// <returns> The new value of the binding. </returns>
        T SetMutableBinding<T>(string name, T value, bool strict);
        
        /// <summary>
        /// Returns the value of an already existing binding from the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <param name="strict"> Indicates whether to use strict mode semantics. </param>
        /// <returns> The value of the binding. </returns>
        object GetBindingValue(string name, bool strict);

        /// <summary>
        /// Deletes a binding from the environment record.
        /// </summary>
        /// <param name="name"> The name of the identifier. </param>
        /// <returns> <c>true</c> if the binding exists and could be deleted, or if the binding
        /// doesn't exist; <c>false</c> if the binding couldn't be deleted. </returns>
        bool DeleteBinding(string name);
    }

}