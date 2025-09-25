using NodeDefines;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected EnemyStateMachine efsm;
    public Node currNode;

    protected virtual void Awake()
    {
        stats = new EntityStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);
    }

    protected virtual void FixedUpdate()
    {
        currNode.RemoveCharacter(stats);
        currNode.AddCharacter(stats);
    }

    protected virtual void CalculateBehaviour()
    {
        if (GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0.1f));
        }

        else
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0.1f));
        }
    }
}
