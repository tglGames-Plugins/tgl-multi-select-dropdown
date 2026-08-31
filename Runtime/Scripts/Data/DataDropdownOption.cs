using System;
using UnityEngine;

namespace TGL.Utilities.UI
{
    [Serializable]
    public class DataDropdownOption
    {
        public Sprite optionSprite;
        public string optionText;

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(optionText) || optionSprite != null;
        }
    }
}