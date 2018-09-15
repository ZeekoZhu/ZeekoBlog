using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using ZeekoBlog.AsciiDoc;

namespace Tests
{
    public class AsciiDocServiceTests
    {
        private AsciiDocService _asciiDocSvc;
        public AsciiDocServiceTests()
        {
            var services = new ServiceCollection();
            services.AddNodeServices();
            services.AddAsciiDoc();
            var provider = services.BuildServiceProvider();
            _asciiDocSvc = provider.GetService<AsciiDocService>();
        }

        [Fact]
        public async Task ItShouldRenderToc()
        {
            const string content =
@"
= Test

== h1

=== h2

==== h3

== 二级标题
";
            var result = await _asciiDocSvc.Process(content);
            result.TableOfContents.Should().HaveCount(4);
            result.TableOfContents
                .Select(x => x.Name)
                .Should().BeEquivalentTo("h1", "h2", "h3", "二级标题");
            result.TableOfContents
                .Select(x => x.Level)
                .Should().BeEquivalentTo(2, 3, 4, 2);
        }

        [Fact]
        public async Task ItShouldRenderSourceCode()
        {
            const string content = @"
[source, fsharp]
----
open System
----

[source, js]
----
console.log(2333)   // <1>
----
";
            var result = await _asciiDocSvc.Process(content);
            result.Languages.Should().BeEquivalentTo("fsharp", "js");
        }
    }
}
