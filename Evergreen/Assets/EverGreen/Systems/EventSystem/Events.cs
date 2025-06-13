using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class MessageEvent : UnityEvent<Sprite, string> { }

public class Events : MonoBehaviour
{
    public static Events Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    [Header("Eventos personalizados")]
    public MessageEvent OnImageTextEvent;
  

    // Adicione novos eventos conforme necessário!
}
