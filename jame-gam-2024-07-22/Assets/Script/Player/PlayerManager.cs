using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public float maxHp;
    [HideInInspector] public float curHp;
    [SerializeField] float decreaseHpPerTime;

    [Header("===== Mouse =====")]
    [SerializeField] LayerMask mouseMask;

    [Header("===== Move =====")]
    [SerializeField] float moveSpeed;
    Vector3 moveDir;

    [Header("===== Dash =====")]
    [SerializeField] float dashForce;
    [SerializeField] float dashDuration;
    public float dashDelay;
    [HideInInspector] public float curDashDelay;

    [Header("===== Skill =====")]
    [Header("- Indicator")]
    [SerializeField] Transform skillRangeIndicator;
    [SerializeField] Transform skillIndicator;
    [SerializeField] Transform spawnParticlePoint;
    [Header("- Detail")]
    [SerializeField] LayerMask skillMask;

    [Header("- Decay Skill")]
    public float decayDelay;
    [HideInInspector] public float curDecayDelay;
    [SerializeField] GameObject decayParticle;
    [SerializeField] GameObject explosiveParticle;
    [SerializeField] GameObject healParticle;

    [Header("- Repair Skill")]
    public float repairDelay;
    [HideInInspector] public float curRepairDelay;
    [SerializeField] GameObject repairParticle;
    [SerializeField] GameObject repairAndHealObjectParticel;

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
        DecreaseHP();
        MoveHandle();

        MoveIndicator();
        ScaleIndicator();

        DecreaseDecayDelay();
        DecreaseRepairDelay();
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
            Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y);
            StartCoroutine(dash(moveDir, dashForce, dashDuration));
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

    void TakeDamage(float damage)
    {
        curHp -= damage;
        if (curHp <= 0)
        {
            Death();
        }
    }

    void RestoreHP(float amount)
    {
        curHp += amount;
        if (curHp > maxHp)
        {
            ResetHP();
        }
    }

    public void Hit(float damage)
    {
        TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        RestoreHP(amount);
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
                skillRangeIndicator.gameObject.SetActive(false);

            }
            else
            {
                Vector3 dir = GetDirToMouse();
                Vector3 newPos = transform.position + dir * curWand.skillRange;
                newPos.y = 0;
                skillIndicator.transform.position = newPos;
                skillRangeIndicator.gameObject.SetActive(true);

            }
        }
        else
        {
            Vector3 dir = GetDirToMouse();
            Vector3 newPos = transform.position + dir * curWand.skillRange;
            newPos.y = 0;
            skillIndicator.transform.position = newPos;
            skillRangeIndicator.gameObject.SetActive(true);

        }
    }

    void ScaleIndicator()
    {
        skillIndicator.localScale = new Vector3(curWand.skillArea * 2f, 1, curWand.skillArea * 2f);
        skillRangeIndicator.localScale = new Vector3(curWand.skillRange * 2, curWand.skillRange * 2, 1);
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
        if (curDecayDelay <= 0)
        {
            allIDamageable = GetAllIDamageable();

            if (GetWorldPosFormMouse(out Vector3 pos))
            {
                InstanceParticle(explosiveParticle, pos, 1f);
                InstancteTailParticle(decayParticle, pos);
            }

            AttackAnimHandle();

            if (allIDamageable.Count > 0)
            {
                for (int i = 0; i < allIDamageable.Count; i++)
                {
                    IDamageable iD = allIDamageable[i];
                    iD.Hit(curWand.decayDamage);
                }
            }

            curDecayDelay = decayDelay;
        }
    }

    public void RepairObject()
    {
        if (CanUseRepair() && curRepairDelay <= 0)
        {
            curHp -= curWand.toUseHP;
            AttackAnimHandle();

            if (GetWorldPosFormMouse(out Vector3 pos))
            {
                InstanceParticle(repairAndHealObjectParticel, pos, 1f);
                InstancteTailParticle(repairParticle, pos);
            }

            allIDamageable = GetAllIDamageable();
            if (allIDamageable.Count > 0)
            {
                for (int i = 0; i < allIDamageable.Count; i++)
                {
                    IDamageable iD = allIDamageable[i];
                    iD.Heal(curWand.repairAmount);
                }
            }

            curRepairDelay = repairDelay;
        }
    }

    void DecreaseDecayDelay()
    {
        if (curDecayDelay > 0)
        {
            curDecayDelay -= Time.deltaTime;
            if (curDecayDelay <= 0)
            {
                curDecayDelay = 0;
            }
        }
    }

    void DecreaseRepairDelay()
    {
        if (curRepairDelay > 0)
        {
            curRepairDelay -= Time.deltaTime;
            if (curRepairDelay <= 0)
            {
                curRepairDelay = 0;
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

    void InstanceParticle(GameObject particle, Vector3 pos, float duration)
    {
        GameObject go = Instantiate(particle, pos, quaternion.identity);
        Destroy(go, duration);
    }

    void InstancteTailParticle(GameObject tail, Vector3 pos)
    {
        GameObject go = Instantiate(tail, spawnParticlePoint.position, Quaternion.identity);
        TailParticle repairTail = go.GetComponent<TailParticle>();

        repairTail.Setup(pos);
    }

    #endregion

    #region Repair

    void DecreaseHP()
    {
        curHp -= Time.deltaTime * decreaseHpPerTime;
        PlayerUI.Instance.UpdateHPFill();
    }

    public bool CanUseRepair()
    {
        return curHp > curWand.toUseHP;
    }

    public void GetRepair()
    {
        curHp += curWand.toGetHP;
        healParticle.SetActive(false);
        healParticle.SetActive(true);
        if (curHp >= maxHp)
        {
            ResetHP();
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(skillIndicator.position, curWand.skillArea);
    }

}
