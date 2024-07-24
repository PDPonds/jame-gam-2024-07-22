using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailParticle : MonoBehaviour
{
    public float speed;

    Vector3 pos;

    public void Setup(Vector3 point)
    {
        pos = point;
    }
    private void Update()
    {
        Move();

        float dist = Vector3.Distance(transform.position, pos);
        if (dist < 0.5f)
        {
            Destroy(gameObject);
        }

    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, speed * Time.deltaTime);
    }

}
