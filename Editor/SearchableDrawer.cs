using System;
using System.Collections.Generic;
using Paulsams.MicsUtils.Searchable.Converters;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Paulsams.MicsUtils.Searchable.Editor
{
    public struct SearchableAttributeParameters
    {
        public readonly Action<SerializedProperty, string> CallbackSetValue;
        public readonly ISearchableConverter Converter;
        public readonly SearchableWindowType WindowType;

        public SearchableAttributeParameters(
            Action<SerializedProperty, string> callbackSetValue = null,
            ISearchableConverter converter = null,
            SearchableWindowType windowType = SearchableWindowType.Advanced)
        {
            CallbackSetValue = callbackSetValue;
            Converter = converter;
            WindowType = windowType;
        }
    }

    public static class SearchableDrawer
    {
        public delegate void SetValueHandler(ISearchableConverter.Element element, int indexChoose);

        private const string _textOnButtonChangeKey = "Change";
        private const float _widthButtonChangeKey = 60f;
        private const float _offsetFromLabelBetweenButton = 5f;

        private static readonly Dictionary<SerializedPropertyType, ISearchableConverter> _converters =
            new Dictionary<SerializedPropertyType, ISearchableConverter>()
            {
                [SerializedPropertyType.Enum] = new EnumConverter(),
            };

        public static class OnGUI
        {
            public static void Draw(Rect position, SerializedProperty property, GUIContent label,
                SearchableAttributeParameters parameters = default)
            {
                if (property.hasMultipleDifferentValues)
                    return;

                position = EditorGUI.PrefixLabel(position, label);

                Rect rectKey = new Rect(position.x, position.y,
                    position.width - _widthButtonChangeKey - _offsetFromLabelBetweenButton, position.height);
                Rect rectButtonChangeKey = new Rect(rectKey.xMax + _offsetFromLabelBetweenButton, position.y,
                    _widthButtonChangeKey, position.height);

                ISearchableConverter converter = GetConverter(property, parameters);

                ISearchableConverter.Element[] keys = converter.Convert(property);
                int indexKey = GetAndClampIndex(property, converter, keys);

                DrawLabel(rectKey, keys, indexKey);

                if (GUI.Button(rectButtonChangeKey, _textOnButtonChangeKey))
                    ShowPopupWindow(position, property, parameters, keys, indexKey);
            }

            private static void DrawLabel(Rect rectKey, ISearchableConverter.Element[] keys, int indexKey)
            {
                string nameKey = keys[indexKey].NameWithDescription;
                GUIStyle styleLabel = EditorStyles.textField;
                styleLabel.richText = true;
                EditorGUI.LabelField(rectKey, nameKey, EditorStyles.textField);
            }
        }

        public static class UIToolkit
        {
            public static VisualElement Create(SerializedProperty property, string label,
                SearchableAttributeParameters parameters = default)
            {
                ISearchableConverter converter = GetConverter(property, parameters);

                ISearchableConverter.Element[] keys = converter.Convert(property);
                int indexKey = GetAndClampIndex(property, converter, keys);

                var container = new VisualElement()
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row
                    }
                };
                var textField = new TextField(label)
                {
                    isReadOnly = true,
                    style =
                    {
                        flexGrow = 1
                    },
                };
                textField.SetValueWithoutNotify(keys[indexKey].NameWithDescription);
                textField.AddToClassList(TextField.alignedFieldUssClassName);

                var changeButton = new Button
                {
                    text = _textOnButtonChangeKey,
                    style =
                    {
                        width = _widthButtonChangeKey,
                        marginLeft = _offsetFromLabelBetweenButton,
                    }
                };
                changeButton.clicked += () =>
                {
                    ShowPopupWindow(changeButton.worldBound, property, parameters, keys, indexKey,
                        element => textField.value = element.NameWithDescription);
                };

                container.Add(textField);
                container.Add(changeButton);
                return container;
            }
        }

        private static void ShowPopupWindow(Rect position, SerializedProperty property,
            SearchableAttributeParameters parameters,
            ISearchableConverter.Element[] keys, int indexKey,
            Action<ISearchableConverter.Element> additionallyCallbackSetValue = null)
        {
            Rect rect = new Rect(position);
            rect.y += EditorGUIUtility.singleLineHeight;

            var readonlyKeys = Array.AsReadOnly(keys);
            SetValueHandler callbackSetValue = (element, index) =>
            {
                SetValue(property, parameters, element, index);
                additionallyCallbackSetValue?.Invoke(element);
            };
            switch (parameters.WindowType)
            {
                case SearchableWindowType.Simple:
                    PopupWindow.Show(rect, new SearchablePopupWindow(readonlyKeys, indexKey, callbackSetValue));
                    break;
                case SearchableWindowType.Advanced:
                    new SearchableAdvancedDropdownWindow(property.displayName, indexKey,
                        readonlyKeys, callbackSetValue).Show(rect);
                    break;
            }
        }

        private static int GetAndClampIndex(SerializedProperty property, ISearchableConverter converter,
            ISearchableConverter.Element[] keys)
        {
            int indexKey = converter.GetIndex(property);

            if (indexKey >= keys.Length || indexKey == -1)
            {
                indexKey = keys.Length - 1;
                converter.SetIndex(property, indexKey);
                property.serializedObject.ApplyModifiedProperties();
            }

            return indexKey;
        }

        private static ISearchableConverter GetConverter(SerializedProperty property,
            SearchableAttributeParameters parameters)
        {
            ISearchableConverter converter = parameters.Converter;
            if (converter == null)
                converter = _converters[property.propertyType];
            return converter;
        }

        private static void SetValue(SerializedProperty property, SearchableAttributeParameters parameters,
            ISearchableConverter.Element element, int index)
        {
            ISearchableConverter converter = parameters.Converter;
            if (converter == null)
                converter = _converters[property.propertyType];

            converter.SetIndex(property, index);
            property.serializedObject.ApplyModifiedProperties();

            parameters.CallbackSetValue?.Invoke(property, element.Name);
        }
    }
}