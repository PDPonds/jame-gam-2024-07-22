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
    [SerializeField] float maxHp;
    float curHp;

    [SerializeField] Material sideMat;
    [SerializeField] Material[] matLife;

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
        SetupMat();
    }

    public void Death()
    {
        SwitchState(GroundObjectState.Disable);
    }

    public void Heal(float damage)
    {
        curHp += damage;
        SetupMat();
        if (curHp >= maxHp)
        {
            ResetHP();
            if (IsState(GroundObjectState.Disable))
            {
                SwitchState(GroundObjectState.Enable);
            }
        }
    }

    public void Hit(float amount)
    {
        if (IsState(GroundObjectState.Enable))
        {
            curHp -= amount;
            SetupMat();
            PlayerManager.Instance.GetRepair();
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

    #region Setup Mat

    void SetupMat()
    {
        List<Material> mat = new List<Material>();
        mat.Add(sideMat);

        switch (curHp)
        {
            case 10:
            case 20:

                mat.Add(matLife[0]);
                meshRenderer.SetMaterials(mat);

                break;
            case 30:
            case 40:

                mat.Add(matLife[1]);
                meshRenderer.SetMaterials(mat);

                break;
            case 50:
            case 60:

                mat.Add(matLife[2]);
                meshRenderer.SetMaterials(mat);

                break;
            case 70:
            case 80:
            case 90:

                mat.Add(matLife[3]);
                meshRenderer.SetMaterials(mat);

                break;
            case 100:

                mat.Add(matLife[4]);
                meshRenderer.SetMaterials(mat);

                break;
        }

    }

    #endregion

}
