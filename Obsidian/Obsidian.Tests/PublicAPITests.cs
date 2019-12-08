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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "<Pending>")]
    internal class PublicAPITests
    {
        private class MyParameterInfo
        {
            internal string? Name { get; set; }
            internal Type? Type { get; set; }
        }
        private class MyOverloadInfo
        {
            internal Type? ReturnType { get; set; }
            internal MyParameterInfo[]? Parameters { get; set; }
        }

        private class MyMethodInfo
        {
            internal string? Name { get; set; }
            internal MyOverloadInfo[]? Overloads { get; set; }
        }

        private class MyTypeInfo
        {
            internal string? FullName { get; set; }
            internal MyMethodInfo[]? Methods { get; set; }
            internal MyPropertyInfo[]? Properties { get; set; }
            internal MyFieldInfo[]? Fields { get; set; }
        }

        private class MyPropertyInfo
        {
            internal string? Name { get; set; }
            internal Type? Type { get; set; }
        }
        private class MyFieldInfo
        {
            internal string? Name { get; set; }
            internal Type? Type { get; set; }
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
        internal void TestAPI()
        {
            var actualPath = TestRunner.APIInfoActual;
            var results = typeof(CompiledTemplate).Assembly.GetTypes().Where(type => type.IsPublic || type.IsNestedPublic).Select(GetTypeInfo).ToArray();
            File.WriteAllText(actualPath, JsonConvert.SerializeObject(results, Formatting.Indented));
            Assert.Fail();
        }
    }
}
