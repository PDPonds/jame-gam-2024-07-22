using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    Transform point;
    Vector3 pos;

    public void Setup(Transform point)
    {
        this.point = point;
        pos = point.position;
    }
    private void Update()
    {

        float dist = Vector3.Distance(transform.position, pos);
        if (dist < 0.5f)
        {
            if (transform.parent == null)
            {
                transform.SetParent(point);
                transform.localPosition = Vector3.zero;
                PlayerUI.Instance.HideDoorInfomation();
            }
        }
        else if (dist < 5f)
        {
            PlayerUI.Instance.ShowDoorInfomation();
            Move();
        }
        else
        {
            Move();
        }

    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, Door.Instance.soulSpeed * Time.deltaTime);
    }
}
