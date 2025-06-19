using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    [Header("Configurações")]
    public float interactionDistance = 2f;
    public float sphereRadius = 0.5f;
    public KeyCode interactionKey = KeyCode.E;
    public LayerMask interactionLayer;

    [Header("Status Atual")]
    public IInteractable nearestInteractable;
    [HideInInspector]
    public IInteractable lastHighlighted;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState == InputState.House)
            FindByRaycast();
        else
            FindNearestByProximity();
    }

    private void FindByRaycast()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            UpdateHighlight(interactable);
        }
        else
        {
            UpdateHighlight(null);
        }
    }

    private void FindNearestByProximity()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance, interactionLayer);
        IInteractable closest = null;
        float shortestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = interactable;
            }
        }

        UpdateHighlight(closest);
    }

    private void UpdateHighlight(IInteractable newInteractable)
    {
        nearestInteractable = newInteractable;

        // Atualiza o outline através do OutlineManager
        OutlineManager.Instance?.Highlight(newInteractable);

        lastHighlighted = newInteractable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (nearestInteractable != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, ((Component)nearestInteractable).transform.position);
        }
    }

    public static void SafeDestroy(IInteractable interactable)
    {
        if (interactable == null) return;

        GameObject go = ((Component)interactable).gameObject;

        OutlineURPEffect outline = go.GetComponent<OutlineURPEffect>();
        if (outline != null)
        {
            outline.DisableOutline();
        }

        if (Instance != null)
        {
            if (Instance.nearestInteractable == interactable)
                Instance.nearestInteractable = null;
            if (Instance.lastHighlighted == interactable)
                Instance.lastHighlighted = null;
        }

        Object.Destroy(go);
    }
}
