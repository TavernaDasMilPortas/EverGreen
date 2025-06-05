using System.Collections.Generic;
using UnityEngine;

public class SequenceMode : IRhythmGameMode
{
    private IRhythmGameController controller;
    private List<char> sequence = new List<char>();
    private List<GameObject> noteObjects = new List<GameObject>();
    private List<char> playerInputs = new List<char>();

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
        playerInputs.Clear();

        controller.feedbackText.text = "Observe!";
    }

    public void UpdateMode()
    {
        if (isShowingSequence && Time.time >= nextDisplayTime)
        {
            if (currentDisplayIndex < sequence.Count)
            {
                ShowSequenceNote(sequence[currentDisplayIndex], currentDisplayIndex);
                currentDisplayIndex++;
                nextDisplayTime = Time.time + displayInterval;
            }
            else
            {
                isShowingSequence = false;
                currentPlayerIndex = 0;
                controller.feedbackText.text = "Sua vez!";

                // Zera textos das notas
                foreach (var noteGO in noteObjects)
                {
                    var note = noteGO.GetComponent<RhythmNote>();
                    if (note != null)
                        note.SetText(' ');
                }
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

        playerInputs.Add(pressedKey);

        // Atualiza nota com input do jogador
        var noteGO = noteObjects[currentPlayerIndex];
        var note = noteGO.GetComponent<RhythmNote>();
        if (note != null)
        {
            note.SetText(pressedKey);
        }

        currentPlayerIndex++;

        if (currentPlayerIndex >= sequence.Count)
        {
            EvaluateSequence();
        }
    }

    private void EvaluateSequence()
    {
        IsModeFinished = true;

        for (int i = 0; i < sequence.Count; i++)
        {
            var noteGO = noteObjects[i];
            var note = noteGO.GetComponent<RhythmNote>();
            if (note != null)
            {
                if (playerInputs[i] == note.GetOriginalKey())
                    note.SetColor(Color.green);
                else
                    note.SetColor(Color.red);
            }
        }

        controller.feedbackText.text = "Sequência completada!";
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

    private void ShowSequenceNote(char key, int index)
    {
        GameObject newNoteGO = GameObject.Instantiate(controller.noteButtonPrefab, GameObject.Find("MiniGamePanel").transform);

        RectTransform noteRect = newNoteGO.GetComponent<RectTransform>();
        RectTransform areaRect = controller.noteArea.GetComponent<RectTransform>();

        // Fixa a coluna no meio da área
        float x = 0f;

        // Espaçamento vertical entre notas
        float spacing = 50f;

        float y = areaRect.rect.height / 2f - spacing * index;

        noteRect.anchoredPosition = new Vector2(x, y);
        noteRect.localScale = Vector3.one * 2f;

        // Configura visual
        var tmpText = newNoteGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = key.ToString();
        }

        var indicatorImage = newNoteGO.GetComponent<UnityEngine.UI.Image>();
        if (indicatorImage != null)
        {
            indicatorImage.color = Color.green;
        }

        var note = newNoteGO.GetComponent<RhythmNote>();
        if (note != null)
        {
            note.Initialize(key, controller.hitZone, NoteType.Sequence, 0);
        }

        noteObjects.Add(newNoteGO);
    }
}
