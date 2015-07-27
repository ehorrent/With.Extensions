With.CSharp
===================

**With** extension method used to copy and update immutable classes (as [_copy and update record expression_](https://msdn.microsoft.com/en-us/library/dd233184.aspx) in F#).

### Usage
For an immutable class defined like below :
```C#
    public class Immutable
    {
        public readonly string FirstField;
        public readonly string SecondField;
        public Immutable(string firstField, string secondField)
        {
            this.FirstField = firstField;
            this.SecondField = secondField;
        }
    }
```
You can create a copy with **one field** modified :
```C#
    var initial = new Immutable("first value", "second value");
    var updated = initial.With(obj => obj.FirstField, "new first value");  

    Debug.Assert(
    	obj2.FirstField == "new first value" &&
    	obj2.SecondField == obj.SecondField);
```

### How does it work ?
For a given immutable class, the extension search for actual values to use as parameters in the constructor (by using parameter's name). All is based on naming conventions to fulfill : **parameter name must match the name of a field/property** (case is ignored).

For example, this class won't work with the extension, because there's not matching fields/properties for _firstField/secondField_ parameters (it will throw an [InvalidOperationException](https://msdn.microsoft.com/en-us/library/system.invalidoperationexception%28v=vs.110%29.aspx)) :
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
