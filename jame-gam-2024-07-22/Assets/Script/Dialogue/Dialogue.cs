using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : Singleton<Dialogue>
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Image tutorialImage;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] TutorialSlot[] tutorialSlot;

    [SerializeField] float textSpeed;

    int tutorialIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (tutorialIndex < tutorialSlot.Length)
        {
            text.text = string.Empty;
            dialoguePanel.SetActive(true);
            Pause.Instance.PauseGame();
            StartCoroutine(TypeLine());
        }
    }

    public void SpaceOrClickOnDialog()
    {
        if (text.text != tutorialSlot[tutorialIndex].lines)
        {
            EndLine();
        }
        else
        {
            EndCurTutorial();
        }
    }

    IEnumerator TypeLine()
    {
        tutorialImage.sprite = tutorialSlot[tutorialIndex].tutorialSprite;
        foreach (char c in tutorialSlot[tutorialIndex].lines.ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void EndLine()
    {
        StopAllCoroutines();
        text.text = tutorialSlot[tutorialIndex].lines;
    }

    void EndCurTutorial()
    {
        tutorialIndex++;
        text.text = string.Empty;
        Pause.Instance.UnPauseGame();
        dialoguePanel.SetActive(false);
    }

    public bool IsDialoguePanelOpen()
    {
        return dialoguePanel.activeSelf;
    }

}

[Serializable]
public class TutorialSlot
{
    public Sprite tutorialSprite;
    public string lines;
}
