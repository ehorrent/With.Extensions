With.Extensions
===================

Extension methods used to copy and update immutable classes (as [_copy and update record expression_](https://msdn.microsoft.com/en-us/library/dd233184.aspx) in F#).

### Usage
```C#
  var source = Tuple.Create("first value", "second value");

  // Copy and update 'Item1' member
  var updated = source.With(obj => obj.Item1, "new first value")
                      .Create();  

  System.Diagnostics.Debug.Assert(
  	updated.Item1 == "new first value" &&
  	updated.Item2 == obj.SecondField);
```
### Chaining
Calling **_With extension_** will cause all future method calls to return wrapped query objects. When you've finished, call **_Create()_** to get the final value.
```C#
  var source = Tuple.Create(1, 2, 3);

  // Only create a query object
  var query = source.With(obj => obj.Item1, 2)
                    .With(obj => obj.Item2, 4);

  // Execute the query to create a new object
  var updated = query.Create();

  System.Diagnostics.Debug.Assert(
    updated.Item1 == 2 &&
    updated.Item2 == 4 &&
    updated.Item3 == 3);
```
### How does it work ?
For a given immutable class, the extension search for actual values to use as parameters in the constructor (by using parameter's name).

### Restrictions
To use the extension, your immutable class must define a **unique constructor**.

### Naming conventions
By default, name of a constructor argument must match the name of a corresponding field/property (using **camel case convention**). For example, if a constructor argument is named 'value', extension will search for a field/property named 'Value'.

When calling **Create**, you can override default behavior by providing your own name converter.
For example, if you use 'm_' prefixes :
```C#
  public class Immutable
  {
    public readonly string m_FirstField;
    public readonly string m_SecondField;

    public Immutable(string firstField, string secondField)
    {
      this.m_FirstField = firstField;
      this.m_SecondField = secondField;
    }

    ...
  }

  ...

  var instance = new Immutable("first value", "second value");

  var updated = instance.With(obj => obj.m_FirstField, "new first value")
                        .Create(name => string.Concat("m_", Naming.CamelCase.Convert(name)));
```

### Download
NuGet package can be downloaded [here](https://www.nuget.org/packages/With.Extensions).
