using FOW;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[System.Serializable]
public struct FloorRange
{
    public string name; // 예: B1, 1F, 2F
    public float minY;
    public float maxY;
}

public class FloorCullingManager : MonoBehaviour
{
    public static FloorCullingManager GetInstance;

    [Header("층 범위 리스트")]
    public List<FloorRange> floors = new List<FloorRange>();

    private Dictionary<EntityStats, FogOfWarRevealer3D> playerRevealers = new Dictionary<EntityStats, FogOfWarRevealer3D>();

    private LayerMask excludeLayerMask = (1<<11)+ (1<<8);

    private float lastUpdateTime = 0f;

    private Renderer[] allRenderers;
    private Collider[] allColliders;

    private void Awake()
    {
        GetInstance = this;
    }

    private void Start()
    {
        RefreshRenderersAndColliders();
        RegisterPlayerRevealer();
        UpdateCullingByCurrentPlayer();
    }

    /// <summary>
    /// 현재 플레이어의 위치를 기준으로 층별 Culling 적용
    /// </summary>
    public void UpdateCullingByCurrentPlayer()
    {
        Transform player = NodePlayerManager.GetInstance.GetCurrentPlayer()?.transform;
        if (player == null) return;

        float y = player.position.y;
        FloorRange? currentFloor = GetFloorByY(y);

        if (currentFloor == null)
        {
            // 해당되는 층이 없으면 전부 보이게
            EnableAllCollisionsAndRenderers();
            return;
        }

        ApplyCulling(currentFloor.Value);
    }

    /// <summary>
    /// 층 구간 리스트 중 Y값이 속하는 층 반환
    /// </summary>
    private FloorRange? GetFloorByY(float y)
    {
        foreach (var f in floors)
        {
            if (y >= f.minY && y < f.maxY)
                return f;
        }
        return null;
    }

    /// <summary>
    /// 렌더러 & 콜라이더 목록 갱신 (씬 오브젝트 추가 시 호출)
    /// </summary>
    public void RefreshRenderersAndColliders()
    {
        allRenderers = FindObjectsOfType<Renderer>(true)
            .Where(r => (excludeLayerMask.value & (1 << r.gameObject.layer)) == 0)
            .ToArray();

        allColliders = FindObjectsOfType<Collider>(true)
            .Where(c => (excludeLayerMask.value & (1 << c.gameObject.layer)) == 0)
            .ToArray();
        
    }

    /// <summary>
    /// 지정된 층 범위에 맞춰 렌더/충돌 활성화
    /// </summary>
    private void ApplyCulling(FloorRange floor)
    {
        if (allRenderers == null || allColliders == null)
            RefreshRenderersAndColliders();

        Transform currentPlayer = NodePlayerManager.GetInstance.GetCurrentPlayer()?.transform;

        // 렌더 처리
        foreach (var r in allRenderers)
        {
            if (r == null) continue;

            // 현재 플레이어 캐릭터는 항상 보이게
            if (currentPlayer != null && r.transform.root == currentPlayer.root)
            {
                r.enabled = true;
                continue;
            }

            float ry = r.transform.position.y;
            bool visible = (ry >= floor.minY && ry < floor.maxY);
            r.enabled = visible;
        }

        // 충돌 처리
        foreach (var c in allColliders)
        {
            if (c == null) continue;

            // 플레이어 캐릭터는 충돌 유지
            if (currentPlayer != null && c.transform.root == currentPlayer.root)
            {
                c.enabled = true;
                continue;
            }

            float cy = c.transform.position.y;
            bool active = (cy >= floor.minY && cy < floor.maxY);
            c.enabled = active;
        }

        // 플레이어 시야 처리
        foreach (var pr in playerRevealers.Values)
        {
            if (pr == null) continue;
            float py = pr.transform.position.y;
            bool revealActive = (py >= floor.minY && py < floor.maxY);
            if (revealActive)
                pr.ViewRadius = 13;
            else
                pr.ViewRadius = 0;
        }
    }

    /// <summary>
    /// 모든 렌더러와 충돌을 다시 켜기 (턴 종료 시 사용)
    /// </summary>
    public void EnableAllCollisionsAndRenderers()
    {
        if (allRenderers == null || allColliders == null)
            RefreshRenderersAndColliders();

        foreach (var r in allRenderers)
        {
            if (r == null) continue;
            r.enabled = true;
        }

        foreach (var c in allColliders)
        {
            if (c == null) continue;
            c.enabled = true;
        }
    }

    public void RegisterPlayerRevealer()
    {
        NodePlayerManager.GetInstance.GetAllPlayers().ForEach(p =>
        {
            var playerRevealer = p.GetComponent<FogOfWarRevealer3D>();
            if (playerRevealer != null && !playerRevealers.ContainsKey(p.playerStats))
                playerRevealers.Add(p.playerStats, playerRevealer);
        });
    }
}
