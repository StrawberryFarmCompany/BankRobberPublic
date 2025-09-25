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
    private BuffColorType colorType;
    public BuffColorType ColorType { get { return colorType; } }
}
public enum BuffColorType { green,red,yellow,cyan,none}
public enum BuffType { moveBonus,rallyCopCall,rallySecCall,securityLevel,aiming,witness,spotted,healBan,heal}