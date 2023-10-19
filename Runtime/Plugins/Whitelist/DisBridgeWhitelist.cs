//Copyright (c) 2021 UdonVR LLC

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DisBridgeWhitelist : DisBridgeWhitelist_Class
    {
        [Tooltip("Leave this blank if you don't want it to unlock automatically.")]
        [SerializeField] private PluginManager manager;
        [Tooltip("If this is True, it will allow All Staff to use the button.")]
        [SerializeField] private bool checkStaff = false;
        [Tooltip("If this is True, it will allow All Supporters to use the button.")]
        [SerializeField] private bool checkSupporters = false;
        [Tooltip("Adds the role specified to the unlock check./nUses Role ID")]
        [SerializeField] private string[] checkRolesID;
        [Tooltip("Adds the role specified to the unlock check./nUses Role Index")]
        [SerializeField] private int[] checkRoles;

        [Tooltip("Defines if the whitelist is global toggleable or not.")]
        [SerializeField]private bool isGlobal = false;
        [UdonSynced]private bool isUnlocked = false;

        private void Start()
        {
            manager.AddPlugin(gameObject);
            foreach (var _obj in TargetsDefaultOn)
            {
                if (_obj != null)
                    _obj.SetActive(true);
            }
            foreach (var _obj in TargetsDefaultOff)
            {
                if (_obj != null)
                    _obj.SetActive(false);
            }
        }

        public override void _UVR_Init()
        {
            if (UnlockCheck()) UnlockWhitelist();
        }

        public override void _ForceUnlock(string _code)
        {
            if (_code == gameObject.name)
                UnlockWhitelist();
        }

        public override void _ForceLock(string _code)
        {
            if (_code == gameObject.name)
                LockWhitelist();
        }

        public override void _BindKeypad(DisBridgeKeypad disBridgeKeypad)
        {
            keypads = DisBridgeFunctions.AddToArray(disBridgeKeypad, keypads);
        }

        private bool UnlockCheck()
        {
            if (checkStaff && manager.IsStaff())
            {
                return true;
            }
            if (checkSupporters && manager.IsSupporter())
            {
                return true;
            }

            if (checkRoles != null && checkRoles.Length != 0)
            {
                for (int i = 0; i < checkRoles.LongLength; i++)
                {
                    if (manager.IsMember(checkRoles[i],Networking.LocalPlayer))
                    {
                        return true;
                    }
                }
            }
            
            if (checkRolesID != null && checkRolesID.Length != 0)
            {
                for (int i = 0; i < checkRolesID.LongLength; i++)
                {
                    if (manager.IsMember(checkRolesID[i],Networking.LocalPlayer))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        private void UnlockWhitelist()
        {
            if (keypads != null && keypads.Length != 0)
            {
                foreach (var _keypad in keypads)
                {
                    _keypad.SetUnlock(true);
                }
            }
            foreach (GameObject _obj in TargetsDefaultOn)
            {
                if (_obj != null)
                    _obj.SetActive(false);
            }
            foreach (GameObject _obj in TargetsDefaultOff)
            {
                if (_obj != null)
                    _obj.SetActive(true);
            }

            if (isGlobal && !isUnlocked)
            {
                Networking.SetOwner(Networking.LocalPlayer,gameObject);
                isUnlocked = true;
                RequestSerialization();
            }
            
        }
        private void LockWhitelist()
        {
            if (keypads != null && keypads.Length != 0)
            {
                foreach (var _keypad in keypads)
                {
                    _keypad.SetUnlock(false);
                }
            }
            foreach (GameObject _obj in TargetsDefaultOn)
            {
                if (_obj != null)
                    _obj.SetActive(true);
            }
            foreach (GameObject _obj in TargetsDefaultOff)
            {
                if (_obj != null)
                    _obj.SetActive(false);
            }
        }

        public override void OnDeserialization()
        {
            if (!isGlobal) return;
            if (isUnlocked) UnlockWhitelist();
            else LockWhitelist();
        }
        
    }
}
