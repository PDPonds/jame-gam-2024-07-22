using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneShake : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] float magintude;

    public void Shake()
    {
        StartCoroutine(shake(duration, magintude));
    }

    IEnumerator shake(float duration, float magnitude)
    {
        Vector3 original = transform.localPosition;

        float time = 0;

        while (time < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            Vector3 pos = new Vector3(x, y, original.z);
            transform.localPosition = pos;

            time += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = original;

    }
}
