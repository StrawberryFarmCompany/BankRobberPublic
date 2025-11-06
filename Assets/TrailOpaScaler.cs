using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailOpaScaler : MonoBehaviour
{
    [SerializeField] TrailRenderer trail;
    float time = 1f;
    private void Update()
    {
        time -= Time.deltaTime * 0.5f;
        trail.materials[0].SetFloat("_opacity", time);
        if (time <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnDisable()
    {
        trail.materials[0].SetFloat("_opacity", 0);
        time = 1f;
    }
    private void OnEnable()
    {
        trail.materials[0].SetFloat("_opacity", 1);
        trail.ResetBounds();
        time = 1f;
    }
}
