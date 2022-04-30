# SearchableAttribute
This package allows you to use the [Searchable] attribute that you can use the pop-up window and the search bar to select the desired element.

## Add to project
To add this package to the project, follow these steps:
1) Open PackageManager;
2) Select "Add package from get URL";
3) Insert links to packages that are dependencies of this package:
    + `https://github.com/Paulsams/MiscUtilities.git`
4) Insert this link `https://github.com/Paulsams/SearchableAttribute.git`

## Dependencies
- Is using:
    + MicsUtilities: https://github.com/Paulsams/MiscUtilities.git

## Opportunities
1) Works with enums without any additional configuration.
```cs
[SerializeField, Searchable] private KeyCode _keyCode;
```

![image](https://github.com/Paulsams/SearchableAttribute/blob/master/Documentation~/Enum%20Example.gif)

2) Allows you to write your custom converter to array string from which you can select and assign a value to a field via SerializeProperty.

More detailed:
when importing this package, you will need to create folders along the path: `Utilities/SearchableAttribute`. The file "NamesConvertersToArrayString" (`Runtime/Names Converters ToArray String.cs`) is needed in order not to hardcode class names when writing them as an argument to an attribute. The `Editor/Custom Converters` folder can simply serve as a folder for creating your custom converters, but this does not prohibit you from creating them anywhere in the project. There is an example of a converter below. It is better to give the name of your castor converter with the suffix `ToArrayString`, because on the side of my code it is this phrase that ends.
```cs
[SerializeField, Searchable(NamesConvertersToArrayString.ItemTypes)] private string _itemType;
```

```cs
using System.Linq;
using UnityEditor;
using Paulsams.MicsUtils;
using UnityEngine;

public class ItemTypesToArrayString : IConvertToArrayString
{
    private readonly IConvertToArrayString.Element[] _items;

    public ItemTypesToArrayString()
    {
        string pathToJson = $"{Application.dataPath}/Samples/SearchableAttribute/Example Enum and One Custom Converter/Runtime/ItemTypes/ItemsTypes.json";

        if (JsonSerializerUtility.TryDeserialize(out ItemType[] itemTypes, pathToJson))
        {
            _items = itemTypes.Select((itemType) => new IConvertToArrayString.Element(itemType.Name, itemType.Category)).ToArray();
        }
        else
        {
            Debug.LogError($"The file with item types was not found on the path: {pathToJson}");
        }
    }

    public IConvertToArrayString.Element[] Convert(SerializedProperty property) => _items;

    public int GetIndex(SerializedProperty property) => System.Array.FindIndex(_items, (element) => element.Name == property.stringValue);

    public void SetIndex(SerializedProperty property, int index) => property.stringValue = _items[index].Name;
}
```

![image](https://github.com/Paulsams/SearchableAttribute/blob/master/Documentation~/Enum%20Example.gif)

## Constructors
```cs
SearchableAttribute(string converterTypeName = null)
```

## Samples
To download the examples for this package:
1) Select this package in PackageManager;
2) Expand the "Samples" tab on the right;
3) And click the "Import" button on the example you are interested in.
