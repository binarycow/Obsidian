using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Obsidian.Tests.FromJinja.CoreTags
{
    public class ForTests
    {
        [SetUp]
        public void CreateEnvironment()
        {
            _Environment = new JinjaEnvironment(settings: new EnvironmentSettings
            {
                TrimBlocks = true
            });
        }
        private JinjaEnvironment _Environment = new JinjaEnvironment();
    }
}
