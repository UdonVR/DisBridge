
using System;
using TMPro;
using UdonSharp;
using UdonVR.DisBridge;
using UdonVR.DisBridge.Plugins;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    public class DisBridgeKeypad : UdonSharpBehaviour
    {
        public PluginManager manager;
        [Tooltip("Target whitelist will be unlocked if keycode is entered.")]
        public DisBridgeWhitelist_Class whitelist;

        [SerializeField] private bool updateFromBot = false;
        [SerializeField] private string modulePrefix = "keyc";

        [SerializeField] private int _keyCode;
        private int _keyCode_temp;
        private int _keyCode_legnth;
        private string _keyCode_user;

        [Tooltip("TextMeshPro text used for the keypad view.")]
        public TextMeshProUGUI view;
        [Tooltip("GameObject used for the placeholder view when the keypad has nothing entered.")]
        public GameObject placeholderView;

        [Tooltip("Auto enters the code if the length of the input code matches the length of the actual code.")]
        public bool autoEnter = true;

        [Tooltip("Defines if the local user's code entry should be censored.")]
        public bool showCode = false;

        [Tooltip("Char that is used to hide the password if `Show Code` is false.")]
        public char hideChar = '•';

        private string hiddenCode = "";
        private string _tmpStr = "";

        private bool unlocked = false;
        
        ////////
        //Auto Unlock
        public bool UnlockIfWhitelisted = true;
        public string[] RoleID;
        public int[] RoleIdex;

        public bool checkStaff = false;
        public bool checkSupporters = false;
        ///////


        [Tooltip("These are OFF by default\nWhen a player's name matches the list, these will TURN ON.")]
        public GameObject[] TargetsDefaultOff;

        [Tooltip("These are ON by default\nWhen a player's name matches the list, these will TURN OFF.")]
        public GameObject[] TargetsDefaultOn;

        private void Start()
        {
            if (_keyCode == 0) _keyCode = -1;
            _keyCode_legnth = _keyCode.ToString().Length;
            if (whitelist != null) whitelist._BindKeypad(this);
            if (manager != null) manager.AddPlugin(gameObject);
            
            foreach (GameObject _obj in TargetsDefaultOn)
            {
                if (_obj != null)
                    _obj.SetActive(true);
            }

            foreach (GameObject _obj in TargetsDefaultOff)
            {
                if (_obj != null)
                    _obj.SetActive(false);
            }
        }

        private String _keyCode1_temp;
        public void _UVR_Init()
        {
            if (!updateFromBot) return;
            if (manager.TryGetModule(modulePrefix,ref _keyCode_temp))
            {
                _keyCode_temp = Mathf.Abs(_keyCode_temp);
                _keyCode = _keyCode_temp;
                _keyCode_legnth = _keyCode.ToString().Length;
            }
        }

        private void TryUnlock()
        {
            if (UnlockCheck())
            {
                _Unlocked();
                return;
            }

            if (string.IsNullOrWhiteSpace(_keyCode_user))
            {
                AddChar(-1);
                placeholderView.SetActive(false);
                view.text = "DENIED";
            }
            else
            {
                if (int.TryParse(_keyCode_user, out int _tmp))
                {
                    if (_tmp == _keyCode)
                    {
                        _Unlocked();
                    }
                }
                else
                {
                    AddChar(-1);
                    placeholderView.SetActive(false);
                    view.text = "DENIED";
                }
            }
        }

        private void _Unlocked()
        {
            unlocked = true;
            placeholderView.SetActive(false);
            view.text = "OPEN";
            UnlockWhitelist();
            if (whitelist != null)
                whitelist._ForceUnlock(whitelist.name);
        }
        
        private bool UnlockCheck()
        {
            if (UnlockIfWhitelisted)
            {
                if (checkStaff && manager.IsStaff()) return true;
                if (checkSupporters && manager.IsSupporter()) return true;
                
                if (RoleIdex != null && RoleIdex.Length != 0)
                {
                    for (int i = 0; i < RoleIdex.LongLength; i++)
                    {
                        if (manager.IsMember(RoleIdex[i],Networking.LocalPlayer))
                        {
                            return true;
                        }
                    }
                }
            
                if (RoleID != null && RoleID.Length != 0)
                {
                    for (int i = 0; i < RoleID.LongLength; i++)
                    {
                        if (manager.IsMember(RoleID[i],Networking.LocalPlayer))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void SetUnlock(bool _lock)
        {
            if (_lock)
            {
                unlocked = true;
                view.text = "OPEN";
                placeholderView.SetActive(false);
            }
            else
            {
                unlocked = false;
                view.text = "";
                placeholderView.SetActive(true);
            }
        }

        private void UpdateView()
        {
            hiddenCode = "";
            for (int i = 0; i < _keyCode_user.Length; i++)
            {
                hiddenCode = $"{hiddenCode}{hideChar}";
            }

            if (_keyCode_user != String.Empty)
            {
                view.text = showCode ? _keyCode_user : hiddenCode;
                placeholderView.SetActive(false);
            }
            else
            {
                view.text = "";
                placeholderView.SetActive(true);
            }

            if (autoEnter && _keyCode_user.Length == _keyCode_legnth)
            {
                TryUnlock();
            }
        }

        private void AddChar(int _num)
        {
            if (unlocked) return;
            _keyCode_user = _num == -1 ? "" : $"{_keyCode_user}{_num}";
            UpdateView();
        }

        private void UnlockWhitelist()
        {
            foreach (GameObject _obj in TargetsDefaultOn)
            {
                if (_obj != null)
                    _obj.SetActive(false);
            }

            foreach (GameObject _obj in TargetsDefaultOff)
            {
                if (_obj != null)
                    _obj.SetActive(true);
            }
        }

        #region Buttons

        public void _But_0()
        {
            AddChar(0);
        }

        public void _But_1()
        {
            AddChar(1);
        }

        public void _But_2()
        {
            AddChar(2);
        }

        public void _But_3()
        {
            AddChar(3);
        }

        public void _But_4()
        {
            AddChar(4);
        }

        public void _But_5()
        {
            AddChar(5);
        }

        public void _But_6()
        {
            AddChar(6);
        }

        public void _But_7()
        {
            AddChar(7);
        }

        public void _But_8()
        {
            AddChar(8);
        }

        public void _But_9()
        {
            AddChar(9);
        }

        public void _But_Clear()
        {
            AddChar(-1);
        }

        public void _But_Enter()
        {
            TryUnlock();
        }

        #endregion
    }
}