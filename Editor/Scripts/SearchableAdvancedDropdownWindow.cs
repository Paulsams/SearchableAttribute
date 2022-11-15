using UnityEditor;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System;

public class SearchableAdvancedDropdownWindow : AdvancedDropdown
{
    public class Item
    {
        public readonly AdvancedDropdownItem DropdownItem;
        public readonly int Index;

        private readonly Dictionary<string, Item> _childrens;

        public Item(AdvancedDropdownItem advancedDropdownItem, int index)
        {
            DropdownItem = advancedDropdownItem;
            _childrens = new Dictionary<string, Item>();
            Index = index;
        }

        public Item Add(string label, string name)
        {
            var childrenDropdownItem = new AdvancedDropdownItem(label);

            if (_childrens.TryGetValue(name, out Item childrenItem) == false)
            {
                childrenItem = new Item(childrenDropdownItem, _childrens.Count);
                _childrens.Add(name, childrenItem);
                DropdownItem.AddChild(childrenDropdownItem);
            }

            return childrenItem;
        }

        public Item GetChildren(string name) => _childrens[name];
    }

    private const float _heightElement = 17f;

    private static readonly float _padding = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2f + 10f;
    private static readonly SettingsForSearchableWindow _settings = new SettingsForSearchableWindow(_padding, _heightElement);

    private readonly ReadOnlyCollection<IConvertToArrayString.Element> _allNames;
    private readonly SearchableDrawer.SetValueHandler _callbackSetValue;
    private readonly Dictionary<AdvancedDropdownItem, int> _dropdownItems = new Dictionary<AdvancedDropdownItem, int>();

    private readonly Item _itemRoot;

    public SearchableAdvancedDropdownWindow(string nameMainCategory, int selectedIndex, ReadOnlyCollection<IConvertToArrayString.Element> allNames, SearchableDrawer.SetValueHandler callbackSetValue) : base(null)
    {
        _allNames = allNames;
        _callbackSetValue = callbackSetValue;

        _itemRoot = new Item(new AdvancedDropdownItem(nameMainCategory), -1);
        CacheBuildRoot();

        SetState(selectedIndex);

        minimumSize = _settings.SizeWindow;
    }

    private void SetState(int selectedIndex)
    {
        AdvancedDropdownState state = new AdvancedDropdownState();
        Type stateType = state.GetType();
        Type thisType = GetType();

        if (selectedIndex != -1)
        {
            var item = _itemRoot;
            var selectedElement = _allNames[selectedIndex];
            var parametersFromSelectedIndex = new object[2];
            var parametersFromGetStateForItem = new object[1];
            var parametersFromSetScrollState = new object[2];

            Item nextItem;
            foreach (var category in selectedElement.SplitedCategories)
            {
                if (category != string.Empty)
                {
                    nextItem = item.GetChildren(category);

                    SetIndex(state, stateType, item, parametersFromSelectedIndex, nextItem);
                    SetScroll(state, stateType, item, parametersFromSetScrollState, nextItem);
                    SetNextChild(state, stateType, parametersFromGetStateForItem, nextItem);

                    item = nextItem;
                }
            }

            nextItem = item.GetChildren(selectedElement.Name);
            SetIndex(state, stateType, item, parametersFromSelectedIndex, nextItem);
            SetScroll(state, stateType, item, parametersFromSetScrollState, nextItem);
        }

        thisType.GetField("m_State", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, state);
    }

    private static void SetNextChild(AdvancedDropdownState state, Type stateType, object[] parametersFromGetStateForItem, Item nextItem)
    {
        parametersFromGetStateForItem[0] = nextItem.DropdownItem;
        stateType.GetMethod("GetStateForItem", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(state, parametersFromGetStateForItem);
    }

    private static void SetScroll(AdvancedDropdownState state, Type stateType, Item item, object[] parametersFromSetScrollState, Item nextItem)
    {
        parametersFromSetScrollState[0] = item.DropdownItem;
        parametersFromSetScrollState[1] = new Vector2(0f, _settings.HeightItem * (nextItem.Index - SettingsForSearchableWindow.HalfShowedElements + 0.5f));
        stateType.GetMethod("SetScrollState", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(state, parametersFromSetScrollState);
    }

    private static void SetIndex(AdvancedDropdownState state, Type stateType, Item item, object[] parametersFromSelectedIndex, Item nextItem)
    {
        parametersFromSelectedIndex[0] = item.DropdownItem;
        parametersFromSelectedIndex[1] = nextItem.Index;
        stateType.GetMethod("SetSelectedIndex", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(state, parametersFromSelectedIndex);
    }

    private void CacheBuildRoot()
    {
        var categoriesElements = _allNames.Select((element) => element.SplitedCategories).ToArray();
        for (int i = 0; i < categoriesElements.Length; ++i)
        {
            Item item = _itemRoot;
            foreach (var category in categoriesElements[i])
            {
                if(category != string.Empty)
                    item = item.Add(category, category);
            }
            item = item.Add(_allNames[i].NameWithDescription, _allNames[i].Name);

            _dropdownItems.Add(item.DropdownItem, i);
        }
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var editorWindow = GetType().GetField("m_WindowInstance", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(this) as EditorWindow;
        editorWindow.maxSize = _settings.SizeWindow;

        editorWindow.GetType().GetField("m_ScrollToSelected", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(editorWindow, false);

        return _itemRoot.DropdownItem;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        int indexChoicedElement = _dropdownItems[item];
        _callbackSetValue.Invoke(_allNames[indexChoicedElement], indexChoicedElement);
    }
}