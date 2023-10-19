//Copyright (c) 2023 UdonVR LLC - All Rights Reserved
using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace UdonVR.DisBridge
{
    public abstract class DisBridgePlugin : UdonSharpBehaviour
    {
        public PluginManager disBridge;
        [HideInInspector] public bool isDebug = false;
        public string[] roleContainerIDs; //only used to build the array for editor script in case role changes were made
        public RoleContainer[] _roleContainers;

        public virtual void _UVR_Init()
        {
            
        }

        protected bool TryGetPlayerRoleContainer(VRCPlayerApi _player, out RoleContainer _roleContainer)
        {
            for (int i = 0; i < _roleContainers.Length; i++)
            {
                if (_roleContainers[i].IsMember(_player))
                {
                    _roleContainer = _roleContainers[i];
                    return true;
                }
            }
            _roleContainer = null;
            return false;
        }
        protected bool TryGetPlayerRoleContainer(VRCPlayerApi _player, out RoleContainer _roleContainer, ref RoleContainer[] _roleContainers)
        {
            for (int i = 0; i < _roleContainers.Length; i++)
            {
                if (_roleContainers[i].IsMember(_player))
                {
                    _roleContainer = _roleContainers[i];
                    return true;
                }
            }
            _roleContainer = null;
            return false;
        }

        protected bool IsMemberInRoles(VRCPlayerApi _player)
        {
            foreach (var _role in _roleContainers)
            {
                if (_role.IsMember(_player)) return true;
            }

            return false;
        }
    }
}