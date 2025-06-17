using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    public static InteractionHandler Instance { get; private set; }

    public float interactionDistance = 2f;
    public float sphereRadius = 0.5f;
    public KeyCode interactionKey = KeyCode.E;

    public IInteractable nearestInteractable;

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

        //Debug.Log($"[InteractionHandler] Procurando interagíveis num raio de {interactionDistance}... ({hits.Length} encontrados)");

        foreach (Collider hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
               // Debug.Log($" - Encontrado: {hit.gameObject.name} a {distance:F2} unidades");

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closest = interactable;
                    //Debug.Log($"   -> Novo mais próximo: {hit.gameObject.name}");
                }
            }
            else
            {
                //Debug.Log($" - {hit.gameObject.name} não implementa IInteractable");
            }
        }

        if (closest != null)
        {
            //Debug.Log($"[InteractionHandler] Objeto mais próximo: {((Component)closest).gameObject.name} a {shortestDistance:F2} unidades");
        }
        else
        {
            //Debug.Log("[InteractionHandler] Nenhum interagível encontrado");
        }

        nearestInteractable = closest;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        if (nearestInteractable != null)
        {
            Gizmos.color = Color.green;
            //Gizmos.DrawLine(transform.position, ((Component)nearestInteractable).transform.position);
        }
    }
}
