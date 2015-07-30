With.Extensions
===================

Extension methods used to copy and update immutable classes (as [_copy and update record expression_](https://msdn.microsoft.com/en-us/library/dd233184.aspx) in F#).

### Usage
```C#
    var source = Tuple.Create("first value", "second value");

    // Copy and update 'Item2' member
    var updated = source.With(obj => obj.Item1, "new first value")
                        .Create();  

    Debug.Assert(
    	updated.Item1 == "new first value" &&
    	updated.Item2 == obj.SecondField);
```
### Chaining
Calling **_With_** will cause all future method calls to return wrapped query objects. When you've finished, call **_Create()_** to get the final value.
```C#
    var source = Tuple.Create(1, 2, 3);

    // Only create a query object
    var query = source.With(obj => obj.Item1, 2)
                      .With(obj => obj.Item2, "new second value");

    // Execute the query to create a new object
    var updated = query.Create();  

    Debug.Assert(
      updated.Item1 == "new first value" &&
      updated.Item2 == "new second value" &&
      updated.Item3 == initial.Item3);
```
### How does it work ?
For a given immutable class, the extension search for actual values to use as parameters in the constructor (by using parameter's name).
All is based on naming conventions : **parameter name must match the name of a field/property** (case is ignored).

For example, the class below won't work with the extension, because there's no matching fields/properties for _firstField and secondField_ parameters :
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
    }

    ...

    var instance = new Immutable("first value", "second value");

    // 'Create()' call will throw an exception
    instance.With(obj => obj.m_FirstField, "new first value")
            .Create();
```

### Download
NuGet package can be downloaded [here](https://www.nuget.org/packages/With.Extensions).
