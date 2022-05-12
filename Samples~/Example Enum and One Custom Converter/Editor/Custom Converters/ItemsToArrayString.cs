using System.Linq;
using UnityEditor;
using Paulsams.MicsUtils;
using UnityEngine;

public class ItemTypesToArrayString : IConvertToArrayString
{
    private readonly IConvertToArrayString.Element[] _items;

    public ItemTypesToArrayString()
    {
        string pathToJson = $"{Application.dataPath}/Samples/SearchableAttribute/1.0.1/Example Enum and One Custom Converter/Runtime/ItemTypes/ItemsTypes.json";

        if (JsonSerializerUtility.TryDeserialize(out ItemType[] itemTypes, pathToJson))
        {
            _items = itemTypes.Select((itemType) => new IConvertToArrayString.Element(category: itemType.Category, itemType.Name)).ToArray();
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