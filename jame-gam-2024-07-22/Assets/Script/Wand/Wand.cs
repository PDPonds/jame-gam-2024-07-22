using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Wand")]
public class Wand : ScriptableObject
{
    public Sprite wandIcon;

    [Header("===== Area =====")]
    public float skillArea;

    [Header("===== Range =====")]
    public float skillRange;

    [Header("===== Damage =====")]
    [Header("- Decay")]
    public float decayDamage;

    [Header("- Repair")]
    public float toGetRepair;
    public float toUseRepair;
    public float repairAmount;

}
