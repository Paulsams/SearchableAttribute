using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleSearchable : MonoBehaviour
{
    [SerializeField, Searchable] private KeyCode _keyCode;
    [SerializeField, Searchable(NamesConvertersToArrayString.ItemTypes)] private string _itemType;
}