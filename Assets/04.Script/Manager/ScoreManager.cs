using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class StageScoreData
{
    public SceneType sceneType;
    public bool isClear;
    public int score;
}

[Serializable]
public class ScoreSaveData
{
    public List<StageScoreData> stageScores = new List<StageScoreData>();
}

public class ScoreManager : SingleTon<ScoreManager>
{
    private Dictionary<SceneType, StageScoreData> stageDataDict = new Dictionary<SceneType, StageScoreData>();
    private string savePath;

    protected override void Init()
    {
        savePath = Application.persistentDataPath + "/scores.json";
        LoadScores();
    }

    public void AddScore(SceneType sceneType, int amount)
    {
        if (!stageDataDict.ContainsKey(sceneType))
        {
            stageDataDict[sceneType] = new StageScoreData
            {
                sceneType = sceneType,
                isClear = false,
                score = 0
            };
        }

        stageDataDict[sceneType].score += amount;
        SaveScores();
    }

    public int GetScore(SceneType sceneType)
    {
        return stageDataDict.TryGetValue(sceneType, out var data) ? data.score : 0;
    }

    public bool IsStageCleared(SceneType sceneType)
    {
        return stageDataDict.TryGetValue(sceneType, out var data) && data.isClear;
    }

    public void SetStageClear(SceneType sceneType, bool clear)
    {
        if (!stageDataDict.ContainsKey(sceneType))
        {
            stageDataDict[sceneType] = new StageScoreData
            {
                sceneType = sceneType,
                score = 0
            };
        }
        stageDataDict[sceneType].isClear = clear;
        SaveScores();
    }

    public void SaveScores()
    {
        var saveData = new ScoreSaveData();
        foreach (var pair in stageDataDict)
        {
            saveData.stageScores.Add(pair.Value);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"스테이지 점수 저장 완료 → {savePath}");
    }

    public void LoadScores()
    {
        stageDataDict.Clear();

        if (!File.Exists(savePath))
        {
            Debug.Log("저장된 점수가 없어 새로 생성합니다.");
            foreach (SceneType type in Enum.GetValues(typeof(SceneType)))
            {
                if (type.ToString().StartsWith("Stage"))
                    stageDataDict[type] = new StageScoreData
                    {
                        sceneType = type,
                        score = 0,
                        isClear = false
                    };
            }
            SaveScores();
            return;
        }

        string json = File.ReadAllText(savePath);
        var saveData = JsonUtility.FromJson<ScoreSaveData>(json);

        foreach (var data in saveData.stageScores)
        {
            stageDataDict[data.sceneType] = data;
            Debug.Log($"{data.sceneType} = {data.score}, Clear: {data.isClear}");
        }

        Debug.Log("스테이지 점수 불러오기 완료");
    }
}
