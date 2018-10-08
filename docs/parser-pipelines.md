## Parser Pipelines

The parser pipelines defines which inline and block parsers will be run across the input generating tags from their output.

You may want to add custom tags to your output or remove existing ones if you don't want to use that functionality.

We provide no method of building a parser pipeline yourself since we wanted to provide helpers so that users could easily fall into the best patterns.

With this in mind you build parser pipelines using the `Parser Pipline Builder` which has some helper methods for replacing or adding parsers before or after other parsers.

```c#
var builder = new ParserPipelineBuilder();
builder.Replace<InterpolationTagParser>(new MyCustomParser());
// OR
builder.Remove<InterpolationTagParser>();
// OR
builder.AddAfter<InterpolationTagParser>(new MyCustomParser());
builder.AddBefore<InterpolationTagParser>(new MyCustomParser());
var pipeline = builder.Build();
```

These methods will work regardless of the type of parser you provide it and will correctly adapt the underlying parser listings.