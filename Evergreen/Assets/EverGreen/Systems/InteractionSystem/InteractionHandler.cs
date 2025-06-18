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
            if (hit == null || hit.gameObject == null)
                continue;

            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null)
                continue;

            if (((Component)interactable) == null)
                continue;

            float distance = Vector3.Distance(transform.position, hit.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closest = interactable;
            }
        }

        // Desativa outline anterior
        if (lastHighlighted != null && ((Component)lastHighlighted) != null && lastHighlighted != closest)
        {
            Outline oldOutline = ((Component)lastHighlighted).GetComponent<Outline>();
            if (oldOutline != null)
            {

                oldOutline.OutlineWidth = 0f; // Modo de destaque oculto
                Debug.Log($"Desativando outline de {((Component)lastHighlighted).gameObject.name}");
            }
        }

        // Ativa outline novo com cor e largura customizada
        if (closest != null && ((Component)closest) != null && closest != lastHighlighted)
        {
            MinigameConfig minigameConfig= ((Component)closest).GetComponent<MinigameConfig>();
            Outline newOutline = ((Component)closest).GetComponent<Outline>();
            if (minigameConfig != null && minigameConfig.isRewarded == false)
            {
                if (newOutline != null)
                {
                    newOutline.OutlineMode = Outline.Mode.OutlineVisible; // Modo de destaque
                    newOutline.OutlineColor = Color.white;         // <- Cor do destaque
                    newOutline.OutlineWidth = 10f;                  // <- Espessura da borda
                    Debug.Log($"Ativando outline de {((Component)closest).gameObject.name}");
                }
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
