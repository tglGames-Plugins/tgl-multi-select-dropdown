# TGL Multi Select Dropdown
A simple Drop down system which allows to select multiple values.
This is useful for adding filters that are either 'on' or 'off'.
We can use image(sprite) or text(string) or both for options.

## Limitations
Have not tested throughly


### How to Use
To use this plugin, follow these steps:
- A prefab is made available, use this to get a working prefab.
- The [DataDropdownOption.cs](./Runtime/Scripts/Data/MultiSelectDropdownOptionData.cs) class objects can be passed as options, or created in the inspector.
- The [MultiSelectDropdown.cs](./Runtime/Scripts/MultiSelectDropdown.cs) class is the main class, this handles the class info, as well as which options are selected or de-selected.
  - Properties:
    - OnValueChanged: Event with currently selected options
    - CaptionImage: The top Image component which shows all currently selected images
    - CaptionText: The top Text component which shows all currently selected Text
    - Options: All available options to choose from (`List<MultiSelectDropdownOptionData>`)
    - Template: Rect Transform of the template - (`MultiSelectDropdownOption optionPrefab`)
    - Values: id number of the currently selected options in the Dropdown (`List<int>`)
  - Methods
    - SetOptions()
    - AddOption()
    - AddOptions()
    - ClearOptions()
    - Hide() - close the drop down 
    - Show() - Open the drop down 
    - SetValues() - Set the values and send the Notifications in 'OnValueChanged' 
    - SetValuesWithoutNotify() - Set the values but does not invoke the event 'OnValueChanged'


## Samples
Samples can be found in "Samples" folder. There is also a scene with the necessary information.

## How to add this package?
- Open unity package manaegr
- On top right, there is a button to add a package
- add a git package (from git URL)
- fill the Https link for the package, in this case, 'https://github.com/tglGames-Plugins/tgl-multi-select-dropdown.git'
- Add
The package will be added under 'TGL FSM' in packages, use as needed.
