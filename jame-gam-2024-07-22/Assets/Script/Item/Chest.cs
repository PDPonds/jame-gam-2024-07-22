using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    #region Ref
    SpriteRenderer spriteRenderer;
    #endregion

    [SerializeField] Transform visual;

    public float maxHp;

    [SerializeField] Sprite disableChestSprite;
    [SerializeField] Sprite rewardSprite;
    [SerializeField] Sprite areadyOpenSprite;

    float curHp;

    bool isOpen;

    private void Awake()
    {
        spriteRenderer = visual.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ResetHP();
    }

    #region IDamageable

    void ResetHP()
    {
        curHp = maxHp;
        spriteRenderer.sprite = disableChestSprite;
    }

    public void Death()
    {
        PlayerUI.Instance.ShowSelectWandPanel(this);
        isOpen = true;
    }

    public void ChangeSpriteToOpenAready()
    {
        spriteRenderer.sprite = areadyOpenSprite;
    }

    public void Heal(float amount)
    {

    }

    public void Hit(float damage)
    {
        if (!isOpen)
        {
            curHp -= damage;
            if (curHp <= 0)
            {
                Death();
            }
        }
    }

    #endregion
}
