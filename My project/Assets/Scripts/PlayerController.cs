using System;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour
{
    [Header("Aircraft Specifications")]
    [Tooltip("Aircraft mass in kilograms")]
    public float mass = 100.0f;
    [Tooltip("Wing area in square meters")]
    public float wingArea = 16.0f;
    [Tooltip("Maximum engine thrust in Newtons")]
    public float maxThrust = 5000f;
    [Tooltip("Wingspan in meters")]
    public float wingspan = 10f;

    [Header("Control Settings")]
    [Range(0.1f, 100f)]
    public float elevatorPower = 50f;
    [Range(0.1f, 100f)]
    public float aileronPower = 50f;
    [Range(0.1f, 100f)]
    public float rudderPower = 30f;
    public float throttleResponse = 2f;
    [Tooltip("How quickly controls respond to input (lower = smoother)")]
    [Range(0.1f, 1f)]
    public float controlSmoothingFactor = 0.1f;

    [Header("Aerodynamic Coefficients")]
    public float CLzero = 0.25f;     // Lift coefficient at zero angle of attack
    public float CLslope = 0.08f;    // Lift curve slope per degree
    public float CDmin = 0.025f;     // Minimum drag coefficient
    public float K = 0.045f;         // Induced drag factor
    public float maxAlpha = 15f;     // Stall angle in degrees

    // Internal variables
    private float currentThrust;
    private Rigidbody rb;
    private float throttle;
    private Vector3 rotationInput;
    private Vector3 currentRotation;
    private float angleOfAttack;
    private float airspeed;
    private const float airDensity = 1.225f;
    private bool isGrounded;

    // UI Elements
    public TMP_Text speedText;
    public TMP_Text thrustText;
    public TMP_Text angleText;
    public TMP_Text wintext;
    public TMP_Text scoretext;
    public TMP_Text timertext;
    public AudioSource Thrusters;
    public ParticleSystem thrusterParticles;

    private float Score = 0;
    private float timer = 60f;
    private float waitTime = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass;
        rb.useGravity = false;
        rb.drag = 0;
        rb.angularDrag = 1;
        currentRotation = Vector3.zero;
        scoretext.text = "Score: " + Score;
    }

    void Update()
    {
        HandleTimer();
        HandleRestartAndQuit();
    }

    void FixedUpdate()
    {
        HandleInput();
        CalculateAerodynamics();
        ApplyForces();
        ApplyRotation();
        UpdateGroundedState();
        UpdateUI();
    }

    void HandleTimer()
    {
        if (timer <= 0)
        {
            wintext.text = "Time's up!";
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        else
        {
            timer -= Time.deltaTime;
            timertext.text = "Time: " + Math.Round(timer).ToString();
        }
    }

    void HandleInput()
    {
        // Throttle input with smooth interpolation
        if (Input.GetKey(KeyCode.LeftShift))
            throttle = Mathf.MoveTowards(throttle, 1f, Time.fixedDeltaTime * throttleResponse);
        else if (Input.GetKey(KeyCode.LeftControl))
            throttle = Mathf.MoveTowards(throttle, 0f, Time.fixedDeltaTime * throttleResponse);

        // Get raw rotation inputs
        Vector3 targetRotation = new Vector3(
            Input.GetAxisRaw("Vertical") * elevatorPower,     // Pitch (W/S)
            Input.GetKey(KeyCode.E) ? rudderPower : Input.GetKey(KeyCode.Q) ? -rudderPower : 0f,  // Yaw (Q/E)
            Input.GetAxisRaw("Horizontal") * aileronPower     // Roll (A/D)
        );

        // Smooth the rotation inputs
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, controlSmoothingFactor);
        rotationInput = currentRotation;

        // Calculate current thrust
        currentThrust = throttle * maxThrust;
    }

    void ApplyForces()
    {
        // Apply gravity
        rb.AddForce(Physics.gravity * mass);

        // Apply thrust
        Vector3 thrustForce = transform.forward * currentThrust;
        rb.AddForce(thrustForce);

        if (airspeed > 0.1f)
        {
            float dynamicPressure = 0.5f * airDensity * airspeed * airspeed;

            // Calculate lift coefficient with stall
            float alphaClamped = Mathf.Clamp(angleOfAttack, -maxAlpha, maxAlpha);
            float CL = CLzero + (CLslope * alphaClamped);

            // Post-stall behavior
            if (Mathf.Abs(angleOfAttack) > maxAlpha)
            {
                float stallFactor = 1.0f - ((Mathf.Abs(angleOfAttack) - maxAlpha) / maxAlpha);
                stallFactor = Mathf.Clamp01(stallFactor);
                CL *= stallFactor;
            }

            // Calculate drag coefficient using drag polar
            float CD = CDmin + (K * CL * CL);

            // Calculate lift and drag forces
            float liftForce = CL * dynamicPressure * wingArea;
            float dragForce = CD * dynamicPressure * wingArea;

            // Apply aerodynamic forces
            Vector3 liftDirection = Vector3.Cross(rb.velocity.normalized, transform.right);
            Vector3 dragDirection = -rb.velocity.normalized;

            rb.AddForce(liftDirection * liftForce);
            rb.AddForce(dragDirection * dragForce);
        }

        // Ground handling
        if (isGrounded)
        {
            // Add rolling resistance
            float rollingResistance = 0.02f;
            Vector3 resistanceForce = -rb.velocity.normalized * rollingResistance * mass * 9.81f;
            rb.AddForce(resistanceForce);

            // Add extra grip when on ground
            Vector3 lateralVelocity = Vector3.ProjectOnPlane(rb.velocity, transform.forward);
            rb.AddForce(-lateralVelocity * mass * 0.5f);
        }
    }

    void ApplyRotation()
    {
        if (airspeed > 0.1f)
        {
            // Calculate control effectiveness based on airspeed
            float controlEffectiveness = Mathf.Clamp01(airspeed / 20f);

            // Apply smoothed rotation with airspeed-based effectiveness
            Vector3 rotationForce = Vector3.Scale(rotationInput, new Vector3(1f, 1f, 1f)) * controlEffectiveness;
            Debug.Log(rotationForce);
            rb.AddRelativeTorque(rotationForce, ForceMode.Force);

            // Add basic stability
            if (Mathf.Abs(rotationInput.magnitude) < 0.1f)
            {
                // Counter-rotation when no input is present
                rb.AddRelativeTorque(-rb.angularVelocity * mass * 0.1f);
            }
        }
    }

    void CalculateAerodynamics()
    {
        // Get airspeed and direction
        Vector3 airVelocity = rb.velocity;
        airspeed = airVelocity.magnitude;

        // Only calculate aero forces if moving
        if (airspeed > 0.1f)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(airVelocity);
            angleOfAttack = Mathf.Atan2(-localVelocity.y, localVelocity.z) * Mathf.Rad2Deg;
        }
        else
        {
            angleOfAttack = 0f;
        }
    }

    void UpdateGroundedState()
    {
        // Raycast to check if we're on the ground
        float rayLength = 2.0f;
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, rayLength);

        // Add ground effect when close to ground
        if (hit.distance < wingspan * 0.5f && !isGrounded)
        {
            float groundEffect = 1.0f - (hit.distance / (wingspan * 0.5f));
            // Increase lift when in ground effect
            rb.AddForce(Vector3.up * groundEffect * mass * Physics.gravity.magnitude * 0.5f);
        }
    }

    void UpdateUI()
    {
        if (speedText) speedText.text = $"Speed: {airspeed * 3.6f:F0} km/h";
        if (thrustText) thrustText.text = $"Thrust: {(throttle * 100):F0}%";
        if (angleText) angleText.text = $"AoA: {angleOfAttack:F1}°";

        // Update effects
        if (Thrusters) Thrusters.volume = throttle;
        if (thrusterParticles)
        {
            var main = thrusterParticles.main;
            main.startSize = throttle;
        }
    }

    void HandleRestartAndQuit()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void AddScore()
    {
        Score += 1;
        scoretext.text = "Score: " + Score;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered with: " + other.gameObject.name);
        if (other.gameObject.CompareTag("danger"))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (other.gameObject.CompareTag("coin"))
        {
            other.gameObject.SetActive(false);
            AddScore();
            if (Score == 10)
            {
                wintext.text = "Mission Complete! Press R to restart";
            }
        }
    }
}