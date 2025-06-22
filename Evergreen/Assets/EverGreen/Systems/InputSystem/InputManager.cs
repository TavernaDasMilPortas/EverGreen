using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }


    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private InputState lastState;

    private void Update()
    {
        if (lastState != GameStateManager.Instance.CurrentState)
        {
            // Estado mudou
            if (GameStateManager.Instance.CurrentState != InputState.Gameplay)
            {
                // Não é gameplay, para o movimento do player
                if (PlayerController.Instance != null)
                    PlayerController.Instance.Stop();
            }
        }

        lastState = GameStateManager.Instance.CurrentState;

        switch (GameStateManager.Instance.CurrentState)
        {
            case InputState.Gameplay:
                HandleGameplayInput();
                break;

            case InputState.Menu:
                HandleMenuInput();
                break;

            case InputState.Minigame:
                HandleMinigameInput();
                break;
            case InputState.Camera:
                HandleCameraInput();
                break;
            case InputState.House:
                HandleHouseInput();
                break;
        }
    }

    private void HandleGameplayInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.Move(h, v);

        }

        // Abre o menu principal (com abas como inventário, mapa etc)
        if (Input.GetKeyDown(KeyCode.I))
        {
            MenuManager.Instance?.ToggleMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerController.Instance.Interagir();
        }

    }

    private void HandleMenuInput()
    {

        // Navegação entre itens (dentro do menu atual)
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Navegando para direita");
            MenuManager.Instance?.Navigate(Vector2.right);
        } 
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Navegando para esquerda");
            MenuManager.Instance?.Navigate(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Navegando para baixo");
            MenuManager.Instance?.Navigate(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Navegando para cima");
            MenuManager.Instance?.Navigate(Vector2.up);

        }
        // Navegação entre abas (ex: Inventário -> Mapa)
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Navengando entre abas para direita");
            MenuManager.Instance?.NavigateTabs(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Navegando entre abas para esquerda");
            MenuManager.Instance?.NavigateTabs(-1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Confirmando ação");
            MenuManager.Instance?.Confirm();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Fechando o Menu");
            MenuManager.Instance?.Cancel();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Abrindo/Fechando menu");
            MenuManager.Instance?.ToggleMainMenu();
        }

    }
    private void HandleMinigameInput()
    {

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key) && IsValidKey(key))
            {
                MinigameManager.Instance.HandleInput(key);
            }
        }
    }
    private void HandleCameraInput()
    {


    }

    private void HandleHouseInput()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.Move(h, v);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            MenuManager.Instance?.ToggleMainMenu();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            PlayerController.Instance.Interagir();
        }
    }
    private bool IsValidKey(KeyCode key)
    {
        return key >= KeyCode.A && key <= KeyCode.Z || key >= KeyCode.Alpha0 && key <= KeyCode.Alpha9;
    }

}
