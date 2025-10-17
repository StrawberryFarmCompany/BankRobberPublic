using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("3인칭 카메라 감도")]
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private Slider moveSlider;
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private Slider clickSlider;

    [Header("1인칭 카메라 감도")]
    [SerializeField] private Slider fpsLookSlider;
    [SerializeField] private List<PlayerController> fpsPlayers;
    [SerializeField] private float fpsMin = 0.01f;
    [SerializeField] private float fpsMax = 0.5f;

    private CameraSensitivityManager csmg;

    void Start()
    {
        csmg = CameraSensitivityManager.GetInstance;

        rotationSlider.value = csmg.RotateSpeed;
        moveSlider.value = csmg.WasdSpeed;
        zoomSlider.value = csmg.ScrollSpeed;
        clickSlider.value = csmg.MoveSpeed;

        rotationSlider.onValueChanged.AddListener(csmg.SetRotateSpeed);
        moveSlider.onValueChanged.AddListener(csmg.SetWasdSpeed);
        zoomSlider.onValueChanged.AddListener(csmg.SetScrollSpeed);
        clickSlider.onValueChanged.AddListener(csmg.SetMoveSpeed);

        if (fpsLookSlider != null)
        {
            fpsLookSlider.minValue = fpsMin;
            fpsLookSlider.maxValue = fpsMax;

            float init = GetFirstFpsSensitivityOrDefault(1f);
            fpsLookSlider.value = init;
            ApplyFpsToAll(init);

            fpsLookSlider.onValueChanged.AddListener(v => ApplyFpsToAll(v));
        }
    }

    void OnEnable()
    {
        if (csmg == null) csmg = CameraSensitivityManager.GetInstance;

        rotationSlider.value = csmg.RotateSpeed;
        moveSlider.value = csmg.WasdSpeed;
        zoomSlider.value = csmg.ScrollSpeed;
        clickSlider.value = csmg.MoveSpeed;

        if (fpsLookSlider != null)
        {
            float cur = GetFirstFpsSensitivityOrDefault(fpsLookSlider.value);
            fpsLookSlider.SetValueWithoutNotify(Mathf.Clamp(cur, fpsMin, fpsMax));
            ApplyFpsToAll(cur);
        }
    }

    private void OnDestroy()
    {
        if (rotationSlider) rotationSlider.onValueChanged.RemoveAllListeners();
        if (moveSlider) moveSlider.onValueChanged.RemoveAllListeners();
        if (zoomSlider) zoomSlider.onValueChanged.RemoveAllListeners();
        if (clickSlider) clickSlider.onValueChanged.RemoveAllListeners();
        if (fpsLookSlider) fpsLookSlider.onValueChanged.RemoveAllListeners();
    }

    float GetFirstFpsSensitivityOrDefault(float def)
    {
        if (fpsPlayers != null)
            foreach (var p in fpsPlayers)
                if (p != null) return p.lookSensitivity;
        return def;
    }

    void ApplyFpsToAll(float v)
    {
        v = Mathf.Clamp(v, fpsMin, fpsMax);
        if (fpsPlayers == null) return;
        foreach (var p in fpsPlayers)
            if (p != null) p.SetLookSensitivity(v);
    }
}
