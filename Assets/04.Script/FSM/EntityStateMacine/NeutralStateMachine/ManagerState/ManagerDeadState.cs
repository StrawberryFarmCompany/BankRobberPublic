using UnityEngine;

public class ManagerDeadState : NeutralState
{
    public ManagerNPC managerNPC;

    public ManagerDeadState(ManagerNPC managerNPC)
    {
        this.managerNPC = managerNPC;
    }

    public override void Enter()
    {
        Debug.Log("Manager 죽음");
        // TODO: 애니메이션 재생 -> 코루틴으로 딜레이 후 Destroy
        GameObject.Destroy(managerNPC.gameObject);
    }
}
