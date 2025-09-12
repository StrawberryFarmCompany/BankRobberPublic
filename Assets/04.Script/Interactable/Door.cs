using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : IInteractable
{
    public void OnInteraction()
    {
        OpenDoor();
    }

    public void UnInteraction()
    {
        CloseDoor();
    }

    public void OpenDoor()
    {

    }

    public void CloseDoor()
    {

    }
}
