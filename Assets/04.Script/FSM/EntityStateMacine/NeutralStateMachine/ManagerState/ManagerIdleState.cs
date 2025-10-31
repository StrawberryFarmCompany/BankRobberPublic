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
        anim.Play("HG_Idle_Pose");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
    }
}
