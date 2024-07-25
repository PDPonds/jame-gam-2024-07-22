using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableControllerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialUIManager.Instance.HideControllerPanel();
            if (Dialogue.tutorialIndex == 1)
            {
                Dialogue.tutorialIndex = 2;
                PlayerUI.Instance.hpBorder.SetActive(true);
                Dialogue.Instance.StartTutorial();
            }
        }
    }
}
