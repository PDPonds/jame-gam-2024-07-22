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

    [SerializeField] string openDoorText;

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

    public void StartDoor()
    {
        text.text = string.Empty;
        dialoguePanel.SetActive(true);
        Pause.Instance.PauseGame();
        StartCoroutine(TypeDoor());
    }

    public void SpaceOrClickOnDialog()
    {
        if (tutorialImage.gameObject.activeSelf)
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
        else
        {
            if (text.text != openDoorText)
            {
                EndDoorLine();
            }
            else
            {
                EndDoor();
            }
        }
    }

    IEnumerator TypeLine()
    {
        tutorialImage.gameObject.SetActive(true);
        tutorialImage.sprite = tutorialSlot[dialogIndex].tutorialSprite;
        foreach (char c in tutorialSlot[dialogIndex].lines.ToCharArray())
        {
            text.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator TypeDoor()
    {
        tutorialImage.gameObject.SetActive(false);
        foreach (char c in openDoorText.ToCharArray())
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
    void EndDoorLine()
    {
        StopAllCoroutines();
        text.text = openDoorText;
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
            PlayerUI.Instance.doorCountBorder.SetActive(true);
            PlayerUI.Instance.UpdateDoorCount();
        }

    }

    void EndDoor()
    {
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
