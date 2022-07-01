
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

namespace UdonVR.DisBridge.Plugins
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RoleTags : UdonSharpBehaviour
    {
        [SerializeField] private PluginManager manager;
        
        public Transform lookAtTarget;
        public float OffsetVal = .75f;
        
        [Header("Tag Stuff")]
        public Transform[] tags;
        private GameObject[] serverBooster;
        private TextMeshProUGUI[] tagText;
        private VRCPlayerApi[] players;
        private bool[] isUsing;

        private bool hasInitalized = false;

        private RoleContainer[] roles;
        
        private string[] boosters;
        
        private void Start()
        {
            manager.AddPlugin(gameObject);
        }
        
        public void _Init()
        {
            Debug.Log("[UdonVR]Supporter Tags Init...");
            boosters = manager.GetBoosterDisplaynames();
            players = new VRCPlayerApi[tags.Length];
            tagText = new TextMeshProUGUI[tags.Length];
            serverBooster = new GameObject[tags.Length];
            isUsing = new bool[tags.Length];

            VRCPlayerApi[] _tmpPlayers = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
            VRCPlayerApi.GetPlayers(_tmpPlayers);

            roles = manager.GetRoles();

            //Debug.Log($"[UdonVR] Init Roles is" + (roles != null ? roles.Length + " long" : "Null"));

            foreach (var _player in _tmpPlayers)
            {
                CheckTag(_player);
            }

            hasInitalized = true;
        }

        public override void PostLateUpdate()
        {
            if (!hasInitalized) return;
            lookAtTarget.position = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            for (int i = 0; i < tags.Length; i++)
            {
                if (isUsing[i])
                {
                    if (!Utilities.IsValid(players[i]))
                    {
                        Debug.LogError("[UdonVR]Player is null, returning");
                        return;
                    }

                    Vector3 tmp = players[i].GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
                    tags[i].position = new Vector3(tmp.x, tmp.y + OffsetVal, tmp.z);
                }
            }
        }

        public void _OpenTag(VRCPlayerApi _player, string _val, bool _isBooster)
        {
            if (!string.IsNullOrEmpty(_player.GetPlayerTag("isTagged")))
            {
                Debug.Log("[UdonVR] Player " + _player.displayName +" is already tagged, returning.");
                return;
            }
            
            _player.GetPlayerTag("isTagged");
            
            int _curTag = 0;
            bool emptyTag = true;
            for (int i = 0; i < tags.Length; i++)
            {
                if (!isUsing[i])
                {
                    _curTag = i;
                    emptyTag = false;
                    break;
                }
            }
            
            if (emptyTag)
            {
                GameObject _obj = VRCInstantiate(tags[0].gameObject);
                _obj.transform.parent = tags[0].parent;
                _curTag = _obj.transform.GetSiblingIndex();

                isUsing = AddBoolToArray(true, isUsing);
                tags = AddTransformToArray(_obj.transform, tags);
                tagText = AddTMProToArray(tags[_curTag].GetComponentInChildren<TextMeshProUGUI>(),tagText);
                serverBooster = AddGameObjectToArray(tags[_curTag].GetChild(0).gameObject, serverBooster);
                players = AddPlayerToArray(_player, players);
            }
            else
            {
                Debug.Log("[UdonVR] Setting player " + _player.displayName +" as player: " +_curTag);
                tagText[_curTag] = tags[_curTag].GetComponentInChildren<TextMeshProUGUI>();
                serverBooster[_curTag] = tags[_curTag].GetChild(0).gameObject;
                isUsing[_curTag] = true;
                players[_curTag] = _player;
            }
            
            tags[_curTag].gameObject.SetActive(true);
            tagText[_curTag].text = _val;
            serverBooster[_curTag].SetActive(_isBooster);
            _player.SetPlayerTag("isTagged","1");
        }
        public void _CloseTag(int _tag)
        {
            Debug.Log("[UdonVR] Closing tag: " +_tag);
            isUsing[_tag] = false;
            tags[_tag].gameObject.SetActive(false);
            tagText[_tag].text = string.Empty;
            serverBooster[_tag].SetActive(false);
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (hasInitalized) CheckTag(player);
        }

        public void CheckTag(VRCPlayerApi player)
        {
            string _username = player.displayName;
            //Debug.Log($"[UdonVR] CheckTag Roles is"+(roles != null? roles.Length+" long" :"Null"));
            for (int i = 0; i < roles.Length; i++)
            {
                string[] _members = roles[i].GetMembers();
                for (int j = 0; j < _members.Length; j++)
                {
                    if (_username == _members[j])
                    {
                        _OpenTag(player,roles[i].roleAltName,IsBooster(_username));
                        return;
                    }
                }
            }
        }

        public override void OnPlayerLeft(VRCPlayerApi _player)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (Utilities.IsValid(players[i]) && players[i] == _player)
                {
                    _CloseTag(i);
                    break;
                }
            }
        }

        private bool IsBooster(string _playerName)
        {
            foreach (var _booster in boosters)
            {
                if (_playerName == _booster)
                {
                    return true;
                }
            }
            return false;
        }
        
        
        //Array
        
        private bool[] AddBoolToArray(bool _val, bool[] _array)
        {
            //for (int i = 0; i < _array.Length; i++)
            //{
            //    _tmpArray[i] = _array[i];
            //}
            bool[] _tmpArray = new bool[0];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new bool[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        private Transform[] AddTransformToArray(Transform _val, Transform[] _array)
        {
            Transform[] _tmpArray = new Transform[0];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new Transform[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        private TextMeshProUGUI[] AddTMProToArray(TextMeshProUGUI _val, TextMeshProUGUI[] _array)
        {
            TextMeshProUGUI[] _tmpArray = new TextMeshProUGUI[0];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new TextMeshProUGUI[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        private GameObject[] AddGameObjectToArray(GameObject _val, GameObject[] _array)
        {
            GameObject[] _tmpArray = new GameObject[0];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new GameObject[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        private VRCPlayerApi[] AddPlayerToArray(VRCPlayerApi _val, VRCPlayerApi[] _array)
        {
            VRCPlayerApi[] _tmpArray = new VRCPlayerApi[0];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new VRCPlayerApi[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        
    }   
}
