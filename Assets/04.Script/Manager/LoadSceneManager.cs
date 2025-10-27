using UnityEngine.SceneManagement;
using UnityEngine;

public enum SceneType
{
    MainTitle,
    LobbyScene,
    TutorialScene,
    Stage01Scene,
    Stage02Scene,
    Stage03Scene,
    Stage04Scene
}

public class LoadSceneManager : SingleTon<LoadSceneManager>
{
    public SceneType curSceneType;

    // 각 스테이지 진입 조건 점수
    public int stage02RequireScore = 90000;
    public int stage03RequireScore = 90000;
    public int stage04RequireScore = 90000;

    public void SceneLoad(SceneType sceneType)
    {
        Time.timeScale = 1f;
        curSceneType = sceneType;
        SceneManager.LoadScene(SceneTypeToString(sceneType));
    }

    public void StageSceneLoad(SceneType sceneType)
    {
        if (CheckEntryCondition(sceneType))
        {
            Time.timeScale = 1f;
            curSceneType = sceneType;
            SceneManager.LoadScene(SceneTypeToString(sceneType));
        }
        else
        {
            Debug.Log($"{sceneType} 으로 넘어갈 조건이 충족되지 않았습니다.");
        }
    }

    private string SceneTypeToString(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.MainTitle: return "MainTitle";
            case SceneType.LobbyScene: return "LobbyScene";
            case SceneType.TutorialScene: return "ProtoTypeScene";
            case SceneType.Stage01Scene: return "ProtoTypeScene";
            case SceneType.Stage02Scene: return "Stage02Scene";
            case SceneType.Stage03Scene: return "Stage03Scene";
            case SceneType.Stage04Scene: return "Stage04Scene";
            default:
                Debug.Log("그딴 씬은 없다");
                return "null";
        }
    }

    public bool CheckEntryCondition(SceneType sceneType)
    {
        var scoreManager = ScoreManager.GetInstance;

        switch (sceneType)
        {
            case SceneType.Stage01Scene:
                return true;

            case SceneType.Stage02Scene:
                return CheckOrUnlockStage(SceneType.Stage01Scene, SceneType.Stage02Scene, stage02RequireScore);

            case SceneType.Stage03Scene:
                return CheckOrUnlockStage(SceneType.Stage02Scene, SceneType.Stage03Scene, stage03RequireScore);

            case SceneType.Stage04Scene:
                return CheckOrUnlockStage(SceneType.Stage03Scene, SceneType.Stage04Scene, stage04RequireScore);

            default:
                return true;
        }
    }

    private bool CheckOrUnlockStage(SceneType prevStage, SceneType targetStage, int requireScore)
    {
        var scoreManager = ScoreManager.GetInstance;

        // 이미 클리어된 스테이지면 바로 입장 가능
        if (scoreManager.IsStageCleared(targetStage))
            return true;

        // 이전 스테이지 점수가 기준 이상이라면 클리어 처리 후 입장 가능
        if (scoreManager.GetScore(prevStage) >= requireScore)
        {
            scoreManager.SetStageClear(targetStage, true);
            Debug.Log($" {targetStage} 해금 완료!");
            return true;
        }

        // 조건 미달
        return false;
    }

    public int GetRequireScore(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Stage01Scene: return 0;
            case SceneType.Stage02Scene: return stage02RequireScore;
            case SceneType.Stage03Scene: return stage03RequireScore;
            case SceneType.Stage04Scene: return stage04RequireScore;
            default: return 0;
        }
    }
}
