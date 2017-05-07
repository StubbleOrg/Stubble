Template Loading
---
Stubbles template loaders are designed to be an agnostic way of retrieving your templates both synchronously and asynchronously.

The Loaders provided by default are:

 - StringLoader : Simply passes through the given Template without any changes (*Default for TemplateLocator*).
 - DictionaryLoader : Has a static dictionary of templates to lookup by key, this is similar to having a statically shared partials dictionary.
 - CompositeLoader : Takes an array of `IStubbleLoader` and sequentially goes through them until a template is found.
   **NOTE: If no template is found at all an exception is thrown of type UnknownTemplateException**

## Partial Templates
Partials are a special case, you can provide a loader for partial templates when building your `StubbleRenderer` which will be used when looking up a partial template. You can also provide a `Dictionary<string, string>` of partials to the render method. To make this work efficiently we create a new composite loader which will first look in the provided dictionary and then fall back to your default partial loader.

## Asynchronous & Synchronous template lookup
The `IStubbleLoader` provides both `Load` and `LoadAsync` methods of looking up a template. The recommendations of the author are that if you need to do async lookups then you should perform the work once and cache it for a period of times so that most lookups are for the most part synchronous.

To facilitate this we are making use of the new `ValueTask` as it's designed to be used the case where the majority of the results will be synchronous and won't allocate any memory in those cases. You can read more about it [here.](http://blog.i3arnon.com/2015/11/30/valuetask/)

### History
The first alpha versions of Stubble only had synchronous methods of retrieving templates so if you were performing a database lookup for your template for example you would halt the thread so if you were rendering on the UI thread or in a server would halt the request thread.

Since when [dogfooding](https://en.wikipedia.org/wiki/Eating_your_own_dog_food) the library I was doing just that and felt I should adjust the interface to allow asynchronous and synchronous lookup.