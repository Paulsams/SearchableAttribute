using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

public interface IConvertToArrayString
{
    public struct Element
    {
        public readonly string Category;

        public readonly string Name;
        public readonly string Description;

        public readonly string NameWithDescription;

        public string[] SplitedCategories => Category.Split("/");

        public Element(string nameWithCategory, string description = "")
        {
            int indexLastSeparator = nameWithCategory.LastIndexOf('/');
            if (indexLastSeparator != -1)
            {
                Category = nameWithCategory.Remove(indexLastSeparator);
                Name = nameWithCategory.Substring(indexLastSeparator + 1);
            }
            else
            {
                Category = "";
                Name = nameWithCategory;
            }

            Description = description;

            NameWithDescription = Name + (string.IsNullOrEmpty(description) ? "" : $" ({description})");
        }
    }

    public Element[] Convert(SerializedProperty property);
    public int GetIndex(SerializedProperty property);
    public void SetIndex(SerializedProperty property, int index);
}