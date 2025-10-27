using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeDefines;

public class Gun : MonoBehaviour
{
    [Tooltip("에너미는 할당해주고, 플레이어는 할당하지 읺기")]
    public GunData data;

    [Header("현재 총기 정보")]
    public string gunName;
    public string gunDescription;
    public GunType type;
    public int maxRounds;
    public int curRounds;
    //public int useRoundsPerShot;
    public int bulletPerOneShot;
    public int noise;
    public float damagePerOneBulletMultiplier;
    //public int minBullerSpread;         //최소 탄 퍼짐 정도
    //public int maxBulletSpread;         // min = 1, max =3 이라면 명중치가 -1 ~ -3 사이 값을 적용된다.
    public int firstRange;
    public int firstRangeAccuracy;
    public int secondRange;
    public int secondRangeAccuracy;
    public int thirdrange;
    public int thirdrangeAccuracy;
    public int awayRangeAccuracy;

    private int ishit = 0;
    public bool makeNoise = false;


    private void Awake()
    {
        if (data != null)
            SetGun(data);
    }

    public void SetGun(GunData gunData)
    {
        gunName = gunData.gunName;
        gunDescription = gunData.gunDescription;
        type = gunData.type;
        maxRounds = gunData.maxRounds;
        curRounds = maxRounds;
        //useRoundsPerShot = gunData.useRoundsPerShot;
        bulletPerOneShot = gunData.bulletPerOneShot;
        noise = gunData.noise;
        damagePerOneBulletMultiplier = gunData.damagePerOneBulletMultiplier;
        //minBullerSpread = gunData.minBullerSpread;
        //maxBulletSpread = gunData.maxBulletSpread;
        firstRange = gunData.firstRange;
        firstRangeAccuracy = gunData.firstRangeAccuracy;
        secondRange = gunData.secondRange;
        secondRangeAccuracy = gunData.secondRangeAccuracy;
        thirdrange = gunData.thirdRange;
        thirdrangeAccuracy = gunData.thirdRangeAccuracy;
        awayRangeAccuracy = gunData.awayRangeAccuracy;
    }

    public void Reload()
    {
        curRounds = maxRounds;
    }

    /// <summary>
    /// 행동 판정까지 들어감 그러니까 따로 다이스 굴려서 안 해도 됨
    /// </summary>
    /// <param name="targetPos"></param>
    /// <param name="hitBonus"></param>
    public void Shoot(Vector3Int targetPos, int hitBonus)
    {
        if(!ConsumeRounds(1/*useRoundsPerShot*/))
        {
            Debug.Log("잔탄수 부족, 불발");
            return;
        }

        EntityStats entityStats = GameManager.GetInstance.GetEntityAt(targetPos);

        if (entityStats == null)
        {
            Debug.Log("검출되는 엔티티가 없음");
            return;
        }

        Debug.Log($"{entityStats.characterName}에게 격발 데미지를 가했음");
        for(int i = 0; i < bulletPerOneShot; i++)
        {
            if(CheckBulletHit(targetPos, hitBonus))
            {
                int result = DiceManager.GetInstance.DirrectRoll(0, 6, 2);
                entityStats.Damaged(result * damagePerOneBulletMultiplier);
                Debug.Log($"{i+1}번째 격발 결과\n{entityStats.characterName}에게 {result * damagePerOneBulletMultiplier} 데미지를 가함 \n남은 HP: {entityStats.CurHp}");
                ishit++;
            }
            else
            {
                Debug.Log($"{i + 1}번째 격발 결과\n불발");
            }
        }

        if (ishit >= 1)
        {
            makeNoise = true;
        }
    }

    //public int GetBulletSpread()
    //{
    //    return Random.Range(minBullerSpread, maxBulletSpread) * (-1);
    //}


    public bool CheckBulletHit(Vector3Int targetPos, int hitBonus)
    {
        int hitAdjustment;
        if (CheckRange(targetPos, firstRange))
        {
            hitAdjustment = firstRangeAccuracy;
        }
        else if (CheckRange(targetPos, secondRange))
        {
            hitAdjustment = secondRangeAccuracy;
        }
        else if (CheckRange(targetPos, thirdrange))
        {
            hitAdjustment = thirdrangeAccuracy;
        }
        else
        {
            hitAdjustment = awayRangeAccuracy;
        }

        hitAdjustment += hitBonus;
        if(GameManager.GetInstance.GetEntityAt(targetPos) != null)
        {
            if (DiceManager.GetInstance.DirrectRoll(0, 6, 3) + hitAdjustment - GameManager.GetInstance.GetEntityAt(targetPos).evasionRate > 0)
                return true;
            else return false;
        }
        else
            return false;
    }
    public bool CheckRange(Vector3Int Pos, int range)
    {
        Vector3Int start = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector3Int current = start + new Vector3Int(x, 0, z);

                Node node = GameManager.GetInstance.GetNode(current);
                if (node == null || !node.isWalkable)
                    continue;

                if (current == Pos) return true;
            }
        }
        return false;
    }

    public bool ConsumeRounds(int needRounds)
    {
        if (curRounds >= needRounds)
        {
            curRounds -= needRounds;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 그냥 탄환이 남아있는지 확인하는 용도
    /// </summary>
    /// <returns></returns>
    public bool CheckAmmo()
    {
        return curRounds > 0;
    }

}
