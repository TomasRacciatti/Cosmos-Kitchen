using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TyperManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI nameBox;
    public TextMeshProUGUI dialogBox;

    [Header("Typing")]
    public float charsPerSecond = 30f;

    [Header("Dependencies")]
    public SpeakerManager speakerManager;

    [Header("Active Speaker")]
    public NPCSpeaker currentSpeaker;

    [Header("Behavior")]
    public bool loopAtEnd = false;
    public bool skipThenAdvanceSameClick = false;  //Estos dos son booleanas para testeo

    public event Action OnDialogueEnd;

    private int index = -1;
    private bool isTyping = false;
    private Coroutine typeCor;
    private string currentText = "";

    public void SetSpeaker(NPCSpeaker speaker)
    {
        //Cancelar tipeo
        CancelTyping();
        if (speakerManager) speakerManager.CancelSpeak();

        currentSpeaker = speaker;
        index = -1;
        ClearUI();
    }

    public void StartOrNext()
    {
        if (currentSpeaker == null || currentSpeaker.lines == null || currentSpeaker.lines.Length == 0)
            return;

        // Si esta escribiendo, completar dialogo y mutear npc
        if (isTyping)
        {
            FinishTyping(currentText);
            if (speakerManager) speakerManager.CancelSpeak();

            if (skipThenAdvanceSameClick)
            {
                AdvanceAndStart();                     // opcional: skip+advance
            }
            return;
        }

        AdvanceAndStart();
    }

    private void AdvanceAndStart()
    {
        index++;

        if (index >= currentSpeaker.lines.Length)
        {
            if (loopAtEnd) index = 0;
            else { OnDialogueEnd?.Invoke(); return; }
        }

        StartLine(index);
    }

    private void StartLine(int i)
    {
        CancelTyping();
        if (speakerManager) speakerManager.CancelSpeak();

        var line = currentSpeaker.lines[i];
        currentText = line != null ? (line.message ?? string.Empty) : string.Empty;

        if (nameBox) nameBox.text = currentSpeaker.npcName;

        typeCor = StartCoroutine(TypeLine(currentText));      // trigger typping
        if (speakerManager) speakerManager.Speak(currentText, currentSpeaker); // trigger voice
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        if (dialogBox) dialogBox.text = "";

        float delay = (charsPerSecond > 0f) ? 1f / charsPerSecond : 0.03f;

        for (int i = 0; i < text.Length; i++)
        {
            if (!isTyping) yield break;                       //Cancelar mid typping
            if (dialogBox) dialogBox.text += text[i];
            yield return new WaitForSecondsRealtime(delay);
        }

        isTyping = false;
    }

    private void FinishTyping(string full)
    {
        CancelTyping();
        if (dialogBox) dialogBox.text = full;
    }

    private void CancelTyping()
    {
        if (typeCor != null)
        {
            StopCoroutine(typeCor);
            typeCor = null;
        }
        isTyping = false;
    }

    private void ClearUI()
    {
        if (nameBox) nameBox.text = "";
        if (dialogBox) dialogBox.text = "";
    }
}


