using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class RhythmNote : MonoBehaviour
{
    public char key;
    public RectTransform hitZone; 
    public RectTransform rectTransform;
    public Image indicatorImage;
    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (indicatorImage == null)
            indicatorImage = GetComponent<Image>();

    }
    public void MoveNote(float deltaTime)
    {
        // Mover a nota horizontalmente (ex: eixo X ou Y)
        transform.Translate(Vector3.left * deltaTime * 100f); // Exemplo básico
    }

    public void MoveDown(float speed)
    {
        rectTransform.anchoredPosition -= new Vector2(0, speed * Time.deltaTime);
    }

    public bool IsOutOfBounds()
    {
        return rectTransform.anchoredPosition.y < -Screen.height; // ou -noteAreaHeight
    }


    public bool IsExpired()
    {
        // Define se a nota já passou do tempo permitido (exemplo básico)
        return transform.localPosition.x < -hitZone.rect.width;
    }


    public float DistanceToHitArea()
    {
        if (hitZone == null)
        {
            Debug.LogWarning("HitZone não atribuída em RhythmNote.");
            return float.MaxValue;
        }

        Vector2 notePos = rectTransform.anchoredPosition;
        Vector2 hitZonePos = hitZone.anchoredPosition;

        return Mathf.Abs(notePos.y - hitZonePos.y); // Distância vertical
    }
    public char GetKey()
    {
        return key;
    }

    public float GetVerticalDistanceToHitZone()
    {
        if (rectTransform == null)
        {
            Debug.LogError($"[RhythmNote] rectTransform está nulo no objeto {gameObject.name}");
            return float.MaxValue;
        }

        if (hitZone == null)
        {
           // Debug.LogError($"[RhythmNote] hitZone está nulo no objeto {gameObject.name}");
            return float.MaxValue;
        }

        return Mathf.Abs(rectTransform.anchoredPosition.y - hitZone.anchoredPosition.y);
    }

    public void Initialize(char assignedKey, RectTransform hitZoneReference)
    {
        key = assignedKey;
        hitZone = hitZoneReference;
    }
    public void AnimateVisualOverLifetime(float duration)
    {
        StartCoroutine(VisualAnimationCoroutine(duration));
    }




    private IEnumerator VisualAnimationCoroutine(float duration)
    {
        float elapsed = 0f;

        // Começa com escala 2x e cor verde
        rectTransform.localScale = Vector3.one * 2f;
        if (indicatorImage != null) indicatorImage.color = Color.green;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            // Escala de 2x para 1x
            rectTransform.localScale = Vector3.one * Mathf.Lerp(2f, 1f, progress);

            // Cor de verde para vermelho
            if (indicatorImage != null)
                indicatorImage.color = Color.Lerp(Color.green, Color.red, progress);

            yield return null;
        }

        // Garantir que finalize exatamente no estado final
        rectTransform.localScale = Vector3.one * 1f;
        if (indicatorImage != null)
            indicatorImage.color = Color.red;
    }


    public bool MatchesKey(KeyCode pressedKey)
    {
        return pressedKey.ToString().ToLower() == key.ToString().ToLower();
    }
}