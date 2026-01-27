using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public class WidgetBootstrapper : MonoBehaviour {
        public void Start() {
            WidgetBehaviour[] widgets = FindObjectsByType<WidgetBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            
            foreach (WidgetBehaviour widget in widgets) {
                SessionWidgetEventDispatcher.Instance.RegisterWidget(widget);
            }
        }
    }
}