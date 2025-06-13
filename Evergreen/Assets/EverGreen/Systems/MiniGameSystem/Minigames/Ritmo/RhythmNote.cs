using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum NoteType
{
    Classic,
    Sequence,
    Piano
}

public class RhythmNote : MonoBehaviour
{
    public char key;

    private char originalKey;
    public RectTransform hitZone;
    public RectTransform rectTransform;
    public Image indicatorImage;
  
    private NoteType noteType;
    private float lifeTime;

    private Coroutine lifeCoroutine;

    private void Awake()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (indicatorImage == null)
            indicatorImage = GetComponent<Image>();
    }

    public void Initialize(char assignedKey, RectTransform hitZoneReference, NoteType type, float duration)
    {
        key = assignedKey;
        hitZone = hitZoneReference;
        noteType = type;
        lifeTime = duration;

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (indicatorImage == null)
            indicatorImage = GetComponent<Image>();

        // Configura visual inicial
        rectTransform.localScale = Vector3.one * 2f;
        if (indicatorImage != null)
            indicatorImage.color = Color.green;

        // Atualiza texto da nota
        var tmpText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = assignedKey.ToString();
        }
        else
        {
            Debug.LogWarning("[RhythmNote] TextMeshProUGUI não encontrado no prefab da nota.");
        }

        // Caso seja modo Classic, inicia a animação de vida
        if (noteType == NoteType.Classic)
        {
            AnimateVisualOverLifetime(duration);
        }

        // Cancela coroutine anterior, se tiver
        if (lifeCoroutine != null)
            StopCoroutine(lifeCoroutine);

        // Inicia o ciclo de vida
        lifeCoroutine = StartCoroutine(LifeCycle());
    }

    public static GameObject SpawnNote(char key, RectTransform hitZone, GameObject notePrefab, Transform parent, Vector2 position, NoteType type, float duration)
    {
        GameObject noteGO = GameObject.Instantiate(notePrefab, parent);
        RectTransform noteRect = noteGO.GetComponent<RectTransform>();
        noteRect.anchoredPosition = position;

        RhythmNote rhythmNote = noteGO.GetComponent<RhythmNote>();
        rhythmNote.Initialize(key, hitZone, type, duration);
        rhythmNote.rectTransform = noteRect;

        return noteGO;
    }

    public void StartLifeCycle(float lifeTime, System.Action onExpire)
    {
        StartCoroutine(LifeCycleCoroutine(lifeTime, onExpire));
    }

    private IEnumerator LifeCycleCoroutine(float lifeTime, System.Action onExpire)
    {
        yield return new WaitForSeconds(lifeTime);
        onExpire?.Invoke();
        DestroyNote();
    }

    public void DestroyNote()
    {
        Destroy(gameObject);
    }

    private IEnumerator LifeCycle()
    {
        switch (noteType)
        {
            case NoteType.Classic:
                yield return new WaitForSeconds(lifeTime);
                Expire();
                break;

            case NoteType.Sequence:
                yield return new WaitForSeconds(3f);
                ZeroKey();
                break;

            case NoteType.Piano:
                while (true)
                {
                    if (IsPastHitZone())
                    {
                        Expire();
                        yield break;
                    }
                    yield return null;
                }
        }
    }

    private void ZeroKey()
    {
        key = '\0';
        var tmpText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null) tmpText.text = "";
    }

    private void Expire()
    {
        Destroy(gameObject);
    }

    public void OnHit()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void AnimateVisualOverLifetime(float duration)
    {
        StartCoroutine(VisualAnimationCoroutine(duration));
    }

    private IEnumerator VisualAnimationCoroutine(float duration)
    {
        float elapsed = 0f;

        rectTransform.localScale = Vector3.one * 2f;
        if (indicatorImage != null) indicatorImage.color = Color.green;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            rectTransform.localScale = Vector3.one * Mathf.Lerp(2f, 1f, progress);
            if (indicatorImage != null)
                indicatorImage.color = Color.Lerp(Color.green, Color.red, progress);
            yield return null;
        }

        rectTransform.localScale = Vector3.one;
        if (indicatorImage != null)
            indicatorImage.color = Color.red;
    }

    public bool IsPastHitZone()
    {
        if (hitZone == null) return false;

        return rectTransform.anchoredPosition.y < hitZone.anchoredPosition.y - hitZone.rect.height / 2f;
    }
    public float DistanceToHitArea()
    {
        if (hitZone == null || rectTransform == null)
        {
            Debug.LogWarning("[RhythmNote] hitZone ou rectTransform não definidos.");
            return float.MaxValue;  // Considera distância infinita se não puder calcular
        }

        return Mathf.Abs(rectTransform.anchoredPosition.y - hitZone.anchoredPosition.y);
    }

    public bool MatchesKey(KeyCode pressedKey)
    {
        return pressedKey.ToString().ToLower() == key.ToString().ToLower();
    }
    public void SetText(char c)
    {
        key = c;
        var tmpText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
            tmpText.text = c.ToString();
    }

    public void SetColor(Color c)
    {
        if (indicatorImage != null)
            indicatorImage.color = c;
    }

    public char GetOriginalKey()
    {
        return originalKey;
    }
}
