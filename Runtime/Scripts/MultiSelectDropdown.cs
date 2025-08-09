using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TGL.Utilities.UI
{

	[RequireComponent(typeof(Button), typeof(ToggleGroup))]
	public class MultiSelectDropdown : MonoBehaviour
	{
		#region InspectorVariables
		[Header("UI Setup")]
		[Tooltip("The drop down button"), SerializeField] private Button dropdownButton; // drop down button for choosing drop down option
		[Tooltip("selected option value when only one option is selected"), SerializeField] private MultiSelectDropdownOption optionShown;

		[Tooltip("scroll view Reference"), SerializeField] private ScrollRect optionsPanel;
		[Tooltip("The image or Text component that shows option to open the options panel"), SerializeField] private Behaviour optionsPanelOpenSymbol;
		[Tooltip("The image or Text component that shows option to close the options panel"), SerializeField] private Behaviour optionsPanelClosedSymbol;

		[Header("options Setup")]
		[Tooltip("The prefab we use to create options"), SerializeField] private MultiSelectDropdownOption optionPrefab; // selected option value
		[Tooltip("The Toggle Group for this drop down"), SerializeField] private ToggleGroup myDropDownToggleGroup;

		[SerializeField] private List<DataDropdownOption> availableDdOptions = new();
		#endregion InspectorVariables

		#region privateVariables
		// options list
		private List<MultiSelectDropdownOptionData> availableOptions = new();

		private List<MultiSelectDropdownOptionData> selectedOptions;
		private List<MultiSelectDropdownOption> generatedOptions = new();
		private ScrollRect generatedOptionsPanel = null; // TODO : remove this when we use object pool

		private bool isOpen;
		private bool usingDataFromInspector;
		private int myDropDownIds;
		#endregion privateVariables

		#region MonoBehaviour_Methods
		private void Awake()
		{
			SetupAndValidate();
			isOpen = false;
			SetListeners();

			usingDataFromInspector = availableDdOptions is { Count: > 0 };
			availableOptions = usingDataFromInspector ? GetDdOptionsFromData(availableDdOptions) : new List<MultiSelectDropdownOptionData>();
		}

		private void OnDestroy()
		{
			RemoveListeners();
		}

		#endregion MonoBehaviour_Methods

		#region private_Methods
		private void SetupAndValidate()
		{
			if (dropdownButton == null)
			{
				dropdownButton = GetComponent<Button>();
			}

			if (dropdownButton == null)
			{
				Debug.LogWarning($"could not find or set the {nameof(dropdownButton)} variable in {nameof(MultiSelectDropdown)}", gameObject);
				return;
			}

			if (myDropDownToggleGroup == null)
			{
				myDropDownToggleGroup = GetComponent<ToggleGroup>();
			}

			if (myDropDownToggleGroup == null)
			{
				Debug.LogWarning($"could not find or set the {nameof(myDropDownToggleGroup)} variable in {nameof(MultiSelectDropdown)}", gameObject);
				return;
			}
			myDropDownToggleGroup.allowSwitchOff = true;

			if (optionPrefab == null)
			{
				Debug.LogWarning($"could not find or set the {nameof(optionPrefab)} variable in {nameof(MultiSelectDropdown)}", gameObject);
				return;
			}

			if (!optionPrefab.isPrefab)
			{
				Debug.LogWarning($"{nameof(optionPrefab)} has not set the {nameof(optionPrefab.isPrefab)} as true, This is supposed to be set.", optionPrefab.gameObject);
				return;
			}
			else
			{
				optionPrefab.gameObject.SetActive(false);
			}

			if (optionsPanel == null)
			{
				Debug.LogWarning($"could not find or set the {nameof(optionsPanel)} variable in {nameof(MultiSelectDropdown)}", gameObject);
				return;
			}
			else
			{
				optionsPanel.gameObject.SetActive(false);
			}
		}

		private void SetListeners()
		{
			dropdownButton.onClick.AddListener(ToggleOptionsPanel);
		}

		private void RemoveListeners()
		{
			dropdownButton.onClick.RemoveListener(ToggleOptionsPanel);
		}

		private List<MultiSelectDropdownOptionData> GetDdOptionsFromData(List<DataDropdownOption> dataOptions)
		{
			List<MultiSelectDropdownOptionData> generatedOptionsData = new();
			foreach (MultiSelectDropdownOptionData instanceDdOption in dataOptions.Select(dataDropdownOption => new MultiSelectDropdownOptionData(dataDropdownOption.optionSprite, dataDropdownOption.optionText, false, myDropDownIds++)))
			{
				generatedOptionsData.Add(instanceDdOption);
			}
			return generatedOptionsData;
		}

		private MultiSelectDropdownOptionData GetDdOptionFromData(DataDropdownOption dataOption)
		{
			MultiSelectDropdownOptionData instanceDdOption = new MultiSelectDropdownOptionData(dataOption.optionSprite, dataOption.optionText, false, myDropDownIds++);
			return instanceDdOption;
		}

		private void ToggleOptionsPanel()
		{
			isOpen = !isOpen;
			if (isOpen)
			{
				OpenOptionsPanel();
			}
			else
			{
				CloseOptionsPanel();
			}
		}

		private void OpenOptionsPanel()
		{
			Debug.Log($"Opening the panel");
			// create the options panel
			if (generatedOptionsPanel == null)
			{
				generatedOptionsPanel = Instantiate(optionsPanel, optionsPanel.transform.parent);
			}
			else
			{
				Debug.LogWarning($"Options panel is already available, we should delete previously created panel", generatedOptionsPanel.gameObject); // TODO : remove this when we use object pool
			}

			GenerateOptionItems();

			generatedOptionsPanel.gameObject.SetActive(true);
		}

		private void VerifySelectedItemsCount(int optionId, bool optionWasSelected)
		{
			int countInspectorSelected = generatedOptions.Count(x => x.IsOptionSelected);
			selectedOptions = availableOptions.Where(x => x.isSelected).ToList();
			int countDataSelected = selectedOptions.Count;
			if (countInspectorSelected != countDataSelected)
			{
				Debug.LogWarning($"When we updated toggle to {optionWasSelected}, for data id {optionId}, we got the counts do not match");
			}
		}

		private void CloseOptionsPanel()
		{
			Debug.Log($"CloseOptionsPanel the panel");
			DestroyOptionItems();

			if (generatedOptionsPanel != null)
			{
				Debug.Log($"generatedOptionsPanel is not null, destroying it");
				Destroy(generatedOptionsPanel.gameObject); // TODO: replace with Object pool or static generated option later
				generatedOptionsPanel = null; // TODO : remove this when we use object pool
				Debug.Log($"generatedOptionsPanel is destroyed");
			}
			else
			{
				Debug.LogWarning($"While closing the options panel, generatedOptionsPanel is already null");
			}
		}

		private void GenerateOptionItems()
		{
			if (generatedOptionsPanel == null)
			{
				Debug.LogWarning($"the options panel {nameof(generatedOptionsPanel)}, is null, where should the options be made?");
				return;
			}
			
			if (availableOptions is { Count: > 0 })
			{
				if (generatedOptions.Count > 0)
				{
					// remove any old options before we generate list of active options
					DestroyOptionItems();
				}
				
				foreach (MultiSelectDropdownOptionData optionData in availableOptions)
				{
					MultiSelectDropdownOption optionInstance = Instantiate(optionPrefab, generatedOptionsPanel.content);
					optionInstance.Initialize(optionData, false);
					optionInstance.gameObject.SetActive(true);
					optionInstance.SetToggleGroup(null);
					// if we use 'myDropDownToggleGroup', we can only select one option
					optionInstance.onToggleValueChanged += VerifySelectedItemsCount;  // TODO: Handle well when we switch to Object Pool, we will need to remove the listener as well in object pool
					generatedOptions.Add(optionInstance);  // TODO: replace with Object pool later
				}
			}
			else
			{
				Debug.LogWarning($"[{nameof(GenerateOptionItems)}]: No options found in {nameof(availableOptions)}", gameObject);
			}
		}

		private void DestroyOptionItems()
		{
			generatedOptions ??= new List<MultiSelectDropdownOption>();
			if (generatedOptions.Count == 0)
			{
				// No options to delete
				return;
			}
			
			foreach (MultiSelectDropdownOption generatedOpt in generatedOptions)
			{
				Destroy(generatedOpt.gameObject); // TODO: replace with Object pool later
			}
			generatedOptions.Clear();
		}
		#endregion private_Methods

		#region public_Methods

		public bool SetOptions(List<DataDropdownOption> dropDownOptions)
		{
			bool optionsWereSet = false;
			if (usingDataFromInspector)
			{
				Debug.LogWarning($"Cannot call {nameof(SetOptions)} when {nameof(usingDataFromInspector)} is true, as it means there are options defined in the inspector");
				return false;
			}

			if (dropDownOptions is { Count: > 0 })
			{
				int invalidCount = dropDownOptions.Count(x => !x.IsValid());
				if (invalidCount > 0)
				{
					Debug.LogWarning($"[{nameof(SetOptions)}]: we are adding {invalidCount} invalid options, they will be removed / not be added");
					dropDownOptions = dropDownOptions.Where(x => x.IsValid()).ToList();
					optionsWereSet = dropDownOptions is { Count: > 0 };
				}
				else
				{
					optionsWereSet = true;
				}

				if (optionsWereSet)
				{
					availableOptions = GetDdOptionsFromData(dropDownOptions);
					if (isOpen)
					{
						GenerateOptionItems();
					}
				}
			}
			else
			{
				Debug.LogWarning($" {nameof(SetOptions)} called with no actual options, it is either null of empty");
			}
			return optionsWereSet;
		}

		public bool AddOptions(List<DataDropdownOption> dropDownOptions)
		{
			bool optionsWereAdded = false;
			if (usingDataFromInspector)
			{
				Debug.LogWarning($"Cannot call {nameof(SetOptions)} when {nameof(usingDataFromInspector)} is true, as it means there are options defined in the inspector");
				return false;
			}

			if (dropDownOptions is { Count: > 0 })
			{
				int invalidCount = dropDownOptions.Count(x => !x.IsValid());
				if (invalidCount > 0)
				{
					Debug.LogWarning($"[{nameof(AddOptions)}]: we are adding {invalidCount} invalid options, they will be removed / not be added");
					dropDownOptions = dropDownOptions.Where(x => x.IsValid()).ToList();
					optionsWereAdded = dropDownOptions is { Count: > 0 };
				}
				else
				{
					optionsWereAdded = true;
				}

				if (optionsWereAdded)
				{
					availableOptions.AddRange(GetDdOptionsFromData(dropDownOptions));
					if (isOpen)
					{
						GenerateOptionItems();
					}
				}
			}
			else
			{
				Debug.LogWarning($" {nameof(AddOptions)} called with no actual options, it is either null of empty");
			}
			return optionsWereAdded;
		}
		
		public bool AddOption(DataDropdownOption dropDownOption)
		{
			bool optionWasAdded = false;
			if (usingDataFromInspector)
			{
				Debug.LogWarning($"Cannot call {nameof(SetOptions)} when {nameof(usingDataFromInspector)} is true, as it means there are options defined in the inspector");
				return false;
			}

			if (dropDownOption.IsValid())
			{
				availableOptions.Add(GetDdOptionFromData(dropDownOption));
				optionWasAdded = true;
				if (isOpen)
				{
					GenerateOptionItems();
				}
			}
			else
			{
				Debug.LogWarning($" {nameof(AddOption)} called with no actual option, it has no valid info");
			}

			return optionWasAdded;
		}
		#endregion public_Methods
	}
}