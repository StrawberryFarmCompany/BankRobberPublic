using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    public EntityData entityData;
    protected PlayerStats stats;
    protected EnemyStateMachine efsm;
    public NavMeshAgent agent;

    protected virtual void Awake()
    {
        stats = new PlayerStats(entityData);
        agent = GetComponent<NavMeshAgent>();
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);
    }
    protected virtual void CalculateBehaviour()
    {
        GameManager.GetInstance.BattleTurn.ChangeState();
        //TODO: 추후 배틀턴 구분 변수 생기면 구분 지어줘야 함.!!!.!.
    }
}