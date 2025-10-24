using UnityEngine;

public class ManagerIdleState : NeutralState
{
    
    public ManagerIdleState(NeutralNPC manager, Animator anim)
    {
        this.neutralNPC = manager;
        this.anim = anim;
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
