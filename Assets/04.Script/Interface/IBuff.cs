using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuff
{
    public BuffData data { get; set; }
    public EntityStats target { get; set; }
    
    public void RegistBuff();
    public void ReleaseBuff();
    /*public static IBuff Factory(BuffData data,BuffType type)
    {

    }*/
}
