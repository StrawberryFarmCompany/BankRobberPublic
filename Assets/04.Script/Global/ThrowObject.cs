using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Throws
{
    Coin,
    Granade,
    FlashBang,
    Smoke
}

public class ThrowObject : MonoBehaviour
{
    [SerializeField] Throws throws;
    private Vector3 endPos;
    private float duration;
    private float height;
    //private GameObject hitEffectPrefab;

    public void Init(Vector3 end, float duration, float height /*, GameObject hitEffect*/)
    {
        this.endPos = end;
        this.duration = duration;
        this.height = height;
        //this.hitEffectPrefab = hitEffect;

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float yOffset = Mathf.Sin(Mathf.PI * t) * height;
            Vector3 pos = Vector3.Lerp(start, endPos, t);
            pos.y += yOffset;
            transform.position = pos;

            yield return null;
        }

        transform.position = endPos;

        Vector3Int noisePos = Vector3Int.RoundToInt(endPos);
        NoiseManager.AddNoise(noisePos, NoiseType.ThrowCoin);
        
        OnHit();
    }

    private void OnHit()
    {
        //if (hitEffectPrefab != null)
        //    Instantiate(hitEffectPrefab, endPos, Quaternion.identity);

        // 투척물이 지속형 오브젝트인지 여부에 따라 처리 다르게
        // 설치형이면 그대로 두고, 일회성(돌/수류탄)이면 파괴

        //효과 넣어주자.
    }
}
