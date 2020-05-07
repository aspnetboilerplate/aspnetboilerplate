## Introduction

**Object Comparator System** is a system which allows you to create comparator for any type of object with any compare type.

### Creating a Comparator
You can create a comparator for any type of object. Just inherit **ObjectComparatorBase**, **ObjectComparatorBase<TBaseType>**  or  **ObjectComparatorBase<TBaseType, TEnumCompareTypes>  where TEnumCompareTypes : Enum**

For example, creating a comparator for string with object and enum:

```csharp
public class StringObjectComparator : ObjectComparatorBase<string, StringCompareTypes>
{
	protected override bool Compare(string baseObject, string compareObject, StringCompareTypes compareTypes)
	{
		switch (compareTypes)
		{
			case StringCompareTypes.Equals:
				return baseObject == compareObject;
			case StringCompareTypes.Contains:
				return baseObject.Contains(compareObject);
			case StringCompareTypes.StartsWith:
				return baseObject.StartsWith(compareObject);
			case StringCompareTypes.EndsWith:
				return baseObject.EndsWith(compareObject);
			case StringCompareTypes.Null:
				return baseObject.IsNullOrEmpty();
			case StringCompareTypes.NotNull:
				return !baseObject.IsNullOrEmpty();
			default:
				throw new ArgumentOutOfRangeException(nameof(compareTypes), compareTypes, null);
		}
	}
}
```



Another example. Creating a more complex comparator.

```csharp
public class ObjectComparatorTestClass
{
	public string Prop1 { get; set; }
	public string Prop2 { get; set; }
}

public class ObjectComparatorTestClassObjectComparator : ObjectComparatorBase<ObjectComparatorTestClass>// you can create comparator for any type of object
{
	public override ImmutableList<string> CompareTypes { get; }

	public ObjectComparatorTestClassObjectComparator()
	{
		CompareTypes = (new List<string>()
		{
			"Equal",
			"FirstProp1BiggerThanSecondProp2AsInt"//any compare type you want 
		}).ToImmutableList();
	}

	protected override bool Compare(ObjectComparatorTestClass baseObject, ObjectComparatorTestClass compareObject, string compareType)
	{
		if (baseObject == null && compareObject == null)
		{
			return true;
		}

		if (baseObject == null || compareObject == null)
		{
			return false;
		}

		switch (compareType)
		{
			case "Equal":
				return baseObject.Prop1.Equals(compareObject.Prop1) && baseObject.Prop2.Equals(compareObject.Prop2);
			case "FirstProp1BiggerThanSecondProp2AsInt":
				return int.Parse(baseObject.Prop1) > int.Parse(compareObject.Prop2);
			default:
				throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null);
		}
	}
}
```

*Or you can directly inherit **ObjectComparatorBase** and manage everything manually.*

### Comparing Objects

After you create a comparator for object you can compare that kind of objects with using **IObjectComparatorManager**

```csharp
public class Test1
{
    private readonly IObjectComparatorManager _objectComparatorManager;
    public Test(IObjectComparatorManager objectComparatorManager)
    {
        _objectComparatorManager = objectComparatorManager;
    }

    public void Compare()
    {
        if(! _objectComparatorManager.HasComparator<string>() || !_objectComparatorManager.CanCompare<string, StringCompareTypes>(StringCompareTypes.StartsWith))
        {
            throw new Exception("Comparator not implemented");
        }	
        bool compareResult = _objectComparatorManager.Compare("test", "te", StringCompareTypes.StartsWith);//returns true
    }
}
```


```csharp
public class Test2
{
    private readonly IObjectComparatorManager _objectComparatorManager;
    public Test(IObjectComparatorManager objectComparatorManager)
    {
        _objectComparatorManager = objectComparatorManager;
    }

    public void Compare()
    {
        if (!_objectComparatorManager.HasComparator<ObjectComparatorTestClass>() || !_objectComparatorManager.CanCompare<ObjectComparatorTestClass>("FirstProp1BiggerThanSecondProp2AsInt"))
        {
            throw new Exception("Comparator not implemented");
        }

        bool compareResult = _objectComparatorManager.Compare(
            new ObjectComparatorTestClass() { Prop1 = "1", Prop2 = "2" },
            new ObjectComparatorTestClass() { Prop1 = "3", Prop2 = "4" },
            "FirstProp1BiggerThanSecondProp2AsInt"
        );
    }
}

```


> Note: You can create multiple comparator which works for same object type with different `CompareTypes`