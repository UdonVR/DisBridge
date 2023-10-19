using System;
using System.ComponentModel;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UdonVR.DisBridge;

namespace UdonVR.DisBridge.Plugins
{
    public class ScriptingDefineExample : UdonSharpBehaviour
    {

        #if UVR_DISBRIDGE
        [Header("DisBridge not detected. Please import it into your project.")]
        public string DisBridgeUrl = "https://github.com/UdonVR/DisBridge/wiki";
        #else
        [SerializeField] private PluginManager manager;

        //Built-in Start method. It's recommended to use "_UVR_Init()" instead of "Start" when making a DisBridge Plugin.
        private void Start()
        {
            manager.AddPlugin(gameObject);
        }

        //Runs when DisBridge finishes pulling the roles.
        //This is required to be Public for the PluginManager to Initialize your plugin
        public void _UVR_Init()
        {

        }
        #endif
    }
}