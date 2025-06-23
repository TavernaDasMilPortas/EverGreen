using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadLandScene()
    {
        SceneManager.LoadScene("Land");
    }
}