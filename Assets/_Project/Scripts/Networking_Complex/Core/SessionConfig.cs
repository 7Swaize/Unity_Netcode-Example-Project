using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


namespace VS.NetcodeExampleProject.Networking {
    public enum NetworkConnectionType {
        Relay,
        Direct
    }
    
    [CreateAssetMenu(fileName = "SessionConfig", menuName = "Netcode/Session Config", order = 0)]
    public class SessionConfig : ScriptableObject {
        [Header("Session Settings")]
        public int maxPlayers = 8;
        public bool isLocked = false;
        public bool isPrivate = false;
        
        [Header("Client Connected Settings")]
        public Vector3 onClientConnectedPosition = Vector3.zero;
        public Quaternion onClientConnectedRotation = Quaternion.identity;
        

        [HideInInspector] public NetworkConnectionType networkConnectionType = NetworkConnectionType.Relay;
        [HideInInspector] public string listenIp = "0.0.0.0";
        [HideInInspector] public int port = 0; // may need to set it to 7777? 

        [HideInInspector] public string publishIp;
        
        private void OnEnable() {
            publishIp = GetLocalIPAddress();
        }

        public string GetUniqueSessionName() {
            return Guid.NewGuid().ToString("N");
        }
        
        private static string GetLocalIPAddress() {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            
            return "127.0.0.1";
        }
    }
}