using System.Collections.Generic;
using TGL.Utilities.UI;
using UnityEngine;

namespace TGL.Utilities.Samples
{
    public class PopulateMultiSelectDd : MonoBehaviour
    {
        [SerializeField] private MultiSelectDropdown dropdown;
        [SerializeField] private List<DataDropdownOption> availableDdOptions = new();

        void Start()
        {
            dropdown.SetOptions(availableDdOptions);
        }
    }
}