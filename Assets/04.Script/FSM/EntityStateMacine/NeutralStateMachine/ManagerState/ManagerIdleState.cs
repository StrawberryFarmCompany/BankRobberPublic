using UnityEngine;

public class ManagerIdleState : NeutralState
{
    public ManagerNPC managerNPC;
    
    public ManagerIdleState(NeutralNPC neutral)
    {
        managerNPC = neutral as ManagerNPC;
    }

    public override void Enter()
    {
        Debug.Log("Manager 대기 상태");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Debug.Log("Manager 대기 상태 종료");
    }
}
