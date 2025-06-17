using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    [Header("Configurações")]
    public float interactionDistance = 2f;
    public float sphereRadius = 0.5f;
    public KeyCode interactionKey = KeyCode.E;

    [Header("Status Atual")]
    public IInteractable nearestInteractable;

    [HideInInspector]
    public IInteractable lastHighlighted;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        FindNearestInteractable();
    }

    private void FindNearestInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactionDistance);
        IInteractable closest = null;
        float shortestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            // Se o objeto do collider foi destruído, ignora
            if (hit == null || hit.gameObject == null)
                continue;

            // Checa se o componente IInteractable ainda existe e não foi destruído
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            // Checa se o objeto do interactable ainda é válido
            if (((Component)interactable) == null)
                continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = interactable;
            }
        }

        // Desativa outline anterior, só se o lastHighlighted ainda existe
        if (lastHighlighted != null && ((Component)lastHighlighted) != null && lastHighlighted != closest)
        {
            OutlineURPEffect outline = ((Component)lastHighlighted).GetComponent<OutlineURPEffect>();
            if (outline != null)
            {
                outline.DisableOutline();
                Debug.Log($"Desativando outline de {((Component)lastHighlighted).gameObject.name}");
            }
        }

        // Ativa outline novo, se válido
        if (closest != null && ((Component)closest) != null && closest != lastHighlighted)
        {
            OutlineURPEffect outline = ((Component)closest).GetComponent<OutlineURPEffect>();
            if (outline != null)
            {
                outline.EnableOutline();
                Debug.Log($"Ativando outline de {((Component)closest).gameObject.name}");
            }
        }

        nearestInteractable = closest;
        lastHighlighted = closest;
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

    /// <summary>
    /// Método seguro para destruir objetos interagíveis, limpando outline e referências antes.
    /// </summary>
    public static void SafeDestroy(IInteractable interactable)
    {
        if (interactable == null) return;

        GameObject go = ((Component)interactable).gameObject;

        // Desativa outline se houver
        OutlineURPEffect outline = go.GetComponent<OutlineURPEffect>();
        if (outline != null)
        {
            outline.DisableOutline();
            Debug.Log($"[InteractionHandler] Outline desativada de {go.name} antes da destruição.");
        }

        // Limpa referências no Instance
        if (Instance != null)
        {
            if (Instance.nearestInteractable == interactable)
            {
                Instance.nearestInteractable = null;
                Debug.Log("[InteractionHandler] nearestInteractable limpo.");
            }
            if (Instance.lastHighlighted == interactable)
            {
                Instance.lastHighlighted = null;
                Debug.Log("[InteractionHandler] lastHighlighted limpo.");
            }
        }

        // Destrói o objeto
        Object.Destroy(go);
    }
}
