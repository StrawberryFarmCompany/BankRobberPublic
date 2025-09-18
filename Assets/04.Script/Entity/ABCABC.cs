using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABCABC : MonoBehaviour
{
    Action StartAction;
    private void Start()
    {
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction += ABC;
        StartAction?.Invoke();
    }

    public void ABC()
    {
        Debug.Log("abc");
    }
}
