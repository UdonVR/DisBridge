
using System;
using TMPro;
using UdonSharp;
using UdonVR.DisBridge;
using UdonVR.DisBridge.Plugins.RoleTagsV2;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class RoleTagsV2_Tag : UdonSharpBehaviour
{
    private bool isUsed;
    public RoleTagsV2_Controller tagController;

    public GameObject thisTag;

    //[UdonSynced] public bool isUsed = false;
    [UdonSynced] public int playerId = -1;
    [UdonSynced] public int roleContainer = -1;
    [UdonSynced] public bool hideMyTagGlobal = false;
    private RoleContainer _roleContainer;
    private VRCPlayerApi thisOwner;
    private int lastPlayer = -1;

    public Image[] tagTintables;

    public GameObject boosterObject;
    
    public GameObject iconObject;
    public Image tagRoleIcon;
    public TextMeshProUGUI tagText;

    public LookAtConstraint lookAt;

    public bool isLocalTag = false;

    private Color __tmpColor;
    public bool _TryGetTag(RoleContainer __roleContainer)
    {
        if (VRC.SDKBase.Utilities.IsValid(VRCPlayerApi.GetPlayerById(playerId)))
        {
            Debug.LogError($"[RoleTagsV2_Tag.33][{gameObject.name}]TryGetTag return False ");
            return false;
        }
        else
        {
            Debug.LogError($"[RoleTagsV2_Tag.38][{gameObject.name}]TryGetTag return True ");
            Networking.SetOwner(Networking.LocalPlayer,gameObject);
            _roleContainer = __roleContainer;
            roleContainer = _roleContainer.transform.GetSiblingIndex();
            thisOwner = Networking.LocalPlayer;
            playerId = thisOwner.playerId;
            tagController.localTag = this;
            isLocalTag = true;
            RequestSerialization();
            SetupTag();
        }
        return true;
    }

    public void UpdateTagDisplay()
    {
        thisTag.SetActive(ShowTag());
    }
    
    public void _ToggleTag()
    {
        if (Networking.LocalPlayer != thisOwner) return;
        hideMyTagGlobal = !hideMyTagGlobal;
        thisTag.SetActive(ShowTag());
        RequestSerialization();
    }

    public override void OnDeserialization()
    {
        if (playerId == -1 || roleContainer == -1)
        {
            ResetTag();
            return;
        }
        thisOwner = VRCPlayerApi.GetPlayerById(playerId);
        if (!VRC.SDKBase.Utilities.IsValid(thisOwner))
        {
            ResetTag();
            return;
        }

        //if (lastPlayer != playerId)
        //{
        //    lastPlayer = playerId;
        //    
        //}
        _roleContainer = tagController.disBridge.GetRole(roleContainer);
        SetupTag();
        thisTag.SetActive(ShowTag());
    }

    public void MoveToOwner()
    {
        if (!isUsed) return;
        if (!VRC.SDKBase.Utilities.IsValid(thisOwner)) { ResetTag(); return; }
        transform.position = thisOwner.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
    }

    public void _SetRole(int _role)
    {
        if (Networking.LocalPlayer != thisOwner) return;
        roleContainer = _role;
        _roleContainer = tagController._roleContainers[_role];
        RequestSerialization();
        SetupTag();
    }
    
    private bool ShowTag()
    {
        if (!isUsed) return false;
        if (tagController.hideAllTags_bool) return false;
        return isLocalTag ? !tagController.hideMyTagLocal_bool : !hideMyTagGlobal;
        
        // if (isLocalTag)
        // {
        //     return !tagController.hideMyTagLocal_bool;
        //     //if (tagController.hideMyTagLocal_bool) return false;
        //     //return true;
        // }
        // else
        // {
        //     return hideTag;
        // }
        
    }
    private void ResetTag()
    {
        if (!isUsed) return;
        isUsed = false;
        thisTag.SetActive(ShowTag());
        iconObject.SetActive(false);
        boosterObject.SetActive(false);
        lookAt.constraintActive = false;
        if (Networking.IsOwner(gameObject))
        {
            playerId = -1;
            roleContainer = -1;
            RequestSerialization();
        }
    }
    private void SetupTag()
    {
        isUsed = true;
        thisTag.SetActive(ShowTag());
        tagText.text = String.IsNullOrWhiteSpace(_roleContainer.roleAltName) ? _roleContainer.roleName : _roleContainer.roleAltName;
        lookAt.constraintActive = true;
        __tmpColor = tagController.tagColors[_roleContainer.transform.GetSiblingIndex()];
        for (int i = 0; i < tagTintables.Length; i++)
        {
            tagTintables[i].color = __tmpColor;
            //tagTintables[i].GraphicUpdateComplete();
        }

        if (_roleContainer.roleIcon != null)
        {
            iconObject.SetActive(true);
            tagRoleIcon.sprite = _roleContainer.roleIcon;
        }
        else
        {
            iconObject.SetActive(false);
        }
        boosterObject.SetActive(tagController.disBridge.IsBooster(thisOwner));
    }
}
