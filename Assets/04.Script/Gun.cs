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

    public enum ShooterSide
    {
        Player,
        Enemy
    }

    public struct FireOutcome
    {
        public bool Fired;
        public bool Hit;        //명중 여부
        public int Penalty;     //거리별 패널티
    }

    public class Gun
    {
        public int MaxAmmo { get; }             //탄창 최대 용량
        public int Ammo { get; private set; }   //현재 남은 탄 수

        public float playerMaxRange = 20f;
        public float enemyMaxRange = 9f;

        public float rangeEpsilon = 0.25f;      //경계선 여유

        public Gun(int maxAmmo, bool startFull = true)
        {
            MaxAmmo = Mathf.Max(1, maxAmmo);
            Ammo = startFull ? MaxAmmo : 0;
        }

        public void Reload() => Ammo = MaxAmmo;

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

        public static float TileDistance(Vector3Int a, Vector3Int b)
        {
            return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
        }

        public static int ComputeRangePenalty(float tileDistance, float eps = 0.25f)
        {
            if (tileDistance <= 5f + eps) return 0;
            if (tileDistance <= 9f + eps) return 2;
            return 5;
        }

        public FireOutcome FireAtTiles_NoDice(
            Vector3Int shooterTile, //사수 타일 좌표
            Vector3Int targetTile,  //타깃 타일 좌표
            PlayerStats shooter,    //사수 스탯
            PlayerStats target,     //타깃 스탯
            ShooterSide side        //플레이어 / 적 사거리 분기
        )
        {
            var outcome = new FireOutcome { Fired = false, Hit = false, Penalty = 0 };

            if (shooter == null || target == null) return outcome;

            //사거리 계산 & 사이드별 최대 사거리 적용
            float dist = TileDistance(shooterTile, targetTile);
            //진영에 따른 사거리 선택
            float maxRange = (side == ShooterSide.Player) ? playerMaxRange : enemyMaxRange;
            //사거리 밖 발사 실패
            if (dist > maxRange + rangeEpsilon) return outcome;

            //탄약 소모
            var r = Shoot(autoReload: true);
            //탄 없음 실패
            if (!r.Fired) return outcome;

            outcome.Fired = true;

            //거리에 따른 패널티 계산
            int penalty = ComputeRangePenalty(dist, rangeEpsilon);
            outcome.Penalty = penalty;

            int score = shooter.accuracyModifier - penalty - target.evasionRate;
            outcome.Hit = (score >= 0);

            return outcome;
        }
    }
}