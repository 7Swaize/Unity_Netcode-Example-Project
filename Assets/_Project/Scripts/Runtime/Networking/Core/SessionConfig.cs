using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace VS.NetcodeExampleProject.Networking {
    public enum NetworkConnectionType {
        Direct,
        Relay
    }
    
    [CreateAssetMenu(fileName = "SessionConfig", menuName = "Netcode/Session Config", order = 0)]
    public class SessionConfig : ScriptableObject {
        [Header("Session Settings")]
        public int maxPlayers = 8;
        public bool isLocked;
        public bool isPrivate;
        public string sessionName;

        [Header("Network Settings")]
        public NetworkConnectionType networkConnectionType = NetworkConnectionType.Relay;
        public string listenIp = "0.0.0.0";
        public int port;
        
        [HideInInspector] public string publishIp = GetLocalIPAddress();
        
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