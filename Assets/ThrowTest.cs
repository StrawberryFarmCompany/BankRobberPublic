using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTest : MonoBehaviour
{
    public GrenadeThrower thrower;

    public void OnThrow()
    {
        thrower.SetThrowingSequence(NodePlayerManager.GetInstance.GetCurrentPlayer());
    }
}
