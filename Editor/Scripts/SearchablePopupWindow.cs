using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class SearchablePopupWindow : PopupWindowContent
{
    private class CurrentIndex
    {
        private int _max;
        private int _selected;

        public CurrentIndex(int countNames, int startIndex)
        {
            _max = countNames;
            _selected = startIndex;
        }

        public int Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                Clamp();
            }
        }

        public void ChangeMax(int offset)
        {
            _max += offset;

            if (_max == 0)
                _selected = -1;
            else
                _selected = Mathf.Clamp(_selected, 0, _max - 1);
        }

        private void Clamp()
        {
            int countIndexes = _max;
            _selected = (countIndexes + _selected % countIndexes) % countIndexes;
        }
    }

    private const int _countShowedElements = 17;
    private const float _halfShowedElements = _countShowedElements / 2f;

    private static readonly float _paddingElement = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    private static readonly float _heightFormSearchField = _paddingElement + 5f;

    private static readonly float _scrollHeight = _countShowedElements * _paddingElement;
    private static readonly Vector2 _sizeWindow = new Vector2(250, _heightFormSearchField + _scrollHeight);
    private static readonly Color32 _colorAboveCurrentIndex = new Color32(62, 95, 150, 255);

    private Vector2 _scrollPosition;
    private GUIStyle _elementButtonStyle;
    private IConvertToArrayString.Element[] _currentNames;
    private ReadOnlyCollection<IConvertToArrayString.Element> _allNames;

    private SearchField _searchField;
    private string _searchedText = "";

    private CurrentIndex _currentIndex;
    private Action<IConvertToArrayString.Element, int> _callbackSetValue;

    private Vector2 ScrollPositionForCurrentIndex
    {
        get
        {
            float futurePosition = _paddingElement * (_currentIndex.Selected - _halfShowedElements + 0.5f);

            int directionSelectedFromCenter = Math.Sign(futurePosition - _scrollPosition.y);
            float difference = futurePosition - (_scrollPosition.y + (_halfShowedElements - 0.5f) * _paddingElement * directionSelectedFromCenter);

            if (Math.Sign(difference) == directionSelectedFromCenter)
                return new Vector2(0f, _scrollPosition.y + difference);
            else
                return _scrollPosition;
        }
    }

    public SearchablePopupWindow(ReadOnlyCollection<IConvertToArrayString.Element> allNames, int startIndex, Action<IConvertToArrayString.Element, int> callbackSetValue)
    {
        _callbackSetValue = callbackSetValue;

        _allNames = allNames;
        _currentNames = allNames.ToArray();
        InitSearchField();

        _currentIndex = new CurrentIndex(_allNames.Count, startIndex);
        _elementButtonStyle = GetStyleForElementButton();

        _scrollPosition = new Vector2(0f, _paddingElement * (_currentIndex.Selected - _halfShowedElements + 0.5f));
    }

    private void InitSearchField()
    {
        _searchField = new SearchField
        {
            autoSetFocusOnFindCommand = false,
        };
        _searchField.downOrUpArrowKeyPressed += OnDownOrUpArrowKeyPressed;
    }

    private GUIStyle GetStyleForElementButton()
    {
        GUIStyle elementButtonStyle = EditorStyles.boldLabel;
        elementButtonStyle.contentOffset = new Vector2(15f, 0f);
        elementButtonStyle.fontSize = 12;
        elementButtonStyle.richText = true;

        return elementButtonStyle;
    }

    public override Vector2 GetWindowSize() => _sizeWindow;

    public override void OnGUI(Rect rect)
    {
        Event guiEvent = Event.current;

        string oldSearchedText = _searchedText;
        _searchedText = _searchField.OnGUI(_searchedText);
        GUIUtility.keyboardControl = _searchField.searchFieldControlID;

        Rect rectStartScroolView = GUILayoutUtility.GetLastRect();
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        {
            int lastNamesCount = _currentNames.Length;

            if (oldSearchedText != _searchedText)
                _currentNames = _allNames.Where((element) => element.Name.IndexOf(_searchedText, StringComparison.CurrentCultureIgnoreCase) != -1).ToArray();

            int offsetCount = _currentNames.Length - lastNamesCount;
            if (offsetCount != 0)
                _currentIndex.ChangeMax(offsetCount);

            for (int i = 0; i < _currentNames.Length; ++i)
            {
                Rect currentRect = EditorGUILayout.GetControlRect();

                CheckForDrawAboveCurrentElement(guiEvent, i, currentRect);

                if (DrawCurrentElementAndCheckPressedButton(currentRect, i))
                    return;
            }
        }
        EditorGUILayout.EndScrollView();

        CheckChoiceElementFromKeyboard(guiEvent);
    }

    private void CheckChoiceElementFromKeyboard(Event guiEvent)
    {
        if (guiEvent.keyCode == KeyCode.Return || guiEvent.keyCode == KeyCode.KeypadEnter)
        {
            if (_currentIndex.Selected != -1)
                ChoiceText(_currentNames[_currentIndex.Selected]);
            else
                editorWindow.Close();
        }
    }

    private bool DrawCurrentElementAndCheckPressedButton(Rect currentRect, int i)
    {
        GUIContent contentButton = new GUIContent(_currentNames[i].TypedText, _currentNames[i].TypedText);
        if (GUI.Button(currentRect, contentButton, _elementButtonStyle))
        {
            ChoiceText(_currentNames[i]);
            return true;
        }
        return false;
    }

    private void CheckForDrawAboveCurrentElement(Event guiEvent, int index, Rect currentRect)
    {
        void DrawAboveCurrentElement() => EditorGUI.DrawRect(currentRect, _colorAboveCurrentIndex);

        if (guiEvent.delta != Vector2.zero)
        {
            if (currentRect.Contains(guiEvent.mousePosition))
            {
                _currentIndex.Selected = index;
                DrawAboveCurrentElement();

                editorWindow.Repaint();
            }
        }
        else if (index == _currentIndex.Selected)
        {
            DrawAboveCurrentElement();
        }
    }

    private void OnDownOrUpArrowKeyPressed()
    {
        Event guiEvent = Event.current;
        int directionChange = 0;
        switch (guiEvent.keyCode)
        {
            case KeyCode.DownArrow:
                directionChange = 1;
                break;
            case KeyCode.UpArrow:
                directionChange = -1;
                break;
        }

        _currentIndex.Selected += directionChange;

        _scrollPosition = ScrollPositionForCurrentIndex;
    }

    private void ChoiceText(IConvertToArrayString.Element element)
    {
        _searchedText = element.Name;

        int indexNeedName = _allNames.IndexOf(element);
        if (indexNeedName != -1)
            _callbackSetValue?.Invoke(element, indexNeedName);

        editorWindow.Close();
    }
}