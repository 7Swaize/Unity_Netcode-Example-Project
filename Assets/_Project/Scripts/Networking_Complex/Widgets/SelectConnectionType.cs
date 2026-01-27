using System.Linq;
using TMPro;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class SelectConnectionType : WidgetBehaviour, ISetupEvents {
        [SerializeField] private SessionConfig sessionConfig;
        [SerializeField] private TMP_Dropdown connectionTypeDropdown;

        private void Start() {
            connectionTypeDropdown.ClearOptions();
            connectionTypeDropdown.AddOptions(
                System.Enum.GetNames(typeof(NetworkConnectionType)).ToList()
            );
            
            // connectionTypeDropdown.value = (int)sessionConfig.networkConnectionType;
            // connectionTypeDropdown.RefreshShownValue();
            connectionTypeDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        private void OnDropdownValueChanged(int value) {
            sessionConfig.networkConnectionType = (NetworkConnectionType)value;
        }
        
        private void OnDestroy() {
            connectionTypeDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }

        public void OnResetButtonClicked() {
            connectionTypeDropdown.interactable = true;
            connectionTypeDropdown.SetValueWithoutNotify(-1);
            connectionTypeDropdown.RefreshShownValue();
        }
    }
}