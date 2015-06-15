using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stubble.Core.Helpers
{
    internal static class ValueHelpers
    {
        internal static bool IsTruthy(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is bool)
            {
                return (bool)value;
            }

            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }

            if (value is IEnumerable)
            {
                return ((IEnumerable<object>) value).Any();
            }

            return true;
        }

        internal static bool IsArray(object value)
        {
            return value is Array;
        }
    }
}
