using UnityEngine;

public abstract class UIMenuTab : MonoBehaviour
{
    public bool IsOpen { get; protected set; }

    public virtual void Toggle(bool value)
    {
        gameObject.SetActive(value);
        IsOpen = value;

        if (value) OnOpen();
        else OnClose();
    }

    public virtual void OnOpen()
    {
        Debug.Log("Aba aberta");
        Toggle(true);
    }

    public virtual void OnClose()
    {
        Debug.Log("Aba fechada");
        Toggle(false);
    }

    public abstract void Navigate(Vector2 direction);
    public abstract void Confirm();
    public abstract void Cancel();
}
