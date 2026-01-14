using UnityEngine;

public class Rotator : MonoBehaviour
{
    void Update()
    {
        transform.localRotation *= Quaternion.Euler(0, 30 * Time.deltaTime, 0);
    }
}
