using UnityEngine;

public class scri : MonoBehaviour
{
    RhythmMinigameController controller;
    void Start()
    {
       controller = FindObjectOfType<RhythmMinigameController>();

    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (controller != null)
            {
                MinigameManager.Instance.StartMinigame(controller);
            }
        }


    }
}
