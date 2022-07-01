using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRCPrefabs.CyanEmu;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RoleBoard : UdonSharpBehaviour
    {
        [SerializeField] private PluginManager manager;

        public int roleContainer = 0;
        public TextMeshProUGUI roleTitle;
        public TextMeshProUGUI roleList;
        public string seperator = "</color> | <#3498db>";
        
        private void Start()
        {
            manager.log($"Registering {nameof(RoleBoard)}:{gameObject.name}...");
            manager.AddPlugin(gameObject);
        }
        
        //Runs when DisBridge finishes pulling the roles.
        public void _Init()
        {
            manager.log($"_Init {nameof(RoleBoard)}:{gameObject.name}...");
            string _roleList = String.Join(seperator, manager.GetRole(roleContainer).GetMembers());
            roleTitle.text = roleTitle.text.Replace("{roleTitle}",manager.GetRole(roleContainer).roleName);
            roleList.text = roleList.text.Replace("{roleList}",_roleList);
        }
    }
}