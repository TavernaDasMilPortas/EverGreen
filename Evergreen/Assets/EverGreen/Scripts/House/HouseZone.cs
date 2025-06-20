using UnityEngine;

public class RestrictionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            CameraManager.Instance.SetFirstPersonView();
            GameStateManager.Instance.SetState(InputState.House);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraManager.Instance.SetPlayerView();
            GameStateManager.Instance.SetState(InputState.Gameplay);
        }
    }

}