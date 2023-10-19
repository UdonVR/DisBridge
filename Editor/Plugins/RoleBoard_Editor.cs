//Copyright (c) 2023 UdonVR LLC - All Rights Reserved
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UdonVR.DisBridge.Plugins
{
    [CustomEditor(typeof(RoleBoard))]
    [CanEditMultipleObjects]
    public class RoleBoard_Editor : Editor
    {
        private string[] roleBoardTypes = new string[3]
            { "Role Container", "Staff" ,"Supporters"};
        public override void OnInspectorGUI()
        {
            RoleBoard __script = (RoleBoard)target;
            DrawPluginManager(__script);

            string __customTitle = EditorGUILayout.TextField("Custom Title:", __script.customTitle);
            if (__customTitle != __script.customTitle)
            {
                Undo.RecordObject(__script,$"Changed customTitle: {__script.customTitle} => {__customTitle}");
                __script.customTitle = __customTitle;
            }
            
            string __separator = EditorGUILayout.TextField("Separator:", __script.separator);
            if (__separator != __script.separator)
            {
                Undo.RecordObject(__script,$"Changed customTitle: {__script.separator} => {__separator}");
                __script.separator = __separator;
            }
            
            
            EditorFunctions.GuiLine();
            DrawDropdown(__script);

            switch (__script.type)
            {
                case 0:
                    int __roleContainer = EditorGUILayout.IntField("Role Index:", __script.roleContainer);
                    __roleContainer = (__roleContainer < 0)? 0 : __roleContainer;
                    if (__roleContainer != __script.roleContainer)
                    {
                        Undo.RecordObject(__script,$"Changed roleContainer: {__script.roleContainer} => {__roleContainer}");
                        __script.roleContainer = __roleContainer;
                    }
                    
                    string __roleID = EditorGUILayout.TextField("Role ID:", __script.roleID);
                    if (__roleID != __script.roleID)
                    {
                        Undo.RecordObject(__script,$"Changed customTitle: {__script.roleID} => {__roleID}");
                        __script.roleID = __roleID;
                    }

                    bool __isFound = false;
                    if (__script.manager != null)
                    {
                        RoleContainer[] __roles = __script.manager.roles.GetComponentsInChildren<RoleContainer>();

                        if (__roleID != null)
                        {
                            for (int i = 0; i < __roles.Length; i++)
                            {
                                if (__roleID == __roles[i].roleId)
                                {
                                    EditorGUILayout.HelpBox($"Role found using RoleID: {__roles[i].roleName}", MessageType.Info);
                                    __isFound = true;
                                    break;
                                }
                            }
                        }
                        
                        if (__isFound == false)
                        {
                            if (__roleContainer < __roles.Length)
                            {
                                EditorGUILayout.HelpBox($"Role found using Role Index: {__roles[__script.roleContainer].roleName}", MessageType.Info);
                                __isFound = true;
                            }
                        }

                        if (!__isFound)
                        {
                            EditorGUILayout.HelpBox($"No role found", MessageType.Error);
                        }
                    }

                    break;
                case 1:
                    //skip
                    break;
                case 2:
                    //skip
                    break;
            }
            
            DrawDebug(__script);
            PrefabUtility.RecordPrefabInstancePropertyModifications(__script);
        }

        private void DrawDropdown(RoleBoard _script)
        {
            int __sel = EditorGUILayout.Popup("Type:",_script.type, roleBoardTypes);
            if (__sel != _script.type)
            {
                Undo.RecordObject(_script,$"Changed type: {_script.type} => {__sel}");
                _script.type = __sel;
            }
        }

        private void DrawPluginManager(RoleBoard _script)
        {
            EditorFunctions.GuiLine();
            GUILayout.BeginHorizontal();
            PluginManager __manager = (PluginManager)EditorGUILayout.ObjectField("Plugin Manager:", _script.manager, typeof(PluginManager), true);
            if (__manager != _script.manager)
            {
                Undo.RecordObject(_script,$"Changed manager: {_script.manager} => {__manager}");
                _script.manager = __manager;
            }

            
            if (_script.manager != null){
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    Selection.activeObject = _script.manager.gameObject;
                }
            }
            GUILayout.EndHorizontal();
            if (_script.manager == null)
            {
                PluginManager[] __managers = FindObjectsOfType<PluginManager>();
                if (__managers == null || __managers.Length > 1)
                {
                    
                    EditorGUILayout.HelpBox("!!! DisBridgeImageLoader is not bound !!!\n\n multiple DisBridgeImageLoaders were found, please bind one manually or below.", MessageType.Error);

                    for (int i = 0; i < __managers.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.ObjectField(__managers[i], typeof(PluginManager), true);
                        if(GUILayout.Button($"Select",GUILayout.Width(100)))
                        {
                            Undo.RecordObject(_script,$"Changed manager: {_script.manager} => {__managers[i]}");
                            _script.manager = __managers[i];
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                else if (__managers.Length == 0)
                {
                    EditorGUILayout.HelpBox("!!! DisBridgeImageLoader is not bound !!!\n\n No DisBridgeImageLoaders were found, please create one.", MessageType.Error);
                }
                else
                {
                    Undo.RecordObject(_script,$"Changed manager: {_script.manager} => {__managers[0]}");
                    _script.manager = __managers[0];
                }
            }
            EditorFunctions.GuiLine();
        }

        private void DrawDebug(RoleBoard _script)
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
    }
}
