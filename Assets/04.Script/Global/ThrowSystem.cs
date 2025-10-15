using UnityEngine;
using System.Collections;

public class ThrowSystem : MonoBehaviour
{
    private static ThrowSystem instance;

    public static ThrowSystem GetInstance
    {
        get
        {
            return instance;
        }
    }


    [SerializeField] private GameObject Coin;
    [SerializeField] private float throwDuration = 0.5f;
    [SerializeField] private float throwHeight = 2f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ExecuteCoinThrow(Vector3 start, Vector3Int targetNode)
    {
        Vector3Int startPos = GameManager.GetInstance.GetVecInt(start + Vector3.up * 1f);
        Vector3Int endPos = targetNode;

        GameObject throws = Instantiate(Coin, startPos, Quaternion.identity);

        // 투척물 애니메이션은 ProjectileBehaviour에서 처리
        throws.GetComponent<ThrowObject>()
                  .Init(endPos, throwDuration, throwHeight);
    }
}
