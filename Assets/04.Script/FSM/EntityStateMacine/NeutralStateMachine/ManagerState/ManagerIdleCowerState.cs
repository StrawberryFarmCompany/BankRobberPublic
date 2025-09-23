using UnityEngine;

public class ManagerIdleCowerState : NeutralState
{
    public ManagerNPC managerNPC;

    public ManagerIdleCowerState(NeutralNPC neutral)
    {
        managerNPC = neutral as ManagerNPC;
    }

    public override void Enter()
    {
        
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        
    }
}
