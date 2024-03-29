﻿//Copyright (c) 2023 UdonVR LLC - All Rights Reserved
using System;
using TMPro;
using UdonSharp;
using UdonVR.DisBridge;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.RoleBoardV2
{
    public class RoleBoardV2 : DisBridgePlugin
    {
        ////////////////////////////////////////////////////////////////////////////////////////

        #region Vars
        public string separator = "<br>";
        [Tooltip("The Text Mesh Pro Component ued for the user list")]
        [SerializeField] public TextMeshProUGUI roleList;
        [Tooltip("The Text Mesh Pro Component ued for the Title")]
        [SerializeField] public TextMeshProUGUI roleTitle;

        public float publicFadeSpeed = 1f;
        public float fadeSpeed = .5f; //this will be half the time of `publicFadeSpeed` 

        [Range(0,2)]
        public int _mode = 0;
        // 0 = useAllStaff
        // 1 = useAllSupporters
        // 2 = use RoleContainers

        public int flags = 0;

        //Configuration variables
        public float scrollSpeed; //Speed of the scroll. Constant no mather how long the list is.
        public float scrollStopPointRelative; //When should the scroll stop. 0 = End. > 0 before reaching the end. < 0 after reaching the end, leaving some blank space at the bottom.
        public float waitTimeBeforeScroll; //Time in seconds that the list will stay still before starting to scroll
        public float waitTimeAfterScroll; //Time in seconds that the list will stay still after finishing the scroll
        public float manualScrollingWaitTime; ////Time in seconds that the list will stay still before resuming scrolling after user manually scrolls

        //Internal variables
        [SerializeField] private GameObject previousRoleButton;
        [SerializeField] private TextMeshProUGUI previousRoleButton_Text;
        //[SerializeField] private GameObject currentRoleButton;
        [SerializeField] private TextMeshProUGUI currentRoleButton_Text;
        [SerializeField] private GameObject nextRoleButton;
        [SerializeField] private TextMeshProUGUI nextRoleButton_Text;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject scrollBarVertical;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private GameObject namesScreen;
        [SerializeField] private Image screenTint;
        [SerializeField] private Image screenBlack;
        //[SerializeField] private MeshRenderer screenMesh;
        //private Material screenMaterial;

        [SerializeField] private int currentRole; //Current role being displayed
        [SerializeField] private float normalizedTime; //Normalized time used for the position of the list while scrolling.
        [SerializeField] private Color alphaColor;
        [SerializeField] private Color alphaColor2;//Color and alpha used for the displayed text
        [SerializeField] private string savedTittleString; //Tittle string saved from editor. Used to keep tags like {roleList}
        [SerializeField] private string saveContentString; //Content string saved from editor. Used to keep tags like {roleList}
        [SerializeField] private float realScrollSpeed; //Speed applied to the scroll. Calculated from scrollSpeed / scrollSpeedRate
        [SerializeField] private float scrollSpeedRate; //Speed rate calculated from the vertical size of the content. content.sizeDelta.y / sizeDeltaYFactor
        [SerializeField] private float sizeDeltaYFactor; //Arbitrary number used to calculate scrollSpeedRate. Default 20. Helps with making speed number smaller and clearer.
        [SerializeField] private float waitTimeScrollTimer; //Timer used before and after scrolling.
        [SerializeField] private float manualScrollingTimer; //Timer used when user manually scrolls.
        [SerializeField] private float lastFrameScrollPosition; //Stores the position of the list on the last frame. Used to detect if user manually scrolled.
        [SerializeField] private bool roleChangeOverride; //If it's true some loop function will redirect to RoleChangeOverride() to force a current role swap.

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////

        #region Unity functions

        private void Start()
        {
            disBridge.log($"Registering {nameof(RoleBoardV2)}:{gameObject.name}...");
            disBridge.AddPlugin(gameObject);
            //screenMaterial = screenMesh.materials[0];
            if (roleTitle != null) savedTittleString = roleTitle.text;
            if (roleList != null) saveContentString = roleList.text;
            if (loadingScreen != null) loadingScreen.SetActive(true);
            if (namesScreen != null) namesScreen.SetActive(false);
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////

        #region Init
        public override void _UVR_Init()
        {
            disBridge.log($"_Init {nameof(RoleBoardV2)}:{gameObject.name}...");

            if (loadingScreen != null) loadingScreen.SetActive(false);
            if (namesScreen != null) namesScreen.SetActive(true);
            
            switch (_mode)
            {
                case 0:
                    _roleContainers = disBridge.GetStaff();
                    break;
                case 1:
                    _roleContainers = disBridge.GetSupporter();
                    break;
                case 2:
                    break;
            }
            currentRole = 0;
            UpdateDisplayWithCurrentRole();
            UpdateButtons();
            __StartScrollLoop();
        }
        #endregion

        #region FadeLoop

        private bool _isFadeStarted = false;
        private bool _isFadeMid = false;
        public float _fadeLoopTime = 0f;

        private void __StartFadeLoop()
        {
            if (_isFadeStarted) return;
            _fadeLoopTime = 0f;
            _isFadeStarted = true;
            _isFadeMid = false;
            __DoFadeLoop();
        }

        public void __DoFadeLoop()
        {
            if (roleChangeOverride)
            {
                RoleChangeOverride();
            }
            else
            {
                _fadeLoopTime += Time.deltaTime;
                normalizedTime = _fadeLoopTime / fadeSpeed;
                if (!_isFadeMid)
                {
                    if (screenTint != null)
                    {
                        alphaColor.a = Mathf.Lerp(.75f, 0, normalizedTime);
                        screenTint.color = alphaColor;
                    }

                    if (screenBlack != null)
                    {
                        alphaColor2.a = Mathf.Lerp(0, .99f, normalizedTime);
                        screenBlack.color = alphaColor2;
                    }
                    
                    //screenMaterial.SetColor("_EmissionColor",alphaColor);
                    //roleList.color = alphaColor;
                    //roleTitle.color = alphaColor;
                }
                else
                {
                    
                    if (screenTint != null)
                    {
                        alphaColor.a = Mathf.Lerp(0, .75f, normalizedTime);
                        screenTint.color = alphaColor;
                    }
                    if (screenBlack != null)
                    {
                        alphaColor2.a = Mathf.Lerp(.99f, 0, normalizedTime);
                        screenBlack.color = alphaColor2;
                    }

                    
                    //screenMaterial.SetColor("_EmissionColor",alphaColor);
                    //roleList.color = alphaColor;
                    //roleTitle.color = alphaColor;
                }

                if (_fadeLoopTime >= fadeSpeed)
                {
                    _fadeLoopTime -= fadeSpeed;
                    SendCustomEventDelayedFrames(_isFadeMid ? nameof(_FadeLoopEnd) : nameof(_FadeLoopMid), 0);
                }
                else
                {
                    SendCustomEventDelayedFrames(nameof(__DoFadeLoop), 0);
                }
            }
        }

        public void _FadeLoopMid()
        {
            _isFadeMid = true;
            if (currentRole < _roleContainers.Length -1)
            {
                currentRole++;
            }
            else
            {
                currentRole = 0;
            }
            UpdateDisplayWithCurrentRole();
            UpdateButtons();
            alphaColor.a = 0;
            alphaColor2.a = 0;
            //roleList.color = alphaColor;
            //roleTitle.color = alphaColor;
            scrollRect.verticalNormalizedPosition = 1;
            SendCustomEventDelayedFrames(nameof(__DoFadeLoop), 0);
        }

        public void _FadeLoopEnd()
        {
            _isFadeStarted = false;
            _isFadeMid = false;
            SendCustomEventDelayedFrames(nameof(__StartScrollLoop), 0);
        }

        #endregion

        #region TextScrollLoop

        private bool _isScrollStarted = false;

        public void __StartScrollLoop()
        {
            if (_isScrollStarted) return;
            _isScrollStarted = true;
            lastFrameScrollPosition = 1;
            waitTimeScrollTimer = 0;
            if(scrollRect.verticalNormalizedPosition >= 0.95)
            {
                scrollSpeedRate = content.sizeDelta.y / sizeDeltaYFactor;
                realScrollSpeed = scrollSpeed / scrollSpeedRate;
                _EarlyDoScrollLoop();
            }
            else
            {
                __ManualScrollingStart();
            }
        }

        //Loop that plays before the main _DoScrollLoop. Stops the scroll before starting for X seconds. X == waitTimeBeforeScroll
        public void _EarlyDoScrollLoop()
        {
            if (roleChangeOverride)
            {
                RoleChangeOverride();
            }
            else
            {
                waitTimeScrollTimer += Time.deltaTime;
                if (waitTimeScrollTimer >= waitTimeBeforeScroll)
                {
                    waitTimeScrollTimer = 0;
                    _DoScrollLoop();
                }
                else
                {
                    SendCustomEventDelayedFrames(nameof(_EarlyDoScrollLoop), 0);
                }
            }
        }

        public void _DoScrollLoop()
        {
            if (roleChangeOverride)
            {
                RoleChangeOverride();
            }
            else
            {
                if (scrollBarVertical.activeInHierarchy)
                {
                    //scrollSpeedRate = content.sizeDelta.y / sizeDeltaYFactor;
                    //realScrollSpeed = scrollSpeed / scrollSpeedRate;
                    if (scrollRect.verticalNormalizedPosition > scrollStopPointRelative)
                    {
                        scrollRect.verticalNormalizedPosition -= realScrollSpeed * Time.deltaTime;
                        if (scrollRect.verticalNormalizedPosition > lastFrameScrollPosition)
                        {
                            lastFrameScrollPosition = scrollRect.verticalNormalizedPosition;
                            __ManualScrollingStart();
                        }
                        else
                        {
                            lastFrameScrollPosition = scrollRect.verticalNormalizedPosition;
                            SendCustomEventDelayedFrames(nameof(_DoScrollLoop), 0);
                        }
                    }
                    else
                    {
                        SendCustomEventDelayedFrames(nameof(_LateDoScrollLoop), 0);
                    }
                }
                else
                {
                    SendCustomEventDelayedFrames(nameof(_LateDoScrollLoop), 0);
                }
            }
        }

        //Loop that plays after the main _DoScrollLoop. Stops the scroll after finishing for X seconds. X == waitTimeAfterScroll
        public void _LateDoScrollLoop()
        {
            if (roleChangeOverride)
            {
                RoleChangeOverride();
            }
            else
            {
                waitTimeScrollTimer += Time.deltaTime;
                if (waitTimeScrollTimer >= waitTimeAfterScroll)
                {
                    waitTimeScrollTimer = 0;
                    __EndScrollLoop();
                }
                else
                {
                    SendCustomEventDelayedFrames(nameof(_LateDoScrollLoop), 0);
                }
            }
        }

        public void __EndScrollLoop()
        {
            _isScrollStarted = false;
            if (_roleContainers.Length == 1)
            {
                scrollRect.verticalNormalizedPosition = 1;
                __StartScrollLoop();
            }
            else
            {
                __StartFadeLoop();
            }
            
        }

        //Resets the timer for manual scrolling and starts __ManualScrollingLoop.
        public void __ManualScrollingStart()
        {
            manualScrollingTimer = 0;
            __ManualScrollingLoop();
        }

        //Stops the scroll loop until X seconds have passed without the user manually scrolling.
        public void __ManualScrollingLoop()
        {
            if (roleChangeOverride)
            {
                RoleChangeOverride();
            }
            else
            {
                if (scrollRect.verticalNormalizedPosition == lastFrameScrollPosition)
                {
                    manualScrollingTimer += Time.deltaTime;
                }
                else
                {
                    manualScrollingTimer = 0;
                }

                lastFrameScrollPosition = scrollRect.verticalNormalizedPosition;
                if (manualScrollingTimer >= manualScrollingWaitTime)
                {
                    scrollSpeedRate = content.sizeDelta.y / sizeDeltaYFactor;
                    realScrollSpeed = scrollSpeed / scrollSpeedRate;
                    SendCustomEventDelayedFrames(nameof(_DoScrollLoop), 0);
                }
                else
                {
                    SendCustomEventDelayedFrames(nameof(__ManualScrollingLoop), 0);
                }
            }
            
        }

        #endregion

        #region Utils
        //Changes the text and the color of the display to the corresponding data of the current role.
        public void UpdateDisplayWithCurrentRole()
        {
            alphaColor = _roleContainers[currentRole].roleColor;
            if (roleList != null)
            {
                string _roleList = "";
                roleList.text = saveContentString;
                _roleList = String.Join(separator, _roleContainers[currentRole].GetMembers());
                roleList.text = _roleList;
                //roleList.color = alphaColor;
            }
            if (roleTitle != null)
            {
                string _roleTitle = "";
                roleTitle.text = savedTittleString;
                _roleTitle = _roleContainers[currentRole].roleName;
                roleTitle.text = _roleTitle;
                //roleTitle.color = alphaColor;
            }
        }

        //Changes the text and the color of the buttons to the corresponding data of the previous, current and next role.
        public void UpdateButtons()
        {
            if(_roleContainers.Length == 1)
            {
                if (previousRoleButton != null) previousRoleButton.SetActive(false);
                if (nextRoleButton != null) nextRoleButton.SetActive(false);
            }
            else
            {
                if (currentRole == 0)
                {
                    if (previousRoleButton_Text != null) previousRoleButton_Text.text = _roleContainers[_roleContainers.Length - 1].roleName;
                    if (nextRoleButton_Text != null) nextRoleButton_Text.text = _roleContainers[currentRole + 1].roleName;
                    //previousRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[_roleContainers.Length - 1].roleColor;
                    //nextRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[currentRole + 1].roleColor;
                }
                else if (currentRole == _roleContainers.Length - 1)
                {
                    if (previousRoleButton_Text != null) previousRoleButton_Text.text = _roleContainers[currentRole - 1].roleName;
                    if (nextRoleButton_Text != null) nextRoleButton_Text.text = _roleContainers[0].roleName;
                    //previousRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[currentRole - 1].roleColor;
                    //nextRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[0].roleColor;
                }
                else
                {
                    if (previousRoleButton_Text != null) previousRoleButton_Text.text = _roleContainers[currentRole - 1].roleName;
                    if (nextRoleButton_Text != null) nextRoleButton_Text.text = _roleContainers[currentRole + 1].roleName;
                    //previousRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[currentRole - 1].roleColor;
                    //nextRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[currentRole + 1].roleColor;
                }
            }
            if (currentRoleButton_Text != null) currentRoleButton_Text.text = _roleContainers[currentRole].roleName;
            //currentRoleButton.GetComponentInChildren<TextMeshProUGUI>().color = _roleContainers[currentRole].roleColor;
        }

        //Forces a change to the next role. I'ts called from the next role button.
        public void _NextRole()
        {
            if (currentRole == _roleContainers.Length - 1)
            {
                currentRole = 0;
            }
            else
            {
                currentRole++;
            }
            roleChangeOverride = true;
        }

        //Forces a change to the previous role. I'ts called from the previous role button.
        public void _PreviousRole()
        {
            if (currentRole == 0)
            {
                currentRole = _roleContainers.Length - 1;
            }
            else
            {
                currentRole--;
            }
            roleChangeOverride = true;
        }

        //Does all the work neccesary to force a role change. 
        //Called when roleChangeOverride = true on any loop function stopping the loop and forcing a new one with the new role.
        public void RoleChangeOverride()
        {
            roleChangeOverride = false;
            _isFadeMid = true;
            _isScrollStarted = false;
            UpdateDisplayWithCurrentRole();
            UpdateButtons();
            alphaColor.a = 0;
            alphaColor2.a = .99f;
            if (screenTint != null) screenTint.color = alphaColor;
            //screenMaterial.SetColor("_EmissionColor",alphaColor);
            //roleList.color = alphaColor;
            //roleTitle.color = alphaColor;
            scrollRect.verticalNormalizedPosition = 1;
            lastFrameScrollPosition = 1;
            _fadeLoopTime = 0;
            waitTimeScrollTimer = 0;
            SendCustomEventDelayedFrames(nameof(__DoFadeLoop), 0);
        }

        #endregion
    }
}
