using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeLoad : MonoBehaviour
{
    private void Awake()
    {
        if (ResourceManager.GetInstance.IsLoaded)
        {

        }
        Destroy(gameObject);
    }
}
