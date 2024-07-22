using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ATKType
{
    Range, AroundUser, RandomArea
}

public class AttackType : ScriptableObject
{
    public GameObject skillParticle;

    public float chargeTime;

    public int count;
    public float delayPerCount;

    [Header("===== Animator =====")]
    public AnimatorOverrideController animOverride;

    [Header("===== Type =====")]
    public ATKType type;

}
