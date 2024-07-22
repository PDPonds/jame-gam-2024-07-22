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
    #endregion

    [SerializeField] Transform visual;
    int curHp;

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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //anim = visual.GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateBehavoir();
    }

    #region Behavior

    public void OnSpawn()
    {
        curHp = enemy.maxHp;
        SwithBehavoir(EnemyBehavior.Spawn);
        //anim.runtimeAnimatorController = enemy.animOverride;
    }

    public void SwithBehavoir(EnemyBehavior behavior)
    {
        this.behavior = behavior;
        switch (behavior)
        {
            case EnemyBehavior.Spawn:
                break;
            case EnemyBehavior.Chase:

                attackAlready = false;
                SetAttackType();

                break;
            case EnemyBehavior.ChargeToAttack:

                curChargeAttack = 0;
                ShowAttackIndicator();

                break;
            case EnemyBehavior.Attack:

                Attack();
                HideAttackIndicator();

                break;
            case EnemyBehavior.Death:
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

                break;
            case EnemyBehavior.Chase:

                agent.SetDestination(PlayerManager.Instance.transform.position);
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
        //anim.runtimeAnimatorController = curAttackType.animOverride;
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

    #endregion

    #region IDamageable

    void ResetHP()
    {
        curHp = enemy.maxHp;
    }

    public void Hit()
    {
        curHp--;
        if (curHp <= 0)
        {
            SwithBehavoir(EnemyBehavior.Death);
        }
    }

    public void Heal()
    {
        curHp++;
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
                Vector3 rangeScale = new Vector3(0, 0, 1) * size;
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

        Collider[] cols = Physics.OverlapSphere(transform.position, area, playerMask);
        if (cols.Length > 0)
        {
            PlayerManager.Instance.Hit();
        }
    }

    #endregion

}
