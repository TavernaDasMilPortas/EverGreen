using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class VictoryDefeatScreenUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI titleText;
    private Image resultImage;
    private Image itemIcon;
    private TextMeshProUGUI itemInfoText;

    private bool canClose = false;

    /// <summary>
    /// Cria a tela de vitória/derrota como filho de um transform.
    /// </summary>
    /// <param name="prefab">O prefab da tela de resultado</param>
    /// <param name="parent">Transform que será o pai</param>
    /// <returns>Instância do VictoryDefeatScreenUI</returns>
    public static VictoryDefeatScreenUI CreateScreen(GameObject prefab, Transform parent)
    {
        GameObject instance = Instantiate(prefab, parent);
        VictoryDefeatScreenUI screen = instance.GetComponent<VictoryDefeatScreenUI>();
        screen.Initialize();
        return screen;
    }

    /// <summary>
    /// Associa dinamicamente os elementos da tela.
    /// </summary>
    public void Initialize()
    {
        // Procura o CanvasGroup no próprio objeto ou filhos
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("[VictoryDefeatScreenUI] CanvasGroup não encontrado no prefab ou seus filhos.");
            return;
        }

        Transform root = canvasGroup.transform;

        Transform resultImageT = root.Find("ResultImage");
        if (resultImageT != null) resultImage = resultImageT.GetComponent<Image>();
        else Debug.LogError("[VictoryDefeatScreenUI] ResultImage não encontrado.");

        Transform titleTextT = root.Find("ResultText");
        if (titleTextT != null) titleText = titleTextT.GetComponent<TextMeshProUGUI>();
        else Debug.LogError("[VictoryDefeatScreenUI] ResultText não encontrado.");

        Transform rewardGroup = root.Find("RewardGroup");
        if (rewardGroup != null)
        {
            Transform rewardImageT = rewardGroup.Find("RewardImage");
            if (rewardImageT != null) itemIcon = rewardImageT.GetComponent<Image>();
            else Debug.LogError("[VictoryDefeatScreenUI] RewardImage não encontrado.");

            Transform rewardTextT = rewardGroup.Find("RewardText");
            if (rewardTextT != null) itemInfoText = rewardTextT.GetComponent<TextMeshProUGUI>();
            else Debug.LogError("[VictoryDefeatScreenUI] RewardText não encontrado.");
        }
        else
        {
            Debug.LogError("[VictoryDefeatScreenUI] RewardGroup não encontrado.");
        }

        if (!canvasGroup || !titleText || !resultImage || !itemIcon || !itemInfoText)
        {
            Debug.LogError("[VictoryDefeatScreenUI] Um ou mais componentes da UI não foram encontrados corretamente.");
        }

        canvasGroup.alpha = 0f;
    }





    /// <summary>
    /// Preenche a tela com os dados e inicia o fade-in.
    /// </summary>
    public void Setup(bool victory, Sprite resultSprite, Sprite itemSprite, string itemName, int amount)
    {
        titleText.text = victory ? "Vitória!" : "Derrota!";
        titleText.color = victory ? Color.white : Color.white;
        resultImage.sprite = resultSprite;
        if (victory == true)
        {
            itemIcon.sprite = itemSprite;
            itemInfoText.text = $"{itemName} x{amount}";
        }
        else
        {
            itemIcon.gameObject.SetActive(false);
            itemInfoText.gameObject.SetActive(false);
        }


        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(time);
            yield return null;
        }

        canClose = true;
    }

    private void Update()
    {
        if (canClose && Input.GetKeyDown(KeyCode.E))
        {
            Destroy(gameObject);
            RhythmGameController.OnResultScreenClosed?.Invoke();
        }
    }
}
