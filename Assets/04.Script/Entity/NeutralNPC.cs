using NodeDefines;
using UnityEngine;

public class NeutralNPC : MonoBehaviour
{
    public EntityData entityData;
    protected PlayerStats stats;
    protected NeutralStateMachine nfsm;
    public Node node;

    protected virtual void Awake()
    {
        stats = new PlayerStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.neutral, CalculateBehaviour);
    }

    protected virtual void CalculateBehaviour()
    {
        if(GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState,0.1f));
        }

        else 
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0.1f));
        }

        //node.AddCharacter(stats);
        //node.RemoveCharacter(stats);
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
