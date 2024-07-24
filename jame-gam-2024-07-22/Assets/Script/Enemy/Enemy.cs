using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy / Enemy")]
public class Enemy : ScriptableObject
{
    [Header("===== HP =====")]
    public int maxHp;
    [Header("===== Attack =====")]
    public List<AttackType> attackType = new List<AttackType>();

}
