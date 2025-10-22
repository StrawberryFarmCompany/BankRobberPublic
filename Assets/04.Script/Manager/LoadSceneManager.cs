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
    // 각 스테이지 진입 조건 점수
    public int stage02RequireScore = 90000;
    public int stage03RequireScore = 90000;
    public int stage04RequireScore = 90000;

    public void SceneLoad(SceneType sceneType)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneTypeToString(sceneType));
    }

    public void StageSceneLoad(SceneType sceneType)
    {
        if (CheckEntryCondition(sceneType))
        {
            Time.timeScale = 1f;
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
            case SceneType.MainTitle:
                return "MainTitle";
            case SceneType.LobbyScene:
                return "LobbyNPCTEST";
            case SceneType.TutorialScene:
                return "MainScene";
                //return "TutorialScene";
            case SceneType.Stage01Scene:
                return "Stage01Scene";
            case SceneType.Stage02Scene:
                return "Stage02Scene";
            case SceneType.Stage03Scene:
                return "Stage03Scene";
            case SceneType.Stage04Scene:
                return "Stage04Scene";
            default:
                Debug.Log("그딴 씬은 없다");
                return "null";

        }
    }

    public bool CheckEntryCondition(SceneType sceneType)
    {
        // 각 스테이지 진입 조건
        switch (sceneType)
        {
            case SceneType.Stage01Scene:
                // 스테이지1은 항상 가능
                return true;

            case SceneType.Stage02Scene:
                // Stage01 점수가 기준 이상이어야 Stage02 진입 가능
                return ScoreManager.GetInstance.GetScore(SceneType.Stage01Scene) >= stage02RequireScore;

            case SceneType.Stage03Scene:
                // Stage02 점수가 기준 이상이어야 Stage03 진입 가능
                return ScoreManager.GetInstance.GetScore(SceneType.Stage02Scene) >= stage03RequireScore;

            case SceneType.Stage04Scene:
                // Stage03 점수가 기준 이상이어야 Stage04 진입 가능
                return ScoreManager.GetInstance.GetScore(SceneType.Stage03Scene) >= stage04RequireScore;

            default:
                // 나머지는 제한 없음
                return true;
        }
    }

    public int GetRequireScore(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Stage01Scene:
                return 0;
            case SceneType.Stage02Scene:
                return stage02RequireScore;
            case SceneType.Stage03Scene:
                return stage03RequireScore;
            case SceneType.Stage04Scene:
                return stage04RequireScore;
            default:
                return 0;
        }
    }
}
