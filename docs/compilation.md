# Compilation

As of alpha-17 stubble now has a compilation renderer which will take a given template and argument and compile a function. The function when called called with parameters (or not) will return the result of the mustache render.

```csharp
var stubble = new StubbleCompilationBuilder().Build();
MyData data = GetMyData();

// Sync
Func<MyData, string> renderFunc  = stubble.Compile("{{Foo}}", data);

// Async
Func<MyData, string> renderFunc = stubble.CompileAsync("{{Foo}}", data);

// Later
var builder = new StringBuilder();
foreach(MyData dataItem in LoadLotsOfData())
{
    builder.Append(renderFunc(dataItem));
}

return builder.ToString();
```

The compilation here will only be done once and all of the rendering will be done per data item.
Any conditional checks in templates will be done on a case by case basis.
Compilation is much faster than runtime rendering however the upfront cost is more and there are some caveats to its usage which are detailed below.
That being said if you can use compilation since you have simple templates or templates with no complicated logic then compilation can heavily speed up your rendering.

### Note for Nustache users

If you're coming from using Nustache you may have run into the fact you can't use recursive partials and you're not able to use the same partial twice in the same template.
Stubble has neither of these limitations so feel free to use them.

## Caveats

There are two caveats to the compilation renderer.

1. The first is that it does not support lambda functions since we're not entirely sure of how we should handle rendering the returned tags.
   This will be decided at a later date if we decide to support it.

2. The second is a limitation around `section` and `inverted section` blocks.
   Due to the way compilation is done if a partial tag is called inside more than 16 block scopes then an exception will be thrown.
   This is due to the way the values of propagated down through the scopes and into the partial call.

3. Dynamic properies have limitations including that you can't use ignore case when looking up properties and it will throw at runtime if a missing property is found.
