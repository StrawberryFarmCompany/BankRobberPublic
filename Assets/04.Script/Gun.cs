using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapons
{
    public struct ShootResult
    {
        public bool Fired;          //발사 여부
        public int Used;            //소모된 탄 수
        public bool AutoReloaded;   //재장전 여부
    }

    public class Gun
    {
        public int MaxAmmo { get; }             //탄창 최대 용량
        public int Ammo { get; private set; }   //현재 남은 탄 수
     
        public Gun(int maxAmmo, bool startFull = true)
        {
            MaxAmmo = Mathf.Max(1, maxAmmo);
            Ammo = startFull ? MaxAmmo : 0;
        }

        public void Reload()
        {
            Ammo = MaxAmmo;
        }

        //발사 가능 여부 (1발 이상)
        public bool CanShoot => Ammo > 0;

        //발사(1발 소모, 0발일 때 재장전 시도 후 발사)
        public ShootResult Shoot(bool autoReload = true)
        {
            var r = new ShootResult { Fired = false, Used = 0, AutoReloaded = false };

            //0발, 자동 재장전 옵션일 시 재장전
            if (Ammo == 0 && autoReload)
            {
                Reload();
                r.AutoReloaded = true;
            }

            //0발 이면 발사 실패
            if (Ammo <= 0) return r;

            //1발 소모, 발사 성공
            Ammo -= 1;
            r.Fired = true;
            r.Used = 1;
            return r;
        }
    }
}