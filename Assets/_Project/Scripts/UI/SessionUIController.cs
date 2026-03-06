using UnityEngine;
using VS.NetcodeExampleProject.Networking;

namespace _Project.Scripts.Runtime.UI {
    public class SessionUIController : MonoBehaviour {
        [SerializeField] private GameObject sessionUIContainer;
        
        private void Update() {
            if (InputCompatibilityWrapper.CheckEscapeKeyPressed()) {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked
                    ? CursorLockMode.None
                    : CursorLockMode.Locked;
            }

            if (InputCompatibilityWrapper.CheckTabKeyPressed()) {
                sessionUIContainer.SetActive(!sessionUIContainer.activeSelf);
            }
        }
    }
}