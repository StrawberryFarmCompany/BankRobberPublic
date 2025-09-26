using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuffDefine
{

    public interface IBuff
    {
        public BuffData Data { get; }
        public EntityStats Target { get; }

        public void RegistBuff();
        public void ReleaseBuff();
        public static IBuff Factory(BuffData data, EntityStats stat, BuffType type)
        {
            switch (type)
            {
                case BuffType.moveBonus:
                    return new MoveBonus(data, stat);
                case BuffType.rallyCopCall:
                    return new RallyCopCall(data, stat);
                case BuffType.rallySecCall:
                    return new RallySecCall(data, stat);
                case BuffType.securityLevel:
                    return new SecurityLevel(data, stat);
                case BuffType.aiming:
                    return new Aiming(data, stat);
                case BuffType.witness:
                    return new Witness(data, stat);
                case BuffType.spotted:
                    return new Spotted(data, stat);
                case BuffType.healBan:
                    return new HealBan(data, stat);
                case BuffType.heal:
                    return new Heal(data, stat);
                default:
                    Debug.LogError("등록되지 않은 버프 이넘값입니다 메세지를 눌러 확인해 주세요");
                    return null;
            }
        }
    }

}