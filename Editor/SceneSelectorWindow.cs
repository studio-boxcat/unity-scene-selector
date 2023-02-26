using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;
using Sirenix.OdinInspector.Editor;
using Object = System.Object;
using static Boxcat.Tools.SceneSelector.GUIStyles;

namespace Boxcat.Tools.SceneSelector
{
    public class SceneSelectorWindow : EditorWindow
    {
        [MenuItem("Tools/Asset Management/Scenes In Project %9", false, 68)]
        static void Init()
        {
            var window = GetWindow(typeof(SceneSelectorWindow), false, "Scenes");
            window.titleContent = TitleContent;
        }

        void OnGUI()
        {
            var scenePath = SceneManager.GetActiveScene().path;
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            DrawSceneLists(sceneAsset);
            GUILayout.FlexibleSpace();
            DrawFooter(sceneAsset);
        }

        [CanBeNull]
        List<(SceneGroup, List<SceneEntry>)> _sceneListCache;

        void DrawSceneLists(SceneAsset activeSceneAsset)
        {
            _sceneListCache ??= SceneListBuilder.Build(false);

            EditorGUILayout.BeginVertical();
            foreach (var (sceneGroup, sceneEntries) in _sceneListCache)
                DrawSceneGroup(sceneGroup, sceneEntries, activeSceneAsset);
            EditorGUILayout.EndVertical();
        }

        void DrawSceneGroup(SceneGroup sceneGroup, List<SceneEntry> sceneEntries, SceneAsset activeSceneAsset)
        {
            var prevBGColor = GUI.backgroundColor;
            GUI.backgroundColor = sceneGroup.Color;
            var foldout = EditorGUILayout.BeginFoldoutHeaderGroup(!sceneGroup.Folded, sceneGroup.Key);
            sceneGroup.Folded = !foldout;
            GUI.backgroundColor = prevBGColor;

            if (foldout)
            {
                foreach (var sceneEntry in sceneEntries)
                    DrawSceneEntry(sceneEntry, activeSceneAsset);
                GUILayout.Space(4);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void DrawSceneEntry(SceneEntry sceneEntry, SceneAsset activeSceneAsset)
        {
            var isPlaying = Application.isPlaying;
            var isSceneActive = ReferenceEquals(sceneEntry.Scene, activeSceneAsset);
            var isScenePlaying = isPlaying && isSceneActive;

            var rect = EditorGUILayout.GetControlRect();
            rect.x += 8;

            EditorGUI.BeginDisabledGroup(isPlaying);
            rect.width = 140;
            var scenePressed = GUI.Button(rect, sceneEntry.Name, SceneButtonStyle);
            EditorGUI.EndDisabledGroup();
            rect.x += 140 + 2;

            EditorGUI.BeginDisabledGroup(isPlaying && !isSceneActive);
            rect.width = 20;
            var playPressed = GUI.Button(rect,
                isScenePlaying ? SceneStopButtonContent : ScenePlayButtonContent, ScenePlayButtonStyle);
            EditorGUI.EndDisabledGroup();

            if (scenePressed)
            {
                var e = Event.current;
                if (e.button == 1)
                {
                    ShowSceneOptions(sceneEntry);
                }
                else if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(sceneEntry.GetPath());
                    GUI.FocusControl("");
                }
            }

            if (playPressed)
            {
                if (isScenePlaying)
                {
                    EditorApplication.isPlaying = false;
                }
                else
                {
                    PlaySceneDirectly(sceneEntry);
                }
            }
        }

        void DrawFooter(SceneAsset activeSceneAsset)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(26));
            var rect = EditorGUILayout.GetControlRect();
            rect.x += 4;
            rect.size = new Vector2(24, 22);

            if (GUI.Button(rect, SceneEntriesButtonContent))
                Inspect(SceneEntries.Instance);
            rect.x += 26;

            if (GUI.Button(rect, SceneGroupsButtonContent))
                Inspect(SceneGroups.Instance);
            rect.x += 26;

            if (GUI.Button(rect, SortButtonContent))
                _sceneListCache = SceneListBuilder.Build(true);
            rect.x += 26;

            EditorGUI.BeginDisabledGroup(SceneEntries.Instance.Contains(activeSceneAsset));
            if (GUI.Button(rect, AddButtonContent))
                SceneEntries.Instance.Add(activeSceneAsset);
            EditorGUI.EndDisabledGroup();


            GUILayout.EndHorizontal();

            static void Inspect(Object obj)
            {
                var w = OdinEditorWindow.InspectObject(obj);
                var rect = w.position;
                rect.position = new Vector2(400, 400);
                rect.size = new Vector2(600, 1200);
                w.position = rect;
            }
        }

        void ShowSceneOptions(SceneEntry sceneEntry)
        {
            var menu = new GenericMenu();
            var handler = (GenericMenu.MenuFunction2) OnSelectPinnedOption;

            foreach (SceneOptions value in Enum.GetValues(typeof(SceneOptions)))
            {
                var menuName = ObjectNames.NicifyVariableName(value.ToString());
                menu.AddItem(new GUIContent(menuName), false, handler, (value, sceneEntry));
            }

            menu.ShowAsContext();

            void OnSelectPinnedOption(object data)
            {
                var (sceneOptions, sceneEntry) = ((SceneOptions Key, SceneEntry SceneEntry)) data;
                switch (sceneOptions)
                {
                    case SceneOptions.Ping:
                        EditorGUIUtility.PingObject(sceneEntry.Scene);
                        return;
                    case SceneOptions.CopyAssetPath:
                        EditorGUIUtility.systemCopyBuffer = sceneEntry.GetPath();
                        return;
                    case SceneOptions.RevealInFinder:
                        EditorUtility.RevealInFinder(sceneEntry.GetPath());
                        return;
                }

                Repaint();
            }
        }

        static void PlaySceneDirectly(SceneEntry sceneEntry)
        {
            _originalStartScene = EditorSceneManager.playModeStartScene;
            EditorSceneManager.playModeStartScene = sceneEntry.Scene;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
            EditorApplication.isPlaying = true;

            static void HandlePlayModeStateChanged(PlayModeStateChange stateChange)
            {
                if (stateChange == PlayModeStateChange.EnteredPlayMode)
                {
                    EditorSceneManager.playModeStartScene = _originalStartScene;
                    _originalStartScene = null;
                    EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
                }
            }
        }

        static SceneAsset _originalStartScene;
    }
}