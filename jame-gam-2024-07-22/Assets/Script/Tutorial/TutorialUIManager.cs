using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIManager : Singleton<TutorialUIManager>
{
    [SerializeField] GameObject controllerPanel;

    public void HideControllerPanel()
    {
        controllerPanel.SetActive(false);
    }

}
