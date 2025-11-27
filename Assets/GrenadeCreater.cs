using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrenadeCreater : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (ResourceManager.GetInstance.GetPreLoad.Count <= 0) yield return new WaitUntil(() => ResourceManager.GetInstance.GetPreLoad.ContainsKey("Thrower"));
        GrenadeThrower thrower = GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["Thrower"]).GetComponent<GrenadeThrower>();
        GetComponent<Button>().onClick.AddListener(() => { thrower.SetThrowingSequence(NodePlayerManager.GetInstance.GetCurrentPlayer()); });
        yield break;
    }
}
