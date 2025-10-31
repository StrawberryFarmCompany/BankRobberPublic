using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenDeadState : NeutralState
{

    public CitizenDeadState(NeutralNPC citizen, Animator anim)
    {
        this.neutralNPC = citizen;
        this.anim = anim;
    }
    public override void Enter()
    {
        //Destroy 쓰면 바로 삭제 되니 애니메이션 먼저 실행되게 해주기
        anim.Play("Dead_Fwd");
        //GameObject.Destroy(neutralNPC.gameObject);
    }
}
