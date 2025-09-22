using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemy : MonoBehaviour
{
    public EntityData baseData;
    private PlayerStats stats;
    private EnemyStateMachine efsm;

    private void Awake()
    {
        // 직원마다 독립된 스탯 생성
        stats = new PlayerStats(baseData);

        // 상태머신 초기화 (기본 상태)
        efsm = new EnemyStateMachine(EnemyStates.PatrolEnemyPatrolState);
    }

    private void Update()
    {
        efsm.Current.Execute(); // 현재 상태 실행
    }

    public void Patrol()
    {
        
    }
}
