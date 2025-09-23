using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralNPC : MonoBehaviour
{
    public EntityData entityData;
    protected PlayerStats stats;
    protected NeutralStateMachine nfsm;

    protected virtual void Awake()
    {
        stats = new PlayerStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);
    }

    protected virtual void CalculateBehaviour()
    {
        GameManager.GetInstance.BattleTurn.ChangeState();
        //TODO: 추후 배틀턴 구분 변수 생기면 구분 지어줘야 함.!!!.!.
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
