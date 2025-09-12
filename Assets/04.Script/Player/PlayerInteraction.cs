using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 3f;
    public LayerMask interactionLayerMask;
    public Camera playerCamera;

    private IInteractable currentTarget;

    private void Update()
    {
        // Ray 디버그용 (씬에서 보임)
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactionRange, Color.green);
    }

    // 상호작용 Player Input에 연결
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactionLayerMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // 기존 타겟 해제
                if (currentTarget != null && currentTarget != interactable)
                {
                    currentTarget.UnInteraction();
                }

                currentTarget = interactable;
                currentTarget.OnInteraction(); // NPC와 실제 상호작용
            }
        }
        else
        {
            // 바라보던 대상에서 벗어나면 UnInteraction
            if (currentTarget != null)
            {
                currentTarget.UnInteraction();
                currentTarget = null;
            }
        }
    }
}
