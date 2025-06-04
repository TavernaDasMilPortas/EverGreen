using UnityEngine;
using TMPro;

public class timer : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI timertext;
    [SerializeField] float remainigtime;
    void Update()
    {
        if (remainigtime > 0)
        {
            remainigtime -= Time.deltaTime;
        }
        else if (remainigtime < 0)
        {
            remainigtime = 0;
        }

        int minutes = Mathf.FloorToInt(remainigtime / 60);
        int seconds = Mathf.FloorToInt(remainigtime % 60);
        timertext.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
