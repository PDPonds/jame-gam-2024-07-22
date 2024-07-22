using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy / AttackType / RandomAreaAttack")]
public class RandomAreaAttack : AttackType
{
    public float maxDistance;
    public float skillArea;

    public RandomAreaAttack()
    {
        type = ATKType.RandomArea;
    }

}
