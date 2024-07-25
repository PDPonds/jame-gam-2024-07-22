using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : Singleton<PlayerUI>
{
    [Header("- HP")]
    public GameObject hpBorder;
    [SerializeField] Image hpFill;

    [Header("- Decay")]
    public GameObject decaySkillBorder;
    [SerializeField] Image decaySkillFill;

    [Header("- Repaire")]
    public GameObject repairSkillBorder;
    [SerializeField] Image repaireSkillFill;

    [Header("- Dash")]
    public GameObject dashSkillBorder;
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

    [Header("- Pause")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] Transform pauseText;
    [Header("- Gmae Result")]
    [SerializeField] GameObject defeatText;
    [SerializeField] GameObject victoryText;
    [SerializeField] GameObject defeatPanel;
    [SerializeField] GameObject victoryPanel;
    [SerializeField] Button restartInDefeatGame;
    [SerializeField] Button restartInVictoryGame;


    [Header("- Door")]
    [SerializeField] GameObject doorInfomation;
    public GameObject doorCountBorder;
    [SerializeField] TextMeshProUGUI doorCountText;

    private void Start()
    {
        restartInDefeatGame.onClick.AddListener(RestartGame);
        restartInVictoryGame.onClick.AddListener(RestartGame);

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
        AudioManager.Instance.PlayOneShot("OpenUI");
        Pause.Instance.PauseGame();
        curChest = chest;
        selectWandPanel.SetActive(true);

        selectNewWand.onClick.RemoveAllListeners();

        Wand wand = allWands.GetRandomWand();
        newWand = wand;
        selectNewWand.onClick.AddListener(() => SelectNewWand(wand));

        UpdateSelectWandInfo();

        LeanTween.scale(selectNewWand.gameObject, Vector3.one, 0.25f).setEaseInOutCubic();
        LeanTween.scale(cancleNewWand.gameObject, Vector3.one, 0.25f).setEaseInOutCubic();

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
        newSkillAreaText.text = $"Attack Area : {newWand.skillArea.ToString("N1")}";
        newSkillRangeText.text = $"Max Attack Range : {newWand.skillRange.ToString("N1")}";
        newDecayDamageText.text = $"Decay Damage : {newWand.decayDamage.ToString("N1")}";
        newToGetHPText.text = $"Mana Leech : {newWand.toGetHP.ToString("N1")}";
        newToUseText.text = $"Mana Transferring : {newWand.toUseHP.ToString("N1")}";
        newRepairAmountText.text = $"Heal Amount : {newWand.repairAmount.ToString("N1")}";

        newSkillAreaStatus.sprite = GetWandStatus(curWand.skillArea, newWand.skillArea);
        newSkillRangeStatus.sprite = GetWandStatus(curWand.skillRange, newWand.skillRange);
        newDecayDamageStatus.sprite = GetWandStatus(curWand.decayDamage, newWand.decayDamage);
        newToGetHPStatus.sprite = GetWandStatus(curWand.toGetHP, newWand.toGetHP);
        newToUseStatus.sprite = GetWandStatus(curWand.toUseHP, newWand.toUseHP);
        newRepairAmountStatus.sprite = GetWandStatus(curWand.repairAmount, newWand.repairAmount);

    }

    Sprite GetWandStatus(float curStatus, float newStatus)
    {
        if (curStatus < newStatus) return statusUp;
        else if (curStatus > newStatus) return statusDown;
        else return statusEquals;
    }

    void SelectNewWand(Wand wand)
    {
        PlayerManager.Instance.curWand = wand;
        AudioManager.Instance.PlayOneShot("UIClick");
        UpdateCurWand();
        HideSelectWandPanel();
    }

    void HideSelectWandPanel()
    {
        LeanTween.scale(selectNewWand.gameObject, Vector3.zero, 0.25f).setEaseInOutCubic();
        LeanTween.scale(cancleNewWand.gameObject, Vector3.zero, 0.25f).setEaseInOutCubic().setOnComplete(() =>
        {
            AudioManager.Instance.PlayOneShot("UIClick");
            curChest.ChangeSpriteToOpenAready();
            Pause.Instance.UnPauseGame();
            selectWandPanel.SetActive(false);
            AudioManager.Instance.PlayOneShot("OpenUI");
        });
    }

    public void UpdateCurWand()
    {
        curWand.sprite = PlayerManager.Instance.curWand.wandIcon;
    }

    public void ShowPause()
    {
        AudioManager.Instance.PlayOneShot("UIClick");
        pausePanel.SetActive(true);
        LeanTween.scale(pauseText.gameObject, Vector3.one, 0.25f).setEaseInOutCubic();
    }

    public void HidePause()
    {
        AudioManager.Instance.PlayOneShot("UIClick");
        LeanTween.scale(pauseText.gameObject, Vector3.zero, 0.25f).setEaseInOutCubic()
            .setOnComplete(() => pausePanel.SetActive(false));
    }

    public void ShowDefeat()
    {
        defeatPanel.SetActive(true);
        AudioManager.Instance.PlayOneShot("OpenUI");
        LeanTween.scale(defeatText, Vector3.one, 0.25f).setEaseInOutCubic();
        LeanTween.scale(restartInDefeatGame.gameObject, Vector3.one, 0.25f).setEaseInOutCubic();
    }

    void RestartGame()
    {
        AudioManager.Instance.PlayOneShot("UIClick");
        SceneManager.LoadScene(0);
    }

    public void ShowDoorInfomation()
    {
        doorInfomation.SetActive(true);
    }

    public void HideDoorInfomation()
    {
        doorInfomation.SetActive(false);
    }

    public void ShowVictory()
    {
        AudioManager.Instance.PlayOneShot("OpenUI");
        victoryPanel.SetActive(true);
        LeanTween.scale(victoryText, Vector3.one, 0.25f).setEaseInOutCubic();
        LeanTween.scale(restartInVictoryGame.gameObject, Vector3.one, 0.25f).setEaseInOutCubic();
    }

    public void UpdateDoorCount()
    {
        doorCountText.text = $"{Door.Instance.count} / 3";
    }

}
