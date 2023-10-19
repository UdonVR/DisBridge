//Copyright (c) 2021 UdonVR LLC

using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SupporterPanel : UdonSharpBehaviour
    {
        [SerializeField] private PluginManager manager;
        [SerializeField] private string separator = "</color> | {color}";

        [SerializeField] private string boosterColor = "<color=#f47fff>";
        [SerializeField] private string supporterColor = "<color=#3498db>";

        [SerializeField] private string boosterHeader = "<size=2>{color}Server Boosters</size>\n<size=1.5>------------------------------------------------------------</size></color>\n<size=1.5>";
        [SerializeField] private TextMeshProUGUI boosterText;

        [SerializeField] private string supporterHeader = "<size=2>{color}Supporter</size>\n<size=1.5>------------------------------------------------------------</size></color>\n<size=1.5>";
        [SerializeField] private TextMeshProUGUI supporterText;

        [SerializeField] private string supporterPpHeader = "<size=2.5>{color}Supporter++</size>\n<size=1.5>------------------------------------------------------------</size></color>\n<size=2>";
        [SerializeField] private TextMeshProUGUI supporterPpText;

        [SerializeField] private string supporterSharpHeader = "<size=3>{color}Supporter#</size>\n<size=1.5>------------------------------------------------------------</size></color>\n<size=2.5>";
        [SerializeField] private TextMeshProUGUI supporterSharpText;

        [SerializeField] private string supporterObjHeader = "<size=4>{color}Obj-Supporter</size>\n<size=1.5>------------------------------------------------------------</size></color>\n<size=3>";
        [SerializeField] private TextMeshProUGUI supporterObjText;

        private void Start()
        {
            manager.AddPlugin(gameObject);
        }

        public void _Init()
        {
            WriteSign();
        }
        
        private void WriteSign()
        {
            string _str = boosterHeader.Replace("{color}", boosterColor);
            string _separator = separator.Replace("{color}", boosterColor);
            string[] _tmp;
            
            //Boosters
            _tmp = manager.GetBoosterDisplaynames();
            Array.Reverse(_tmp);
            _str = _str + boosterColor + _tmp[0];
            for (int i = 1; i < _tmp.Length; i++)
            {
                _str = _str + _separator + _tmp[i];
            }
            boosterText.text = _str;

            //Supporter
            _separator = separator.Replace("{color}", supporterColor);
            _str = supporterHeader.Replace("{color}", supporterColor);
            _tmp = manager.GetRoleDisplaynames(6);
            Array.Reverse(_tmp);
            _str = _str + supporterColor + _tmp[0];
            for (int i = 1; i < _tmp.Length; i++)
            {
                _str = _str + _separator + _tmp[i];
            }
            supporterText.text = _str;

            //SupporterPp
            _separator = separator.Replace("{color}", supporterColor);
            _str = supporterPpHeader.Replace("{color}", supporterColor);
            _tmp = manager.GetRoleDisplaynames(5);
            Array.Reverse(_tmp);
            _str = _str + supporterColor+ _tmp[0];
            for (int i = 1; i < _tmp.Length; i++)
            {
                _str = _str + _separator + _tmp[i];
            }
            supporterPpText.text = _str;

            //SupporterSharp
            _separator = separator.Replace("{color}", supporterColor);
            _str = supporterSharpHeader.Replace("{color}", supporterColor);
            _tmp = manager.GetRoleDisplaynames(4);
            Array.Reverse(_tmp);
            _str = _str + supporterColor+ _tmp[0];
            for (int i = 1; i < _tmp.Length; i++)
            {
                _str = _str + _separator + _tmp[i];
            }
            supporterSharpText.text = _str;

            //SupporterObj
            _separator = separator.Replace("{color}", supporterColor);
            _str = supporterObjHeader.Replace("{color}", supporterColor);
            _tmp = manager.GetRoleDisplaynames(3);
            Array.Reverse(_tmp);
            _str = _str + supporterColor+ _tmp[0];
            for (int i = 1; i < _tmp.Length; i++)
            {
                _str = _str + _separator + _tmp[i];
            }
            supporterObjText.text = _str;
        }
    }
}