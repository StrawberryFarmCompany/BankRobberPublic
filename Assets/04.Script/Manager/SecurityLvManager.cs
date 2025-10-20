using System.Collections.Generic;
using UnityEngine;

public class SecurityLvManager : MonoBehaviour
{
    public static SecurityLvManager Instance;

    [Header("소환할 CopEnemy 프리팹")]
    [SerializeField] private GameObject copEnemyPrefab;

    [Header("소환 위치 후보들")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("소환할 적 수")]
    [SerializeField] private int spawnCount = 3;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SecurityData.OnBattlePhase += SpawnCopEnemies;
    }

    private void OnDisable()
    {
        SecurityData.OnBattlePhase -= SpawnCopEnemies;
    }

    private void SpawnCopEnemies()
    {
        if (copEnemyPrefab == null)
        {
            Debug.LogError("[SecurityLvManager] CopEnemy 프리팹이 지정되지 않음");
            return;
        }

        Debug.Log("[SecurityLvManager] 전투 페이즈 : 경찰 소환중");

        // 기존 소환된 적 제거 (중복 방지)
        foreach (var e in spawnedEnemies)
        {
            if (e != null)
            {
                Destroy(e);
            }
        }
        spawnedEnemies.Clear();

        // 지정된 스폰 포인트에서 CopEnemy 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 스폰 위치 후보가 있다면 그중 하나를 선택하고, 없으면 자기 자신(SecurityLvManager)의 위치 사용
            Transform spawnPos = spawnPoints.Length > 0 ? spawnPoints[i % spawnPoints.Length] : transform;

            GameObject enemy = Instantiate(copEnemyPrefab, spawnPos.position, Quaternion.identity);
            spawnedEnemies.Add(enemy);
        }
    }
}
