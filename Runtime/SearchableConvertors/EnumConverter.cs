#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
#endif

namespace Paulsams.Searchable.Converters
{
    public class EnumConverter
#if UNITY_EDITOR
        : ISearchableConverter
    {
        public ISearchableConverter.Element[] Convert(SerializedProperty property) => property.enumNames
            .Select((enumValue) => new ISearchableConverter.Element(enumValue))
            .ToArray();

        public int GetIndex(SerializedProperty property) => property.enumValueIndex;

        public void SetIndex(SerializedProperty property, int index) => property.enumValueIndex = index;
    }
#else
{ }
#endif
}