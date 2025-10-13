using System;
using UnityEngine;

namespace _Project.Scripts.Runtime.UI {
    public class SessionUIController : MonoBehaviour {
        [SerializeField] private GameObject sessionUIContainer;
        
        private void Update() {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                sessionUIContainer.SetActive(!sessionUIContainer.activeSelf);
            }
        }
    }
}