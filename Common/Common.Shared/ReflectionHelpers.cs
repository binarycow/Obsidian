using ExpressionParser;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common
{
    public static class ReflectionHelpers
    {
        public static Type GetCommonBaseClass(IEnumerable<Type> types)
        {
            return GetCommonBaseClassArr(types.ToArray());
        }
        private static Type GetCommonBaseClassArr(Type[] types)
        {
            if (types.Length == 0)
                return (typeof(object));
            else if (types.Length == 1)
                return (types[0]);

            // Copy the parameter so we can substitute base class types in the array without messing up the caller
            Type[] temp = new Type[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                temp[i] = types[i];
            }

            bool checkPass = false;

            Type tested = temp[0];

            while (!checkPass)
            {
                tested = temp[0];

                checkPass = true;

                for (int i = 1; i < temp.Length; i++)
                {
                    if (tested.Equals(temp[i]))
                        continue;
                    else
                    {
                        // If the tested common basetype (current) is the indexed type's base type
                        // then we can continue with the test by making the indexed type to be its base type
                        if (tested.Equals(temp[i].BaseType))
                        {
                            temp[i] = temp[i].BaseType;
                            continue;
                        }
                        // If the tested type is the indexed type's base type, then we need to change all indexed types
                        // before the current type (which are all identical) to be that base type and restart this loop
                        else if (tested.BaseType.Equals(temp[i]))
                        {
                            for (int j = 0; j <= i - 1; j++)
                            {
                                temp[j] = temp[j].BaseType;
                            }

                            checkPass = false;
                            break;
                        }
                        // The indexed type and the tested type are not related
                        // So make everything from index 0 up to and including the current indexed type to be their base type
                        // because the common base type must be further back
                        else
                        {
                            for (int j = 0; j <= i; j++)
                            {
                                temp[j] = temp[j].BaseType;
                            }

                            checkPass = false;
                            break;
                        }
                    }
                }

                // If execution has reached here and checkPass is true, we have found our common base type, 
                // if checkPass is false, the process starts over with the modified types
            }

            // There's always at least object
            return tested;
        }

        internal static MethodInfo? GetCallable(object? left)
        {
            left = left ?? throw new ArgumentNullException(nameof(left));
            var allMethods = left.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var callable = allMethods.FirstOrDefault(method => method.GetCustomAttribute<CallableAttribute>(true) != null);
            return callable;
        }

        internal static bool TryGetIDictionary(object item, out IEnumerable<KeyValuePair<object, object?>> dictionary)
        {
            dictionary = Enumerable.Empty<KeyValuePair<object, object?>>();
            var type = item?.GetType() ?? typeof(object);

            if (type.IsAssignableToGenericType(typeof(IDictionary<,>), out var _) == false) return false;

            var keys = type.GetProperty("Keys").GetValue(item);
            var indexer = type.GetProperty("Item");
            if (TryGetIEnumerable(keys, out var keyObjects) == false || keyObjects == null) return false;

            var list = new List<KeyValuePair<object, object?>>();

            foreach(var key in keyObjects)
            {
                if (key == null) continue;
                list.Add(new KeyValuePair<object, object?>(key, indexer.GetValue(item, new[] { key })));
            }
            dictionary = list;
            return true;
        }

        internal static bool TryGetIEnumerable(object? enumerableObject, out IEnumerable<object?> items)
        {
            items = Enumerable.Empty<object?>();
            var type = enumerableObject?.GetType() ?? typeof(object);
            if (type.IsAssignableToGenericType(typeof(IEnumerable<>), out var _) == false) return false;
            var list = new List<object?>();

            var enumerator = type.GetMethod("GetEnumerator").Invoke(enumerableObject, Array.Empty<object>());
            var moveNextMethod = enumerator.GetType().GetMethod("MoveNext");
            var currentProperty = enumerator.GetType().GetProperty("Current");

            while(Convert.ToBoolean(moveNextMethod.Invoke(enumerator, Array.Empty<object>()), CultureInfo.InvariantCulture))
            {
                list.Add(currentProperty.GetValue(enumerator));
            }

            items = list;
            return true;
        }

        internal static bool IsTuple(object? obj, out PropertyInfo[] tupleProperties)
        {
            tupleProperties = Array.Empty<PropertyInfo>();
            var type = obj?.GetType() ?? typeof(object);
            if (type.Namespace != "System" || type.Name.StartsWith("Tuple`", StringComparison.InvariantCulture) == false) return false;
            if (int.TryParse(type.Name.Replace("Tuple`", ""), out var tupleLength) == false) return false;

            tupleProperties = Enumerable.Range(0, tupleLength).Select(arrayIndex =>
            {
                var itemNumber = arrayIndex + 1;
                var property = type.GetProperty($"Item{itemNumber}");
                if (property == default) throw new NotImplementedException();
                return property;
            }).ToArray();
            return true;
        }


        internal static object MakeGenericList(IEnumerable<object?> listItems)
        {
            listItems = listItems ?? throw new ArgumentNullException(nameof(listItems));

            var listItemArray = listItems.ToArray();
            var types = listItemArray.Select(listItem => listItem?.GetType() ?? typeof(object));
            var commonType = GetCommonBaseClass(types);

            var openGenericListType = typeof(List<>);
            var closedGenericListType = openGenericListType.MakeGenericType(commonType);

            var list = Activator.CreateInstance(closedGenericListType);

            var addMethod = closedGenericListType.GetMethod(nameof(List<int>.Add));
            foreach (var item in listItemArray)
            {
                addMethod.Invoke(list, new object?[] { item });
            }
            return list;
        }
        internal static object MakeGenericTuple(IEnumerable<object?> tupleItems)
        {

            tupleItems = tupleItems ?? throw new ArgumentNullException(nameof(tupleItems));

            var tupleItemsArray = tupleItems.ToArray();

            if (tupleItemsArray.Length < 1 || tupleItemsArray.Length > 7) throw new NotImplementedException();

            var types = tupleItemsArray.Select(listItem => listItem?.GetType() ?? typeof(object)).ToArray();



            var openGenericType = typeof(Tuple<string, string>).Assembly.GetType($"System.Tuple`{tupleItemsArray.Length}");
            var closedGenericType = openGenericType.MakeGenericType(types);

            var constructor = closedGenericType.GetConstructor(types);
            return constructor.Invoke(tupleItemsArray);
        }

        internal static object?[] Unpack(object? packedObject, int expectedItemCount)
        {
            if (expectedItemCount == 1) return new object?[] { packedObject };
            if (packedObject == null) throw new NullReferenceException();

            var array = CollectionEx.ToArray(packedObject);


            if (array != null)
            {
                if(array.Length < expectedItemCount)
                {
                    throw new RuntimeException($"Not enough values to unpack (expected {expectedItemCount}, got {array.Length})");
                }
                if(array.Length > expectedItemCount)
                {
                    throw new RuntimeException($"Too many values to unpack (expected {expectedItemCount}, got {array.Length})");
                }
                return array;
            }

            throw new NotImplementedException();
        }

    }
}
