using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTon <T> where T : SingleTon<T>,new()
{
    private static T instance;
    public static T GetInstance 
    { 
        get 
        {
            if (instance == null)
            {
                instance = new T();
                instance.Init();
                //YOON : 추후 씬 전환 이벤트 등록 Reset함수
            }
            return instance;
        } 
    }
    /// <summary>
    /// 초기세팅
    /// </summary>
    protected virtual void Init()
    {

    }
    /// <summary>
    /// 씬 전환시 초기화될것들
    /// </summary>
    public virtual void Reset()
    {

    }
}
public class MonoSingleTon <T>  : MonoBehaviour where T : MonoSingleTon<T>,new()
{
    private static T instance;
    public static T GetInstance 
    { 
        get 
        {
            if (instance == null)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
                DontDestroyOnLoad(instance.gameObject);
                instance.Init();
                //YOON : 추후 씬 전환 이벤트 등록 Reset함수
            }
            return instance;
        } 
    }
    /// <summary>
    /// 초기세팅
    /// </summary>
    protected virtual void Init()
    {

    }
    /// <summary>
    /// 씬 전환시 초기화될것들
    /// </summary>
    public virtual void OnSceneChange()
    {

    }
}
