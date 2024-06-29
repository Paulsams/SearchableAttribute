using UnityEditor;
using UnityEngine;

namespace Paulsams.Searchable.Editor
{
    public class SettingsForSearchableWindow
    {
        public const int CountShowedElements = 17;
        public const float HalfShowedElements = CountShowedElements / 2f;

        private const float _widthWidnow = 250;
        private static readonly float _standartHeightItem = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public readonly float HeightItem;
        public readonly float PaddingSizeWindow;

        public readonly Vector2 SizeWindow;

        public SettingsForSearchableWindow(float paddingSizeWindow)
            : this(paddingSizeWindow, _standartHeightItem) { }

        public SettingsForSearchableWindow(float paddingSizeWindow, float heightItem)
        {
            HeightItem = heightItem;
            PaddingSizeWindow = paddingSizeWindow;

            SizeWindow = new Vector2(_widthWidnow, heightItem * CountShowedElements + paddingSizeWindow);
        }
    }
}
