using UnityEngine;
using TMPro;
using System; // Needed for Action

public class timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timertext;
    [SerializeField] float startTime = 60f;

    private float remainingTime;

    public static event Action OnTimerEnded; // Static event for easy access

    void Start()
    {
        remainingTime = startTime;
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                remainingTime = startTime;

                OnTimerEnded?.Invoke(); // Fire the event
            }
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timertext.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
