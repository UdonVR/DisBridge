//Copyright (c) 2023 UdonVR LLC - All Rights Reserved
using System;
using UdonSharp;
using UdonVR.DisBridge;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.RoleBoardV2
{
    public class RoleBoardV2 : DisBridgePlugin
    {
        ////////////////////////////////////////////////////////////////////////////////////////

        #region Vars

        public float displayTime = 60f;
        public float publicFadeSpeed = 1f;
        public float fadeSpeed = .5f; //this will be half the time of `publicFadeSpeed` 

        [Range(0,2)]
        public int _mode = 0;
        // 0 = useAllStaff
        // 1 = useAllSupporters
        // 2 = use RoleContainers

        public int flags = 0;

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////

        #region Unity functions

        private void Start()
        {
            
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////

        #region FadeLoop

        private bool _isFadeStarted = false;
        private bool _isFadeMid = false;
        private float _fadeLoopTime = 0f;

        private void __StartFadeLoop()
        {
            if (_isFadeStarted) return;
            _fadeLoopTime = 0f;
            _isFadeStarted = true;
            _isFadeMid = false;
            __DoFadeLoop();
        }

        private void __DoFadeLoop()
        {
            _fadeLoopTime += Time.deltaTime;
            // TODO lerp alpha change of text depending on isFadeMid state
            if (_fadeLoopTime >= fadeSpeed)
            {
                _fadeLoopTime -= fadeSpeed;
                SendCustomEventDelayedFrames(_isFadeMid ? nameof(_FadeLoopEnd) : nameof(_FadeLoopMid), 0);
            }
        }

        public void _FadeLoopMid()
        {
            _isFadeMid = true;
            // TODO text color swap + BG color swap
            SendCustomEventDelayedFrames(nameof(__DoFadeLoop), 0);
        }

        public void _FadeLoopEnd()
        {
            //TODO idk
        }

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////
        /// TODO TextScrollLoop
        /// This is based on the ScrollRect position and not Time, not sure how to write this at all
        /// this should move at a fixed rate based on how large the scroll rect has gotten and should be dynamic enough
        /// to restart scrolling from any point within the scroll rect
        ///
        /// ie: when user manually scrolls, it should end the loop immediately
        /// after X seconds, it should start scrolling again at the same fixed rate to the bottom, and continue the system.

        #region TextScrollLoop

        private bool _isScrollStarted = false;

        private float _scrollLoopTime = 0f;

        private void __StartScrollLoop()
        {
            if (_isScrollStarted) return;
            _isScrollStarted = true;
            _DoScrollLoop();
        }

        public void _DoScrollLoop()
        {
            //TODO idk what to do here, see above
        }

        private void __EndScrollLoop()
        {
            _isScrollStarted = false;
        }

        #endregion
    }
}