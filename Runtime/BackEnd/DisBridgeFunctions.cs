//Copyright (c) 2021 UdonVR LLC

using UdonSharp;
using UdonVR.DisBridge.Plugins;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace UdonVR.DisBridge
{
    public class DisBridgeFunctions : UdonSharpBehaviour
    {
        public static bool IsArrayNullOrEmpty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }

        #region AddToArray
public static GameObject[] AddToArray(GameObject _val, GameObject[] _array)
{
    GameObject[] _tmpArray = new GameObject[1];
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
public static DisBridgeKeypad[] AddToArray(DisBridgeKeypad _val, DisBridgeKeypad[] _array)
{
    DisBridgeKeypad[] _tmpArray = new DisBridgeKeypad[1];
    if (_array == null || _array.Length == 0)
    {
        _tmpArray[0] = _val;
    }
    else
    {
        _tmpArray = new DisBridgeKeypad[_array.Length + 1];
        _array.CopyTo(_tmpArray,0);
        _tmpArray[_tmpArray.Length - 1] = _val;
    }
    return _tmpArray;
}
        public static RoleContainer[] AddToArray(RoleContainer _val, RoleContainer[] _array)
        {
            RoleContainer[] _tmpArray = new RoleContainer[1];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new RoleContainer[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        public static VRCPlayerApi[] AddToArray(VRCPlayerApi _val, VRCPlayerApi[] _array)
        {
            VRCPlayerApi[] _tmpArray = new VRCPlayerApi[1];
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
        public static Transform[] AddToArray(Transform _val, Transform[] _array)
        {
            Transform[] _tmpArray = new Transform[1];
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
        public static string[] AddToArray(string _val, string[] _array)
        {
            string[] _tmpArray = new string[1];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new string[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        public static int[] AddToArray(int _val, int[] _array)
        {
            int[] _tmpArray = new int[1];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new int[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        public static float[] AddToArray(float _val, float[] _array)
        {
            float[] _tmpArray = new float[1];
            if (_array == null || _array.Length == 0)
            {
                _tmpArray[0] = _val;
            }
            else
            {
                _tmpArray = new float[_array.Length + 1];
                _array.CopyTo(_tmpArray,0);
                _tmpArray[_tmpArray.Length - 1] = _val;
            }
            return _tmpArray;
        }
        #endregion

    }
}