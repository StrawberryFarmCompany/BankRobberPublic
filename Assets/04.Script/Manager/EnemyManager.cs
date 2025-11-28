using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<EnemyNPC> enemies = new List<EnemyNPC>();
    private int doneCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, StartEnemyTurn);
    }

    public void RegisterEnemy(EnemyNPC npc)
    {
        if (!enemies.Contains(npc))
            enemies.Add(npc);
    }

    // Enemy 턴 시작할 때 호출
    public void StartEnemyTurn()
    {
        doneCount = 0;

        // 모든 enemy의 isEndTurn 초기화
        foreach (var e in enemies)
        {
            if (e == null) continue;
            e.isEndTurn = false;
            e.isMoving = false;
            e.stats.ResetForNewTurn();
            e.stats.NodeUpdates(e.transform.position);

            e.TakeTurn();
        }
    }

    // 한 Enemy의 행동이 끝날 때 EnemyNPC가 호출함
    public void ReportEnemyDone(EnemyNPC npc)
    {
        doneCount++;

        // 전부 행동 끝났으면 턴 종료
        if (doneCount >= enemies.Count)
        {
            EndEnemyTurn();
        }
    }

    private void EndEnemyTurn()
    {
        // 여기서 턴 넘기기
        Debug.Log("모든 Enemy 행동 끝 -> Enemy 턴 종료");

        TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));

        // 소음 초기화
        NoiseManager.ClearNoises();
    }
}
