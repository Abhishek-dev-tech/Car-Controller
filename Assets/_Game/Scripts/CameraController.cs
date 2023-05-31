using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Space(10)]
    [SerializeField] private float smoothTime;

    private Vector3 velocity = Vector3.zero;

    private void FixedUpdate()
    {
        Follow();
    }

    private void Follow()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }
}
