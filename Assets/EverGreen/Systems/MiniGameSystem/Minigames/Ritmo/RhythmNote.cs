using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RhythmNote : MonoBehaviour
{
    public TextMeshProUGUI keyText;
    public Image indicatorImage;

    private char assignedKey;
    private float spawnTime;
    private float lifetime = 2f;
    private char noteKey;

    public void Initialize(char key)
    {
        assignedKey = key;
        keyText.text = key.ToString().ToUpper();
        spawnTime = Time.time;
        noteKey = char.ToUpper(key);
    }

    public void UpdateNote(float speed)
    {
        float lifeProgress = (Time.time - spawnTime) / lifetime;
        indicatorImage.color = Color.Lerp(Color.green, Color.red, lifeProgress);

        // Reduz apenas o indicatorImage
        float maxScale = 1.5f;
        float minScale = 1f;
        float currentScale = Mathf.Lerp(maxScale, minScale, lifeProgress);
        indicatorImage.rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
    public bool MatchesKey(KeyCode key)
    {
        return key.ToString().ToUpper() == assignedKey.ToString();
    }

    public bool IsWithinHitWindow(float hitWindow)
    {
        float elapsed = Time.time - spawnTime;
        return Mathf.Abs(elapsed - lifetime / 2f) <= hitWindow;
    }

    public bool HasExpired()
    {
        return Time.time - spawnTime > lifetime;
    }

    public char GetKey()
    {
        return noteKey;
    }

}
