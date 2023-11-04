using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    NavMeshAgent navMeshAgent;
    Player_Input playerInputActions;
    InputAction clickAction;
    LayerMask raycastLayerMask;
    [SerializeField] float maxRayDistance = 500;
#if UNITY_EDITOR
    [SerializeField] float debugRayTime = 1.0f;
    [SerializeField] Color debugRayColor = Color.blue;
#endif

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerInputActions = new Player_Input();
        clickAction = playerInputActions.Player.Click;
        clickAction.performed += OnClickPerformed;
        clickAction.Enable();
        raycastLayerMask = ~(1 << LayerMask.NameToLayer("Structure"));
    }

    void OnDestroy()
    {
        clickAction.performed -= OnClickPerformed;
        clickAction.Disable();
    }

    void OnClickPerformed(InputAction.CallbackContext context)
    {
        if (navMeshAgent != null && navMeshAgent.enabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            // First, try to hit the "Structure" layer
            if (Physics.Raycast(ray, out hit, maxRayDistance, LayerMask.GetMask("Structure")))
            {
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, hit.collider.ClosestPoint(hit.point), debugRayColor, debugRayTime);
#endif
                Vector3 closestPoint = hit.collider.ClosestPoint(hit.point);
                MoveTo(closestPoint);
            }
            // If not hitting a "Structure", then move to the hit point of any other layer
            else if (Physics.Raycast(ray, out hit, maxRayDistance, raycastLayerMask))
            {
#if UNITY_EDITOR
                Debug.DrawLine(ray.origin, hit.point, debugRayColor, debugRayTime);
#endif
                MoveTo(hit.point);
            }

        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (navMeshAgent == null) return;
        navMeshAgent.destination = destination;
        navMeshAgent.isStopped = false;
    }
}
