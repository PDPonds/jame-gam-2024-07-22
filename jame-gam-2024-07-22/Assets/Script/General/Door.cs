using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Singleton<Door>
{
    public int count;

    public Transform doorPos1;
    public Transform doorPos2;
    public Transform doorPos3;

    public GameObject soulParticle;
    public float soulSpeed;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (doorPos1.childCount > 0 && doorPos2.childCount > 0 && doorPos3.childCount > 0)
            {
                PlayerUI.Instance.ShowVictory();
                Pause.Instance.PauseGame();
            }
            else
            {
                Dialogue.Instance.StartDoor();
            }
        }
    }

}
