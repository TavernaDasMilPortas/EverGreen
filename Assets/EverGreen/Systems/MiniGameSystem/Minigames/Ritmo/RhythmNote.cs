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

    public void Initialize(char key)
    {
        assignedKey = key;
        keyText.text = key.ToString().ToUpper();
        spawnTime = Time.time;
    }

    public void UpdateNote(float speed)
    {
        transform.localScale -= Vector3.one * speed * Time.deltaTime;

        float lifeProgress = (Time.time - spawnTime) / lifetime;
        indicatorImage.color = Color.Lerp(Color.green, Color.red, lifeProgress);
    }

    public bool MatchesKey(KeyCode key)
    {
        return key.ToString().ToLower() == assignedKey.ToString();
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
}
