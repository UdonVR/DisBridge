//Copyright (c) 2023 UdonVR LLC - All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

namespace UdonVR.DisBridge.RoleBoardV2
{
    [CustomEditor(typeof(RoleBoardV2))]
    [CanEditMultipleObjects]
    public class RoleBoardV2_Editor : Editor
    {
        private String[] _modeLabels = new string[3] { "All Staff", "All Supporters", "Use Roles" };
        private void OnEnable()
        {
            RoleBoardV2 myScript = (RoleBoardV2)target;
            
        }

        //////////////////////////////////////////////////////////////////////////
        #region GUI

        public override void OnInspectorGUI()
        {
            RoleBoardV2 myScript = (RoleBoardV2)target;
            EditorFunctions.PlaymodeWarning(myScript);
            EditorFunctions.DrawPluginManagerCheck(myScript, false, true);
            EditorFunctions.GuiLine();
            DrawModeSelector(myScript);
            if (myScript._mode == 2) EditorFunctions.DrawRoleContainerSelector(myScript);
            EditorFunctions.GuiLine();
            
            float _publicdDisplayTime = EditorGUILayout.FloatField("Display Time: ",myScript.displayTime);
            if (_publicdDisplayTime != myScript.displayTime)
            {
                Undo.RecordObject(myScript,$"Changed displayTime: {myScript.displayTime} => {_publicdDisplayTime}");
                myScript.displayTime = _publicdDisplayTime;
            }
            
            float _publicFadeSpeed = EditorGUILayout.FloatField("Fade Speed: ",myScript.publicFadeSpeed);
            if (_publicFadeSpeed != myScript.publicFadeSpeed)
            {
                Undo.RecordObject(myScript,$"Changed publicFadeSpeed: {myScript.publicFadeSpeed} => {_publicFadeSpeed}");
                myScript.publicFadeSpeed = _publicFadeSpeed;
                myScript.fadeSpeed = _publicFadeSpeed * .5f;
            }

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

        private void DrawModeSelector(RoleBoardV2 _script)
        {
            int __sel = EditorGUILayout.Popup("Type:",_script._mode, _modeLabels);
            if (__sel != _script._mode)
            {
                Undo.RecordObject(_script,$"Changed type: {_script._mode} => {__sel}");
                _script._mode = __sel;
            }
        }

        #endregion
        //////////////////////////////////////////////////////////////////////////
    }
}