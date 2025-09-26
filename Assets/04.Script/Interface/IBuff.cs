using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    public BuffData Data { get;}
    public EntityStats Target { get;}
    
    public void RegistBuff();
    public void ReleaseBuff();
    /*public static IBuff Factory(BuffData data,BuffType type)
    {

    }*/
}
