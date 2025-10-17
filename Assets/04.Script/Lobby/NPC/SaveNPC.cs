using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum SceneType
{
    MainScene,
    PuzzleScene
}

[System.Serializable]
public class ObjectData
{
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ;
    public string prefabName; // 어떤 프리팹인지
}

[System.Serializable]
public class ObjectDataList
{
    public List<ObjectData> objects = new List<ObjectData>();
}

public class SaveNPC : MonoBehaviour
{
    private string savePath => Application.persistentDataPath;

    public void Save(GameObject obj, SceneType scene)
    {
        string filePath = $"{savePath}/save{scene}.json";
        ObjectDataList dataList;

        // 기존 데이터 불러오기
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            dataList = JsonUtility.FromJson<ObjectDataList>(existingJson);
        }
        else
        {
            dataList = new ObjectDataList();
        }

        // 새 데이터 추가
        ObjectData data = new ObjectData();
        data.posX = obj.transform.position.x;
        data.posY = obj.transform.position.y;
        data.posZ = obj.transform.position.z;
        data.rotX = obj.transform.eulerAngles.x;
        data.rotY = obj.transform.eulerAngles.y;
        data.rotZ = obj.transform.eulerAngles.z;
        data.prefabName = obj.name.Replace("(Clone)", "");

        dataList.objects.Add(data);

        // 다시 저장
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(filePath, json);
    }

    public List<GameObject> Load(SceneType scene)
    {
        string filePath = $"{savePath}/save{scene}.json";
        if (!File.Exists(filePath)) return null;

        string json = File.ReadAllText(filePath);
        ObjectDataList dataList = JsonUtility.FromJson<ObjectDataList>(json);

        List<GameObject> loadedObjects = new List<GameObject>();

        foreach (ObjectData data in dataList.objects)
        {
            GameObject prefab = Resources.Load<GameObject>("NPC/" + data.prefabName);
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab);
            obj.transform.position = new Vector3(data.posX, data.posY, data.posZ);
            obj.transform.eulerAngles = new Vector3(data.rotX, data.rotY, data.rotZ);

            loadedObjects.Add(obj);
        }

        return loadedObjects;
    }

    public void Remove(SceneType scene, GameObject prefab)
    {
        string prefabName = prefab.name.Replace("(Clone)", "");

        // 1) 씬에 있는 모든 NPC 탐색
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC"); // 태그를 붙여두면 더 안전함

        foreach (GameObject npc in npcs)
        {
            if (npc.name.Replace("(Clone)", "") == prefabName)
            {
                Destroy(npc); // 씬에서 제거
            }
        }

        // 2) JSON 세이브 파일에서도 제거
        string filePath = $"{savePath}/save{scene}.json";
        if (!File.Exists(filePath)) return;

        string json = File.ReadAllText(filePath);
        ObjectDataList dataList = JsonUtility.FromJson<ObjectDataList>(json);

        dataList.objects.RemoveAll(d => d.prefabName == prefabName);

        string newJson = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(filePath, newJson);
    }
}
