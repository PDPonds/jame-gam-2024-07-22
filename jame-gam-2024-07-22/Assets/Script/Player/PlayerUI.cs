using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
{
    [Header("- HP")]
    [SerializeField] Image hpFill;

    [Header("- Decay")]
    [SerializeField] Image decaySkillFill;

    [Header("- Repaire")]
    [SerializeField] Image repaireManaFill;
    [SerializeField] Image repaireSkillFill;

    [Header("- Dash")]
    [SerializeField] Image dashFill;

    private void Update()
    {
        UpdateDashFill();
        UpdateDecayFill();
        UpdateRepairFill();
    }

    private void Start()
    {
        UpdateRepaireManaFill();
    }

    public void UpdateHPFill()
    {
        float percent = PlayerManager.Instance.curHp / PlayerManager.Instance.maxHp;
        hpFill.fillAmount = percent;
    }

    public void UpdateRepaireManaFill()
    {
        float percent = PlayerManager.Instance.curRepairMana / PlayerManager.Instance.maxRepairMana;
        repaireManaFill.fillAmount = percent;
    }

    void UpdateDashFill()
    {
        float percent = PlayerManager.Instance.curDashDelay / PlayerManager.Instance.dashDelay;
        dashFill.fillAmount = percent;
    }

    void UpdateDecayFill()
    {
        float percent = PlayerManager.Instance.curDecayDelay / PlayerManager.Instance.decayDelay;
        decaySkillFill.fillAmount = percent;
    }

    void UpdateRepairFill()
    {
        if (PlayerManager.Instance.curRepairDelay > 0)
        {
            float percent = PlayerManager.Instance.curRepairDelay / PlayerManager.Instance.repairDelay;
            repaireSkillFill.fillAmount = percent;
        }
        else
        {
            if (PlayerManager.Instance.CanUseRepair())
            {
                repaireSkillFill.fillAmount = 0;
            }
            else
            {
                repaireSkillFill.fillAmount = 1;
            }
        }
    }

}
