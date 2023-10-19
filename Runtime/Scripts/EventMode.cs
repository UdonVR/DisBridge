//Copyright (c) 2021 UdonVR LLC

using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class EventMode : UdonSharpBehaviour
    {
        [Header("Settings")] [SerializeField] private bool canUnlock = false;
        [SerializeField] private bool showIfUnAccessible = true;
        [Header("BackEnd")] [SerializeField] private PluginManager Supporters;
        [SerializeField] private GameObject buttonToggle;
        [SerializeField] private GameObject[] unlockObjects_Local;
        [SerializeField] private GameObject[] lockObjects_Local;
        [SerializeField] private GameObject[] unlockObjects_Global;
        [SerializeField] private GameObject[] lockObjects_Global;
        [SerializeField] private TextMeshProUGUI text;

        [Header("Plugins")]
        //[SerializeField] private GameobjectToggle_OnPlayerTriggerEnter supporterTagToggle;

        [UdonSynced]
        private bool isUnlocked = false;

        private string localPlayer;

        public void Start()
        {
            foreach (var _obj in unlockObjects_Global)
            {
                _obj.SetActive(true);
            }

            foreach (var _obj in lockObjects_Global)
            {
                _obj.SetActive(false);
            }

            foreach (var _obj in unlockObjects_Local)
            {
                _obj.SetActive(true);
            }

            foreach (var _obj in lockObjects_Local)
            {
                _obj.SetActive(false);
            }
        }

        public void Init()
        {
            if (!showIfUnAccessible)
            {
                TryShow();
            }

            localPlayer = Networking.LocalPlayer.displayName;
            TryGlobalUnlock();
            TryLocalUnlock();
        }

        private void TryShow()
        {
            if (Supporters.IsStaff(Networking.LocalPlayer))
            {
                gameObject.SetActive(true);
            }
        }

        public void _TryUnlock()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            IsSupporter();
        }

        private void IsSupporter()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            if (Supporters.IsStaff(Networking.LocalPlayer))
            {
                if (canUnlock)
                {
                    isUnlocked = !isUnlocked;
                }
                else
                {
                    isUnlocked = true;
                }

                RequestSerialization();
                TryGlobalUnlock();
                TryLocalUnlock();
                return;
            }
        }

        public override void OnDeserialization()
        {
            TryGlobalUnlock();
            if (Supporters.IsStaff(Networking.LocalPlayer)) TryLocalUnlock();
        }

        private void TryGlobalUnlock()
        {
            foreach (var _obj in unlockObjects_Global)
            {
                _obj.SetActive(!isUnlocked);
            }

            foreach (var _obj in lockObjects_Global)
            {
                _obj.SetActive(isUnlocked);
            }
        }

        private void TryLocalUnlock()
        {
            text.text = isUnlocked ? "(unlocked)" : "(locked)";
            buttonToggle.SetActive(isUnlocked);
            if (!Supporters.IsStaff(Networking.LocalPlayer))
            {
                foreach (var _obj in unlockObjects_Local)
                {
                    _obj.SetActive(true);
                }

                foreach (var _obj in lockObjects_Local)
                {
                    _obj.SetActive(false);
                }
            }
            else
            {
                foreach (var _obj in unlockObjects_Local)
                {
                    _obj.SetActive(!isUnlocked);
                }

                foreach (var _obj in lockObjects_Local)
                {
                    _obj.SetActive(isUnlocked);
                }
            }
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            if (Supporters.IsStaff(requestedOwner))
            {
                return true;
            }

            return false;
        }

        private void ToggleSupporterTags(bool _set)
        {
            //supporterTagToggle.thisToggle.isOn = _set;
            //supporterTagToggle.DisableInteractive = _set;
            //supporterTagToggle.target.SetActive(!_set);
        }
    }
}