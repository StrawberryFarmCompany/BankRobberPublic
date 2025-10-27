using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CitizenFleeState : NeutralState
{
    public CitizenFleeState(NeutralNPC citizen, Animator anim)
    {
        this.neutralNPC = citizen;
        this.anim = anim;
    }
    public override void Enter()
    {
        anim.Play("HG_Move");
    }

    public override void Exit() 
    {
        anim.Play("HG_Idle_Pose");
    }
    
    //void OnAnimatorMove()
    //{
    //    if(anim)
    //    {
    //        for (int i=0; i < 5; i++)
    //        {
    //            neutralNPC.gameObject.transform.position += Vector3.forward * 1;
    //        }
    //    }
    //}

}
