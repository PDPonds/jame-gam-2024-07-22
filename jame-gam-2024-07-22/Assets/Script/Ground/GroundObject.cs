using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundObjectState
{
    Enable, Disable
}

public class GroundObject : MonoBehaviour, IDamageable
{

    #region Ref

    MeshRenderer meshRenderer;

    #endregion

    [SerializeField] GameObject outline;

    [SerializeField] Transform visusal;
    [SerializeField] int maxHp;
    int curHp;

    [SerializeField] GroundObjectState state;

    private void Awake()
    {
        meshRenderer = visusal.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        ResetHP();
    }

    #region IDamageable

    void ResetHP()
    {
        curHp = maxHp;
    }

    public void Death()
    {
        SwitchState(GroundObjectState.Disable);
    }

    public void Heal()
    {
        curHp++;
        if (curHp >= maxHp)
        {
            ResetHP();
            if (IsState(GroundObjectState.Disable))
                SwitchState(GroundObjectState.Enable);
        }
    }

    public void Hit()
    {
        if (IsState(GroundObjectState.Enable))
        {
            curHp--;
            PlayerManager.Instance.AddRepairMana();
            if (curHp <= 0)
            {
                Death();
            }
        }
    }
    #endregion

    #region State

    public void SwitchState(GroundObjectState state)
    {
        this.state = state;
        switch (state)
        {
            case GroundObjectState.Enable:
                visusal.gameObject.SetActive(true);
                break;
            case GroundObjectState.Disable:
                visusal.gameObject.SetActive(false);
                break;
        }

        GameManager.Instance.BrakeNav();

    }

    public bool IsState(GroundObjectState state)
    {
        return this.state == state;
    }

    #endregion

    #region Outline

    public void ShowOutline()
    {
        outline.gameObject.SetActive(true);
    }

    public void HideOutline()
    {
        outline.gameObject.SetActive(false);
    }

    #endregion

}
