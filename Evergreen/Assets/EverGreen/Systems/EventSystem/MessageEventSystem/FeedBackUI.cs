using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FeedbackUI : MonoBehaviour
{
    public Image imageComponent;       // Componente de imagem (pode ser nulo)
    public TextMeshProUGUI messageText; // Componente de texto

    public float fadeDuration = 2f;    // Duração do fade
    public float displayTime = 1.5f;   // Tempo antes de começar o fade

    /// <summary>
    /// Configura a UI com uma imagem e uma mensagem.
    /// </summary>
    public void Setup(Sprite image, string message)
    {
        messageText.text = message;

        if (image == null)
        {
            imageComponent.gameObject.SetActive(false);
        }
        else
        {
            imageComponent.sprite = image;
            imageComponent.gameObject.SetActive(true);
        }

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;

        // Captura as cores iniciais
        Color textColor = messageText.color;
        Color? imageColor = imageComponent != null ? imageComponent.color : (Color?)null;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            messageText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);

            if (imageComponent != null && imageComponent.gameObject.activeSelf)
            {
                imageComponent.color = new Color(imageColor.Value.r, imageColor.Value.g, imageColor.Value.b, alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
