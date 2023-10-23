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
            
            float _publicFadeSpeed = EditorGUILayout.FloatField("Fade Speed: ",myScript.publicFadeSpeed);
            if (_publicFadeSpeed != myScript.publicFadeSpeed)
            {
                Undo.RecordObject(myScript,$"Changed publicFadeSpeed: {myScript.publicFadeSpeed} => {_publicFadeSpeed}");
                myScript.publicFadeSpeed = _publicFadeSpeed;
                myScript.fadeSpeed = _publicFadeSpeed * .5f;
            }

            float _scrollSpeed = EditorGUILayout.FloatField("Scroll speed: ", myScript.scrollSpeed);
            if (_scrollSpeed != myScript.scrollSpeed)
            {
                Undo.RecordObject(myScript, $"Changed scrollSpeed: {myScript.scrollSpeed} => {_scrollSpeed}");
                myScript.scrollSpeed = _scrollSpeed;
            }

            float _scrollStopPointRelative = EditorGUILayout.FloatField("Scroll stop point: ", myScript.scrollStopPointRelative);
            if (_scrollStopPointRelative != myScript.scrollStopPointRelative)
            {
                Undo.RecordObject(myScript, $"Changed scrollStopPointRelative: {myScript.scrollStopPointRelative} => {_scrollStopPointRelative}");
                myScript.scrollStopPointRelative = _scrollStopPointRelative;
            }

            float _waitTimeBeforeScroll = EditorGUILayout.FloatField("Wait time before scroll: ", myScript.waitTimeBeforeScroll);
            if (_waitTimeBeforeScroll != myScript.waitTimeBeforeScroll)
            {
                Undo.RecordObject(myScript, $"Changed waitTimeBeforeScroll: {myScript.waitTimeBeforeScroll} => {_waitTimeBeforeScroll}");
                myScript.waitTimeBeforeScroll = _waitTimeBeforeScroll;
            }

            float _waitTimeAfterScroll = EditorGUILayout.FloatField("Wait time after scroll: ", myScript.waitTimeAfterScroll);
            if (_waitTimeAfterScroll != myScript.waitTimeAfterScroll)
            {
                Undo.RecordObject(myScript, $"Changed waitTimeAfterScroll: {myScript.waitTimeAfterScroll} => {_waitTimeAfterScroll}");
                myScript.waitTimeAfterScroll = _waitTimeAfterScroll;
            }

            float _manualScrollingWaitTime = EditorGUILayout.FloatField("Wait time after manual scroll: ", myScript.manualScrollingWaitTime);
            if (_manualScrollingWaitTime != myScript.manualScrollingWaitTime)
            {
                Undo.RecordObject(myScript, $"Changed manualScrollingWaitTime: {myScript.manualScrollingWaitTime} => {_manualScrollingWaitTime}");
                myScript.manualScrollingWaitTime = _manualScrollingWaitTime;
            }





            DrawDebug(myScript);
            PrefabUtility.RecordPrefabInstancePropertyModifications(myScript);
        }

        private void DrawDebug(RoleBoardV2 _script)
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
