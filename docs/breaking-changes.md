# Breaking Changes

## Stubble.Core
- Dictionary value getters are no longer aware of case insensitivity by default.
This is because we don't if the provided dictionary is already case insensitive or not.
Our recommendation is to provide dictionaries with case insensitive settings if you require casing to not matter. 
If you don't have control over the data being provided you can overwrite the default value getters to provide your own taking into account the new `ignoreCase` parameter.
  - It is worth noting that we have special cased dynamic objects in that we will create a case insensitive dictionary for the dynamic values properties when looking up keys. Since this is likely to be very slow we advise against ignoring case unless it is vital on dynamic objects.
- Value getters are now bound to a delegate interface instead of Func which allows better naming at the same time we added a `ignoreCase` parameter which will need to be handled or ignored in the value getter implementation itself.