using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Wheel Settings")]
    [SerializeField] private float maxSteerAngle = 30f;
    [SerializeField] private Transform[] steeringPivots; // Y-axis steer
    [SerializeField] private Transform[] rollingWheels;  // X-axis roll


    private PlayerInput playerInput;
    private InputAction goForwardAction;
    private InputAction brakeAction;
    private InputAction turnAction;


    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        goForwardAction = playerInput.actions["Throttle"];
        brakeAction = playerInput.actions["Brake"];
        turnAction = playerInput.actions["Turn"];
    }

    void Update()
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
        float forward = goForwardAction.ReadValue<float>(); 
        float brake = brakeAction.ReadValue<float>();
        float throttle = forward - brake;
        float turn = turnAction.ReadValue<float>();         // Left/Right input

        // Calculate forward force
        Vector3 SpeedForce = transform.forward * throttle * acceleration * Time.deltaTime;

        
        // Apply forward force if below max speed
        if (rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(SpeedForce, ForceMode.Acceleration);
        }

        // Only apply turning when the car is moving forward (velocity > threshold)
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            // Apply turn torque for steering
            float turnForce = turn * turnSpeed;
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, turnForce, rb.angularVelocity.z);
        }


    }
    private void UpdateWheels()
    {
        // Get steering input
        float steerAngle = turnAction.ReadValue<float>() * maxSteerAngle;

        float direction = Mathf.Sign(Vector3.Dot(rb.linearVelocity, transform.forward));

        // Roll and steer the wheels
        foreach (Transform wheel in rollingWheels)
        {
            
            // Roll the wheels (simulate forward/backward wheel rotation)
            wheel.Rotate(Vector3.right, direction * rb.linearVelocity.magnitude * Time.deltaTime * 360f);
            
        }

        foreach (Transform pivot
            in steeringPivots)
        {
            // Steer the front wheels
            Vector3 localEuler = pivot.localEulerAngles;
            localEuler.y = steerAngle; // Apply the steering angle
            pivot.localEulerAngles = localEuler;
        }
    }
}
