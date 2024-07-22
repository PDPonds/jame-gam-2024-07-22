using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>, IDamageable
{
    #region Ref
    Rigidbody rb;
    Animator anim;
    SpriteRenderer spriteRenderer;
    #endregion

    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public Vector2 mousePos;

    [SerializeField] Transform visaul;

    [Header("===== HP =====")]
    [SerializeField] int maxHp;
    int curHp;

    [Header("===== Mouse =====")]
    [SerializeField] LayerMask mouseMask;

    [Header("===== Move =====")]
    [SerializeField] float moveSpeed;
    Vector3 moveDir;

    [Header("===== Skill =====")]
    [Header("- Indicator")]
    [SerializeField] Transform skillIndicator;
    [Header("- Detail")]
    [SerializeField] LayerMask skillMask;
    [SerializeField] float area;
    [Header("- Repair Skill")]
    [SerializeField] float maxRepairMana;
    [SerializeField] float repairManaMul;

    [SerializeField] float useRepairManaMul;

    float curRepairMana;

    List<IDamageable> allIDamageable = new List<IDamageable>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = visaul.GetComponent<Animator>();
        spriteRenderer = visaul.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetHP();
    }

    private void Update()
    {
        MoveHandle();

        MoveIndicator();
        ScaleIndicator();
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

        MoveAnim();
    }

    void MoveAnim()
    {
        if (moveInput != Vector2.zero)
        {
            anim.SetBool("isWalk", true);
            if (moveInput.x > 0) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
    }

    #endregion

    #region Mouse

    public bool GetWorldPosFormMouse(out Vector3 worldPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, mouseMask))
        {
            worldPos = hit.point;
            return true;
        }
        worldPos = transform.position;
        return false;
    }

    public Vector3 GetDirToMouse()
    {

        if (GetWorldPosFormMouse(out Vector3 mouseWorldPos))
        {
            Vector3 dir = mouseWorldPos - transform.position;
            dir.Normalize();
            dir.y = 0;
            return dir;
        }
        return Vector3.zero;
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

    #region Skill

    void MoveIndicator()
    {
        if (GetWorldPosFormMouse(out Vector3 pos))
        {
            pos.y = 0;
            skillIndicator.transform.position = pos;
        }
    }

    void ScaleIndicator()
    {
        skillIndicator.localScale = Vector3.one * (area * 2f);
    }

    Collider[] GetColliderInArea()
    {
        Collider[] col = Physics.OverlapSphere(skillIndicator.position, area, skillMask);
        return col;
    }

    List<IDamageable> GetAllIDamageable()
    {
        List<IDamageable> damageables = new List<IDamageable>();
        Collider[] cols = GetColliderInArea();
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageables.Add(damageable);
                }
            }
        }

        return damageables;
    }

    public void DecayObject()
    {
        allIDamageable = GetAllIDamageable();
        if (allIDamageable.Count > 0)
        {
            for (int i = 0; i < allIDamageable.Count; i++)
            {
                IDamageable iD = allIDamageable[i];
                iD.Hit();
            }
        }
    }

    public void RepairObject()
    {
        if (CanUseRepair())
        {
            RemoveRepairMana();
            allIDamageable = GetAllIDamageable();
            if (allIDamageable.Count > 0)
            {
                for (int i = 0; i < allIDamageable.Count; i++)
                {
                    IDamageable iD = allIDamageable[i];
                    iD.Heal();
                }
            }
        }
    }

    #endregion

    #region Repair Mana

    public void AddRepairMana()
    {
        curRepairMana += repairManaMul;
        if (curRepairMana >= maxRepairMana)
        {
            curRepairMana = maxRepairMana;
        }
    }

    public void RemoveRepairMana()
    {
        curRepairMana -= useRepairManaMul;
    }

    public bool CanUseRepair()
    {
        return curRepairMana >= useRepairManaMul;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(skillIndicator.position, area);
    }

}
