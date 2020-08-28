using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance = null;
    static public T Instance
    {
        get
        {
            if (_instance)
                return _instance;

            _instance = FindObjectOfType<T>();
            if (_instance)
                return _instance;

            GameObject go = new GameObject(nameof(T));
            _instance = go.AddComponent<T>();
            return _instance;
        }
    }
}
