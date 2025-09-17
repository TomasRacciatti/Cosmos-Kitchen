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
    public bool loopAtEnd = false;                 // if false, stops on last line
    public bool skipThenAdvanceSameClick = false;  // normally: first click finishes, next click advances

    public event Action OnDialogueEnd;

    private int index = -1;
    private bool isTyping = false;
    private Coroutine typeCor;
    private string currentText = "";

    public void SetSpeaker(NPCSpeaker speaker)
    {
        // fully cancel current session
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

        // If mid-typing: finish instantly & stop voice
        if (isTyping)
        {
            FinishTyping(currentText);                 // shows full text & sets isTyping=false
            if (speakerManager) speakerManager.CancelSpeak();

            if (skipThenAdvanceSameClick)
            {
                AdvanceAndStart();                     // optional: skip+advance in one click
            }
            return;                                    // default: require a second click to advance
        }

        // Not typing -> go to next line
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
        // defensive: stop anything pending
        CancelTyping();
        if (speakerManager) speakerManager.CancelSpeak();

        var line = currentSpeaker.lines[i];
        currentText = line != null ? (line.message ?? string.Empty) : string.Empty;

        if (nameBox) nameBox.text = currentSpeaker.npcName;

        typeCor = StartCoroutine(TypeLine(currentText));      // typing starts
        if (speakerManager) speakerManager.Speak(currentText, currentSpeaker); // voice starts
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        if (dialogBox) dialogBox.text = "";

        float delay = (charsPerSecond > 0f) ? 1f / charsPerSecond : 0.03f;

        for (int i = 0; i < text.Length; i++)
        {
            if (!isTyping) yield break;                       // cancelled mid-line
            if (dialogBox) dialogBox.text += text[i];
            yield return new WaitForSecondsRealtime(delay);
        }

        isTyping = false;                                     // finished naturally
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


