using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Obsidian.Tests.Utilities;

namespace Obsidian.Tests
{
    public class PublicAPITests
    {
        private class TypeInfo
        {
            public string? FullName { get; set; }
        }

        private TypeInfo GetTypeInfo(Type type)
        {
            return new TypeInfo()
            {
                FullName = type.FullName
            };
        }

        [Test]
        public void TestAPI()
        {
            var actualPath = TestRunner.APIInfo_Actual;
            var results = typeof(Template).Assembly.GetTypes().Select(GetTypeInfo).ToArray();
            File.WriteAllText(actualPath, JsonConvert.SerializeObject(results));
        }
    }
}
