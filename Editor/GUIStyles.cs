using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Boxcat.Tools.SceneSelector
{
    static class GUIStyles
    {
        public static readonly GUIContent TitleContent;

        public static readonly GUIStyle SceneButtonStyle;
        public static readonly GUIContent ScenePlayButtonContent;
        public static readonly GUIContent SceneStopButtonContent;
        public static readonly GUIStyle ScenePlayButtonStyle;

        static readonly IconContent _addButtonContent;
        static readonly IconContent _sceneEntriesButtonContent;
        static readonly IconContent _sceneGroupsButtonContent;
        static readonly IconContent _sortButtonContent;
        public static GUIContent AddButtonContent => _addButtonContent.Get();
        public static GUIContent SceneEntriesButtonContent => _sceneEntriesButtonContent.Get();
        public static GUIContent SceneGroupsButtonContent => _sceneGroupsButtonContent.Get();
        public static GUIContent SortButtonContent => _sortButtonContent.Get();

        static GUIStyles()
        {
            TitleContent = new GUIContent("Scenes", EditorGUIUtility.IconContent("d_Scene").image);
            SceneButtonStyle = new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleLeft};
            ScenePlayButtonContent = new("\u25BA", "Play directly");
            SceneStopButtonContent = new("\u25A0", "Stop");
            ScenePlayButtonStyle = new GUIStyle(GUI.skin.button) {alignment = TextAnchor.MiddleCenter};
            _addButtonContent = new IconContent(SdfIconType.PlusCircle);
            _sceneEntriesButtonContent = new IconContent(SdfIconType.ListUl);
            _sceneGroupsButtonContent = new IconContent(SdfIconType.Collection);
            _sortButtonContent = new IconContent(SdfIconType.SortDown);
        }

        readonly struct IconContent
        {
            readonly SdfIconType _iconType;
            readonly GUIContent _cachedContent;

            public IconContent(SdfIconType iconType)
            {
                _iconType = iconType;
                _cachedContent = new GUIContent();
            }

            public GUIContent Get()
            {
                if (!_cachedContent.image)
                    _cachedContent.image = SdfIcons.CreateTransparentIconTexture(_iconType, Color.white, 32, 32, 0);
                return _cachedContent;
            }
        }
    }
}