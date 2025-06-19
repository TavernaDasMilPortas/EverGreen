using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [Header("Abas do Menu Principal")]
    public List<UIMenuTab> menuTabs = new List<UIMenuTab>();
    private int currentTabIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Debug.Log("MenuManager instanciado com " + menuTabs.Count + " abas.");
    }

    public void ToggleMainMenu()
    {
        Debug.Log("ToggleMainMenu chamado");

        if (menuTabs.Count == 0)
        {
            Debug.LogWarning("Nenhuma aba disponível no menu.");
            return;
        }

        bool isOpen = menuTabs[currentTabIndex].IsOpen;
        bool newState = !isOpen;

        Debug.Log($"Aba atual: {currentTabIndex}, Estado atual: {isOpen}, Novo estado: {newState}");

        menuTabs[currentTabIndex].Toggle(newState);
        GameStateManager.Instance.SetState(newState ? InputState.Menu : InputState.Gameplay);

        Debug.Log("Estado do InputManager agora é: " + GameStateManager.Instance.CurrentState);
    }

    public void Navigate(Vector2 direction)
    {
        Debug.Log("Tentando navegar com direção: " + direction);

        if (menuTabs.Count > 0 && menuTabs[currentTabIndex].IsOpen)
        {
            Debug.Log("Chamando Navigate() na aba " + currentTabIndex);
            menuTabs[currentTabIndex].Navigate(direction);
        }
        else
        {
            Debug.LogWarning("Navegação bloqueada: Aba não está aberta ou lista vazia.");
        }
    }

    public void NavigateTabs(int direction)
    {
        Debug.Log("Tentando mudar de aba. Direção: " + direction);

        if (menuTabs.Count <= 1)
        {
            Debug.LogWarning("Mudança de aba bloqueada: Menos de 2 abas.");
            return;
        }

        if (!menuTabs[currentTabIndex].IsOpen)
        {
            Debug.LogWarning("Mudança de aba bloqueada: Aba atual não está aberta.");
            return;
        }

        menuTabs[currentTabIndex].OnClose();
        currentTabIndex = (currentTabIndex + direction + menuTabs.Count) % menuTabs.Count;
        Debug.Log("Nova aba selecionada: " + currentTabIndex);
        menuTabs[currentTabIndex].OnOpen();
    }

    public void Confirm()
    {
        Debug.Log("Confirm pressionado");

        if (menuTabs.Count == 0 || !menuTabs[currentTabIndex].IsOpen)
        {
            Debug.LogWarning("Confirmação bloqueada: Nenhuma aba aberta.");
            return;
        }

        Debug.Log("Chamando Confirm() na aba " + currentTabIndex);
        menuTabs[currentTabIndex].Confirm();
    }

    public void Cancel()
    {
        Debug.Log("Cancel pressionado");

        if (menuTabs.Count == 0 || !menuTabs[currentTabIndex].IsOpen)
        {
            Debug.LogWarning("Cancel bloqueado: Nenhuma aba aberta.");
            return;
        }

        Debug.Log("Chamando Cancel() na aba " + currentTabIndex);
        menuTabs[currentTabIndex].Cancel();
        menuTabs[currentTabIndex].OnClose();

        GameStateManager.Instance.SetState(InputState.Gameplay);
        Debug.Log("Estado do InputManager agora é: " + GameStateManager.Instance.CurrentState);
    }
}
