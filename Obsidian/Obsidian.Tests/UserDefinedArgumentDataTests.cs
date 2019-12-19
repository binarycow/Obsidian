using System;
using System.Collections.Generic;
using System.Text;
using ExpressionParser;
using NUnit.Framework;
using System.Linq;
using ExpressionParser.Configuration;
using ExpressionParser.Exceptions;

namespace Obsidian.Tests
{
    public class UserDefinedArgumentDataTests
    {

        [Test]
        public void TestNoArguments()
        {
            var result = UserDefinedArgumentData.Create(JinjaLanguageDefinition.Instance,
                Array.Empty<ParameterDeclaration>(),
                Array.Empty<object?>()
            );
            PerformTest(result, Array.Empty<(string Name, object? Value, bool Provided)>());
        }

        [Test]
        public void TestAllRequiredAndProvided()
        {
            var result = UserDefinedArgumentData.Create(JinjaLanguageDefinition.Instance,
                new[]
                {
                    new ParameterDeclaration("a"),
                    new ParameterDeclaration("b"),
                    new ParameterDeclaration("c"),
                },
                new object?[] {
                    123,
                    456,
                    789
                }
            );
            PerformTest(result, new (string Name, object? Value, bool Provided)[] {
                (Name: "a", Value: 123, Provided: true),
                (Name: "b", Value: 456, Provided: true),
                (Name: "c", Value: 789, Provided: true),
            });
        }


        [Test]
        public void TestSkippingAnOptional()
        {
            var result = UserDefinedArgumentData.Create(JinjaLanguageDefinition.Instance,
                new []
                {
                    new ParameterDeclaration("name"),
                    new ParameterDeclaration("value", null),
                    new ParameterDeclaration("type", "text"),
                },
                new object?[] {
                    "password",
                    ("type", "password")
                }
            );
            PerformTest(result, new (string Name, object? Value, bool Provided)[] {
                (Name: "name", Value: "password", Provided: true),
                (Name: "value", Value: null, Provided: false),
                (Name: "type", Value: "password", Provided: true),
            });
        }



        private void PerformTest(UserDefinedArgumentData arguments, (string Name, object? Value, bool Provided)[] expectedDefinedPositional)
        {
            var actualDefinedPositional = arguments.DefinedPositionalArguments.ToArray();
            Assert.AreEqual(expectedDefinedPositional.Length, actualDefinedPositional.Length);
            for(int i=0;i<actualDefinedPositional.Length;++i)
            {
                Assert.AreEqual(expectedDefinedPositional[i].Name, actualDefinedPositional[i].Name);
                Assert.AreEqual(expectedDefinedPositional[i].Value, actualDefinedPositional[i].Value);
                Assert.AreEqual(expectedDefinedPositional[i].Provided, actualDefinedPositional[i].Provided);
            }
        }


    }
}
