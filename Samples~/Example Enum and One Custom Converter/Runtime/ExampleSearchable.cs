using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSearchable : MonoBehaviour
{
    [SerializeField, Searchable] private KeyCode _keyCode;
    [SerializeField, Searchable(NamesConvertersToArrayString.ItemTypes)] private string _itemType;

    [SerializeField, Searchable(windowType: SearchableWindowType.Simple)] private KeyCode _keyCodeSimpleWindow;
    [SerializeField, Searchable(NamesConvertersToArrayString.ItemTypes, SearchableWindowType.Simple)] private KeyCode _itemTypeSimpleWindow;
}