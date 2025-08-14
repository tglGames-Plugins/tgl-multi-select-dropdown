using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TGL.Utilities.UI
{
	[RequireComponent(typeof(Toggle))]
	public class MultiSelectDropdownOption : MonoBehaviour
	{
#region variables
		private int dataId;
		private bool _isSelected;
		private MultiSelectDropdownOptionData myOptionData;
		
		/// <summary>
		/// Set for the prefab which is used to make all options
		/// </summary>
		[Tooltip("Set for the prefab which is used to make all options"), SerializeField] internal bool isPrefab;
		[SerializeField] private Image optionSprite;
		[SerializeField] private TMP_Text optionText;
		[SerializeField] private Toggle optionToggle;
		private bool invokeToggle;
		
		internal Action<int, bool> onToggleValueChanged;
#endregion variables


#region properties
		internal bool IsOptionSelected => optionToggle.isOn;
#endregion properties


#region MonoBehaviour_Methods
		private void Awake()
		{
			if (optionToggle == null)
			{
				optionToggle = GetComponent<Toggle>();
			}
		}

		private void OnEnable()
		{
			if (!isPrefab && myOptionData != null)
			{
				UpdateOptionValues();
			}
		}

		private void OnDestroy()
		{
			if (!isPrefab)
			{
				optionToggle.onValueChanged.RemoveListener(UpdateToggle);
			}
		}
#endregion MonoBehaviour_Methods

		
#region internal_Methods
		internal void SetSelected(bool value)
		{
			if (isPrefab)
			{
				Debug.LogWarning($"Cannot call {nameof(SetSelected)} when {nameof(isPrefab)} is true");
				return;
			}
			
			if (value == _isSelected) 
			{
				return;
			}

			_isSelected = value;
			myOptionData.isSelected = value;
			
			UpdateToggleByCode();
			onToggleValueChanged?.Invoke(dataId, value);
		}

		internal void SetToggleGroup(ToggleGroup group)
		{
			optionToggle.group = group;
		}

		internal void Initialize(MultiSelectDropdownOptionData optionData, bool _isPrefab)
		{
			isPrefab = _isPrefab;
			if (!IsValidSetup())
			{
				Debug.LogWarning($"The option of type {nameof(MultiSelectDropdownOption)} is not set up properly, please check", gameObject);
			}

			if (_isPrefab)
			{
				Debug.LogWarning($"Cannot call {nameof(Initialize)} when {nameof(isPrefab)} is true");
				return;
			}
			UpdateOption(optionData);
			optionToggle.onValueChanged.AddListener(UpdateToggle);
			invokeToggle = true;
		}

		internal void UpdateOption(MultiSelectDropdownOptionData optionData)
		{
			if (isPrefab)
			{
				Debug.LogWarning($"Cannot call {nameof(UpdateOption)} when {nameof(isPrefab)} is true");
				return;
			}
			myOptionData = optionData;
			dataId = myOptionData.dataId;
			
			UpdateOptionValues();
		}
#endregion internal_Methods


#region private_Methods
		private void UpdateToggle(bool opSelected)
		{
			myOptionData.isSelected = opSelected;
			_isSelected = opSelected;
			
			if (invokeToggle)
			{
				onToggleValueChanged?.Invoke(dataId, opSelected);
			}
		}
		
		private void UpdateOptionValues()
		{
			if (myOptionData.usesBoth)
			{
				optionSprite.sprite = myOptionData.GetOptionSprite;
				optionText.text = myOptionData.GetOptionText;
				
				// optional
				optionSprite.enabled = true;
				optionText.enabled = true;
				optionSprite.gameObject.SetActive(true);
				optionText.gameObject.SetActive(true);
			}
			else
			{
				// optional to disable and enable options
				optionSprite.enabled = myOptionData.usesOnlyImage;
				optionSprite.gameObject.SetActive(myOptionData.usesOnlyImage);
				optionText.enabled = !myOptionData.usesOnlyImage;
				optionText.gameObject.SetActive(!myOptionData.usesOnlyImage);
				
				if (myOptionData.usesOnlyImage)
				{
					optionSprite.sprite = myOptionData.GetOptionSprite;
				}
				else
				{
					optionText.text = myOptionData.GetOptionText;
				}
			}

			UpdateToggleByCode();
		}

		private void UpdateToggleByCode()
		{
			invokeToggle = false;
			optionToggle.isOn = myOptionData.IsOptionSelected;
			invokeToggle = true;
		}

		private bool IsValidSetup()
		{
			return optionSprite != null && optionText != null && optionToggle != null;
		}
#endregion private_Methods
	}
}