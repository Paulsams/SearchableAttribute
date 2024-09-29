using System;
using System.Collections.Generic;
using System.Linq;
using Paulsams.MicsUtils.Searchable.Converters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Paulsams.MicsUtils.Searchable.Editor
{
    [InitializeOnLoad]
    [CustomPropertyDrawer(typeof(SearchableAttribute))]
    public class SearchableAttributePropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<Type, ISearchableConverter> _converters =
            new Dictionary<Type, ISearchableConverter>();

        static SearchableAttributePropertyDrawer()
        {
            var typesConverters = TypeCache.GetTypesDerivedFrom<ISearchableConverter>();
            foreach (var type in typesConverters.Where(type => type.IsAbstract == false && type.IsInterface == false))
                _converters.Add(type, Activator.CreateInstance(type) as ISearchableConverter);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) =>
            SearchableDrawer.UIToolkit.Create(property, property.displayName, CreateParameters());

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
            SearchableDrawer.OnGUI.Draw(position, property, label, CreateParameters());

        private SearchableAttributeParameters CreateParameters()
        {
            var searchableAttribute = (SearchableAttribute)attribute;
            var converter = searchableAttribute.ConverterType == null
                ? null
                : _converters[searchableAttribute.ConverterType];

            return new SearchableAttributeParameters(
                converter: converter,
                windowType: searchableAttribute.WindowType
            );
        }
    }
}