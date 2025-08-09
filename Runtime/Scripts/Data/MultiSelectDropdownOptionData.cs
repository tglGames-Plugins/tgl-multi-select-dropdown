using System;
using UnityEngine;

namespace TGL.Utilities.UI
{
	[Serializable]
	internal class MultiSelectDropdownOptionData
	{
#region variables
		internal int dataId;
		internal Sprite optionSprite;
		internal string optionText;
		
		/// <summary>
		/// if true, we do not have <see cref="optionText"/>
		/// </summary>
		internal bool usesOnlyImage;
		
		/// <summary>
		/// if true, uses both <see cref="optionSprite"/> and <see cref="optionText"/>
		/// </summary>
		internal bool usesBoth;
		internal bool isSelected;
#endregion variables

#region properties
		public Sprite GetOptionSprite => optionSprite;
		public string GetOptionText => optionText;
		public bool IsOptionSelected => isSelected;
#endregion properties

#region constructor
		public MultiSelectDropdownOptionData(Sprite _optionSprite, string _optionText, bool _isSelected, int optionId)
		{
			if (_optionSprite == null && string.IsNullOrEmpty(_optionText))
			{
				Debug.LogWarning($"TGL: {nameof(MultiSelectDropdownOptionData)}: there is no sprite or text for this option, please do not create empty options");
				return;
			}
			
			usesBoth = _optionSprite != null && !string.IsNullOrEmpty(_optionText);
			if (!usesBoth)
			{
				usesOnlyImage = _optionSprite != null;
			}
			else
			{
				usesOnlyImage = false;
			}

			if (usesBoth)
			{
				optionSprite = _optionSprite;
				optionText = _optionText;
			}
			else
			{
				if (usesOnlyImage)
				{
					optionSprite = _optionSprite;
				}
				else
				{
					optionText = _optionText;
				}
			}
			isSelected = _isSelected;
			dataId = optionId;
		}
#endregion constructor
	}

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
