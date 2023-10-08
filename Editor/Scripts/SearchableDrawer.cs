using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public struct SearchableAttributeParameters
{
    public readonly Action<SerializedProperty, string> CallbackSetValue;
    public readonly IConvertToArrayString Converter;
    public readonly SearchableWindowType WindowType;

    public SearchableAttributeParameters(Action<SerializedProperty, string> callbackSetValue = null, IConvertToArrayString converter = null, SearchableWindowType windowType = SearchableWindowType.Advanced)
    {
        CallbackSetValue = callbackSetValue;
        Converter = converter;
        WindowType = windowType;
    }
}

public class SearchableDrawer
{
    public delegate void SetValueHandler(IConvertToArrayString.Element element, int indexChoose);

    private const float _widthButtonChangeKey = 60f;
    private const float _offsetFromLabelBetweenButton = 5f;

    private static readonly Dictionary<SerializedPropertyType, IConvertToArrayString> _converters = new Dictionary<SerializedPropertyType, IConvertToArrayString>()
    {
        [SerializedPropertyType.Enum] = new EnumToArrayString(),
    };

    public void Draw(Rect position, SerializedProperty property, GUIContent label, SearchableAttributeParameters parameters = default)
    {
        if (property.hasMultipleDifferentValues == false)
        {
            position = EditorGUI.PrefixLabel(position, label);

            Rect rectKey = new Rect(position.x, position.y, position.width - _widthButtonChangeKey - _offsetFromLabelBetweenButton, position.height);
            Rect rectButtonChangeKey = new Rect(rectKey.xMax + _offsetFromLabelBetweenButton, position.y, _widthButtonChangeKey, position.height);

            IConvertToArrayString converter = GetConverter(property, parameters);

            IConvertToArrayString.Element[] keys = converter.Convert(property);
            int indexKey = GetAndClampIndex(property, converter, keys);

            DrawLabel(rectKey, keys, indexKey);

            if (GUI.Button(rectButtonChangeKey, "Change"))
                ShowPupopWindow(position, property, parameters, keys, indexKey);
        }
    }

    private static void DrawLabel(Rect rectKey, IConvertToArrayString.Element[] keys, int indexKey)
    {
        string nameKey = keys[indexKey].NameWithDescription;
        GUIStyle styleLabel = EditorStyles.textField;
        styleLabel.richText = true;
        EditorGUI.LabelField(rectKey, nameKey, EditorStyles.textField);
    }

    private void ShowPupopWindow(Rect position, SerializedProperty property, SearchableAttributeParameters parameters, IConvertToArrayString.Element[] keys, int indexKey)
    {
        Rect rect = new Rect(position);
        rect.y += EditorGUIUtility.singleLineHeight;

        var readonlyKeys = Array.AsReadOnly(keys);
        SetValueHandler callbackSetValue = (element, index) => SetValue(property, parameters, element, index);
        switch (parameters.WindowType)
        {
            case SearchableWindowType.Simple:
                PopupWindow.Show(rect, new SearchablePopupWindow(readonlyKeys, indexKey, callbackSetValue));
                break;
            case SearchableWindowType.Advanced:
                new SearchableAdvancedDropdownWindow(property.displayName, indexKey, readonlyKeys, callbackSetValue).Show(rect);
                break;
        }
    }

    private static int GetAndClampIndex(SerializedProperty property, IConvertToArrayString converter, IConvertToArrayString.Element[] keys)
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

    private static IConvertToArrayString GetConverter(SerializedProperty property, SearchableAttributeParameters parameters)
    {
        IConvertToArrayString converter = parameters.Converter;
        if (converter == null)
            converter = _converters[property.propertyType];
        return converter;
    }

    private void SetValue(SerializedProperty property, SearchableAttributeParameters parameters, IConvertToArrayString.Element element, int index)
    {
        IConvertToArrayString converter = parameters.Converter;
        if (converter == null)
            converter = _converters[property.propertyType];

        converter.SetIndex(property, index);
        property.serializedObject.ApplyModifiedProperties();

        parameters.CallbackSetValue?.Invoke(property, element.Name);
    }
}