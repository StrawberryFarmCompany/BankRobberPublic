using UnityEngine;

public class ManagerIdleCowerState : NeutralState
{
    public ManagerNPC managerNPC;

    public ManagerIdleCowerState(ManagerNPC managerNPC)
    {
        this.managerNPC = managerNPC;
    }

    public override void Enter()
    {
        
    }

    // 대미지 받았을 시
    public override void Execute()
    {
        managerNPC.TakeDamage();
    }

    public override void Exit()
    {
        
    }
}
