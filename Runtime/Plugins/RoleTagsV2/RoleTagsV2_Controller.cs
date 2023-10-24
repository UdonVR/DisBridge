
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins.RoleTagsV2
{
    public class RoleTagsV2_Controller : DisBridgePlugin
    {
        public RoleTagsV2_Tag[] tags;
        public bool useHeadtracker;
        public Transform headTracker;

        public byte roleMode;

        public byte tagTransparency = 255;

        public RoleTagsV2_Tag localTag;

        //UI
        public Button hideMyTagLocal_Toggle;
        public GameObject hideMyTagLocal;
        public bool hideMyTagLocal_bool;
        private bool hideMyTagLocal_bool_local;

        public Button hideMyTagGlobal_Toggle;
        public GameObject hideMyTagGlobal;

        public GameObject hideAllTags;
        public bool hideAllTags_bool;

        public Color[] tagColors;

        public Transform tagButtonParent;
        public RoleTagV2_Button[] tagButtons;
        public GridLayoutGroup buttonGrid;
        public RectTransform buttonRect;
        public GameObject buttonParent;
        public GameObject loadingParent;

        public RoleContainer[] admins;
        private bool isAdmin = false;

        public bool IsAdmin => isAdmin;
        public bool hideButtons = true;

        void Start()
        {
            disBridge.AddPlugin(gameObject);

            int _i = disBridge.GetRoles().Length;
            Color32 _tmpColor;
            tagColors = new Color[_i];
            for (int i = 0; i < _i; i++)
            {
                _tmpColor = disBridge.GetRole(i).roleColor;
                tagColors[i] = new Color32(_tmpColor.r, _tmpColor.g, _tmpColor.b, tagTransparency);
            }

        }

        public void _UpdateGrid()
        {
            buttonGrid.enabled = true;
            buttonGrid.enabled = false;
        }

        public override void _UVR_Init()
        {
            //hasInitalized = true;
            
            buttonParent.SetActive(true);
            loadingParent.SetActive(false);

            switch (roleMode)
            {
                case 0: //all Roles
                    _roleContainers = disBridge.GetRoles();
                    break;
                case 1: //Staff
                    _roleContainers = disBridge.GetStaff();
                    break;
                case 2: //Supporters
                    _roleContainers = disBridge.GetSupporter();
                    break;
                case 3: // Selector
                    break;
            }
            
            tagButtons = new RoleTagV2_Button[_roleContainers.Length];
            for (int i = 0; i < tagButtons.Length - 1; i++)
            {
                Instantiate(tagButtonParent.GetChild(0).gameObject, tagButtonParent);
            }
            tagButtons = tagButtonParent.GetComponentsInChildren<RoleTagV2_Button>();
            buttonRect.sizeDelta = new Vector2(buttonGrid.cellSize.x,buttonGrid.cellSize.y * _roleContainers.Length);
            SendCustomEventDelayedFrames(nameof(_UpdateGrid),0);

            foreach (var _role in admins)
            {
                if (_role == null) continue;
                if (_role.IsMember(Networking.LocalPlayer))
                {
                    isAdmin = true;
                    break;
                }
            }
            
            if (Networking.LocalPlayer.isMaster)
            {
                _Init_Delay();
            }
            else
            {
                SendCustomEventDelayedSeconds(nameof(_Init_Delay),5f);
            }

            for (int i = 0; i < _roleContainers.Length; i++)
            {
                tagButtons[i]._InitButton(_roleContainers[i],hideButtons);
            }

            if (IsMemberInRoles(Networking.LocalPlayer))
            {
                hideMyTagLocal_Toggle.interactable = hideMyTagGlobal_Toggle.interactable = true;
            }
            else
            {
                
            }
        }

        public void _Init_Delay()
        {
            if (TryGetPlayerRoleContainer(Networking.LocalPlayer, out RoleContainer _tmpRoleContainer))
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    if (tags[i]._TryGetTag(_tmpRoleContainer)) { break; }
                }
            }
        }

        public override void PostLateUpdate()
        {
            if (useHeadtracker)
            {
                headTracker.position = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            }
            for (int i = 0; i < tags.Length; i ++)
            {
                tags[i].MoveToOwner();
            }
        }
        
        //UI Events
        public void _Toggle_hideMyTagLocal()
        {
            hideMyTagLocal_bool = !hideMyTagLocal_bool;
            hideMyTagLocal_bool_local = hideMyTagLocal_bool;
            localTag.UpdateTagDisplay();
            hideMyTagLocal.SetActive(hideMyTagLocal_bool);
        }

        public void _Toggle_hideMyTagGlobal()
        {
            localTag._ToggleTag();
            hideMyTagGlobal.SetActive(localTag.hideMyTagGlobal);
            
            hideMyTagLocal_bool = localTag.hideMyTagGlobal || hideMyTagLocal_bool_local;
            localTag.UpdateTagDisplay();
            hideMyTagLocal.SetActive(hideMyTagLocal_bool);
        }
        
        public void _Toggle_hideAllTags()
        {
            hideAllTags_bool = !hideAllTags_bool;
            foreach (var _var in tags)
            {
                _var.UpdateTagDisplay();
            }
            hideAllTags.SetActive(hideAllTags_bool);

            hideMyTagLocal_Toggle.interactable = hideMyTagGlobal_Toggle.interactable = !hideAllTags_bool;
        }

        public void _SetTag(int _role)
        {
            Debug.LogError("help 2");
            localTag._SetRole(_role);
            for (int i = 0; i < _roleContainers.Length; i++)
            {
                if (_role == i) continue;
                tagButtons[i].selector.SetActive(false);
            }
        }
    }
}