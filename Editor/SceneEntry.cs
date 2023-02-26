using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Boxcat.Tools.SceneSelector
{
    [Serializable]
    class SceneEntry
    {
        [SerializeField, Required, AssetsOnly]
        [OnValueChanged(nameof(Scene_OnValueChanged))]
        public SceneAsset Scene;
        [Delayed]
        public string Name;
        [Delayed]
        [TableColumnWidth(120, false)]
        [ValueDropdown(nameof(Group_Dropdown), NumberOfItemsBeforeEnablingSearch = 0)]
        [OnValueChanged(nameof(Group_OnValueChanged))]
        public string Group = "Default";
        [TableColumnWidth(60, false)]
        public int OrderInGroup;

        public string GetPath() => AssetDatabase.GetAssetPath(Scene);

        public SceneEntry(SceneAsset scene)
        {
            Scene = scene;
            Name = ObjectNames.NicifyVariableName(scene.name);
        }

        void Scene_OnValueChanged(SceneAsset sceneAsset)
        {
            Name = ObjectNames.NicifyVariableName(sceneAsset.name);
        }

        ValueDropdownList<string> Group_Dropdown()
            => SceneGroups.Instance.DropdownList;

        void Group_OnValueChanged()
            => OrderInGroup = default;
    }
}