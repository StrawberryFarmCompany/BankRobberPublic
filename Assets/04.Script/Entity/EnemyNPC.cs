using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNPC : MonoBehaviour
{
    public EntityData entityData;
    protected PlayerStats stats;
    protected EnemyStateMachine efsm;

    protected virtual void Awake()
    {
        stats = new PlayerStats(entityData);
    }
}