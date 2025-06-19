using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [Header("Câmeras Virtuais")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera topDownCamera;
    public CinemachineVirtualCamera firstPersonCamera;

    [Header("Configuração")]
    public KeyCode switchKey = KeyCode.Tab;

    private bool isTopDown = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        SetPlayerView(); // Começa com a câmera da jogadora
    }

    private void Update()
    {
        if (Input.GetKeyDown(switchKey))
        {
            if (isTopDown)
            { SetPlayerView();
                GameStateManager.Instance.SetState(InputState.Gameplay);
            }
            else
            {
                PlayerController.Instance.Stop();
                SetTopDownView();
                GameStateManager.Instance.SetState(InputState.Camera);
            }
                

        }
    }

    public void SetTopDownView()
    {
        playerCamera.Priority = 0;
        topDownCamera.Priority = 10;
        firstPersonCamera.Priority = 0;
        isTopDown = true;
    }

    public void SetPlayerView()
    {
        topDownCamera.Priority = 0;
        playerCamera.Priority = 10;
        firstPersonCamera.Priority = 0;
        isTopDown = false;
    }
    public void SetFirstPersonView()
    {
        playerCamera.Priority = 0;
        topDownCamera.Priority = 0;
        firstPersonCamera.Priority = 10;
        isTopDown = false;
    }
}
