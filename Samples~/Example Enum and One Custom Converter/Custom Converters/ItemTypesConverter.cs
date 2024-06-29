#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using Paulsams.MicsUtils;
using Paulsams.Searchable.Converters;
using UnityEngine;
#endif

namespace Paulsams.Searchable.Example
{
    public class ItemTypesConverter
#if UNITY_EDITOR
        : ISearchableConverter
    {
        private readonly ISearchableConverter.Element[] _items;

        public ItemTypesConverter()
        {
            string pathToJson = $"{Application.dataPath}/Samples/SearchableAttribute/2.0.0/Example Enum and One Custom Converter/ItemTypes/ItemsTypes.json";

            if (JsonSerializerUtility.TryDeserialize(out ItemType[] itemTypes, pathToJson))
            {
                _items = itemTypes.Select((itemType) => new ISearchableConverter.Element($"{itemType.Category}/{itemType.Name}")).ToArray();
            }
            else
            {
                Debug.LogError($"The file with item types was not found on the path: {pathToJson}");
            }
        }

        public ISearchableConverter.Element[] Convert(SerializedProperty property) => _items;

        public int GetIndex(SerializedProperty property) => System.Array.FindIndex(_items, (element) =>
            element.Name == property.stringValue);

        public void SetIndex(SerializedProperty property, int index) =>
            property.stringValue = _items[index].Name;
    }
#else
{ }
#endif
}