using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Rook,
    Bishop,
    Knight
}

public class CharacterManager : MonoBehaviour
{
    private static CharacterManager _instance;
    public static CharacterManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new GameObject("CharacterManager").AddComponent<CharacterManager>();
            }
            return _instance;
        }
    }

    public Player player
    {
        get { return _Player; }
        set { _Player = value; }
    }
    private Player _Player;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            if(_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}
