#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using Paulsams.Searchable.Converters;
using UnityEngine;
#endif

namespace Paulsams.Searchable.Example
{
    public class ItemTypesConverter
#if UNITY_EDITOR
        : ISearchableConverter
    {
        // This kind of sophistication is only needed for JsonUtility
        [System.Serializable]
        private class Wrapper
        {
            [System.Serializable]
            public struct ItemType
            {
                public string Name;
                public string Category;

                public ItemType(string name, string category)
                {
                    Name = name;
                    Category = category;
                }
            }

            public ItemType[] Items;
        }

        private readonly ISearchableConverter.Element[] _items;

        public ItemTypesConverter()
        {
            string pathToJson = $"{Application.dataPath}/Samples/SearchableAttribute/2.0.1/" +
                                "Example Enum and One Custom Converter/ItemTypes/ItemsTypes.json";

            if (File.Exists(pathToJson) == false)
            {
                Debug.LogError($"File with item types was not found on the path: {pathToJson}");
                return;
            }

            // In order not to add a new dependency in the form of the same Newtonsoft.Json - I use JsonUtility.
            // I do not recommend doing this at all
            var json = File.ReadAllText(pathToJson);
            json = "{\"Items\":" + json + "}";
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            _items = wrapper.Items
                .Select((itemType) => new ISearchableConverter.Element($"{itemType.Category}/{itemType.Name}"))
                .ToArray();
        }

        public ISearchableConverter.Element[] Convert(SerializedProperty property) => _items;

        public int GetIndex(SerializedProperty property) =>
            System.Array.FindIndex(_items, (element) => element.Name == property.stringValue);

        public void SetIndex(SerializedProperty property, int index) =>
            property.stringValue = _items[index].Name;
    }
#else
{ }
#endif
}