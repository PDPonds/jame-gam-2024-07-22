using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyBehavior
{
    Spawn, Chase, ChargeToAttack, Attack, Death
}

public class EnemyController : MonoBehaviour, IDamageable
{
    #region Ref
    NavMeshAgent agent;
    Rigidbody rb;
    Animator anim;
    SpriteRenderer spriteRenderer;
    #endregion

    [SerializeField] Transform visual;
    float curHp;

    [Header("===== Enemy ======")]
    [SerializeField] Enemy enemy;
    [SerializeField] EnemyBehavior behavior;

    [Header("===== Attack ======")]
    [SerializeField] LayerMask playerMask;
    [Header("- Range Attack")]
    [SerializeField] Transform spawnSkillPoint;
    [SerializeField] Transform rangeSkillIndicator;

    [Header("- Around User Attack")]
    [SerializeField] Transform aroundUserSkillIndicator;

    bool attackAlready;

    AttackType curAttackType;

    float curChargeAttack;

    float deathTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        anim = visual.GetComponent<Animator>();
        spriteRenderer = visual.GetComponent<SpriteRenderer>();

        OnSpawn();
    }

    private void Update()
    {
        UpdateBehavoir();
        FlipSpriteWithPlayerDir();
    }

    #region Behavior

    public void OnSpawn()
    {
        curHp = enemy.maxHp;
        SwithBehavoir(EnemyBehavior.Spawn);
    }

    public void SwithBehavoir(EnemyBehavior behavior)
    {
        this.behavior = behavior;
        switch (behavior)
        {
            case EnemyBehavior.Spawn:
                anim.Play("OnSpawn");
                break;
            case EnemyBehavior.Chase:

                deathTime = 5f;
                attackAlready = false;
                SetAttackType();

                break;
            case EnemyBehavior.ChargeToAttack:

                anim.Play("OnChargeAttack");
                curChargeAttack = 0;
                ShowAttackIndicator();

                break;
            case EnemyBehavior.Attack:

                anim.Play("OnAttack");
                Attack();
                HideAttackIndicator();

                break;
            case EnemyBehavior.Death:
                anim.Play("OnDeath");
                Death();
                break;
        }
    }

    public void UpdateBehavoir()
    {
        switch (behavior)
        {
            case EnemyBehavior.Spawn:

                StopMove();
                if (IsAnimationEnd("OnSpawn"))
                {
                    SwithBehavoir(EnemyBehavior.Chase);
                }

                break;
            case EnemyBehavior.Chase:

                agent.SetDestination(PlayerManager.Instance.transform.position);
                anim.SetBool("isWalk", true);
                float dist = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position);
                float maxDist = GetAttackRange();
                if (dist <= maxDist)
                {
                    SwithBehavoir(EnemyBehavior.ChargeToAttack);
                }

                break;
            case EnemyBehavior.ChargeToAttack:

                StopMove();
                LookAt(PlayerManager.Instance.transform.position);
                curChargeAttack += Time.deltaTime;
                if (curChargeAttack >= curAttackType.chargeTime)
                {
                    SwithBehavoir(EnemyBehavior.Attack);
                }

                break;
            case EnemyBehavior.Attack:

                StopMove();

                break;
            case EnemyBehavior.Death:

                StopMove();
                deathTime -= Time.deltaTime;
                if (deathTime <= 0)
                {
                    Destroy(gameObject);
                }

                break;
        }
    }

    public bool IsBehavior(EnemyBehavior behavior)
    {
        return this.behavior == behavior;
    }

    void StopMove()
    {
        agent.SetDestination(transform.position);
        agent.velocity = Vector3.zero;
        anim.SetBool("isWalk", false);
    }

    public void LookAt(Vector3 pos)
    {
        Vector3 targetDir = pos - transform.position;
        targetDir.Normalize();
        targetDir.y = 0;

        if (targetDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            Quaternion playerRot = Quaternion.Slerp(transform.rotation, targetRot, 10 * Time.deltaTime);

            transform.rotation = playerRot;
        }
    }

    void SetAttackType()
    {
        int index = UnityEngine.Random.Range(0, enemy.attackType.Count);
        AttackType attackType = enemy.attackType[index];
        curAttackType = attackType;
        anim.runtimeAnimatorController = curAttackType.animOverride;
    }

    float GetAttackRange()
    {
        float attackRange = 0;
        switch (curAttackType.type)
        {
            case ATKType.Range:
                RangeAttack rangeAttack = (RangeAttack)curAttackType;
                attackRange = rangeAttack.time * rangeAttack.speed;

                break;
            case ATKType.AroundUser:
                AroundUserAttack around = (AroundUserAttack)curAttackType;
                attackRange = around.area;

                break;
            case ATKType.RandomArea:
                RandomAreaAttack randomAreaAttack = (RandomAreaAttack)curAttackType;
                attackRange = randomAreaAttack.maxDistance;

                break;
        }
        return attackRange;
    }

    bool IsAnimationEnd(string name)
    {
        return anim.GetCurrentAnimatorStateInfo(0).IsName(name) &&
            anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f;
    }

    void FlipSpriteWithPlayerDir()
    {
        Vector3 right = transform.TransformDirection(transform.right);
        Vector3 dir = PlayerManager.Instance.transform.position - transform.position;
        dir = dir.normalized;
        if (Vector3.Dot(right, dir) <= 0)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    #endregion

    #region IDamageable

    void ResetHP()
    {
        curHp = enemy.maxHp;
    }

    public void Hit(float damage)
    {
        curHp -= damage;
        PlayerManager.Instance.GetRepair();
        if (curHp <= 0)
        {
            SwithBehavoir(EnemyBehavior.Death);
        }
    }

    public void Heal(float amount)
    {
        curHp -= amount;
        if (curHp >= enemy.maxHp)
        {
            ResetHP();
        }
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Attack

    public void Attack()
    {
        if (!attackAlready)
        {
            switch (curAttackType.type)
            {
                case ATKType.Range:
                    StartCoroutine(SpawnSkill(() => InstanceRangeSkill()));
                    break;
                case ATKType.AroundUser:
                    StartCoroutine(SpawnSkill(() => AttactPlayerAroundUser()));
                    break;
                case ATKType.RandomArea:
                    break;
            }
        }
    }

    public void ShowAttackIndicator()
    {
        switch (curAttackType.type)
        {
            case ATKType.Range:

                rangeSkillIndicator.gameObject.SetActive(true);
                RangeAttack range = (RangeAttack)curAttackType;
                float size = range.time * range.speed;
                Vector3 rangeScale = new Vector3(1, 1, 1 * size) ;
                rangeSkillIndicator.localScale = rangeScale;

                break;
            case ATKType.AroundUser:

                aroundUserSkillIndicator.gameObject.SetActive(true);
                AroundUserAttack around = (AroundUserAttack)curAttackType;
                float area = around.area;
                Vector3 aroundScale = new Vector3(1, 0, 1) * (area * 2);
                aroundUserSkillIndicator.localScale = aroundScale;

                break;
            case ATKType.RandomArea:
                break;
        }
    }

    public void HideAttackIndicator()
    {
        rangeSkillIndicator.gameObject.SetActive(false);
        aroundUserSkillIndicator.gameObject.SetActive(false);
    }

    IEnumerator SpawnSkill(Action skill)
    {
        int count = 0;
        attackAlready = true;
        while (count < curAttackType.count)
        {
            count++;
            skill?.Invoke();
            yield return new WaitForSeconds(curAttackType.delayPerCount);
        }

        yield return null;
        SwithBehavoir(EnemyBehavior.Chase);

    }

    void InstanceRangeSkill()
    {
        GameObject go = Instantiate(curAttackType.skillParticle, spawnSkillPoint.position, Quaternion.identity);
        RangeSkillObject rangeObj = go.GetComponent<RangeSkillObject>();
        rangeObj.Setup(transform.forward, (RangeAttack)curAttackType);
    }

    void AttactPlayerAroundUser()
    {
        AroundUserAttack around = (AroundUserAttack)curAttackType;
        float area = around.area;

        GameObject particle = Instantiate(around.skillParticle, transform.position, Quaternion.identity);
        Destroy(particle, 1f);

        Collider[] cols = Physics.OverlapSphere(transform.position, area, playerMask);
        if (cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].TryGetComponent<PlayerManager>(out PlayerManager playerManager))
                {
                    playerManager.Hit(around.damage);
                }
            }
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (curAttackType != null)
        {
            if (curAttackType is AroundUserAttack area)
            {
                Gizmos.DrawWireSphere(transform.position, area.area);
            }
        }
    }

}
