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

            bool boolValue;
            var parseResult = bool.TryParse(value.ToString(), out boolValue) ? (bool?)boolValue : null;
            
            if (parseResult.HasValue || value is bool)
            {
                return parseResult ?? (bool)value;
            }

            if (value is string)
            {
                return !string.IsNullOrEmpty((string)value);
            }

            if (value is IEnumerable)
            {
                return ((IEnumerable)value).GetEnumerator().MoveNext();
            }

            return true;
        }

        internal static bool IsArray(object value)
        {
            return value is Array;
        }
    }
}
