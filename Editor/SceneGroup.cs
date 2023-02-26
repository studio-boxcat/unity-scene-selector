using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Boxcat.Tools.SceneSelector
{
    [Serializable]
    class SceneGroup
    {
        [Delayed, OnValueChanged(nameof(OnKeyChanged))]
        public string Key = "Default";
        [TableColumnWidth(90, false)]
        public int Order;
        [TableColumnWidth(50, false)]
        public Color Color = Color.white;

        string _hiddenKey;
        bool _hiddenValue;
        string _foldedKey;
        bool _foldedValue;

        [ShowInInspector]
        [TableColumnWidth(50, false)]
        public bool Hidden
        {
            get
            {
                Init();
                return _hiddenValue;
            }
            set
            {
                Init();
                if (_hiddenValue == value) return;
                _hiddenValue = value;
                PlayerPrefs.SetInt(_hiddenKey, value ? 1 : 0);
            }
        }

        [HideInInspector]
        public bool Folded
        {
            get
            {
                Init();
                return _foldedValue;
            }
            set
            {
                Init();
                if (_foldedValue == value) return;
                _foldedValue = value;
                PlayerPrefs.SetInt(_foldedKey, value ? 1 : 0);
            }
        }

        void Init()
        {
            if (_hiddenKey == null)
            {
                _hiddenKey = "SceneGroup_Hidden:" + Key.GetHashCode();
                _hiddenValue = PlayerPrefs.GetInt(_hiddenKey) == 1;
            }

            if (_foldedKey == null)
            {
                _foldedKey = "SceneGroup_Folded:" + Key.GetHashCode();
                _foldedValue = PlayerPrefs.GetInt(_foldedKey) == 1;
            }
        }

        void OnKeyChanged()
        {
            _hiddenKey = default;
            _foldedKey = default;
        }
    }
}