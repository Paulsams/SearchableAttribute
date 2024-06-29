using System;
using UnityEngine;

public enum SearchableWindowType
{
    Simple,
    Advanced,
};

[Serializable]
public class SearchableAttribute : PropertyAttribute
{
    public readonly Type ConverterType;
    public readonly SearchableWindowType WindowType;

    public SearchableAttribute(Type converterType = null,
        SearchableWindowType windowType = SearchableWindowType.Advanced)
    {
        ConverterType = converterType;
        WindowType = windowType;
    }
}