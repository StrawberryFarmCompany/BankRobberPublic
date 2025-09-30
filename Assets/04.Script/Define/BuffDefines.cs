namespace BuffDefine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class BuffData
    {
        private ushort key;
        public ushort Key { get { return key; } }
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
            key = data.key;
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

        private int duration;
        public MoveBonus(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
            target.movementSpeed += Data.StatusValue;
            target.movement += Data.StatusValue;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.movementSpeed -= Data.StatusValue;
            target.movement -= Data.StatusValue;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class RallyCopCall : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public RallyCopCall(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class RallySecCall : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public RallySecCall(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class SecurityLevel : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        public EntityStats Target { get { return null; } }
        public SecurityLevel(BuffData data, EntityStats target)
        {
            this.data = data;
        }

        public void RegistBuff()
        {

        }

        public void ReleaseBuff()
        {
            ushort level = 0;
            if (data.StatusValue < ushort.MaxValue)
            {
                level = (ushort)data.StatusValue;
            }
            else
            {
                level = 3;
            }
            GameManager.GetInstance.securityData.SetSecLevel(level);
        }

        public void CountDuration()
        {
            
        }
    }
    public class Aiming : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public Aiming(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
            
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class Witness : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public Witness(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class Spotted : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public Spotted(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class HealBan : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public HealBan(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    }
    public class Heal : IBuff
    {
        private BuffData data;
        public BuffData Data { get { return data; } }
        private EntityStats target;
        public EntityStats Target { get { return target; } }

        private int duration;
        public Heal(BuffData data, EntityStats target)
        {
            this.data = data;
            this.target = target;
        }

        public void RegistBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount += CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += CountDuration;
        }

        public void ReleaseBuff()
        {
            GameManager.GetInstance.BattleTurn.BuffCount -= CountDuration;
            GameManager.GetInstance.NoneBattleTurn.BuffCount -= CountDuration;
            target.RemoveBuff(this);
        }

        public void CountDuration()
        {
            duration -= 1;
            if (duration <= 0)
            {
                ReleaseBuff();
            }
        }
    } 
}