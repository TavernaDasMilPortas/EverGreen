using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReloadOnWater : MonoBehaviour
{
    [Header("Tempo de espera antes do reload")]
    public float reloadDelay = 2f;

    private bool isReloading = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isReloading && collision.gameObject.CompareTag("Water"))
        {
            StartCoroutine(ReloadSceneWithDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isReloading && other.CompareTag("Water"))
        {
            StartCoroutine(ReloadSceneWithDelay());
        }
    }

    private IEnumerator ReloadSceneWithDelay()
    {
        isReloading = true;

        yield return new WaitForSeconds(reloadDelay);

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
