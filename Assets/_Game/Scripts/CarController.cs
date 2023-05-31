using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum DriveMode
    {
        Front,
        Rear,
        AWD
    };

    public DriveMode driveMode = DriveMode.Rear;

    [Space(10)]
    [SerializeField] private float accelerationMultiplier;

    [Range(10, 45)]
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private float breakForce;

    [Space(10)]
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Space(10)]
    [SerializeField] private GameObject frontLeftWheel;
    [SerializeField] private GameObject frontRightWheel;
    [SerializeField] private GameObject rearLeftWheel;
    [SerializeField] private GameObject rearRightWheel;

    [Space(10)]
    [SerializeField] private Transform centerOfMass;

    private Rigidbody rb;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;

    private bool handBreak;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.transform.localPosition;
    }

    private void FixedUpdate()
    {
        UserInput();
        HandleMotor();
        HandleSteering();
        CheckBreak();
    }

    private void UserInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        handBreak = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        float appliedTorque = verticalInput * accelerationMultiplier;

        frontLeftWheelCollider.motorTorque = driveMode == DriveMode.Rear ? 0 : appliedTorque;
        frontRightWheelCollider.motorTorque = driveMode == DriveMode.Rear ? 0 : appliedTorque;
        rearLeftWheelCollider.motorTorque = driveMode == DriveMode.Front ? 0 : appliedTorque;
        rearRightWheelCollider.motorTorque = driveMode == DriveMode.Front ? 0 : appliedTorque;

        currentbreakForce = handBreak ? breakForce : 0f;
        ApplyBreaking();
    }
    
    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void CheckBreak()
    {

        if(handBreak)
        {
            frontLeftWheel.GetComponent<Wheel>().StartEmitting();
            frontRightWheel.GetComponent<Wheel>().StartEmitting();
            rearLeftWheel.GetComponent<Wheel>().StartEmitting();
            rearRightWheel.GetComponent<Wheel>().StartEmitting();
        }
        else
        {
            frontLeftWheel.GetComponent<Wheel>().StopEmitting();
            frontRightWheel.GetComponent<Wheel>().StopEmitting();
            rearLeftWheel.GetComponent<Wheel>().StopEmitting();
            rearRightWheel.GetComponent<Wheel>().StopEmitting();
        }
    }
}
