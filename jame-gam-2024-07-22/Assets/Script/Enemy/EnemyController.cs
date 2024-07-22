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

    [Header("===== Enemy ======")]
    [SerializeField] Enemy enemy;
    [SerializeField] EnemyBehavior behavior;

    [Header("===== Range Attack ======")]
    [SerializeField] Transform spawnSkillPoint;
    [SerializeField] Transform rangeSkillIndicator;
    int curHp;

    AttackType curAttackType;

    float curChargeAttack;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        //anim = visual.GetComponent<Animator>();
    }

    private void Start()
    {
        //anim.runtimeAnimatorController = enemy.animOverride;
        SwithBehavoir(EnemyBehavior.Chase);
    }

    private void Update()
    {
        UpdateBehavoir();
    }

    #region Behavior

    public void SwithBehavoir(EnemyBehavior behavior)
    {
        this.behavior = behavior;
        switch (behavior)
        {
            case EnemyBehavior.Spawn:
                break;
            case EnemyBehavior.Chase:

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
        switch (curAttackType.type)
        {
            case ATKType.Range:
                InstanceRangeSkill();
                SwithBehavoir(EnemyBehavior.Chase);
                break;
            case ATKType.AroundUser:
                break;
            case ATKType.RandomArea:
                break;
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
                Vector3 scale = new Vector3(0, 0, 1) * size;
                rangeSkillIndicator.localScale = scale;

                break;
            case ATKType.AroundUser:
                break;
            case ATKType.RandomArea:
                break;
        }
    }

    public void HideAttackIndicator()
    {
        rangeSkillIndicator.gameObject.SetActive(false);
    }

    void InstanceRangeSkill()
    {
        GameObject go = Instantiate(curAttackType.skillParticle, spawnSkillPoint.position, Quaternion.identity);
        RangeSkillObject rangeObj = go.GetComponent<RangeSkillObject>();
        rangeObj.Setup(transform.forward, (RangeAttack)curAttackType);
    }

    #endregion

}
