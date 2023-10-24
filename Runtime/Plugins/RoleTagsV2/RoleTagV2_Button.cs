
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins.RoleTagsV2
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RoleTagV2_Button : UdonSharpBehaviour
    {
        public RoleTagsV2_Controller tagController;
        public TextMeshProUGUI buttonText;
        public Image buttonIcon;
        [SerializeField]private RoleContainer thisRoleContainer;
        public GameObject selector;
        public Button button;

        private bool hasInit = false;

        public void _InitButton(RoleContainer _roleContainer,bool _hideButton)
        {
            Debug.LogError($"{gameObject.name} {transform.GetSiblingIndex()}");
            if (hasInit) return;
            hasInit = true;
            Debug.LogError(_roleContainer.name);
            thisRoleContainer = _roleContainer;
            if (buttonIcon != null && thisRoleContainer.roleIcon != null)
            {
                buttonIcon.enabled = true;
                buttonIcon.sprite = thisRoleContainer.roleIcon;
            }
            buttonText.text = thisRoleContainer.roleName;

            
            
            if (thisRoleContainer.IsMember(Networking.LocalPlayer) || tagController.IsAdmin) { }
            else
            {
                if (_hideButton)
                {
                    gameObject.SetActive(false);
                }
                else
                {
                    button.interactable = false;
                }
                
            }
            Debug.LogError("_InitButton End");
        }

        public void _SetTag()
        {
            Debug.LogError("help me");
            if (thisRoleContainer.IsMember(Networking.LocalPlayer) || tagController.IsAdmin)
            {
                selector.SetActive(true);
                tagController._SetTag(transform.GetSiblingIndex());
            }
        }
    }
}