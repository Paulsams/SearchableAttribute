using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnumToArrayString : IConvertToArrayString
{
    public IConvertToArrayString.Element[] Convert(SerializedProperty property) => property.enumNames.Select((enumValue) => new IConvertToArrayString.Element(enumValue)).ToArray();

    public int GetIndex(SerializedProperty property) => property.enumValueIndex;

    public void SetIndex(SerializedProperty property, int index) => property.enumValueIndex = index;
}