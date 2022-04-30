using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IConvertToArrayString
{
    public struct Element
    {
        public readonly string Name;
        public readonly string Desription;

        public readonly string TypedText;

        public Element(string name, string desription = "")
        {
            Name = name;
            Desription = desription;

            TypedText = name + (string.IsNullOrEmpty(desription) ? "" : $" ({desription})");
        }
    }

    public Element[] Convert(SerializedProperty property);
    public int GetIndex(SerializedProperty property);
    public void SetIndex(SerializedProperty property, int index);
}