using UnityEngine;

public class LookatCam : MonoBehaviour
{
    private void Update()
    {
        lookatCamera();
    }

    void lookatCamera()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

}
