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
        //각 산출된 행동을 여기에 담아 실행TaskManager.GetInstance.AddTurnBehaviour(/*추가할 행동 함수*/);
    }
    protected virtual void CalculateBehaviour()
    {

    }
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
