using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GunType
{
    HandGun,
    SubMachineGun,
    AssaultRifle,
    SniperRifle,
    ShotGun
}

[CreateAssetMenu(fileName = "GunData", menuName = "New GunData")]
public class GunData : ScriptableObject
{
    public string gunName;
    public string gunDescription;

    public GunType type;

    public int maxRounds;
    //public int useRoundsPerShot;    //한 번 쏠 때 쓰는 탄환 양
    public int bulletPerOneShot;        //격발 시 발사되는 탄환수
    public int noise;
    public float damagePerOneBulletMultiplier; //발당 데미지: 2d6 * 계수, 총데미지: 2d6 * 계수 * 탄환수
    //public int minBullerSpread;         //최소 탄 퍼짐 정도
    //public int maxBulletSpread;         // min = 1, max =3 이라면 명중치가 -1 ~ -3 사이 값을 적용된다.

    public int firstRange;
    public int firstRangeAccuracy;      //범위의 명중률보정치 (음수의 형태로)
    public int secondRange;
    public int secondRangeAccuracy;
    public int thirdRange;
    public int thirdRangeAccuracy;
    public int awayRangeAccuracy;

}