//Copyright (c) 2023 UdonVR LLC - All Rights Reserved

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace UdonVR.DisBridge.Plugins.RoleTagsV2
{
    [CustomEditor(typeof(RoleTagsV2_Controller))]
    [CanEditMultipleObjects]
    public class RoleTagsV2_Editor : Editor
    {
        private string[] _mode = new string[4] { "All Roles", "Staff", "Supporters", "Select Roles"};
        public override void OnInspectorGUI()
        {
            RoleTagsV2_Controller myScript = (RoleTagsV2_Controller)target;
            EditorFunctions.PlaymodeWarning(myScript);
            
            EditorFunctions.DrawPluginManagerCheck(myScript,false);
            EditorFunctions.GuiLine();
            DrawHeadtracker(myScript);
            EditorFunctions.GuiLine();
            byte _roleMode = (byte)EditorGUILayout.Popup("Mode:",myScript.roleMode,_mode);
            //0 = all roles
            //1 = Staff
            //2 = Supporters
            //3 = Select Roles
            if (_roleMode != myScript.roleMode)
            {
                Undo.RecordObject(myScript,$"Changed roleMode: {myScript.roleMode} => {_roleMode}");
                myScript.roleMode = _roleMode;
            }
            switch (_roleMode)
            {
                    case 0:
                        EditorGUILayout.HelpBox($"All of your roles will be selectable.", MessageType.None);
                        break;
                    case 1:
                        EditorGUILayout.HelpBox($"All of your Staff roles will be selectable.", MessageType.None);
                        break;
                    case 2:
                        EditorGUILayout.HelpBox($"All of your Supporter roles will be selectable.", MessageType.None);
                        break;
                    case 3:
                        EditorGUILayout.HelpBox($"Select what roles you want to use below.", MessageType.None);
                        EditorFunctions.DrawRoleContainerSelector(myScript);
                        break;
            }
            EditorFunctions.GuiLine();
            DrawAdminRoles(myScript);
            DrawDebug(myScript);
            PrefabUtility.RecordPrefabInstancePropertyModifications(myScript);
        }
        
        private void DrawDebug(DisBridgePlugin _script)
        {
            bool _isDebug = EditorGUILayout.Toggle(_script.isDebug);
            if (_isDebug != _script.isDebug)
            {
                Undo.RecordObject(_script,$"Changed isDebug: {_script.isDebug} => {_isDebug}");
                _script.isDebug = _isDebug;
            }

            if (_script.isDebug)
            {
                DrawDefaultInspector();
            }
        }

        private void DrawAdminRoles(RoleTagsV2_Controller _script)
        {
            Undo.RecordObject(_script,$"Changed Admin Roles");
            EditorGUILayout.HelpBox($"Admin Roles\nMembers of these roles will have access to display any role.", MessageType.None);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Admin")) AddAdmin(_script);
            if (GUILayout.Button("Remove Admin")) RemoveAdmin(_script);
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < _script.admins.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _script.admins[i] = (RoleContainer)EditorGUILayout.ObjectField(_script.admins[i], typeof(RoleContainer), true);
                // TODO remove button
                EditorGUILayout.EndHorizontal();
            }
        }

        private void AddAdmin(RoleTagsV2_Controller _script)
        {
            Undo.RecordObject(_script,"Add Admin");
            int _i = _script.admins.Length + 1;
            Array.Resize(ref _script.admins,_i );
        }

        private void RemoveAdmin(RoleTagsV2_Controller _script)
        {
            Undo.RecordObject(_script,"Remove Admin");
            int _i = _script.admins.Length - 1;
            if (_i <= 0) _i = 0;
            Array.Resize(ref _script.admins,_i );
        }

        private void DrawHeadtracker(RoleTagsV2_Controller _script)
        {
            Undo.RecordObject(_script,"Headtracker Edited");
            EditorGUILayout.BeginHorizontal();
            _script.useHeadtracker = EditorGUILayout.Toggle("use Headtracker", _script.useHeadtracker);
            EditorGUI.BeginDisabledGroup(!_script.useHeadtracker);
            _script.headTracker = (Transform)EditorGUILayout.ObjectField(_script.headTracker,typeof(Transform), true);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }
}