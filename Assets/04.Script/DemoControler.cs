using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class DemoControler : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]NavMeshAgent agent;
    [SerializeField]Camera mainCamera;
    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.point);
                Debug.Log(hit.collider.gameObject.name);
                Vector3Int vec = GameManager.GetInstance.GetNode(hit.point).GetCenter;
                if (GameManager.GetInstance.IsExistNode(vec))
                {
                    agent.SetDestination(vec);

                }
            }
        }
    }
}
