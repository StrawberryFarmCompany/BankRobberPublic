using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class NPCLoadScene : MonoBehaviour
{
    private string[] sceneNames = {
        "Test01",
        "Test02"
    };
    private bool isSceneName = false;
    public DialogueManager dialogueManager;

    private void Start()
    {
        //PlayerManager.Instance.Player.dialogueManager.npcLoadScene = this;
        dialogueManager.npcLoadScene = this;
    }

    public void LoadScene(string sceneName)
    {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.Log("중복씬");
            return;
        }
        foreach (string name in sceneNames)
        {
            Debug.Log($"{name} : {sceneName}");
            if (name == sceneName)
            {
                isSceneName = true;
                break;
            }
        }
        if (isSceneName)
        {
            Debug.Log($"Loading scene: {sceneName}");
            isSceneName = false;
            SceneManager.LoadScene(sceneName);
        }

    }
}
