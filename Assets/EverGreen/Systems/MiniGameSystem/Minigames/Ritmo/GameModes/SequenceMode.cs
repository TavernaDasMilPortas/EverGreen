using System.Collections.Generic;
using UnityEngine;

public class SequenceMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private List<char> sequence = new List<char>();
    private List<GameObject> noteObjects = new List<GameObject>();

    private int currentDisplayIndex = 0;
    private int currentPlayerIndex = 0;

    private float displayInterval = 0.7f;
    private float nextDisplayTime;

    private bool isShowingSequence = true;
    private int sequenceLength = 3;

    public bool IsModeFinished { get; private set; } = false;

    public SequenceMode(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void Initialize(IRhythmGameController controller)
    {
        this.controller = controller;
    }

    public void StartMode()
    {
        GenerateSequence(sequenceLength);
        nextDisplayTime = Time.time + displayInterval;
        currentPlayerIndex = 0;
        currentDisplayIndex = 0;
        isShowingSequence = true;

        controller.feedbackText.text = "Observe!";
    }

    public void UpdateMode()
    {
        if (isShowingSequence && Time.time >= nextDisplayTime)
        {
            if (currentDisplayIndex < sequence.Count)
            {
                ShowSequenceNote(sequence[currentDisplayIndex]);
                currentDisplayIndex++;
                nextDisplayTime = Time.time + displayInterval;
            }
            else
            {
                isShowingSequence = false;
                currentPlayerIndex = 0;
                controller.feedbackText.text = "Sua vez!";
            }
        }
    }

    public void HandleInput(KeyCode key)
    {
        if (isShowingSequence || IsModeFinished) return;

        char pressedKey = key.ToString().ToLower()[0];

        if (currentPlayerIndex >= sequence.Count)
        {
            controller.feedbackText.text = "Já completou a sequência!";
            return;
        }

        char expectedKey = sequence[currentPlayerIndex];
        string result;

        if (pressedKey == expectedKey)
        {
            result = $"Acertou {pressedKey}!";
            currentPlayerIndex++;
        }
        else
        {
            result = $"Errou! Esperava {expectedKey}, mas recebeu {pressedKey}.";
            IsModeFinished = true;
        }

        controller.feedbackText.text = result;

        // Opcional: realce a nota correspondente
        if (currentPlayerIndex - 1 < noteObjects.Count && noteObjects[currentPlayerIndex - 1] != null)
        {
            var img = noteObjects[currentPlayerIndex - 1].GetComponent<UnityEngine.UI.Image>();
            if (img != null)
                img.color = pressedKey == expectedKey ? Color.green : Color.red;
        }

        // Finaliza se o jogador completou a sequência com sucesso
        if (currentPlayerIndex >= sequence.Count && !IsModeFinished)
        {
            controller.feedbackText.text = "Sequência Completa!";
            IsModeFinished = true;
        }
    }

    private void GenerateSequence(int length)
    {
        sequence.Clear();
        noteObjects.Clear();

        for (int i = 0; i < length; i++)
        {
            char randomKey = controller.difficultyData.allowedKeys[Random.Range(0, controller.difficultyData.allowedKeys.Length)];
            sequence.Add(randomKey);
        }
    }

    private void ShowSequenceNote(char key)
    {
        GameObject newNoteGO = GameObject.Instantiate(controller.noteButtonPrefab, controller.noteArea);

        RectTransform noteRect = newNoteGO.GetComponent<RectTransform>();
        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();

        float areaWidth = areaRect.rect.width;
        float areaHeight = areaRect.rect.height;

        float x = Random.Range(-areaWidth / 2f + noteRect.rect.width / 2f, areaWidth / 2f - noteRect.rect.width / 2f);
        float y = Random.Range(-areaHeight / 2f + noteRect.rect.height / 2f, areaHeight / 2f - noteRect.rect.height / 2f);

        noteRect.anchoredPosition = new Vector2(x, y);
        noteRect.localScale = Vector3.one * 2f;

        // TextMeshProUGUI da nota
        var tmpText = newNoteGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = key.ToString();
        }

        // Indicador visual
        var indicatorImage = newNoteGO.GetComponent<UnityEngine.UI.Image>();
        if (indicatorImage != null)
        {
            indicatorImage.color = Color.green;
        }

        // RhythmNote
        RhythmNote note = newNoteGO.GetComponent<RhythmNote>();
        if (note != null)
        {
            note.Initialize(key, controller.hitZone);
        }

        // Armazena o GameObject para depois marcar acerto/erro
        noteObjects.Add(newNoteGO);

        // Destruir depois de um tempo? Agora NÃO, pois será usado visualmente na comparação
        // GameObject.Destroy(newNoteGO, displayInterval * 0.8f); 
    }
}
