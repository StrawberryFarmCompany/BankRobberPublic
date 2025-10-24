using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using BuffDefine;
public class ResourceManager : SingleTon<ResourceManager>
{
    public bool IsLoaded { get { return preloaded != null && buffDatas != null; } }
    private Dictionary<string, object> preloaded;
    public Dictionary<string, object> GetPreLoad { get { return preloaded; } }
    private Dictionary<ushort, BuffData> buffDatas;
    public Dictionary<ushort, BuffData> GetBuffData { get { return buffDatas; } }

    protected override void Init()
    {
        base.Init();
        preloaded = new Dictionary<string, object>();
        buffDatas = new Dictionary<ushort, BuffData>();

        SetBuffData();
        PreLoadAsyncAll("PreLoad",null);
    }
    protected void SetBuffData()
    {
        LoadAsync<TextAsset>("BuffStatus", (cb) =>
        {
            DataWrapper<ParsingBuffData> data = JsonUtility.FromJson<DataWrapper<ParsingBuffData>>(cb.text);
            Dictionary<ushort, BuffData> tempData = new Dictionary<ushort, BuffData>();
            foreach (ParsingBuffData item in data.data)
            {
                tempData.Add(item.key, new BuffData(item));
            }
            buffDatas = tempData;
        }, false);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">받아올 오브젝트의 타입</typeparam>
    /// <param name="key">어드레서블 내 키값</param>
    /// <param name="callback">EX) (obj)=>{targetInstance = obj}</param>
    public void LoadAsync<T>(string key, Action<T> callback, bool isCaching = false)
    {
        if (key.Contains(".sprite"))
        {
            key = $"{key}[{key.Replace(".sprite", "")}]";
        }
        AsyncOperationHandle<T> infoAsyncOP = Addressables.LoadAssetAsync<T>(key);
        infoAsyncOP.Completed += (op) =>
        {

            callback?.Invoke(infoAsyncOP.Result);
            if (isCaching) Addressables.Release(infoAsyncOP);
        };
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">����Ÿ��</typeparam>
    /// <param name="label">Ÿ�� Ű��</param>
    /// <param name="callback">(obj)=>{targetInstance = obj}</param>
    public void LoadAsyncAll<T>(string label, Action<(string, T)[]> callback, bool isCaching = false)
    {
        var labelKeys = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        labelKeys.WaitForCompletion();

        Debug.Log(labelKeys.Result);
        if (labelKeys.Result.Count == 0) { Debug.LogError($"{label}���� ����ֽ��ϴ�."); callback.Invoke(null); }

        int doneCount = 0;

        (string, T)[] tempT = new (string, T)[labelKeys.Result.Count];
        for (int i = 0; i < tempT.Length; i++)
        {
            int curIndex = i;
            string curKey = labelKeys.Result[i].PrimaryKey;
            LoadAsync<T>(labelKeys.Result[i].PrimaryKey, (result) =>
            {
                tempT[curIndex].Item1 = curKey;
                tempT[curIndex].Item2 = result;
                doneCount++;
                if (doneCount == labelKeys.Result.Count)
                {
                    callback?.Invoke(tempT);
                }
            }, isCaching);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="label">어드레서블 내 라벨 이름</param>
    /// <param name="callback"> 콜백으로 이후 처리</param>
    public void PreLoadAsyncAll(string label, Action<int, int> callback)
    {
        var oper = Addressables.LoadResourceLocationsAsync(label, typeof(object));
        oper.WaitForCompletion();
        if (oper.Result.Count == 0) { Debug.LogError($"{label}해당 라벨에 해당하는 오브젝트가 없습니다."); callback.Invoke(0, 0); }//�ش��ϴ� Ű�� ������� null�� ����

        int curr = 1;
        for (int i = 0; i < oper.Result.Count; i++)
        {
            string key = oper.Result[i].PrimaryKey;
            int max = oper.Result.Count;

            if (preloaded.ContainsKey(key))
            {
                callback?.Invoke(max, curr);
                curr++;
                continue;
            }

            LoadAsync<object>(key, (result) =>
            {
                callback?.Invoke(max, curr);
                curr++;
                if (result == null)
                {
                    Debug.Log("Ÿ���� �ùٸ��� ����");
                }
                else
                {
                    Debug.Log($"{key}프리로드 성공");

                    preloaded.Add(key, result);
                }
            }, false);
        }
    }
    #region Json
    private string GetSavePath() => Application.persistentDataPath;

    public bool SaveData<T>(T data, string fileName, bool isOverride)
    {
        string path = Path.Combine(GetSavePath(), fileName);
        string jsonStr = JsonUtility.ToJson(data);
        Debug.Log(path);
        if (File.Exists(path))
        {
            if (isOverride == true)
            {
                File.WriteAllText(path, jsonStr);
                return true;
            }
        }
        else
        {
            if (!File.Exists(GetSavePath()))
            {
                Directory.CreateDirectory(GetSavePath());
            }
            StreamWriter file = File.CreateText(path);
            file.WriteLine(jsonStr);
            return true;
        }
        return false;
    }
    public T LoadData<T>(string fileName) where T : new()
    {
        string path = Path.Combine(GetSavePath(), fileName);

        if (File.Exists(path))
        {
            T result;
            try
            {
                result = JsonUtility.FromJson<T>(File.ReadAllText(path));
            }
            catch
            {
                //�Ľ� ���н�
                result = default(T);
            }
            return result;
        }
        else
        {
            Directory.CreateDirectory(GetSavePath());
            return default(T);
        }
    }
    #endregion
}
public class DataWrapper<T>
{
    public T[] data;
}