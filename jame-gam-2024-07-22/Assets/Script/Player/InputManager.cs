using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerInputAction action;

    private void OnEnable()
    {
        if (action == null)
        {
            action = new PlayerInputAction();
            action.PlayerInput.Move.performed += i => PlayerManager.Instance.moveInput = i.ReadValue<Vector2>();

            action.PlayerInput.MousePos.performed += i => PlayerManager.Instance.mousePos = i.ReadValue<Vector2>();

            action.PlayerInput.RepairMagic.performed += i => PlayerManager.Instance.RepairObject();

            action.PlayerInput.DecayMagic.performed += i => OnLeftClick();

            action.PlayerInput.Dash.performed += i => OnSpace();

            action.PlayerInput.Pause.performed += i => Pause.Instance.TogglePauseButton();

        }

        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    void OnLeftClick()
    {
        if(Dialogue.Instance.IsDialoguePanelOpen())
        {
            Dialogue.Instance.SpaceOrClickOnDialog();
        }
        else
        {
            PlayerManager.Instance.DecayObject();
        }
    }

    void OnSpace()
    {
        if (Dialogue.Instance.IsDialoguePanelOpen())
        {
            Dialogue.Instance.SpaceOrClickOnDialog();
        }
        else
        {
            PlayerManager.Instance.Dash();
        }
    }

}
