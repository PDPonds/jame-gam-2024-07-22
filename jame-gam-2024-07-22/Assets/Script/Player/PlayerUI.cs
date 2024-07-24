using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
{
    [Header("- HP")]
    [SerializeField] Image hpFill;

    [Header("- Decay")]
    [SerializeField] Image decaySkillFill;

    [Header("- Repaire")]
    [SerializeField] Image repaireSkillFill;

    [Header("- Dash")]
    [SerializeField] Image dashFill;

    [Header("- Chest")]
    [SerializeField] AllWands allWands;
    Chest curChest;
    Wand newWand;
    [SerializeField] GameObject selectWandPanel;
    [SerializeField] Button selectNewWand;
    [SerializeField] Button cancleNewWand;
    [Header("- Cur Wand")]
    [SerializeField] Image curWand;

    [Header("- Select New Wand")]
    [Header("- Cur Wand")]
    [SerializeField] Image curWandIcon;
    [SerializeField] TextMeshProUGUI curSkillAreaText;
    [SerializeField] TextMeshProUGUI curSkillRangeText;
    [SerializeField] TextMeshProUGUI curDecayDamageText;
    [SerializeField] TextMeshProUGUI curToGetHPText;
    [SerializeField] TextMeshProUGUI curToUseText;
    [SerializeField] TextMeshProUGUI curRepairAmountText;

    [Header("- New Wand")]
    [SerializeField] Image newWandIcon;
    [SerializeField] TextMeshProUGUI newSkillAreaText;
    [SerializeField] TextMeshProUGUI newSkillRangeText;
    [SerializeField] TextMeshProUGUI newDecayDamageText;
    [SerializeField] TextMeshProUGUI newToGetHPText;
    [SerializeField] TextMeshProUGUI newToUseText;
    [SerializeField] TextMeshProUGUI newRepairAmountText;

    [SerializeField] Image newSkillAreaStatus;
    [SerializeField] Image newSkillRangeStatus;
    [SerializeField] Image newDecayDamageStatus;
    [SerializeField] Image newToGetHPStatus;
    [SerializeField] Image newToUseStatus;
    [SerializeField] Image newRepairAmountStatus;


    [SerializeField] Sprite statusUp;
    [SerializeField] Sprite statusDown;
    [SerializeField] Sprite statusEquals;

    private void Start()
    {
        cancleNewWand.onClick.AddListener(HideSelectWandPanel);
        UpdateCurWand();
    }

    private void Update()
    {
        UpdateDashFill();
        UpdateDecayFill();
        UpdateRepairFill();
    }

    public void UpdateHPFill()
    {
        float percent = PlayerManager.Instance.curHp / PlayerManager.Instance.maxHp;
        hpFill.fillAmount = percent;
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

    public void ShowSelectWandPanel(Chest chest)
    {
        curChest = chest;
        selectWandPanel.SetActive(true);

        selectNewWand.onClick.RemoveAllListeners();

        Wand wand = allWands.GetRandomWand();
        newWand = wand;
        selectNewWand.onClick.AddListener(() => SelectNewWand(wand));

        UpdateSelectWandInfo();

    }

    void UpdateSelectWandInfo()
    {
        Wand curWand = PlayerManager.Instance.curWand;

        curWandIcon.sprite = curWand.wandIcon;
        curSkillAreaText.text = $"Attack Area : {curWand.skillArea.ToString("N1")}";
        curSkillRangeText.text = $"Max Attack Range : {curWand.skillRange.ToString("N1")}";
        curDecayDamageText.text = $"Decay Damage : {curWand.decayDamage.ToString("N1")}";
        curToGetHPText.text = $"Mana Leech : {curWand.toGetHP.ToString("N1")}";
        curToUseText.text = $"Mana Transferring : {curWand.toUseHP.ToString("N1")}";
        curRepairAmountText.text = $"Heal Amount : {curWand.repairAmount.ToString("N1")}";

        newWandIcon.sprite = newWand.wandIcon;
        newSkillAreaText.text = $"Attack Area : {curWand.skillArea.ToString("N1")}";
        newSkillRangeText.text = $"Max Attack Range : {curWand.skillRange.ToString("N1")}";
        newDecayDamageText.text = $"Decay Damage : {curWand.decayDamage.ToString("N1")}";
        newToGetHPText.text = $"Mana Leech : {curWand.toGetHP.ToString("N1")}";
        newToUseText.text = $"Mana Transferring : {curWand.toUseHP.ToString("N1")}";
        newRepairAmountText.text = $"Heal Amount : {curWand.repairAmount.ToString("N1")}";

        newSkillAreaStatus.sprite = GetWandStatus(curWand.skillArea, newWand.skillArea);
        newSkillRangeStatus.sprite = GetWandStatus(curWand.skillRange, newWand.skillRange);
        newDecayDamageStatus.sprite = GetWandStatus(curWand.decayDamage, newWand.decayDamage);
        newToGetHPStatus.sprite = GetWandStatus(curWand.toGetHP, newWand.toGetHP);
        newToUseStatus.sprite = GetWandStatus(curWand.toUseHP, newWand.toUseHP);
        newRepairAmountStatus.sprite = GetWandStatus(curWand.repairAmount, newWand.repairAmount);

    }

    Sprite GetWandStatus(float curStatus, float newStatus)
    {
        if (curStatus > newStatus) return statusUp;
        else if (curStatus < newStatus) return statusDown;
        else return statusEquals;
    }

    void SelectNewWand(Wand wand)
    {
        PlayerManager.Instance.curWand = wand;
        UpdateCurWand();
        HideSelectWandPanel();
    }

    void HideSelectWandPanel()
    {
        curChest.ChangeSpriteToOpenAready();
        selectWandPanel.SetActive(false);
    }

    public void UpdateCurWand()
    {
        curWand.sprite = PlayerManager.Instance.curWand.wandIcon;
    }

}
