using System;
using UnityEngine;

public enum SearchableWindowType
{
    Simple,
    Advanced,
};

[System.Serializable]
public class SearchableAttribute : PropertyAttribute
{
    public readonly string ConverterTypeName;
    public readonly SearchableWindowType WindowType;

    public SearchableAttribute(string converterTypeName = null, SearchableWindowType windowType = SearchableWindowType.Advanced)
    {
        ConverterTypeName = converterTypeName;
        WindowType = windowType;
    }
}