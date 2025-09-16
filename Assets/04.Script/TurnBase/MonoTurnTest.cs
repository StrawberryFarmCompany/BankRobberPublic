using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoTurnTest : MonoBehaviour
{
    public TurnTypes type;
    void Start()
    {
        GameManager.GetInstance.Turn.AddStartPointer(type, OnTurnStart);
        GameManager.GetInstance.Turn.AddEndPointer(type, OnTurnEnd);
    }

    private void OnTurnStart()
    {
        Debug.Log($"{type}턴 시작");
    }
    private void OnTurnEnd()
    {
        Debug.Log($"{type}턴 종료");
    }
}
