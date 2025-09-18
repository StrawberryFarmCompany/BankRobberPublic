using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRangeUI : MonoBehaviour
{
    public Transform player;
    public PlayerStats playerStats;

    public GameObject tilePrefab;
    public GameObject tilePrefab1;  //페널티 -1
    public GameObject tilePrefab2;  //페널티 -2

    public float radiusNoPenalty = 3.25f;
    public float radiusPenalty1 = 4.25f;
    public float radiusPenalty2 = 5.25f;

    private readonly List<GameObject> active = new();
    private readonly Queue<GameObject> pool0 = new();
    private readonly Queue<GameObject> pool1 = new();
    private readonly Queue<GameObject> pool2 = new();
    private readonly Dictionary<GameObject, int> bandOf = new();

    void OnDisable() => Hide();

    public void ShowFor(Transform t, PlayerStats stats)
    {
        player = t;
        playerStats = stats;
        Show();
    }

    public void Show()
    {
        if (!player || playerStats == null) return;
        if (!tilePrefab || !tilePrefab1 || !tilePrefab2) return;

        Hide();

        float r0 = Mathf.Max(0f, radiusNoPenalty);
        float r1 = Mathf.Max(r0, radiusPenalty1);
        float r2 = Mathf.Max(r1, radiusPenalty2);

        int scanR = Mathf.CeilToInt(r2);
        var c = new Vector3Int(Mathf.RoundToInt(player.position.x), 0, Mathf.RoundToInt(player.position.z));

        float r0sq = r0 * r0;
        float r1sq = r1 * r1;
        float r2sq = r2 * r2;

        for (int dz = -scanR; dz <= scanR; dz++)
            for (int dx = -scanR; dx <= scanR; dx++)
            {
                float d2 = dx * dx + dz * dz;
                if (d2 > r2sq) continue;

                int band = (d2 <= r0sq) ? 0 : (d2 <= r1sq ? 1 : 2);

                var cell = new Vector3Int(c.x + dx, 0, c.z + dz);
                var go = Get(band);

                // 고정 오프셋(0.04f)로 살짝 띄워서 Z-fighting 방지
                go.transform.position = new Vector3(cell.x, player.position.y, cell.z);

                go.SetActive(true);
                active.Add(go);
                bandOf[go] = band;
            }
    }

    public void Hide()
    {
        for (int i = 0; i < active.Count; i++)
        {
            var g = active[i];
            if (!g) continue;

            g.SetActive(false);

            if (!bandOf.TryGetValue(g, out int band)) band = 0;
            switch (band)
            {
                case 0: pool0.Enqueue(g); break;
                case 1: pool1.Enqueue(g); break;
                default: pool2.Enqueue(g); break;
            }
        }
        active.Clear();
        bandOf.Clear();
    }

    public void Refresh()
    {
        Hide();
        Show();
    }

    GameObject Get(int band)
    {
        Queue<GameObject> pool = (band == 0) ? pool0 : (band == 1 ? pool1 : pool2);
        while (pool.Count > 0)
        {
            var g = pool.Dequeue();
            if (g) return g;
        }
        var prefab = (band == 0) ? tilePrefab : (band == 1 ? tilePrefab1 : tilePrefab2);
        return Instantiate(prefab);
    }
}
