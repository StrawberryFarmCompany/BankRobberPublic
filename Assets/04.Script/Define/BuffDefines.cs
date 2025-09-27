namespace BuffDefine
{
    using System;
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
        public BuffData(ParsingBuffData data)
        {
            this.statusName = data.statusName;
            this.disStatusNameEng = data.disStatusNameEng;
            this.disStatusNameKor = data.disStatusNameKor;
            this.descEng = data.descEng;
            this.descKor = data.descKor;
            this.duration = data.duration;
            this.statusValue = data.statusValue;
            this.statusType = data.statusType;
            this.colorType = data.colorType;
        }
    }
    [Serializable]
    public class ParsingBuffData
    {
        public ushort key;//key로 ResourceManager의 buffdata에 키값등록
        public string statusName;
        public string disStatusNameEng;
        public string disStatusNameKor;
        public string descEng;//설명
        public string descKor;
        public int duration;
        public int statusValue;
        public BuffType statusType;
        public BuffColorType colorType;
    }
    public enum BuffColorType { green, red, yellow, cyan, none }
    public enum BuffType { moveBonus, rallyCopCall, rallySecCall, securityLevel, aiming, witness, spotted, healBan, heal }

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
    public class SecurityLevel : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        public SecurityLevel(BuffData data, EntityStats target)
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
    public class Spotted : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        public Spotted(BuffData data, EntityStats target)
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
}