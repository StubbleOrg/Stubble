using System;
using System.Collections.Generic;
using Stubble.Core.Performance.Data;
using Stubble.Core.Tests;

namespace Stubble.Core.Performance.Candidates
{
    abstract class BaseTestCandidate : ITestCandidate
    {
        protected BaseTestCandidate()
        {
            StronglyTypedTestClass.StaticField = 1;
            StronglyTypedTestClass.StaticProperty = 1;
        }

        public abstract TimeSpan RunTest(int iterations);
        public virtual KeyValuePair<string, object> GetRenderTestCase(int index)
        {
            switch (index%13)
            {
                case 0:
                    return new KeyValuePair<string, object>("{{Foo}}", new {Foo = "Bar"});
                case 1:
                    return new KeyValuePair<string, object>("Just Literal Data", null);
                case 2:
                    return new KeyValuePair<string, object>("Inline Lambda {{Foo}}",
                        new {Foo = new Func<object>(() => "Bar")});
                case 3:
                    return new KeyValuePair<string, object>("Section Lambda {{#Foo}} Section Stuff {{/Foo}}",
                        new {Foo = new Func<string, object>(str => "Bar")});
                case 4:
                    return
                        new KeyValuePair<string, object>(
                            "Section Lambda {{#Foo}} Child Section {{#ChildSection}} Stuff! {{/ChildSection}} {{/Foo}}",
                            new {Foo = new {ChildSection = true}});
                case 5:
                    return new KeyValuePair<string, object>("{{Foo}} New Tags {{=<% %>=}} <% Bar %> ",
                        new {Foo = "Bar", Bar = "Foo"});
                case 6:
                    return new KeyValuePair<string, object>(
                        "{{Foo}} New Tags {{=<% %>=}} <% Bar %> <%=<| |>=%> <|Foo|>", new {Foo = "Bar", Bar = "Foo"});
                case 7:
                    return new KeyValuePair<string, object>("{{Property}}", new StronglyTypedTestClass {Property = 1});
                case 8:
                    return new KeyValuePair<string, object>("{{StaticField}}", new StronglyTypedTestClass());
                case 9:
                    return new KeyValuePair<string, object>("{{StaticProperty}}", new StronglyTypedTestClass());
                case 10:
                    return new KeyValuePair<string, object>("{{Field}}", new StronglyTypedTestClass {Field = 1});
                case 11:
                    return new KeyValuePair<string, object>("{{StaticMethodWithNoArgs}}", new StronglyTypedTestClass());
                case 12:
                    return new KeyValuePair<string, object>("{{^StaticMethodWithArgs}}Stuff{{/StaticMethodWithArgs}}",
                        new StronglyTypedTestClass());
                default:
                    return new KeyValuePair<string, object>("Not Happening", null);
            }
        }
    }
}
