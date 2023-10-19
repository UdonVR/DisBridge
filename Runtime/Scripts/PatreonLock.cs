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
    public class PatreonLock : UdonSharpBehaviour
    {
        [Header("Settings")] [SerializeField] private bool canUnlock = false;
        [SerializeField] private bool showIfUnAccessible = true;
        [Header("BackEnd")] [SerializeField] private PluginManager supporters;
        [SerializeField] private string[] extraUnlocks;
        [SerializeField] private GameObject buttonToggle;
        [SerializeField] private GameObject[] unlockObjects;
        [SerializeField] private GameObject[] lockObjects;
        [SerializeField] private TextMeshProUGUI text;

        [UdonSynced] private bool isUnlocked;
        private string localPlayer;

        private void Start()
        {
            if (!showIfUnAccessible) TryShow();
            localPlayer = Networking.LocalPlayer.displayName;
            TryUnlock();
        }

        private void TryShow()
        {
            if (TrySupporter())
            {
                gameObject.SetActive(false);
            }
        }

        public void _TryUnlock()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            IsSupporter();
        }

        private void IsSupporter()
        {
            if (TrySupporter())
            {
                if (canUnlock) isUnlocked = !isUnlocked;
                else
                {
                    isUnlocked = true;
                }
                RequestSerialization();
                TryUnlock();
            }
        }

        public override void OnDeserialization()
        {
            TryUnlock();
        }

        private void TryUnlock()
        {
            Debug.Log("[UdonVR]Running TryUnlock");
            text.text = isUnlocked ? "(unlocked)" : "(locked)";
            buttonToggle.SetActive(isUnlocked);
            foreach (var _obj in unlockObjects)
            {
                _obj.SetActive(!isUnlocked);
            }

            foreach (var _obj in lockObjects)
            {
                _obj.SetActive(isUnlocked);
            }
        }

        public override bool OnOwnershipRequest(VRCPlayerApi requestingPlayer, VRCPlayerApi requestedOwner)
        {
            return TrySupporter();
        }

        private bool TrySupporter()
        {
            foreach (var _player in extraUnlocks)
            {
                if (_player == Networking.LocalPlayer.displayName) return true;
            }

            if (supporters.IsSupporter(Networking.LocalPlayer))
            {
                return true;
            }
            return false;
        }
    }
}