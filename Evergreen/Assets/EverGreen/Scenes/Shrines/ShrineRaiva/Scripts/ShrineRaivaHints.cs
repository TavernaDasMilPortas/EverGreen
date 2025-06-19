using UnityEngine;

public class ShrineRaivaHints : MonoBehaviour
{
    
    void Start()
    {
        string cameraKey = CameraManager.Instance.switchKey.ToString().ToUpper();
        ActionHintManager.Instance.ShowHint($"{cameraKey}"," - Mudar camera",0);    
    }

    // Update is called once per frame
    void Update()
    {
        if (InteractionHandler.Instance.nearestInteractable != null && GameStateManager.Instance.CurrentState == InputState.Gameplay)
        {
            ActionHintManager.Instance.ShowHint("E", " - Interagir", 10);
        }
        else if (ActionHintManager.Instance.IsHintActive("E") && InteractionHandler.Instance.nearestInteractable == null || GameStateManager.Instance.CurrentState != InputState.Gameplay)
        {
            ActionHintManager.Instance.HideHint("E");
        }
    }
}
