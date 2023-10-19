//Copyright (c) 2021 UdonVR LLC

using System;
using UdonSharp;
using UdonVR.DisBridge;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DisbridgeLocalToggle : UdonSharpBehaviour
    {
        [SerializeField] private PluginManager manager;
        public GameObject[] _targets;
        public GameObject[] _targets2;
        public bool isPhysical = false;
        [Space]
        [Tooltip("If the user cannot use the button, it will not be visible if this is set to True.")]
        [SerializeField] private bool offIfNotUsable = true;
        [Tooltip("If this is True, it will allow All Staff to use the button.")]
        [SerializeField] private bool checkStaff = false;
        [Tooltip("If this is True, it will allow All Supporters to use the button.")]
        [SerializeField] private bool checkSupporters = false;
        [Tooltip("Adds the role specified to the unlock check.")]
        [SerializeField] private int[] checkRoles;
        
        [Header("Backend")]
        [Tooltip("This GameObject gets turned off if `Off If Not Usable` is TRUE\n\n!!MAKE SURE THERE ARE NO SCRIPTS ON THIS OBJECT!!\nscripts do not run if they get turned off.")]
        [SerializeField] private GameObject button;
        [Tooltip("This Collider gets turned off if `Off If Not Usable` is TRUE.\nIf you using UI buttons, leave this empty.")]
        [SerializeField] private Collider buttonCollider;
        private bool isOn = false;
        [Tooltip("This GameObject gets turned on or off depending on the toggle state.\n\n!!MAKE SURE THERE ARE NO SCRIPTS ON THIS OBJECT!!\nscripts do not run if they get turned off.")]
        [SerializeField] private GameObject toggleIndicator;

        public void Start()
        {
            manager.AddPlugin(gameObject);
            if (button != null) button.SetActive(!offIfNotUsable); 
            if (buttonCollider != null) buttonCollider.enabled = (!offIfNotUsable);
            _UpdateState();
            DisableInteractive = !isPhysical;
        }

        public void _UVR_Init()
        {
            if (checkStaff && manager.IsStaff(Networking.LocalPlayer))
            {
                if (button != null) button.SetActive(true);
                if (buttonCollider != null) buttonCollider.enabled = true;
            }
            else if (checkSupporters && manager.IsSupporter(Networking.LocalPlayer))
            {
                if (button != null) button.SetActive(true);
                if (buttonCollider != null) buttonCollider.enabled = true;
            }
            else
            {
                if (checkRoles == null) return;
                for (int i = 0; i < checkRoles.LongLength; i++)
                {
                    if (manager.IsMember(checkRoles[i],Networking.LocalPlayer))
                    {
                        if (button != null) button.SetActive(true);
                        if (buttonCollider != null) buttonCollider.enabled = true;
                        return;
                    }
                }
            }
        }
        
        public override void Interact()
        {
            _Toggle();
        }

        public void _Toggle()
        {
            if (!manager.HasInit()) return;

            if (checkStaff && manager.IsStaff(Networking.LocalPlayer))
            {
                _DoToggle();
            }
            else if (checkSupporters && manager.IsSupporter(Networking.LocalPlayer))
            {
                _DoToggle();
            }
            else
            {
                if (checkRoles == null) return;
                for (int i = 0; i < checkRoles.LongLength; i++)
                {
                    if (manager.IsMember(checkRoles[i],Networking.LocalPlayer))
                    {
                        _DoToggle();
                        return;
                    }
                }
            }
            
        }

        public void _DoToggle()
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            isOn = !isOn;
            _UpdateState();
            RequestSerialization();
        }

        public void _UpdateState()
        {
            foreach (var _obj in _targets)
            {
                _obj.SetActive(isOn);
            }
            foreach (var _obj in _targets2)
            {
                _obj.SetActive(!isOn);
            }
            if (toggleIndicator != null)
                toggleIndicator.SetActive(isOn);
        }
    }
}