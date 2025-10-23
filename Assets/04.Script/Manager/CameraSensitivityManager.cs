using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSensitivityManager : MonoSingleTon<CameraSensitivityManager>
{
    public float MoveSpeed { get; private set; } = 0.1f;
    public float WasdSpeed { get; private set; } = 20f;
    public float RotateSpeed { get; private set; } = 45f;
    public float ScrollSpeed { get; private set; } = 0.1f;

    public event Action OnSensitivityChanged;

    private static bool isShuttingDown = false;
    public static bool IsShuttingDown => isShuttingDown;

    protected override void Init()
    {
        LoadValues();

        Application.quitting += () =>
        {
            isShuttingDown = true;
        };
    }

    private void LoadValues()
    {
        MoveSpeed = PlayerPrefs.GetFloat("MoveSpeed", 0.1f);
        WasdSpeed = PlayerPrefs.GetFloat("WasdSpeed", 20f);
        RotateSpeed = PlayerPrefs.GetFloat("RotateSpeed", 45f);
        ScrollSpeed = PlayerPrefs.GetFloat("ScrollSpeed", 0.1f);
    }

    private void SaveValues()
    {
        PlayerPrefs.SetFloat("MoveSpeed", MoveSpeed);
        PlayerPrefs.SetFloat("WasdSpeed", WasdSpeed);
        PlayerPrefs.SetFloat("RotateSpeed", RotateSpeed);
        PlayerPrefs.SetFloat("ScrollSpeed", ScrollSpeed);
        PlayerPrefs.Save();
    }

    public void SetMoveSpeed(float value)
    {
        if (isShuttingDown) return;
        MoveSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    public void SetWasdSpeed(float value)
    {
        if (isShuttingDown) return;
        WasdSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    public void SetRotateSpeed(float value)
    {
        if (isShuttingDown) return;
        RotateSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    public void SetScrollSpeed(float value)
    {
        if (isShuttingDown) return;
        ScrollSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    private void OnDestroy()
    {
        isShuttingDown = true;
    }
}
