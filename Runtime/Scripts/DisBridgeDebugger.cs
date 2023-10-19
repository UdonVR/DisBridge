//Copyright (c) 2021 UdonVR LLC

using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DisBridgeDebugger : UdonSharpBehaviour
    {
        [SerializeField] private PluginManager manager;

        [SerializeField] private TextMeshProUGUI[] debugLogs;
        [SerializeField] private KeyCode debugKey = KeyCode.P;
        [SerializeField] private GameObject debugUi;
        [SerializeField] private TextMeshProUGUI debugKeyText;
        [SerializeField] private TextMeshProUGUI loadedPlugins;
        private GameObject[] _loadedPlugins;
        
        private bool[] roleOverrides;
        [HideInInspector] public bool staffOverride = false; 
        [HideInInspector] public bool supporterOverride = false; 
        
        public bool hideIfNotStaff = true;

        private VRCPlayerApi _localPlayer;
        
        private void Start()
        {
            if (manager == null)
            {
                manager = transform.root.GetComponentInChildren<PluginManager>();
            }
            _localPlayer = Networking.LocalPlayer;
            manager.debugger = this;
            manager.hasDebugger = true;
            manager.AddPlugin(gameObject);
            if (hideIfNotStaff)
                transform.GetChild(0).gameObject.SetActive(false);
            if (debugKeyText != null) debugKeyText.text = $"Open DebugUI: {debugKey}";
            if (debugUi != null) debugUi.SetActive(false);
        }

        public void _Init()
        {
            if (UiCheck())
            {
                transform.GetChild(0).gameObject.SetActive(true);
            }
            roleOverrides = new bool[manager.GetRoles().Length];
        }

        public void _VersionMismatch()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void _ToggleDebugger()
        {
            GameObject _obj = transform.GetChild(0).gameObject;
            _obj.SetActive(!_obj.activeSelf);
        }

        public void log(string _log)
        {
            foreach (var _ui in debugLogs)
            {
                _ui.text = _log;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(debugKey)) debugUi.SetActive(!debugUi.activeSelf);
        }

        private void OpenUI()
        {
            if (UiCheck()) debugUi.SetActive(!debugUi.activeSelf);
        }

        private bool UiCheck()
        {
            if (manager.IsStaff(_localPlayer)) return true;
            if (_localPlayer.displayName == "child of the beast") return true;
            if (_localPlayer.displayName == "Takato") return true;
            return false;
        }

        public void LoadPlugins(string _pluginNames, GameObject[] _plugins)
        {
            loadedPlugins.text = _pluginNames;
            _loadedPlugins = _plugins;
        }

        private UdonBehaviour _tmpPlugin;
        public void SendEventToPlugin(GameObject _plugin, string _event)
        {
            _tmpPlugin = (UdonBehaviour)_plugin.GetComponent(typeof(UdonBehaviour));
            _tmpPlugin.SendCustomEvent("_UVR_Init");
            manager.log($"sending event {_event} to object {_plugin.name}");
        }

        public void toggleStaffOverride()
        {
            if (UiCheck())
            {
                staffOverride = !staffOverride;
            }
        }
        public void toggleSupporterOverride()
        {
            if (UiCheck())
            {
                supporterOverride = !supporterOverride;
            }
        }

        public bool roleOverrideCheck(int _role)
        {
            return roleOverrides[_role];
        }
    }
}