using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy / AttackType / AroundUserAttack")]
public class AroundUserAttack : AttackType
{
    public float area;

    public AroundUserAttack()
    {
        type = ATKType.AroundUser;
    }
}
