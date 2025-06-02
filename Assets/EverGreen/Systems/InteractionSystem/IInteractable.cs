using UnityEngine;

public interface IInteractable
{
    void Interact();
    Item RequiredItem { get; }
    int RequiredItemQuantity { get; }
    string InteractionMessage { get; }
}
