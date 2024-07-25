using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.transform.position = PlayerManager.Instance.checkPoint;
            PlayerManager.Instance.Hit(20f);
        }
    }
}
