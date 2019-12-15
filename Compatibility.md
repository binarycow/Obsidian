# Compatibility

This document aims to cover the compatibility of Jinja2Sharp vs. the python library Jinja2.

I do not intend at this point to expose the API the same way as Jinja2.  If someone is interested in that level of customization, they would need to fork the library.  I do plan, however, on allowing for the capability of extensions, but the implementation of this has yet to be determined.

## Legend

- ✅ 🡺 The feature is fully supported
- ℹ️ 🡺 The feature is generally supported, but there are some differences to take into account
- ⚠️ 🡺 There is limited support for the feature.  Results may be unexpected.
- ❌ 🡺 The feature is not supported.

----

## Core Features

#### ℹ️ Variables 

Jinja2Sharp supports member access using the same syntax as C#.  

For properties, fields, or methods, you must use the following syntax:

`{{ foo.bar }}`

For indexers (such as dictionaries or arrays), you can use either of the following:

`{{ foo["bar"] }}`

`{{ foo.bar }}`


#### ❌ Tests 

Tests are not currently implemented.

#### ✅ Comments 

Comments are fully supported.

#### ℹ️ WhiteSpace Control 

WhiteSpace Control works for the most part, but there may be some control structures or cases where I did not properly account for it.

#### ✅ Escaping 

`raw` blocks are fully supported

#### ❌ Line Statements 

Line Statements are not currently implemented.

#### ℹ️ Manual HTML Escaping 

I have only tested the following notation:

`{{ user.username|e }}`

In theory, the following should be acceptable, but I have not yet tested it:

`{{ e(user.username) }}`

#### ❌ Automatic HTML Escaping 

----

## ✅ Template Inheritance 

Inheritence works... but extra output is displayed...  This is the feature I'm currently working on.

#### ✅ Super Blocks 

#### ✅ Named Block End-Tags 

#### ❌ Block Scope 

#### ❌ Template Objects

----

## Control Structures

### For

ℹ️ The `For` block requires that the expression evaluate to one of the following: 

- `Array`
- `IEnumerable`
- Any object that has a method named `GetEnumerator`, with that returns an object that:
    - has a method named `MoveNext`, that takes no parameters, that also returns a `bool`
    - has a property named `Current`, that returns on `object`

❌ Specifying more than one variable name is not supported.

✅ The `elif` branch is supported

⚠️ Only the following properties of the `loop` variable are implemented.

- TODO

❌ Cycling

### ✅ If

### ✅ Macros

### ✅ Call

### ✅ Assignments

### ✅ Block Assignments

### ✅ Include

### ❌ Import

----

## Expressions

### Literals

- ℹ️ **String Literals** - Use the C# syntax for string or character literals.
- ✅ **Numeric Literals** - Numeric literals follow the C# syntax
- ✅ **List Literals** - Not supported
- ✅ **Tuple Literals** - Not supported
- ✅ **Dict Literals** - Not supported
- ✅ **Boolean Literals** - the values `true` and `True` can be used for a true value, and `false` and `False` can be used for a false value
- ✅ **Null/None Literals** - The values `none` and `None` can be used to represent a null/none value.

### Operators

The following operators are supported:

- TODO

### ❌ If expressions

### Methods

- ℹ️ **C# Methods** - I haven't quite tested the results of calling a `void` method
- ❌ **Python Methods** - I have no plans on supporting calling Python methods.


----

## ⚠️ Filters

 The following filters are implemented:

- TODO

----

## ❌ Tests

----

## ❌ Global Functions

----

## ❌ Extensions
