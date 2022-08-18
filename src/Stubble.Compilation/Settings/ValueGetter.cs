using System;
using System.Linq.Expressions;

namespace Stubble.Compilation.Settings
{
    /// <summary>
    /// Delegate type for value getters.
    /// </summary>
    /// <param name="type">The type of member to lookup the value on.</param>
    /// <param name="instance">An expression tothe member to lookup the value on.</param>
    /// <param name="key">The key to lookup.</param>
    /// <param name="ignoreCase">If case should be ignored when looking up value.</param>
    /// <returns>The expression to find the value or null if not found.</returns>
    public delegate Expression ValueGetterDelegate(Type type, Expression instance, string key, bool ignoreCase);

    /// <summary>
    /// Represents a specific value getter for a type match.
    /// </summary>
    public readonly struct ValueGetter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueGetter"/> struct.
        /// </summary>
        /// <param name="key">The type for the value getter for merging.</param>
        /// <param name="typeMatchCheck">The function for checking for if the type matches.</param>
        /// <param name="valueGetterMethod">The method for retrieving the value from the type.</param>
        public ValueGetter(Type key, Func<Type, bool> typeMatchCheck, ValueGetterDelegate valueGetterMethod)
        {
            Key = key;
            TypeMatchCheck = typeMatchCheck;
            ValueGetterMethod = valueGetterMethod;
        }

        /// <summary>
        /// Gets the key for the value getter.
        /// </summary>
        public Type Key { get; }

        /// <summary>
        /// Gets the function for checking if a type matches the getter.
        /// </summary>
        public Func<Type, bool> TypeMatchCheck { get; }

        /// <summary>
        /// Gets the method for retrieving the value from the matched type.
        /// </summary>
        public ValueGetterDelegate ValueGetterMethod { get; }
    }
}
