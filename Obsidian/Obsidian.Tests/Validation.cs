using NUnit.Framework;
using Obsidian.Loaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class Validation
    {
        [Test]
        public void ValidateInvalidTemplate()
        {
            var environment = new JinjaEnvironment();
            var result = environment.ValidateTemplateFromString("{% if a is asdf %}{% endif %}");
            Assert.AreEqual(false, result);
        }
        [Test]
        public void ValidateValidTemplate()
        {
            var environment = new JinjaEnvironment();
            var result = environment.ValidateTemplateFromString("{% if a is divisibleby 3 %}{% endif %}");
            Assert.AreEqual(true, result);
        }

        [Test]
        public void ValidateInvalidVariables()
        {
            var templateString = "{{ a.ToString(\"O\") }}";
            var vars = new Dictionary<string, object?>
            {
                { "a", new object() }
            };
            var environment = new JinjaEnvironment();
            //var result = environment.ValidateTemplateFromString(templateString);
            //Assert.AreEqual(true, result);
            var template = environment.FromString(templateString);
            var result = template.Validate(vars);
            Assert.AreEqual(false, result);
        }
        [Test]
        public void ValidateValidVariables()
        {
            var templateString = "{{ a.ToString(\"O\") }}";
            var vars = new Dictionary<string, object?>
            {
                { "a", DateTime.Now }
            };
            var environment = new JinjaEnvironment();
            //var result = environment.ValidateTemplateFromString(templateString);
            //Assert.AreEqual(true, result);
            var template = environment.FromString(templateString);
            var result = template.Validate(vars);
            Assert.AreEqual(true, result);
        }
    }
}
