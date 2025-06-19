using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private MoveChanPhisical mover;
 // <- Novo modo

    [Header("Restrição de área especial")]
    public float mouseSensitivity = 100f;

    private void Awake()
    {
        Instance = this;
        mover = GetComponent<MoveChanPhisical>();
    }
    public LayerMask groundLayer; // defina "Ground" no Inspector

    public Transform mouseFollower;

   public void Move(float h, float v)
    {
        if (GameStateManager.Instance.CurrentState == InputState.House)
        {
            // Usa a rotação da câmera (ou transform do objeto com o FirstPersonLook)
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            // Remove qualquer inclinação vertical
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // Direção no mundo com base no input
            Vector3 worldDirection = forward * v + right * h;

            // Converte a direção de volta para componentes locais
            float localH = Vector3.Dot(worldDirection, right);
            float localV = Vector3.Dot(worldDirection, forward);

            mover.SetMoveInput(localH, localV);
        }
        else
        {
            // Modo normal (terceira pessoa, etc.)
            mover.SetMoveInput(h, v);
        }
    }




    public void Jump()
    {
            mover.TriggerJump();
    }

    public void Attack()
    {
        mover.TriggerAttack();
    }

    public void Stop()
    {
        mover.StopMovement();
    }

    public void Interagir()
    {
        Stop();
        InteractionHandler.Instance.nearestInteractable.Interact();
    }

    // Chamar ao entrar/sair da área especial

}
