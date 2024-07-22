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

            action.PlayerInput.DecayMagic.performed += i => PlayerManager.Instance.DecayObject();

        }

        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

}
