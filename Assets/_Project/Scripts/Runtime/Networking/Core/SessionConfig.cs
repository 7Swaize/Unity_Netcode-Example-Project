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
        public int port = 0; // maybe should be 7777?

        [HideInInspector] public string publishIp;
        
        private void OnEnable() {
            publishIp = GetExternalIPAddress();
            Debug.Log($"Using {publishIp} for publish ip");
        }
        
        // https://stackoverflow.com/questions/3253701/get-public-external-ip-address
        private static string GetExternalIPAddress() {
            return new WebClient().DownloadString("https://ipv4.icanhazip.com/").TrimEnd();
        }
        
        private static string GetLocalIPAddress() {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    string ipStr = ip.ToString();
                    // ip address in lan | would recommend relay for anything public (not lan)
                    if (ipStr.StartsWith("192.168")) { 
                        Debug.Log($"Using {ipStr} for publish ip");
                        return ipStr;
                    }
                }
            }
            
            return "127.0.0.1";
        }
    }
}