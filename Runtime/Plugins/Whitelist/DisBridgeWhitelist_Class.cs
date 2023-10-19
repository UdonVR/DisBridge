using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UdonSharp;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    public abstract class DisBridgeWhitelist_Class : UdonSharpBehaviour
    {
        [Tooltip("These are OFF by default\nWhen a player's name matches the list, these will TURN ON.")]
        public GameObject[] TargetsDefaultOff;

        [Tooltip("These are ON by default\nWhen a player's name matches the list, these will TURN OFF.")]
        public GameObject[] TargetsDefaultOn;

        [HideInInspector]public DisBridgeKeypad[] keypads;


        public virtual void _UVR_Init()
        {
            
        }

        public abstract void _ForceUnlock(string _code);
        public abstract void _ForceLock(string _code);
        public abstract void _BindKeypad(DisBridgeKeypad disBridgeKeypad);
    }
}