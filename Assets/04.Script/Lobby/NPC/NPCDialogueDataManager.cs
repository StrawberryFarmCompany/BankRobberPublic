using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NPCDialogueDataManager : MonoBehaviour
{
    public static NPCDialogueDataManager Instance;

    public Dictionary<ENPC, (int branch, int index)> npcDialogueStates = new Dictionary<ENPC, (int branch, int index)>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        npcDialogueStates = LoadNpcStates();
    }
    [System.Serializable]
    public class NpcState
    {
        public ENPC npc;
        public int branch;
        public int index;
    }

    [System.Serializable]
    public class NpcStateWrapper
    {
        public List<NpcState> states = new List<NpcState>();
    }

    private string SavePath => Path.Combine(Application.persistentDataPath, "npcStates.json");

    public void SaveNpcStates(Dictionary<ENPC, (int branch, int index)> inputData)
    {
        NpcStateWrapper wrapper = new NpcStateWrapper();
        foreach (var kvp in inputData)
        {
            wrapper.states.Add(new NpcState
            {
                npc = kvp.Key,
                branch = kvp.Value.branch,
                index = kvp.Value.index
            });
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(SavePath, json);
        npcDialogueStates = inputData;
    }

    public Dictionary<ENPC, (int branch, int index)> LoadNpcStates()
    {
        if (!File.Exists(SavePath)) return npcDialogueStates;

        string json = File.ReadAllText(SavePath);
        NpcStateWrapper wrapper = JsonUtility.FromJson<NpcStateWrapper>(json);

        npcDialogueStates.Clear();
        foreach (var state in wrapper.states)
        {
            npcDialogueStates[state.npc] = (state.branch, state.index);
        }
        return npcDialogueStates;
    }
}
