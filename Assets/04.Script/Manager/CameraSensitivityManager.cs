using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraSensitivityManager : MonoSingleTon<CameraSensitivityManager>
{
    public float MoveSpeed { get; private set; } = 0.1f;
    public float WasdSpeed { get; private set; } = 20f;
    public float RotateSpeed { get; private set; } = 45f;
    public float ScrollSpeed { get; private set; } = 0.1f;

    public event Action OnSensitivityChanged;

    protected override void Init()
    {
        LoadValues();
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
        MoveSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    public void SetWasdSpeed(float value)
    {
        WasdSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    public void SetRotateSpeed(float value)
    {
        RotateSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }

    public void SetScrollSpeed(float value)
    {
        ScrollSpeed = value;
        SaveValues();
        OnSensitivityChanged?.Invoke();
    }
}
