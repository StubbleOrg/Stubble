Extensibility
---

### Value Lookup
You can add your own ValueGetters for mapping how to retrieve values from given types by a key. Stubble provides a set of defaults for:

- `IDictionary<string, object>`
- `IDictionary`
- `object`

You have to register a Func mapping a Type which is used during value discovery. The Func has to be the type `Func<string, object, object>(key, value) => object`

### Truthiness Evaluation
Stubble performs truthy checks to determine if a section should be rendered (or if you're using an inverted tag should not be rendered). Defaults are provided for all common falsey types however these don't cover all possibilities that could be added using the ValueGetter extensions above.

The solution to this is that you can provide check functions which are ran through sequentially until a true or false value is returned. To pass on the check (as the value isn't applicable for your check) a null should be returned.

An example of why is is required is a truthy check on `DataTable` would be that it has a `MyDataTable.Rows.Count > 0` to be truthy as it does not implement `IEnumerable`. See Nustache for this [specific bug](https://github.com/jdiamond/Nustache/issues/92).

### Renderer Extensions
Stubble uses a visitor pattern based rendering system with registered tag renderers which determine how the given tag will be rendered.

This allows for a distinct separation of the parsing and rendering steps, making it easier to cache and extend with new renderers without having to change the parser.

When adding a new renderer the advice is to add a new `IStubbleBuilder` (the easiest way is to inherit from `StubbleBuilder`) and returns your renderer.
More details on how to create this are in the section on builder extensions below.

The place to start when looking at adding a new renderer are the `StubbleVisitorRenderer` and classes that perform the rendering `StringRender`, `RendererBase` and `MustacheTokenRenderer`.

### Builder Extensions
Creating a new builder allows you to add build methods relating to your renderer that the user can set using the existing fluent interface.

`IStubbleBuilder<T>` has a method `SetBuilderType` which creates an instance of the new builder and copies over the current settings.
We recommend adding an extension method to simplify this for the user, something like.

```c#
public static class CustomBuilderExtensions
{
    public static CustomBuilder SetCustomBuilder<T>(this IStubbleBuilder<T> builder)
    {
        return builder.SetBuilderType<CustomBuilder>();
    }
}
```

The user can then use your builder in place of the standard builder without having to learn a new syntax and can discover overloads in the existing way.

```c#
var stubble = new StubbleBuilder()
    .SetCustomBuilder()
    .DoCustomAction("Custom Setting")
    .Build();
```

The best classes to give you an idea of where to begin are `StubbleBuilder` and `StubbleBuilder<T>`

### Tag Parsing Extensions
Tag parsing is performed in a very explicit method with a low garbage collection overhead for optimal performance. This creates code that is very verbose and lengthily however allows flexibility and performance.

New tag parsers are added to the `ParserPipelineBuilder` as either `InlineParsers` or `BlockParsers`.
These are handled in order and so if you're replacing a tag parser it's recommended to remove the existing parser before hand. If you're adding a new tag make sure the syntax doesn't collide with the opening syntax of any existing tags or that it comes before that tags parser.

The built pipeline should be cached if it needs to be reused and can be passed in as an overload into the static `MustacheParser` class if all you want is to parse the tags into an AST.

`IRendererSettingsBuilder` contains a `SetParserPipeline(ParserPipeline)` method which allows the user to set a specific pipeline to be used during parsing allowing new tag parsers to be added during setup.

The best classes to look at to give you an idea of where to begin are `CommentTagParser` and `InlineParser`

### Rendering
If you're adding a new tag you probably want that tag to be rendered. To do that you're going to need to add a new `MustacheTokenRenderer` for the specific renderer you want to render for. Confused? Yeah I'm terrible at naming things.

- RendererBase (`StringRender`)
    - MustacheTokenRenderer (`StringObjectRenderer : MustacheTokenRenderer<StringRender, TToken>`)

The top level `RendererBase` defines that this is a class that can render mustache tags in some way, that can be to a string or to a compiled function.

The `MustacheTokenRenderer`'s are implementations for a specific renderer of how to render a given tag for that renderer. This allows for explicit definitions about how a tag is rendered for a specific renderer base.

#### Tag Rendering Extensions
At the simplest level to write a tag renderer you need to implement `ITokenRenderer` however there are some more explicit classes that can help you such as `StringObjectRenderer<T>` which implies to the renderer that you implement rendering to a string for the tag `T` and that implies the type of the renderer will be `StringRender`.

`IRenderSettingsBuilder` has an OrderedList of TokenRenderers allow you to add new renderers for your tags, this is not a particularity intuitive interface currently however it's something we hope to improve over time.