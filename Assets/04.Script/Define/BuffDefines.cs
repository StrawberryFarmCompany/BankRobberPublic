using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffData
{
    private string statusName;
    public string StatusName { get { return statusName; } }
    private string disStatusNameEng;
    public string DisStatusNameEng { get { return disStatusNameEng; } }
    private string disStatusNameKor;
    public string DisStatusNameKor { get { return disStatusNameKor; } }
    private string descEng;//설명
    public string DescEng { get { return descEng; } }//설명
    private string descKor;
    public string DescKor { get { return descKor; } }
    private int duration;
    public int Duration { get { return duration; } }
    private int statusValue;
    public int StatusValue { get { return statusValue; } }
    private BuffType statusType;
    public BuffType StatusType { get { return statusType; } }
    private BuffColorType colorType;
    public BuffColorType ColorType { get { return colorType; } }
}
public enum BuffColorType { green,red,yellow,cyan,none}
public enum BuffType { moveBonus,rallyCopCall,rallySecCall,securityLevel,aiming,witness,spotted,healBan,heal}

public class MoveBonus : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public MoveBonus(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }

    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class RallyCopCall : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public RallyCopCall(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class RallySecCall : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public RallySecCall(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class RecurityLevel : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public RecurityLevel(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class Aiming : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public Aiming(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class Witness : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public Witness(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class SpottedBonus : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public SpottedBonus(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class HealBan : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public HealBan(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}
public class Heal : IBuff
{
    private BuffData data;
    public BuffData Data { get { return data; } }
    private EntityStats target;
    public EntityStats Target { get { return target; } }

    public Heal(BuffData data, EntityStats target)
    {
        this.data = data;
        this.target = target;
    }
    public void RegistBuff()
    {
        
    }

    public void ReleaseBuff()
    {
        
    }
}