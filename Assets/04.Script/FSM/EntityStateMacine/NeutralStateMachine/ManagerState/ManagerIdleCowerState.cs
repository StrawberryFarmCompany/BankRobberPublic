using UnityEngine;

public class ManagerIdleCowerState : NeutralState
{

    public ManagerIdleCowerState(NeutralNPC manager, Animator anim)
    {
        this.neutralNPC = manager;
        this.anim = anim;
    }

    public override void Enter()
    {
        anim.Play("Ducking_Idle");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        
    }
}
