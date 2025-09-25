using NodeDefines;
using UnityEngine;

public class NeutralNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected NeutralStateMachine nfsm;
    public Node currNode;

    protected virtual void Awake()
    {
        stats = new EntityStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.neutral, CalculateBehaviour);
    }

    protected virtual void FixedUpdate()
    {
        currNode.RemoveCharacter(stats);
        currNode.AddCharacter(stats);
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
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
