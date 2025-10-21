using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageScoreData
{
    public SceneType sceneType;
    public int score;
}

[Serializable]
public class ScoreSaveData
{
    public List<StageScoreData> stageScores = new List<StageScoreData>();
}

public class ScoreManager : SingleTon<ScoreManager>
{
    private Dictionary<SceneType, int> stageScores = new Dictionary<SceneType, int>();
    private string savePath;

    protected override void Init()
    {
        savePath = Application.persistentDataPath + "/scores.json";
        LoadScores();
    }

    public void AddScore(SceneType sceneType, int amount)
    {
        if (!stageScores.ContainsKey(sceneType))
            stageScores[sceneType] = 0;

        stageScores[sceneType] += amount;
        SaveScores();
    }

    public int GetScore(SceneType sceneType)
    {
        return stageScores.TryGetValue(sceneType, out int score) ? score : 0;
    }

    public void SaveScores()
    {
        var saveData = new ScoreSaveData();

        foreach (var pair in stageScores)
        {
            saveData.stageScores.Add(new StageScoreData
            {
                sceneType = pair.Key,
                score = pair.Value
            });
        }

        string json = JsonUtility.ToJson(saveData, true);
        System.IO.File.WriteAllText(savePath, json);
        Debug.Log($"스테이지 점수 저장 완료 → {savePath}");
    }

    public void LoadScores()
    {
        if (!System.IO.File.Exists(savePath))
        {
            Debug.Log("저장된 점수가 없어 새로 생성합니다.");
            foreach (SceneType type in Enum.GetValues(typeof(SceneType)))
            {
                if (type.ToString().StartsWith("Stage"))
                    stageScores[type] = 0;
            }
            SaveScores();
            return;
        }

        string json = System.IO.File.ReadAllText(savePath);
        var saveData = JsonUtility.FromJson<ScoreSaveData>(json);

        stageScores.Clear();
        foreach (var data in saveData.stageScores)
        {
            stageScores[data.sceneType] = data.score;
            Debug.Log($"{data.sceneType} = {data.score}");
        }

        Debug.Log("스테이지 점수 불러오기 완료");
    }
}
