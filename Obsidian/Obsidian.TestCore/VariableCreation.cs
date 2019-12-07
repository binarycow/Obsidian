using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common;
using System.Collections;

namespace Obsidian.TestCore
{
    internal static class VariableCreation
    {
        public static Dictionary<string, object?> GetVariables(string filename)
        {
            var fileText = File.ReadAllText(filename);
            if (!(JsonConvert.DeserializeObject(fileText) is JObject x))
            {
                throw new NotImplementedException();
            }
            var obj = ToObject(x);
            if(obj is Dictionary<string, object?> objDict)
            {
                return objDict;
            }
            objDict = new Dictionary<string, object?>();

            var keysProperty = obj.GetType().GetProperty("Keys");
            var indexer = obj.GetType().GetProperty("Item");
            var keys = keysProperty?.GetValue(obj);
            if(!(keys is IEnumerable enumerableKeys))
            {
                throw new NotImplementedException();
            }
            foreach(var keyObj in enumerableKeys)
            {
                var key = keyObj.ToString();
                var value = indexer.GetValue(obj, new object[] { key });
                objDict.Add(key, value);
            }
            return objDict;

        }

        internal static object ToObject(JObject jsonObject)
        {
            var children = jsonObject.Children().Select(child =>
            {
                if (child is JProperty jProperty)
                {
                    return new
                    {
                        jProperty.Name,
                        Value = ToObject(jProperty.Value),
                    };
                }
                throw new NotImplementedException();
            }).ToArray();

            var commonBaseType = ReflectionHelpers.GetCommonBaseClass(children.Select(child => child.Value?.GetType()).NonNullItems());

            var dictionaryType = typeof(Dictionary<,>);
            var genericType = dictionaryType.MakeGenericType(typeof(string), commonBaseType);
            
            
            var returnObject = Activator.CreateInstance(genericType);
            if(returnObject == null)
            {
                throw new NotImplementedException();
            }
            var addMethod = genericType.GetMethod("Add");
            if(addMethod == null)
            {
                throw new NotImplementedException();
            }
            foreach(var child in children)
            {
                addMethod.Invoke(returnObject, new object[] { child.Name, child.Value });
            }
            return returnObject;
        }



        public static object? ToObject(JToken token)
        {
            return token switch
            {
                JArray arry => ToObject(arry),
                JObject jobj => ToObject(jobj),
                JValue value => ToObject(value),
                _ => throw new NotImplementedException(),
            };
        }

        public static object ToObject(JArray array)
        {
            array = array ?? throw new ArgumentNullException(nameof(array));
            var childrenObjects = array.Children().Select(child => ToObject(child)).ToArray();
            var baseType = ReflectionHelpers.GetCommonBaseClass(childrenObjects.NonNullItems().Select(obj => obj.GetType()));


            var listType = typeof(List<>);
            var genericType = listType.MakeGenericType(baseType);

            var retVal = Activator.CreateInstance(genericType);
            if(retVal == null)
            {
                throw new NotImplementedException();
            }

            var addMethod = genericType.GetMethod("Add");
            if (addMethod == null)
            {
                throw new NotImplementedException();
            }
            foreach (var child in childrenObjects)
            {
                addMethod.Invoke(retVal, new object?[] { child });
            }
            return retVal;
        }

        public static object? ToObject(JValue value)
        {
            if (value == null) return null;
            return value.Type switch
            {
                JTokenType.String => value.ToObject<string>(),
                JTokenType.Integer => value.ToObject<int>(),
                JTokenType.Boolean => value.ToObject<bool>(),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
