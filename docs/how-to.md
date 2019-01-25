# How to use

Stubble follows the mustache spec explicitly so anything that works in other spec compliant mustache libraries such as [mustache.js](https://github.com/janl/mustache.js), [mustache.java](https://github.com/spullara/mustache.java) and [mustache.php](https://github.com/bobthecow/mustache.php) should work without any issues.

The full spec can be found [here.](mustache.github.io/mustache.5.html)

## Stubble Specifics

Stubble is designed to be as slim as possible providing almost no utility functions for finding templates or converting your data into the right format. That is left up to the user however extension packages will be provided in future for common use cases such as Json.Net and System.Data.

### Standard Example Usecase

```csharp
using Stubble.Core.Builders;
using System.IO;
using System.Text;

// Sync
public void SyncRender() {
  var stubble = new StubbleBuilder().Build();
  var myObj = new MyObj();
  using (StreamReader streamReader = new StreamReader(@".\Path\To\My\File.Mustache", Encoding.UTF8))
  {
      var output = stubble.Render(streamReader.ReadToEnd(), myObj);
      // Do Stuff
  }
}

// Async
public async Task SyncRender() {
  var stubble = new StubbleBuilder().Build();
  var myObj = new MyObj();
  using (StreamReader streamReader = new StreamReader(@".\Path\To\My\File.Mustache", Encoding.UTF8))
  {
      var content = await streamReader.ReadToEndAsync();
      var output = await stubble.RenderAsync(content, myObj);
      // Do Stuff
  }
}
```

OR

```csharp
var stubble = new StubbleBuilder().Build();
Dictionary<string, object> dataHash = GetMyData();

//Sync
var output  = stubble.Render("{{Foo}}", dataHash);

// Async
var output  = await stubble.RenderAsync("{{Foo}}", dataHash);
```

It's as simple as that.

### Configuration

To configure your stubble instance you can provide a configuration function in which you set any specifics you like.
An example of this is below:

```csharp
var stubble = new StubbleBuilder()
  .Configure(settings => {
    settings.IgnoreCaseOnKeyLookup = true;
    settings.MaxRecursionDepth = 512;
    settings.AddJsonNet(); // Extension method from extension library
  })
  .Build();

Dictionary<string, object> dataHash = GetMyData();
var output  = stubble.Render("{{Foo}}", dataHash);
```

### Lambdas

> **Note:** These details only affect the Stubble.Core renderer and not the compilation renderer since compilation does not currently support lambdas.

Stubble implements the mustache language extension for Lambdas which are anonymous functions which you can use to in your templates.
There are two types of Lambdas in Stubble **Tag Lambdas** and **Section Lambdas**.

If the dynamic argument is defined, it is the context of the function much like using "this" however since the compiler can't know what you mean in your function the dynamic argument is used to be interrogated at runtime.

#### Interpolation: or how I learnt to return tags and love them

If you return tags from your lambda they will be expanded before being rendered. This way you you can build new templates to return from your templates. _My head hurts..._

#### Tag Lambdas

Tag lambdas are rendered in place of the tag in the template. They have to be the type `Func<dynamic, object>` or `Func<object>`

**Example**

```csharp
var obj = new {
   Bar = "Bar",
   Foo = new Func<dynamic, object>((dyn) => { return dyn.Bar; })
};

stubble.Render("{{Foo}}", obj); // Outputs: "Bar"
```

#### Section Lambdas

Section lambdas are used to wrap sections of a template. The contents of the section is passed into the lambda as an argument. They have to be of the type `Func<string, dynamic, object>` or `Func<string, object>`

**Example**

```csharp
var obj = new {
   Bar = "Bar",
   Foo = new Func<dynamic, string, object>((dyn, str) => { return str.Replace("World", dyn.Bar); })
};

stubble.Render("{{Foo}} Hello World {{/Foo}}", obj); //Outputs: " Hello Bar "

var obj2 = new {
   Bar = "Bar",
   Foo = new Func<string, object>((str) => { return "Foo {{Bar}}"; })
};

stubble.Render("{{Foo}} Hello World {{/Foo}}", obj2); //Outputs: "Foo Bar"
```
