using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour, IDamageable
{
    #region Ref
    SpriteRenderer spriteRender;
    #endregion

    [SerializeField] Transform visual;
    [SerializeField] Sprite freshFlower;
    [SerializeField] Transform spawnParticlePoint;

    bool isFresh;

    private void Start()
    {
        spriteRender = visual.GetComponent<SpriteRenderer>();
    }

    public void Death()
    {

    }

    public void Heal(float amount)
    {
        if (!isFresh)
        {
            Door.Instance.count++;
            PlayerUI.Instance.UpdateDoorCount();

            spriteRender.sprite = freshFlower;
            isFresh = true;

            GameObject go = Instantiate(Door.Instance.soulParticle, spawnParticlePoint.position, Quaternion.identity);
            Soul soul = go.GetComponent<Soul>();

            if (Door.Instance.count == 1)
            {
                soul.Setup(Door.Instance.doorPos1);
            }
            else if (Door.Instance.count == 2)
            {
                soul.Setup(Door.Instance.doorPos2);
            }
            else if (Door.Instance.count == 3)
            {
                soul.Setup(Door.Instance.doorPos3);
            }
        }
    }

    public void Hit(float damage)
    {

    }

}
