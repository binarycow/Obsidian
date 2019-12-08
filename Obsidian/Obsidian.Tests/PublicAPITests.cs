using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using Obsidian.Templates;
using Obsidian.TestCore;

namespace Obsidian.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated public classes", Justification = "<Pending>")]
    public class PublicAPITests
    {
        private class MyParameterInfo
        {
            public string? Name { get; set; }
            public Type? Type { get; set; }
        }
        private class MyOverloadInfo
        {
            public Type? ReturnType { get; set; }
            public MyParameterInfo[]? Parameters { get; set; }
        }

        private class MyMethodInfo
        {
            public string? Name { get; set; }
            public MyOverloadInfo[]? Overloads { get; set; }
        }

        private class MyTypeInfo
        {
            public string? FullName { get; set; }
            public MyMethodInfo[]? Methods { get; set; }
            public MyPropertyInfo[]? Properties { get; set; }
            public MyFieldInfo[]? Fields { get; set; }
        }

        private class MyPropertyInfo
        {
            public string? Name { get; set; }
            public Type? Type { get; set; }
        }
        private class MyFieldInfo
        {
            public string? Name { get; set; }
            public Type? Type { get; set; }
        }

        private MyParameterInfo GetParameterInfo(ParameterInfo parameter)
        {
            return new MyParameterInfo
            {
                Name = parameter.Name,
                Type = parameter.ParameterType
            };
        }
        private MyOverloadInfo GetOverloadInfo(MethodInfo overload)
        {
            return new MyOverloadInfo
            {
                ReturnType = overload.ReturnType,
                Parameters = overload.GetParameters().Select(GetParameterInfo).ToArray()
            };
        }

        private MyMethodInfo GetMethodInfo(IGrouping<string, MethodInfo> overloads)
        {
            return new MyMethodInfo
            {
                Name = overloads.Key,
                Overloads = overloads.Select(GetOverloadInfo).ToArray()
            };
        }

        private MyMethodInfo[] GetMethodInfo(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName)
                .GroupBy(method => method.Name)
                .Select(GetMethodInfo)
                .ToArray();
        }
        private MyPropertyInfo GetPropertyInfo(PropertyInfo property)
        {
            return new MyPropertyInfo
            {
                Name = property.Name,
                Type = property.PropertyType
            };
        }
        private MyPropertyInfo[] GetPropertyInfo(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(GetPropertyInfo)
                .ToArray();
        }
        private MyFieldInfo GetFieldInfo(FieldInfo property)
        {
            return new MyFieldInfo
            {
                Name = property.Name,
                Type = property.FieldType
            };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private MyFieldInfo[] GetFieldInfo(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Select(GetFieldInfo)
                .ToArray();
        }

        private MyTypeInfo GetTypeInfo(Type type)
        {
            
            return new MyTypeInfo()
            {
                FullName = type.FullName,
                Methods = GetMethodInfo(type),
                Properties = GetPropertyInfo(type)
            };
        }

        [Test]
        public void TestAPI()
        {
            var actualPath = TestRunner.APIInfoActual;
            var results = typeof(CompiledTemplate).Assembly.GetTypes().Where(type => type.IsPublic || type.IsNestedPublic).Select(GetTypeInfo).ToArray();
            File.WriteAllText(actualPath, JsonConvert.SerializeObject(results, Formatting.Indented));
            Assert.Fail();
        }
    }
}
