using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Boxcat.Tools.SceneSelector
{
    class SceneGroups : ScriptableObject
    {
        static SceneGroups _instance;

        public static SceneGroups Instance
            => _instance ??= AssetDatabase.LoadAssetAtPath<SceneGroups>("Assets/Editor Default Resources/SceneSelector/SceneGroups.asset");

        [TableList(AlwaysExpanded = true)]
        public List<SceneGroup> List;

        ValueDropdownList<string> _dropdownListCache;

        public ValueDropdownList<string> DropdownList
        {
            get
            {
                if (_dropdownListCache != null)
                    return _dropdownListCache;
                _dropdownListCache = new ValueDropdownList<string>();
                foreach (var group in List)
                    _dropdownListCache.Add(group.Key);
                return _dropdownListCache;
            }
        }

        [Button(ButtonSizes.Medium), PropertyOrder(-1)]
        public void Sort()
        {
            List.Sort((a, b) =>
            {
                if (a.Order != b.Order)
                    return a.Order.CompareTo(b.Order);
                return string.Compare(a.Key, b.Key, StringComparison.Ordinal);
            });

            _dropdownListCache = null;
        }
    }
}