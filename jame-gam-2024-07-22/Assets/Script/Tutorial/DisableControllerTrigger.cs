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
            if (Dialogue.Instance.tutorialIndex == 1)
            {
                Dialogue.Instance.tutorialIndex = 2;
                PlayerUI.Instance.hpBorder.SetActive(true);
                Dialogue.Instance.StartTutorial();
            }
        }
    }
}
