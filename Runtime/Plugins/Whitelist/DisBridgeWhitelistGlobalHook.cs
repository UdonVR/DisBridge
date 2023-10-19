
using UdonSharp;
using UdonVR.DisBridge;
using UdonVR.DisBridge.Plugins;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DisBridgeWhitelistGlobalHook : DisBridgeWhitelist_Class
    {
        [SerializeField] private DisBridgeWhitelist_Class[] whitelists;
        [UdonSynced]private bool isUnlocked = false;

        public override void _UVR_Init()
        {
            
        }
        
        public override void _ForceUnlock(string _code)
        {
            if (_code == gameObject.name)
            {
                Networking.SetOwner(Networking.LocalPlayer,gameObject);
                isUnlocked = true;
                RequestSerialization();
                OnDeserialization();
            }
        }
        
        public override void _ForceLock(string _code)
        {
            if (_code == gameObject.name)
            {
                Networking.SetOwner(Networking.LocalPlayer,gameObject);
                isUnlocked = false;
                RequestSerialization();
                OnDeserialization();
            }
        }
        
        public override void _BindKeypad(DisBridgeKeypad disBridgeKeypad)
        {
            keypads = DisBridgeFunctions.AddToArray(disBridgeKeypad, keypads);
        }
        
        public override void OnDeserialization()
        {
            if (isUnlocked)
            {
                foreach (var whitelist in whitelists)
                {
                    whitelist._ForceUnlock(whitelist.name);
                }
            }
            else
            {
                foreach (var whitelist in whitelists)
                {
                    whitelist._ForceLock(whitelist.name);
                }
            }
        }
    }
}