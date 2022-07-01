using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonVR.DisBridge;
using TMPro;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DisBridgeDebugger : UdonSharpBehaviour
    {
        [SerializeField] private PluginManager manager;

        [SerializeField] private TextMeshProUGUI debugUi;

        private void Start()
        {
            manager.debugger = this;
        }

        public void log(string _log)
        {
            debugUi.text = _log;
        }
    }
}