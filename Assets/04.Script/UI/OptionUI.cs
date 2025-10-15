using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionUI : MonoBehaviour
{
    [Header("카메라 감도")]
    [SerializeField] private Slider rotationSlider;
    [SerializeField] private Slider moveSlider;
    [SerializeField] private Slider zoomSlider;
    [SerializeField] private Slider clickSlider;

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
    }

    void OnEnable()
    {
        if (csmg == null) csmg = CameraSensitivityManager.GetInstance;

        rotationSlider.value = csmg.RotateSpeed;
        moveSlider.value = csmg.WasdSpeed;
        zoomSlider.value = csmg.ScrollSpeed;
        clickSlider.value = csmg.MoveSpeed;
    }

    private void OnDestroy()
    {
        rotationSlider.onValueChanged.RemoveAllListeners();
        moveSlider.onValueChanged.RemoveAllListeners();
        zoomSlider.onValueChanged.RemoveAllListeners();
        clickSlider.onValueChanged.RemoveAllListeners();
    }
}
