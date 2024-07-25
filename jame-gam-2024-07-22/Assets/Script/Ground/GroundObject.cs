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

    MeshRenderer groundMeshRen;
    MeshRenderer repairMeshRen;

    #endregion

    [SerializeField] GameObject outline;

    [SerializeField] Transform visusal;
    [SerializeField] Transform repairVisual;

    [SerializeField] float maxHp;
    float curHp;

    [SerializeField] Material sideMat;
    [SerializeField] Material[] matLife;
    [SerializeField] Material[] matRepair;

    [SerializeField] GroundObjectState state;

    [SerializeField] bool isDestroyOnStart;

    private void Awake()
    {
        groundMeshRen = visusal.GetComponent<MeshRenderer>();
        repairMeshRen = repairVisual.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        if (isDestroyOnStart)
        {
            curHp = 0;
            Death();
        }
        else
        {
            ResetHP();
        }
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
        if (Dialogue.tutorialIndex == 3)
        {
            Dialogue.tutorialIndex = 4;
            PlayerUI.Instance.repairSkillBorder.SetActive(true);
            PlayerUI.Instance.dashSkillBorder.SetActive(true);
            Dialogue.Instance.StartTutorial();
        }
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
                SetupMat();

                if (Dialogue.tutorialIndex == 4)
                {
                    Dialogue.tutorialIndex = 5;
                }

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
                repairVisual.gameObject.SetActive(false);
                break;
            case GroundObjectState.Disable:
                visusal.gameObject.SetActive(false);
                repairVisual.gameObject.SetActive(true);
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
        if (state == GroundObjectState.Enable)
        {

            mat.Add(sideMat);

            if (curHp <= 20)
            {
                mat.Add(matLife[0]);
            }
            else if (curHp > 20 && curHp <= 40)
            {
                mat.Add(matLife[1]);
            }
            else if (curHp > 40 && curHp <= 60)
            {
                mat.Add(matLife[2]);
            }
            else if (curHp > 60 && curHp <= 90)
            {
                mat.Add(matLife[3]);
            }
            else if (curHp > 90 && curHp <= 100)
            {
                mat.Add(matLife[4]);
            }

            groundMeshRen.SetMaterials(mat);
        }
        else if (state == GroundObjectState.Disable)
        {
            if (curHp == 0)
            {
                repairMeshRen.material = matRepair[0];
            }
            else if (curHp > 0 && curHp <= 20)
            {
                repairMeshRen.material = matRepair[1];

            }
            else if (curHp > 20 && curHp <= 40)
            {
                repairMeshRen.material = matRepair[2];

            }
            else if (curHp > 40 && curHp <= 60)
            {
                repairMeshRen.material = matRepair[3];

            }
            else if (curHp > 60 && curHp <= 90)
            {
                repairMeshRen.material = matRepair[4];
            }
            else if (curHp > 90 && curHp <= 100)
            {
                repairMeshRen.material = matRepair[5];
            }
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        if (isDestroyOnStart)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }

}
