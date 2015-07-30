With.Extensions
===================

Extension methods used to copy and update immutable classes (as [_copy and update record expression_](https://msdn.microsoft.com/en-us/library/dd233184.aspx) in F#).

### Single value update
```C#
    var initial = Tuple.Create("first value", "second value");
    var updated = initial.With(obj => obj.Item1, "new first value")
                         .Value();  

    Debug.Assert(
    	obj2.Item1 == "new first value" &&
    	obj2.Item2 == obj.SecondField);
```
### Chaining
When updating multiple fields / properties, you can use Chain() to be more efficient.
Calling Chain() will cause all future method calls to return query objects. When you've finished the computation, call Value() to retrieve the final updated value.
```C#
    var initial = Tuple.Create("first value", "second value", "third value");
    var updated = initial.With(obj => obj.Item1, "new first value")
                         .With(obj => obj.Item2, "new second value")
                         .Value();  

    Debug.Assert(
    	updated.Item1 == "new first value" &&
    	updated.Item2 == "new second value" &&
      updated.Item3 == initial.Item3);
```
### How does it work ?
For a given immutable class, the extension search for actual values to use as parameters in the constructor (by using parameter's name). All is based on naming conventions : **parameter name must match the name of a field/property** (case is ignored).

For example, the class below won't work with the extension, because there's not matching fields/properties for _firstField and secondField_ parameters (it will throw an [InvalidOperationException](https://msdn.microsoft.com/en-us/library/system.invalidoperationexception%28v=vs.110%29.aspx)) :
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
```
```C#
    var instance = new Immutable("first value", "second value");
    instance.With(obj => obj.m_FirstField, "new first value");
```
