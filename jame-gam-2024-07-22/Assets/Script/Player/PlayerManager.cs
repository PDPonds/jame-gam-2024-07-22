using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>, IDamageable
{
    #region Ref
    Rigidbody rb;
    #endregion

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 mousePos;

    [Header("===== HP =====")]
    [SerializeField] int maxHp;
    [SerializeField] int curHp;

    [Header("===== Move =====")]
    [SerializeField] float moveSpeed;
    Vector3 moveDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ResetHP();
    }

    private void Update()
    {
        MoveHandle();
    }

    #region Controller

    void MoveHandle()
    {

        moveDir = Camera.main.transform.forward * moveInput.y;
        moveDir = moveDir + Camera.main.transform.right * moveInput.x;
        moveDir.Normalize();
        moveDir.y = 0;
        moveDir = moveDir * moveSpeed;


        rb.velocity = new Vector3(moveDir.x, rb.velocity.y, moveDir.z);
    }



    #endregion

    #region IDamageable

    void ResetHP()
    {
        curHp = maxHp;
    }

    void TakeDamage()
    {
        curHp--;
        if (curHp <= 0)
        {
            Death();
        }
    }

    void RestoreHP()
    {
        curHp++;
        if (curHp > maxHp)
        {
            ResetHP();
        }
    }

    public void Hit()
    {
        TakeDamage();
    }

    public void Heal()
    {
        RestoreHP();
    }

    public void Death()
    {
        Debug.Log("Death");
    }

    #endregion

}
