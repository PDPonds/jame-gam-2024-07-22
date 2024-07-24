using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillIndicator : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GroundTrigger"))
        {
            if (other.TryGetComponent<GroundObject>(out GroundObject groundObject))
            {
                groundObject.ShowOutline();
            }

            if (other.TryGetComponent<GrassGroundObject>(out GrassGroundObject grassGroundObject))
            {
                grassGroundObject.ShowOutline();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("GroundTrigger"))
        {
            if (other.TryGetComponent<GroundObject>(out GroundObject groundObject))
            {
                groundObject.HideOutline();
            }

            if (other.TryGetComponent<GrassGroundObject>(out GrassGroundObject grassGroundObject))
            {
                grassGroundObject.HideOutline();
            }
        }
    }

}
