using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : Singleton<Dialogue>
{
    public static int tutorialIndex = 1;
    public static int dialogIndex = 0;

    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Image tutorialImage;
    [SerializeField] TextMeshProUGUI text;

    [SerializeField] TutorialSlot[] tutorialSlot;

    [SerializeField] float textSpeed;

    public void StartTutorial()
    {
        if (dialogIndex < tutorialSlot.Length)
        {
            text.text = string.Empty;
            dialoguePanel.SetActive(true);
            Pause.Instance.PauseGame();
            StartCoroutine(TypeLine());
        }
    }

    public void SpaceOrClickOnDialog()
    {
        if (text.text != tutorialSlot[dialogIndex].lines)
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
        tutorialImage.sprite = tutorialSlot[dialogIndex].tutorialSprite;
        foreach (char c in tutorialSlot[dialogIndex].lines.ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void EndLine()
    {
        StopAllCoroutines();
        text.text = tutorialSlot[dialogIndex].lines;
    }

    void EndCurTutorial()
    {
        dialogIndex++;
        text.text = string.Empty;
        Pause.Instance.UnPauseGame();
        dialoguePanel.SetActive(false);

        if (tutorialIndex == 2)
        {
            tutorialIndex = 3;
            PlayerUI.Instance.decaySkillBorder.SetActive(true);
            StartTutorial();
        }
        else if (tutorialIndex == 3)
        {
            PlayerManager.Instance.skillIndicator.gameObject.SetActive(true);
        }

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
