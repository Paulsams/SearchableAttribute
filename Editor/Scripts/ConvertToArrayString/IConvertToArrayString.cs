using System;
using System.Collections.ObjectModel;
using UnityEditor;

public interface IConvertToArrayString
{
    public struct Element
    {
        public readonly string Category;

        public readonly string Name;
        public readonly string Description;

        public readonly string NameWithDescription;

        public string[] SplitedCategories => Category.Split("/");

        public Element(string category, string name, string description = "") : this(name, description)
        {
            Category = category;
        }

        public Element(string name, string description = "")
        {
            Category = "";
            Name = name;
            Description = description;

            NameWithDescription = name + (string.IsNullOrEmpty(description) ? "" : $" ({description})");
        }
    }

    public Element[] Convert(SerializedProperty property);
    public int GetIndex(SerializedProperty property);
    public void SetIndex(SerializedProperty property, int index);
}