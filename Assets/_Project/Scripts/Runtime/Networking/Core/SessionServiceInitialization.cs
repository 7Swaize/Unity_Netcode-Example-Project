using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public static class NetworkWidgetServiceInitialization {
        public static bool IsInitialized { get; private set; }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init() {
            IsInitialized = false;
        }

        public static void ServicesInitialized() {
            IsInitialized = true;
            SessionWidgetEventDispatcher.Instance.OnServicesInitialized();
        }
    } 
    
    public class SessionServiceInitialization : IServiceInitialization {
        public async Task InitializeAsync() {
            try {
                if (UnityServices.State != ServicesInitializationState.Initialized) {
                    await UnityServices.InitializeAsync();
                }

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            
            NetworkWidgetServiceInitialization.ServicesInitialized();
        }
    }
    
    public interface IServiceInitialization {
        public Task InitializeAsync();
    }
}