using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionHintManager : MonoBehaviour
{
    public static ActionHintManager Instance;

    [Header("Referências")]
    public GameObject actionHintPanel;
    public GameObject actionHintPrefab;
    [SerializeField] private Sprite pressedSprite;

    private class ActionHint
    {
        public GameObject gameObject;
        public int priority;
        public string key;
        public Sprite originalSprite;
        public Image buttonImage;

        public ActionHint(GameObject obj, int prio, string keyLabel)
        {
            gameObject = obj;
            priority = prio;
            key = keyLabel;

            Transform buttonTransform = gameObject.transform.Find("AHButtom");
            if (buttonTransform != null)
            {
                buttonImage = buttonTransform.GetComponent<Image>();
                if (buttonImage != null)
                {
                    originalSprite = buttonImage.sprite;
                    Debug.Log($"[ActionHint] Sprite original salvo para '{keyLabel}'");
                }
                else
                {
                    Debug.LogWarning($"[ActionHint] 'Image' não encontrado em AHButtom de '{keyLabel}'");
                }
            }
            else
            {
                Debug.LogWarning($"[ActionHint] AHButtom não encontrado em '{keyLabel}'");
            }
        }
    }

    private Dictionary<string, ActionHint> activeHints = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        Debug.Log("texte1");
        foreach (var hint in activeHints.Values)
        {
            Debug.Log("texte2");
            if (TryParseKey(hint.key, out KeyCode keyCode))
            {
                Debug.Log("texte3");
                if (hint.buttonImage != null)
                {
                    Sprite targetSprite = Input.GetKey(keyCode) ? pressedSprite : hint.originalSprite;
                    if (hint.buttonImage.sprite != targetSprite)
                    {
                        hint.buttonImage.sprite = targetSprite;
                        Debug.Log($"[Update] Sprite de '{hint.key}' alterado para: {(Input.GetKey(keyCode) ? "pressedSprite" : "originalSprite")}");
                    }
                }
                else
                {
                    Debug.LogWarning($"[Update] buttonImage nulo para '{hint.key}'");
                }
            }
            else
            {
                Debug.LogWarning($"[Update] Key inválida: '{hint.key}'");
            }
        }
    }

    public void ShowHint(string key, string message, int priority = 0)
    {
        if (activeHints.ContainsKey(key))
        {
            var hint = activeHints[key];
            var text = hint.gameObject.transform.Find("ActionText")?.GetComponent<TMP_Text>();
            if (text != null) text.text = message;
            hint.priority = priority;

            SortHints();
            return;
        }

        GameObject hintItem = Instantiate(actionHintPrefab, actionHintPanel.transform);
        hintItem.name = $"Hint_{key}";

        var keyLabel = hintItem.transform.Find("AHButtom/KeyLabel")?.GetComponent<TMP_Text>();
        var actionText = hintItem.transform.Find("ActionText")?.GetComponent<TMP_Text>();

        if (keyLabel != null) keyLabel.text = key;
        else Debug.LogWarning($"[ShowHint] KeyLabel não encontrado em '{key}'");

        if (actionText != null) actionText.text = message;
        else Debug.LogWarning($"[ShowHint] ActionText não encontrado em '{key}'");

        activeHints.Add(key, new ActionHint(hintItem, priority, key));
        SortHints();
    }

    public void HideHint(string key)
    {
        if (activeHints.ContainsKey(key))
        {
            Destroy(activeHints[key].gameObject);
            activeHints.Remove(key);
        }
    }

    public void ClearHints()
    {
        foreach (var hint in activeHints.Values)
        {
            Destroy(hint.gameObject);
        }
        activeHints.Clear();
    }

    private void SortHints()
    {
        List<ActionHint> sorted = new(activeHints.Values);
        sorted.Sort((a, b) => b.priority.CompareTo(a.priority));

        for (int i = 0; i < sorted.Count; i++)
        {
            sorted[i].gameObject.transform.SetSiblingIndex(i);
        }
    }

    public bool IsHintActive(string key)
    {
        return activeHints.ContainsKey(key);
    }

    private bool TryParseKey(string key, out KeyCode keyCode)
    {
        string normalized = NormalizeKeyLabel(key);

        try
        {
            keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), normalized, true);
            return true;
        }
        catch
        {
            Debug.LogWarning($"[TryParseKey] Falha ao interpretar tecla: '{key}' (normalizado: '{normalized}')");
            keyCode = KeyCode.None;
            return false;
        }
    }

    private string NormalizeKeyLabel(string key)
    {
        // Remover espaços e forçar capitalização de primeira letra apenas, exceto casos especiais
        key = key.Trim().ToLower();

        return key switch
        {
            "ctrl" or "control" => "LeftControl",
            "shift" => "LeftShift",
            "alt" => "LeftAlt",
            "esc" or "escape" => "Escape",
            "enter" => "Return",
            "space" or "spacebar" => "Space",
            "tab" => "Tab",
            "backspace" => "Backspace",
            "delete" => "Delete",
            "insert" => "Insert",
            "home" => "Home",
            "end" => "End",
            "pgup" or "pageup" => "PageUp",
            "pgdown" or "pagedown" => "PageDown",
            "up" => "UpArrow",
            "down" => "DownArrow",
            "left" => "LeftArrow",
            "right" => "RightArrow",
            "numpad0" => "Keypad0",
            "numpad1" => "Keypad1",
            "numpad2" => "Keypad2",
            "numpad3" => "Keypad3",
            "numpad4" => "Keypad4",
            "numpad5" => "Keypad5",
            "numpad6" => "Keypad6",
            "numpad7" => "Keypad7",
            "numpad8" => "Keypad8",
            "numpad9" => "Keypad9",
            _ => char.IsLetterOrDigit(key[0]) ? key.ToUpper() : key // fallback para letras e números
        };
    }

}
