using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassGroundObject : MonoBehaviour, IDamageable
{
    #region Ref

    MeshRenderer groundMeshRen;

    #endregion


    [SerializeField] GameObject outline;
    [SerializeField] Transform visusal;

    [SerializeField] float maxHp;
    float curHp;

    [SerializeField] Material sideMat;
    [SerializeField] Material[] grassMat;

    [SerializeField] bool hasLife;

    private void Start()
    {
        groundMeshRen = visusal.GetComponent<MeshRenderer>();
    }

    public void Death()
    {
        hasLife = false;
        SetupMat();
    }

    public void Heal(float amount)
    {
        curHp += amount;
        if (curHp > maxHp)
        {
            curHp = maxHp;
            if (!hasLife)
            {
                hasLife = true;
                SetupMat();
            }
        }
    }

    public void Hit(float damage)
    {
        if (hasLife)
        {
            curHp -= damage;
            PlayerManager.Instance.GetRepair();
            if (curHp <= 0)
            {
                Death();
            }
        }
    }

    void SetupMat()
    {
        List<Material> mat = new List<Material>();
        mat.Add(sideMat);

        if (hasLife)
        {
            mat.Add(grassMat[0]);
        }
        else
        {
            mat.Add(grassMat[1]);
        }

        groundMeshRen.SetMaterials(mat);
    }

    public void ShowOutline()
    {
        outline.SetActive(true);
    }

    public void HideOutline()
    {
        outline.SetActive(false);
    }

}
