using System.Collections.Generic;
using UnityEngine;

public class SequenceMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private List<char> sequence = new List<char>();
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
    }

    public void UpdateMode()
    {
        if (isShowingSequence && Time.time >= nextDisplayTime)
        {
            if (currentPlayerIndex < sequence.Count)
            {
                ShowSequenceNote(sequence[currentPlayerIndex]);
                currentPlayerIndex++;
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

        char pressedKey = key.ToString().ToUpper()[0];

        if (pressedKey == sequence[currentPlayerIndex])
        {
            controller.feedbackText.text = $"Acertou {pressedKey}!";
            currentPlayerIndex++;

            if (currentPlayerIndex >= sequence.Count)
            {
                controller.feedbackText.text = "Sequência Completa!";
                IsModeFinished = true;
            }
        }
        else
        {
            controller.feedbackText.text = $"Errou! Esperava {sequence[currentPlayerIndex]}";
            IsModeFinished = true;
        }
    }

    private void GenerateSequence(int length)
    {
        sequence.Clear();
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

        RhythmNote note = newNoteGO.GetComponent<RhythmNote>();
        note.Initialize(key, controller.hitZone);

        // Destroy após exibir
        GameObject.Destroy(newNoteGO, displayInterval * 0.8f);
    }
}

