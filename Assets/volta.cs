using UnityEngine;

public class PlayerReturn : MonoBehaviour
{
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Record initial position
        timer.OnTimerEnded += ReturnToStart; // Subscribe to timer event
    }

    void ReturnToStart()
    {
        transform.position = startPosition;
    }

    void OnDestroy()
    {
        timer.OnTimerEnded -= ReturnToStart; // Unsubscribe to avoid memory leaks
    }
}