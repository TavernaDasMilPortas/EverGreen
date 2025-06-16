using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        MapGenerator.Instance.NextPhase();
    }
}
