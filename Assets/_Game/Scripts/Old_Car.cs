using UnityEditor;
using UnityEngine;

public class Old_Car : MonoBehaviour
{
    public float suspensionRestDist;
    public float springStrength;
    public float springDamper;

    [Space(10)]
    [Range(0f, 1f)]
    public float tireGripFactor;
    public float tireMass;
    public float tireRadius;

    [Space(10)]
    public float carTopSpeed;
    public float maxBreakForce;

    [Space(10)]
    [Tooltip("X-axis: Car's speed as % of top speed\n" +
        "Y-axis: % of available torque")]
    public AnimationCurve powerCurve;
    public AnimationCurve breakCurve;

    [Space(10)]
    [Range(10, 45)]
    [SerializeField] private float maxSteerAngle;

    [Space(10)]
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField]private Transform rearLeftWheel;
    [SerializeField]private Transform rearRightWheel;

    [Space(10)]
    [SerializeField] private Transform frontLeftWheelMesh;
    [SerializeField] private Transform frontRightWheelMesh;
    [SerializeField] private Transform rearLeftWheelMesh;
    [SerializeField] private Transform rearRightWheelMesh;

    private Vector3 frontLeftWheelOriginalPos;
    private Vector3 frontRightWheelOriginalPos;
    private Vector3 rearLeftWheelOriginalPos;
    private Vector3 rearRightWheelOriginalPos;

    private Rigidbody carRigidbody;

    private float horizontalInput;
    private float verticalInput;

    private float tireCurrentSpeed;
    private float visualTireLength;

    private Vector3 tirePositionInLastFrame;

    private void Start()
    {
        frontLeftWheelOriginalPos = frontLeftWheelMesh.position;
        frontRightWheelOriginalPos = frontRightWheelMesh.position;
        rearLeftWheelOriginalPos = rearLeftWheelMesh.position;
        rearRightWheelOriginalPos = rearRightWheelMesh.position;

        carRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        WheelSuspensionForce(frontLeftWheel, frontLeftWheelMesh, frontLeftWheelOriginalPos);
        WheelSuspensionForce(frontRightWheel, frontRightWheelMesh, frontRightWheelOriginalPos);
        WheelSuspensionForce(rearLeftWheel, rearLeftWheelMesh, rearLeftWheelOriginalPos);
        WheelSuspensionForce(rearRightWheel, rearRightWheelMesh, rearRightWheelOriginalPos);

        SteeringForce(frontLeftWheel);
        SteeringForce(frontRightWheel);
        SteeringForce(rearLeftWheel);
        SteeringForce(rearRightWheel);

        GetInput();
        HandleSteering();

        Move(frontLeftWheel);
        Move(frontRightWheel);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void HandleSteering()
    {
        float currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheel.localRotation = Quaternion.Euler(Vector3.up * currentSteerAngle);
        frontRightWheel.localRotation = Quaternion.Euler(Vector3.up * currentSteerAngle);
    }

    private void TirePlacement(Transform tireTransform)
    {
        visualTireLength = 2 * Mathf.PI * tireRadius;

        tireCurrentSpeed = Vector3.Dot(tireTransform.forward, (tireTransform.position - tirePositionInLastFrame));

        //tireCurrentSpeed = (tireTransform.position - tirePositionInLastFrame).magnitude;

        float valueToRotate = tireCurrentSpeed / tireRadius;
        tireTransform.Rotate(Vector3.right, valueToRotate * 360, Space.Self);

        tirePositionInLastFrame = tireTransform.position;
    }

    private void TirePositionAndRotation(Transform tireTransform, RaycastHit hit)
    {
        
    }

    private void Move(Transform tireTransform)
    {
        Vector3 accelDir = tireTransform.forward;

        if(verticalInput > 0)
        {
            float carSpeed = Vector3.Dot(transform.forward, carRigidbody.velocity);

            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

            float availableTorque = powerCurve.Evaluate(normalizedSpeed) * verticalInput;

            carRigidbody.AddForceAtPosition(accelDir * availableTorque, tireTransform.position);
        }
        else if(verticalInput < 0)
        {
            float carSpeed = Vector3.Dot(transform.forward, carRigidbody.velocity);

            float normalizedSpeed = Mathf.Clamp01(Mathf.Abs(carSpeed) / carTopSpeed);

            float availableTorque = breakCurve.Evaluate(normalizedSpeed) * verticalInput;

            carRigidbody.AddForceAtPosition(accelDir * availableTorque, tireTransform.position);
        }
    }

    private void WheelSuspensionForce(Transform tireTransform, Transform tireTransformMesh, Vector3 tireOriginalPos)
    {
        RaycastHit tireHit;

        if (Physics.Raycast(tireTransform.position, -tireTransform.up, out tireHit, suspensionRestDist))
        {
            Vector3 springDir = tireTransform.up;

            Vector3 tireWorldVel = carRigidbody.GetPointVelocity(tireTransform.position);

            float offset = suspensionRestDist - tireHit.distance;

            float vel = Vector3.Dot(springDir, tireWorldVel);

            float force = (offset * springStrength) - (vel * springDamper);

            carRigidbody.AddForceAtPosition(springDir * force, tireTransform.position);

            tireTransformMesh.position = tireHit.point + transform.up * tireRadius;

        }
        else
        {
            tireTransformMesh.localPosition = tireOriginalPos + transform.up * tireRadius;
        }
    }

    private void SteeringForce(Transform tireTransform)
    {
        Vector3 steeringDir = tireTransform.right;

        Vector3 tireWorldVel = carRigidbody.GetPointVelocity(tireTransform.position);

        float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);

        float desiredVelChanger = -steeringVel * tireGripFactor;

        float desiredAccel = desiredVelChanger / Time.fixedDeltaTime;

        carRigidbody.AddForceAtPosition(steeringDir * tireMass * desiredAccel, tireTransform.position);
    }
}
