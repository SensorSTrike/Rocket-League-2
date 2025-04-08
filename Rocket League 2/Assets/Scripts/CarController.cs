using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Wheel Settings")]
    [SerializeField] private Transform[] frontWheels;
    [SerializeField] private Transform[] allWheels;
    [SerializeField] private Transform[] backWheels;
    [SerializeField] private float maxSteerAngle = 30f;

    private PlayerInput playerInput;
    private InputAction throttleAction;
    private InputAction turnAction;


    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        throttleAction = playerInput.actions["Throttle"];
        turnAction = playerInput.actions["Turn"];
    }

    void FixedUpdate()
    {
        HandleMovement();
        UpdateWheels();
    }

    private void Jump()
    {

    }

    private void HandleMovement()
    {
        // Get input values for throttle and turn
        float throttle = throttleAction.ReadValue<float>(); // Forward/Backward input
        float turn = turnAction.ReadValue<float>();         // Left/Right input

        // Calculate forward force
        Vector3 forwardForce = transform.forward * throttle * acceleration;

        // Apply forward force if below max speed
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(forwardForce, ForceMode.Acceleration);
        }

        // Apply turn torque for steering
        float turnForce = turn * turnSpeed;
        rb.angularVelocity = new Vector3(rb.angularVelocity.x, turnForce, rb.angularVelocity.z);
    }

    private void UpdateWheels()
    {
        // Get steering input
        float steerAngle = turnAction.ReadValue<float>() * maxSteerAngle;

        // Roll and steer the wheels
        foreach (Transform wheel in allWheels)
        {
            // Roll the wheels (simulate forward/backward rotation)
            wheel.Rotate(Vector3.right, rb.linearVelocity.magnitude * Time.fixedDeltaTime * 360f, Space.Self);
            
        }

        foreach (Transform frontWheel in frontWheels)
        {
            // Steer the front wheels
            //Vector3 localEulerAngles = frontWheel.localEulerAngles;
            //localEulerAngles.y = steerAngle; // Apply the steering angle
            //frontWheel.localEulerAngles = localEulerAngles;
        }
    }
}
