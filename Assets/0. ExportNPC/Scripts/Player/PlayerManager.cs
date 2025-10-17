//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerManager : MonoBehaviour
//{
//    private static PlayerManager _instance;
//    public static PlayerManager Instance
//    {
//        get
//        {
//            if (_instance == null)
//            {
//                Debug.Log("어디야");
//                _instance = new GameObject("PlayerManager").AddComponent<PlayerManager>();
//            }
//            return _instance;
//        }

//    }

//    public Player Player
//    {
//        get { return _player; }
//        set { _player = value; }
//    }
//    private Player _player;

//    void Awake()
//    {
//        if (_instance == null)
//        {
//            _instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//}
