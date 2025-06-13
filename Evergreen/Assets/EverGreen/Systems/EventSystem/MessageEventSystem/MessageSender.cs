using UnityEngine;

public class MessageSender : MonoBehaviour
{
    public static MessageSender Instance;

    [Header("Prefab e posição de spawn")]
    public GameObject feedbackPrefab;
    public Transform spawnParent;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Chama uma mensagem com imagem (opcional) e texto.
    /// </summary>
    /// <param name="sprite">Imagem que será exibida (pode ser nula)</param>
    /// <param name="message">Texto a ser exibido</param>
    public void ShowMessage(Sprite sprite, string message)
    {
        if (feedbackPrefab == null || spawnParent == null)
        {
            Debug.LogWarning("MessageSender: feedbackPrefab ou spawnParent não definidos.");
            return;
        }

        GameObject instance = Instantiate(feedbackPrefab, spawnParent);
        FeedbackUI feedback = instance.GetComponent<FeedbackUI>();

        if (feedback != null)
        {
            feedback.Setup(sprite, message);
        }
        else
        {
            Debug.LogWarning("O prefab não possui um componente FeedbackUI.");
        }
    }
}
