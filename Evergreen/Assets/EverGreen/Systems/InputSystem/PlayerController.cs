using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private MoveChanPhisical mover;
    public GameObject Fauna;

    [Header("Restrição de área especial")]
    public float mouseSensitivity = 100f;

    private Coroutine disableFaunaCoroutine;
    private Coroutine ableFaunaCoroutine;

    private void Awake()
    {
        Instance = this;
        mover = GetComponent<MoveChanPhisical>();
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState == InputState.House)
        {
            if (disableFaunaCoroutine == null)
                disableFaunaCoroutine = StartCoroutine(DelayedDisableFauna());
        }
        else
        {

            // Se sair do modo House antes de completar o delay
            if (disableFaunaCoroutine != null)
            {
                StopCoroutine(disableFaunaCoroutine);
                disableFaunaCoroutine = null;
            }
            ableFaunaCoroutine = StartCoroutine(DelayedAbleFauna());
        }
    }

    private IEnumerator DelayedDisableFauna()
    {
        yield return new WaitForSeconds(1.8f);
        Fauna.SetActive(false);
        disableFaunaCoroutine = null; // Libera para rodar novamente se necessário
    }

    private IEnumerator DelayedAbleFauna()
    {
        yield return new WaitForSeconds(0.3f);
        Fauna.SetActive(true);
        disableFaunaCoroutine = null; // Libera para rodar novamente se necessário
    }


    public void Move(float h, float v)
    {
        if (GameStateManager.Instance.CurrentState == InputState.House)
        {
            h = 0f;

            Vector3 forward = mover.currentCamera.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 worldDirection = forward * v;
            float localV = Vector3.Dot(worldDirection, forward / 2);

            mover.SetMoveInput(0f, localV);
        }
        else
        {
            mover.SetMoveInput(h, v);
        }
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
}
