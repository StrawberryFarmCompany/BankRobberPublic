using NodeDefines;
using UnityEngine;

public class NeutralNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected NeutralStateMachine nfsm;

    protected virtual void Awake()
    {
        stats = new EntityStats(entityData);

        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, GameManager.GetInstance.NoneBattleTurn.NPCDefaultEnterPoint);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.neutral, CalculateBehaviour);
    }

    protected virtual void Start()
    {
        stats.currNode = GameManager.GetInstance.GetNode(transform.position);
    }
    protected virtual void FixedUpdate()
    {
        stats.NodeUpdates(transform.position);
    }

    protected virtual void CalculateBehaviour()
    {
        if (GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { }, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));
        }
        else
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { }, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0f));
        }
    }


    protected virtual void Update()
    {
        
    }
}
