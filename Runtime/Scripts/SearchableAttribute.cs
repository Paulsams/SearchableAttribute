using System;
using UnityEngine;

[System.Serializable]
public class SearchableAttribute : PropertyAttribute
{
    public readonly string ConverterTypeName;

    public SearchableAttribute(string converterTypeName = null)
    {
        ConverterTypeName = converterTypeName;
    }
}