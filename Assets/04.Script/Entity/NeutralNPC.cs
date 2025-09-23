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

    }
    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
