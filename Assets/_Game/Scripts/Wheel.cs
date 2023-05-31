using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private WheelCollider wheelCollider;

    [SerializeField] private TrailRenderer trailRenderer;

    private void FixedUpdate()
    {
        UpdateWheel(wheelCollider);
    }

    private void UpdateWheel(WheelCollider wheelCollider)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);
        transform.rotation = rotation * Quaternion.Euler(0, 0, 90);
        transform.position = position;
    }

    public void StartEmitting()
    {
        trailRenderer.emitting = true;
    }

    public void StopEmitting()
    {
        trailRenderer.emitting = false;
    }
}
