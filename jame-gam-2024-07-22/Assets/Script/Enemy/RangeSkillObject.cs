using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeSkillObject : MonoBehaviour
{
    RangeAttack skill;
    Vector3 moveDir;

    float speed;
    float time;

    public void Setup(Vector3 dir, RangeAttack skill)
    {
        moveDir = dir;
        this.skill = skill;
        speed = skill.speed;
        time = skill.time;

        Destroy(gameObject, time);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(moveDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

}
