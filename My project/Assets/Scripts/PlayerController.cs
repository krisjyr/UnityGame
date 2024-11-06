using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Movement values
    public float maxSpeed = 4000.0f;
    public float maxThrust = 100.0f;
    public float thrustAcceleration = 15.0f;
    public float dragCoefficient = 0.1f;
    public float gravityForce = 9.81f;
    public float stallAngle = 20.0f;
    public float stallSpeedReduction = 0.5f;
    public float liftCoefficient = 1.0f;
    public float minLiftSpeed = 500.0f;

    // Control authority thresholds
    public float minControlSpeed = 50.0f;
    public float fullControlSpeed = 400.0f;

    // Smoothing values
    public float rotationSmoothSpeed = 10f;    // Higher = faster smoothing
    private Vector3 smoothedRotationInput;           // The rotation we're smoothing towards
    private Vector3 currentRotationVelocity;  // Used for SmoothDamp

    // Maximum rotation speeds
    public float turnSpeed = 15.0f;
    public float pitchSpeed = 45.0f;
    public float rollSpeed = 45.0f;

    // Current values
    private float currentSpeed = 0.0f;
    private float currentThrust = 0.0f;
    private Vector3 velocity;
    private float controlAuthority = 0.0f;

    // UI elements
    public AudioSource Thrusters;
    public TMP_Text scoreText;
    public TMP_Text winText;
    public TMP_Text thrustText;
    public TMP_Text speedText;
    public TMP_Text controlText;
    private Rigidbody rb;
    private int score = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = false;
            rb.drag = 0.0f;
            rb.angularDrag = 0.5f;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        scoreText.GetComponent<TMP_Text>();
        scoreText.text = "Score: " + score;
    }

    void Update()
    {
        UpdateControlAuthority();
        InputHandling();
        ApplyPhysics();
        UpdateVolume();

        thrustText.text = $"Thrust: {Math.Round(currentThrust)}% / {Math.Round(maxThrust)}%";
        speedText.text = $"Speed: {Math.Round(currentSpeed)} km/h";
        if (controlText != null)
        {
            controlText.text = $"Control: {Mathf.Round(controlAuthority * 100)}%";
        }
    }

        private void UpdateVolume()
    {
        if (Thrusters != null)
        {
            float normalizedSpeed = Mathf.Clamp01((currentThrust - 0) / (100 - 0));
            float adjustedVolume = Mathf.Lerp(0, 100, normalizedSpeed);
            
            Thrusters.volume = adjustedVolume;
        }
    }


    void UpdateControlAuthority()
    {
        controlAuthority = Mathf.Clamp01((currentSpeed - minControlSpeed) / (fullControlSpeed - minControlSpeed));
    }

    void InputHandling()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentThrust = Mathf.Min(currentThrust + thrustAcceleration * Time.deltaTime, maxThrust);
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentThrust = Mathf.Max(currentThrust - thrustAcceleration * Time.deltaTime, 0f);
        }

        Vector3 rotation = Vector3.zero;

        // Basic input
        if (Input.GetKey(KeyCode.S)) rotation.x -= pitchSpeed;
        if (Input.GetKey(KeyCode.W)) rotation.x += pitchSpeed;
        if (Input.GetKey(KeyCode.A)) rotation.z += rollSpeed;
        if (Input.GetKey(KeyCode.D)) rotation.z -= rollSpeed;
        if (Input.GetKey(KeyCode.Q)) rotation.y -= turnSpeed;
        if (Input.GetKey(KeyCode.E)) rotation.y += turnSpeed;

        rotation *= controlAuthority;

        smoothedRotationInput = Vector3.SmoothDamp(
            smoothedRotationInput,
            rotation,
            ref currentRotationVelocity,
            1f / rotationSmoothSpeed
        );

        if (controlAuthority > 0)
        {
            transform.Rotate(smoothedRotationInput * Time.deltaTime, Space.Self);
        }
    }

    void ApplyPhysics()
    {
        Vector3 thrustForce = transform.forward * (currentThrust / maxThrust * maxSpeed);

        Vector3 dragForce = -rb.velocity.normalized * dragCoefficient * rb.velocity.sqrMagnitude;

        float angleOfAttack = Vector3.Angle(transform.forward, rb.velocity.normalized);
        if (rb.velocity.magnitude < 0.1f) angleOfAttack = 0f;

        float normalizedSpeed = Mathf.Clamp01(currentSpeed / minLiftSpeed);
        float liftMultiplier = 1.0f;

        if (Mathf.Abs(angleOfAttack) > stallAngle)
        {
            liftMultiplier = Mathf.Lerp(stallSpeedReduction, 1.0f,
                (stallAngle * 2 - Mathf.Abs(angleOfAttack)) / stallAngle);
            liftMultiplier = Mathf.Max(0.1f, liftMultiplier);
        }

        Vector3 liftForce = transform.up * liftCoefficient * normalizedSpeed * liftMultiplier * 100f; // Increased lift multiplier

        Vector3 gravityForces = Vector3.down * this.gravityForce * rb.mass;

        rb.AddForce(thrustForce + dragForce + liftForce + gravityForces);

        currentSpeed = rb.velocity.magnitude;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with: " + collision.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
    }
}