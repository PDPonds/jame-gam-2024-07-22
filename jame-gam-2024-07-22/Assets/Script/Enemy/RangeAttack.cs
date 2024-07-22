using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy / AttackType / RangeAttack")]
public class RangeAttack : AttackType
{
    public float speed;
    public float time;

    public RangeAttack()
    {
        type = ATKType.Range;
    }

}
