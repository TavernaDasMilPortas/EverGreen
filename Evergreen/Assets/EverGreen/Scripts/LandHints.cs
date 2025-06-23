using UnityEngine;

public class LandHints : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
