using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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
    [Header("===== Wand =====")]
    public Wand curWand;

    [Header("===== HP =====")]
    [SerializeField] int maxHp;
    int curHp;

    [Header("===== Mouse =====")]
    [SerializeField] LayerMask mouseMask;

    [Header("===== Move =====")]
    [SerializeField] float moveSpeed;
    Vector3 moveDir;

    [Header("===== Dash =====")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    [SerializeField] float dashDelay;
    float curDashDelay;

    [Header("===== Skill =====")]
    [Header("- Indicator")]
    [SerializeField] Transform skillRangeIndicator;
    [SerializeField] Transform skillIndicator;
    [Header("- Detail")]
    [SerializeField] LayerMask skillMask;
    [Header("- Repair Skill")]
    [SerializeField] float maxRepairMana;

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

        CountDownDelay();
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
            if (moveInput.x > 0) spriteRenderer.flipX = false;
            else spriteRenderer.flipX = true;
        }
        else
        {
            anim.SetBool("isWalk", false);
        }
    }

    #endregion

    #region Dash

    public void Dash()
    {
        if (curDashDelay <= 0)
        {
            StartCoroutine(dash(GetDirToMouse(), dashForce, dashDuration));
            DashAnimHandle();
            curDashDelay = dashDelay;
        }
    }

    IEnumerator dash(Vector3 dashDir, float dashSpeed, float duration)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            if (dashDir != Vector3.zero)
            {
                Vector3 dir = dashDir.normalized;

                rb.AddForce(dir * dashSpeed / 2, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce(transform.forward * dashSpeed / 2, ForceMode.Impulse);
            }

            rb.constraints = RigidbodyConstraints.FreezePositionY;
            rb.freezeRotation = true;
            yield return null;
            rb.constraints = RigidbodyConstraints.None;
            rb.freezeRotation = true;
        }

        yield return null;
    }

    void CountDownDelay()
    {
        if (curDashDelay > 0)
        {
            curDashDelay -= Time.deltaTime;
            if (curDashDelay <= 0)
            {
                curDashDelay = 0;
            }
        }
    }

    void DashAnimHandle()
    {
        anim.Play("Dash");
        FlipSpriteWithMouseDir();
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
            float dist = Vector3.Distance(transform.position, pos);
            if (dist <= curWand.skillRange)
            {
                skillIndicator.transform.position = pos;
            }
            else
            {
                Vector3 dir = GetDirToMouse();
                Vector3 newPos = transform.position + dir * curWand.skillRange;
                newPos.y = 0;
                skillIndicator.transform.position = newPos;
            }
        }
    }

    void ScaleIndicator()
    {
        skillIndicator.localScale = Vector3.one * (curWand.skillArea * 2f);
        skillRangeIndicator.localScale = Vector3.one * (curWand.skillRange * 2);
    }

    Collider[] GetColliderInArea()
    {
        Collider[] col = Physics.OverlapSphere(skillIndicator.position, curWand.skillArea, skillMask);
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
        AttackAnimHandle();
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
            AttackAnimHandle();
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

    void AttackAnimHandle()
    {
        anim.Play("Attack");
        FlipSpriteWithMouseDir();
    }

    void FlipSpriteWithMouseDir()
    {
        Vector3 right = transform.TransformDirection(transform.right);
        Vector3 dir = GetDirToMouse();
        if (Vector3.Dot(right, dir) <= 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    #endregion

    #region Repair Mana

    public void AddRepairMana()
    {
        curRepairMana += curWand.toGetRepairMana;
        if (curRepairMana >= maxRepairMana)
        {
            curRepairMana = maxRepairMana;
        }
    }

    public void RemoveRepairMana()
    {
        curRepairMana -= curWand.toUseRepairMana;
    }

    public bool CanUseRepair()
    {
        return curRepairMana >= curWand.toUseRepairMana;
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(skillIndicator.position, curWand.skillArea);
    }

}
