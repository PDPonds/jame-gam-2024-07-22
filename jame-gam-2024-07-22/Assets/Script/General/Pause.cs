using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pause : Singleton<Pause>
{
    public bool isPause;

    public void TogglePauseButton()
    {
        if (isPause)
        {
            UnPauseGame();
            PlayerUI.Instance.HidePause();
        }
        else
        {
            PauseGame();
            PlayerUI.Instance.ShowPause();
        }
    }

    public void PauseGame()
    {
        isPause = true;

        PlayerManager.Instance.enabled = false;

        AudioManager.Instance.Pause("Walk");

        PlayerManager.Instance.rb.useGravity = false;
        PlayerManager.Instance.rb.velocity = Vector3.zero;

        RangeSkillObject[] skillObject = FindObjectsOfType<RangeSkillObject>();
        if (skillObject.Length > 0)
        {
            for (int i = 0; i < skillObject.Length; i++)
            {
                skillObject[i].enabled = false;
            }
        }

        EnemyController[] eCon = FindObjectsOfType<EnemyController>();
        if (eCon.Length > 0)
        {
            for (int i = 0; i < eCon.Length; i++)
            {
                eCon[i].enabled = false;
            }
        }

        Animator[] anim = FindObjectsOfType<Animator>();
        if (anim.Length > 0)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].enabled = false;
            }
        }

        NavMeshAgent[] agent = FindObjectsOfType<NavMeshAgent>();
        if (agent.Length > 0)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                agent[i].enabled = false;
            }
        }
    }

    public void UnPauseGame()
    {
        isPause = false;

        PlayerManager.Instance.enabled = true;

        PlayerManager.Instance.rb.useGravity = true;

        RangeSkillObject[] skillObject = FindObjectsOfType<RangeSkillObject>();
        if (skillObject.Length > 0)
        {
            for (int i = 0; i < skillObject.Length; i++)
            {
                skillObject[i].enabled = true;
            }
        }


        EnemyController[] eCon = FindObjectsOfType<EnemyController>();
        if (eCon.Length > 0)
        {
            for (int i = 0; i < eCon.Length; i++)
            {
                eCon[i].enabled = true;
            }
        }

        Animator[] anim = FindObjectsOfType<Animator>();
        if (anim.Length > 0)
        {
            for (int i = 0; i < anim.Length; i++)
            {
                anim[i].enabled = true;
            }
        }

        NavMeshAgent[] agent = FindObjectsOfType<NavMeshAgent>();
        if (agent.Length > 0)
        {
            for (int i = 0; i < agent.Length; i++)
            {
                agent[i].enabled = true;
            }
        }

    }

}
