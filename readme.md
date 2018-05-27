# Stubble [![Build Status](https://img.shields.io/appveyor/ci/Romanx/stubble-ceybe/master.svg?style=flat-square)](https://ci.appveyor.com/project/Romanx/stubble-ceybe/branch/master) [![Build Status](https://travis-ci.org/StubbleOrg/Stubble.svg?branch=master)](https://travis-ci.org/StubbleOrg/Stubble) [![Coverage Status](https://img.shields.io/coveralls/StubbleOrg/Stubble.svg?style=flat-square)](https://coveralls.io/github/StubbleOrg/Stubble) [![Pre-release Build Nuget](https://img.shields.io/nuget/vpre/Stubble.Core.svg?style=flat-square&label=nuget%20pre)](https://www.nuget.org/packages/Stubble.Core/) [![Stable Nuget](https://img.shields.io/nuget/v/Stubble.Core.svg?style=flat-square)](https://www.nuget.org/packages/Stubble.Core/)

<img align="right" width="160px" height="160px" src="assets/logo-256.png">

### Trimmed down {{mustache}} templates in .NET

Stubble is an implementation of the [Mustache](http://mustache.github.com/) template system in C# (but is usable from any .NET language).

For a language-agnostic overview of mustache's template syntax, see the `mustache(5)` [manpage](http://mustache.github.com/mustache.5.html).

Stubble is tested against the mustache specification and is `v.1.1.2, including lambdas` compliant, this means that your templates in other languages will work with Stubble provided they match the spec!

It is licensed under the MIT License which can be found [here.](/licence.md)

### Why should I use Stubble?

Stubble is designed to be a spec compliant mustache renderer with only the bare essentials, _you could say the rest has been trimmed down!_

Stubble provides no methods of finding your templates, no complicated logic for getting values from your objects or special types, no non-spec tags for rendering or logic and only the necessaries to make it a simple and fast parser and renderer.

### Okay I'm convinced, how do I get it and use it?

At the moment Stubble is in alpha but please feel free to grab it from the pre-release feed on nuget.org by clicking on the badge above! Another option for the more adventurous is to download the source and build it yourself.

For how to use Stubble I'd recommend reading the how to use guide [here.](/docs/how-to.md)

### Performance

We use [BenchmarkDotNet](benchmarkdotnet.org) to optimize our performance to to allow us to compare our _real_ performance against our closest comparable measure which is [Nustache](https://github.com/jdiamond/Nustache/).

Our benchmarks can be found in the repo and we test using the Tweet benchmarks found in the [Mustache.java](https://github.com/spullara/mustache.java) repository that we have implemented in C#. We've tried to be as fair as possible giving each their optimal scenario so we can focus on raw numbers.

The test itself measures how long it takes to render a timeline of tweets with partials, inverted sections default values and missing data.

![image](/docs/Benchmarks.png)

The numbers here represent the baseline values graphed from the timeline test with warm-ups and outliers removed, please feel free to checkout the repository and run the benchmarks to verify the results or if there's a better way to benchmark the library, we're always open to improvements.

### Extensibility

Stubble exposes certain internal structures for parsing and rendering extensions to be added in a loosely coupled way.
These extensions can be added on to the `IRendererSettingsBuilder` as extension methods to simplify it for users.

For more detail on the types of Extensibility and how to extend stubble please see the [extensibility docs here.](/docs/extensibility.md).

### Compilation

Stubble provides compilation of templates to functions that take strongly typed arguments based on how you configure the stubble template compiler.
To use compilation, simple create a `StubbleCompilationRenderer` and call Compile or Compile async after configuring it.

For more detailed information and edgecases please see the [compilation docs here.](/docs/compilation.md).

### Template Loading

Stubble comes in the box with very few template loaders but provides an interface and extension points which allow you to provide your own async and sync methods to get templates from a given name or use one that has already been created in a separate package.

The implementation of this feature is heavily inspired by [bobthecow's](https://github.com/bobthecow/) implementation of loaders in [mustache.php](https://github.com/bobthecow/mustache.php/).

For more detail on template loading please see the [template loading docs here.](/docs/template-loading.md).

### Why not use [Nustache](https://github.com/jdiamond/Nustache/) instead?

If Stubble doesn't do what you need then you should use Nustache! It's a great tool and provides the same base functionality as Stubble provides for the default Mustache spec _(I know because I'm a contributor and current maintainer of that project!)_.

However it does provide lots of extra features, such as a variety of input types, helpers and compilation which increases its complexity and some which are extensions to the Mustache spec (such as helpers). If you need any of those pieces of functionality I'd highly recommend you use Nustache... at least until there are Extensions for Stubble which provide the functionality your after!

## Credits

Straight Razor by Vectors Market from the Noun Project
