using System.Collections.Generic;
using UnityEngine;

public class NeutralManager : MonoBehaviour
{
    public static NeutralManager Instance;

    private List<NeutralNPC> neutrals = new List<NeutralNPC>();
    private int doneCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.neutral, StartNeutralTurn);
    }

    public void RegisterNeutral(NeutralNPC npc)
    {
        if (!neutrals.Contains(npc))
            neutrals.Add(npc);
    }

    // Neutral 턴 시작할 때 호출
    private void StartNeutralTurn()
    {
        doneCount = 0;

        // 모든 neutral의 isEndTurn 초기화
        foreach (var n in neutrals)
        {
            n.isEndTurn = false;
            n.isMoving = false;
            n.stats.ResetForNewTurn();
            n.stats.NodeUpdates(n.transform.position);

            n.TakeTurn();
        }
    }

    // 한 Neutral의 행동이 끝날 때 NeutralNPC가 호출함
    public void ReportNeutralDone(NeutralNPC neutral)
    {
        doneCount++;

        // 전부 행동 끝났으면 턴 종료
        if (doneCount >= neutrals.Count)
        {
            EndNeutralTurn();
        }
    }

    private void EndNeutralTurn()
    {
        TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));
    }
}
