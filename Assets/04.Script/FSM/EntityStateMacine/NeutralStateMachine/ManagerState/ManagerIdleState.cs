using UnityEngine;

public class ManagerIdleState : NeutralState
{
    Manager manager;

    public override void Enter()
    {
        
    }

    public override void Execute()
    {
        // Ally 발각 시 전환
        if (manager.CanSeeAlly)
        {
            manager.ChangeToIdleCower();
        }
    }

    public override void Exit()
    {
        
    }


}
