using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Boxcat.Tools.SceneSelector
{
    class SceneEntries : ScriptableObject
    {
        static SceneEntries _instance;

        public static SceneEntries Instance
            => _instance ??= AssetDatabase.LoadAssetAtPath<SceneEntries>("Assets/Editor Default Resources/SceneSelector/SceneEntries.asset");


        [TableList(AlwaysExpanded = true)]
        public List<SceneEntry> List;

        public bool Contains(SceneAsset scene)
        {
            if (scene is null)
                return false;

            foreach (var sceneEntry in List)
            {
                if (sceneEntry.Scene == scene)
                    return true;
            }

            return false;
        }

        public void Add(SceneAsset scene)
        {
            Undo.RecordObject(this, "Add Scene Entry");
            List.Add(new SceneEntry(scene));
        }

        public void Sort(SceneGroups groups)
        {
            var groupToIndex = new Dictionary<string, int>(groups.List.Select((group, index)
                => KeyValuePair.Create(group.Key, index)));

            List.Sort((a, b) =>
            {
                // Group Index 비교.
                if (a.Group != b.Group)
                {
                    if (groupToIndex.TryGetValue(a.Group, out var groupIndexA) == false)
                        groupIndexA = int.MaxValue;
                    if (groupToIndex.TryGetValue(b.Group, out var groupIndexB) == false)
                        groupIndexB = int.MaxValue;
                    if (groupIndexA != groupIndexB)
                        return groupIndexA.CompareTo(groupIndexB);
                }

                // Group Index 가 같다면 OrderInGroup 비교.
                if (a.OrderInGroup != b.OrderInGroup)
                    return a.OrderInGroup.CompareTo(b.OrderInGroup);

                // OrderInGroup 도 같다면 Name 을 비교.
                return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
            });
        }

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void Sort()
        {
            Sort(SceneGroups.Instance);
        }
    }
}