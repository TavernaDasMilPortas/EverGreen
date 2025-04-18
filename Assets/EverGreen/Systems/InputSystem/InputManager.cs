using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] public InputState CurrentState { get; private set; } = InputState.Gameplay;

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

    private void Update()
    {
       

        switch (CurrentState)
        {
            case InputState.Gameplay:
                HandleGameplayInput();
                break;

            case InputState.Menu:
                HandleMenuInput();
                break;

            case InputState.Minigame:
                // HandleMinigameInput();
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

            if (Input.GetButtonDown("Jump"))
                PlayerController.Instance.Jump();

            if (Input.GetButtonDown("Fire1"))
                PlayerController.Instance.Attack();
        }

        // Abre o menu principal (com abas como inventário, mapa etc)
        if (Input.GetKeyDown(KeyCode.I))
        {
            MenuManager.Instance?.ToggleMainMenu();
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

    public void SetState(InputState newState)
    {
        CurrentState = newState;
    }
}
