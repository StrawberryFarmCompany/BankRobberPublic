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

    [Header("음량 조절")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    private bool isInitialized = false;

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

        // 사운드
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", SoundManager.Instance.DefaultBGMVolume);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", SoundManager.Instance.DefaultSFXVolume);

        bgmSlider.value = bgmVolume;
        sfxSlider.value = sfxVolume;

        SoundManager.Instance.SetBGMVolume(bgmVolume);
        SoundManager.Instance.SetSFXVolume(sfxVolume);

        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        isInitialized = true;
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

        // 옵션창이 다시 켜질 때, 현재 볼륨 반영
        if (SoundManager.Instance == null) return;

        bgmSlider.SetValueWithoutNotify(SoundManager.Instance.GetBGMVolume());
        sfxSlider.SetValueWithoutNotify(SoundManager.Instance.GetSFXVolume());
    }

    private void OnDestroy()
    {
        if (CameraSensitivityManager.IsShuttingDown) return;
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

    private void OnBGMVolumeChanged(float value)
    {
        if (!isInitialized) return;
        SoundManager.Instance.SetBGMVolume(value);
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (!isInitialized) return;
        SoundManager.Instance.SetSFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
