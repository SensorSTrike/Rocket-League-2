using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    [SerializeField] private float acceleration = 100f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float gripStrength = 5f;

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

        playerInput = GetComponent<PlayerInput>(); // Ensure a PlayerInput component is attached
        throttleAction = playerInput.actions["Throttle"];
        turnAction = playerInput.actions["Turn"];
    }

    void Update()
    {
        HandleMovement();
        ApplyTireGrip();
        UpdateWheels();
    }

    private void HandleMovement()
    {
        float forwardInput = throttleAction.ReadValue<float>(); // Supports both keys & gamepad
        float turnInput = turnAction.ReadValue<float>();

        if (forwardInput != 0)
        {
            Vector3 moveDirection = rb.transform.forward;
            Vector3 force = moveDirection * forwardInput * acceleration;

            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(force, ForceMode.Acceleration);
            }
        }

        rb.AddTorque(Vector3.up * turnInput * turnSpeed);
    }

    private void ApplyTireGrip()
    {
        Vector3 lateralVelocity = Vector3.Project(rb.linearVelocity, transform.right);
        rb.AddForce(-lateralVelocity * gripStrength, ForceMode.Acceleration);
    }

    private void UpdateWheels()
    {
        float speed = Vector3.Dot(rb.linearVelocity, transform.forward); // signed forward speed
        float rotationAmount = speed * Time.deltaTime * 360f;

        float steerInput = turnAction.ReadValue<float>();
        float steerAngle = steerInput * maxSteerAngle;

        foreach (Transform wheel in backWheels)
        {
            // Just roll the back wheels
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }

        foreach (Transform wheel in frontWheels)
        {
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }
    }
}
