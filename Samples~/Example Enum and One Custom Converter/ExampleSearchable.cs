using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paulsams.Searchable.Example
{
    public class ExampleSearchable : MonoBehaviour
    {
        [SerializeField, Searchable] private KeyCode _keyCode;
        [SerializeField, Searchable(typeof(ItemTypesConverter))] private string _itemType;

        [SerializeField, Searchable(windowType: SearchableWindowType.Simple)]
        private KeyCode _keyCodeSimpleWindow;

        [SerializeField, Searchable(typeof(ItemTypesConverter), SearchableWindowType.Simple)]
        private string _itemTypeSimpleWindow;
    }
}