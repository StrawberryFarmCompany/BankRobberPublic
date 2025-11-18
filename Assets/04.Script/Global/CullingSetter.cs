using UnityEngine;
using UnityEngine.UI;

public class CullingSetter : MonoBehaviour
{
    public Renderer[] renderers;
    public Collider[] colliders;
    public Image[] images; 

    private void Start()
    {
        if(FloorCullingManager.GetInstance != null)
        {
            FloorCullingManager.GetInstance.RegisterRenderersAndColliders(renderers, colliders, images);
        }
        Destroy(this);
    }
}
