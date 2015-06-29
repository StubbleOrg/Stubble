using System;
using System.Collections.Generic;

namespace Stubble.Core.Classes
{
    internal class TypeBySubclassAndAssignableImpl : Comparer<Type>
    {
        public static IComparer<Type> TypeBySubclassAndAssignable()
        {
            return new TypeBySubclassAndAssignableImpl();
        }

        public override int Compare(Type x, Type y)
        {
            //Standard Comparision Checks
            if (x == null && y == null)
                return 0;
            if (x == null)
                return 1; // x is after y
            if (y == null)
                return -1; // x is before y
            if (x == y)
                return 0;

            var result = 0;
            if (x.IsSubclassOf(y))
            {
                result = -1;
            }
            else if (y.IsSubclassOf(x))
            {
                result = 1;
            }
            else if (x.IsAssignableFrom(y))
            {
                result = 1;
            }
            else if (y.IsAssignableFrom(x))
            {
                result = -1;
            }

            return result;
        }
    }
}
