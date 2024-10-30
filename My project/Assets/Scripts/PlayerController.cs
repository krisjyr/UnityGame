using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerControlelr : MonoBehaviour
{
    public float speed = 10.0f;
    public float maxSpeed = 200.0f;
    public float thrust = 0.0f;
    public float maxThrust = 100.0f;


    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private float roll = 0.0f;

    public float turnSpeed = 45.0f;
    public float pitchSpeed = 45.0f;
    public float rollSpeed = 45.0f;

    public AudioSource Waypoint;
    public TMP_Text scoreText;
    public TMP_Text winText;
    public TMP_Text thrustText;
    public TMP_Text speedText;
    private Rigidbody rb;




    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.useGravity = true;
            rb.drag = 0.1f;
            rb.angularDrag = 0.5f;
        }

        scoreText.GetComponent<TMP_Text>();
        scoreText.text = "Score: " + score;
    }

    // Update is called once per frame
    void Update()
    {
        InputHandling();
        Physics();
    }

    void InputHandling()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && thrust <= maxThrust)
        {
            thrust += 1.0f;
            if (thrust >= maxThrust)
            {
                thrust = maxThrust;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && thrust >= 0.0f)
        {
            thrust -= 1.0f;
            if (thrust <= 0.0f)
            {
                thrust = 0.0f;
            }
        }

        float pitchInput = 0;
        float yawInput = 0;
        float rollInput = 0;



        if (Input.GetKeyDown(KeyCode.W))
        {
            pitchInput -= 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            pitchInput += 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            yawInput -= 1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            yawInput += 1;
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            rollInput += 1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            rollInput -= 1;
        }

        pitch = pitchInput * pitchSpeed * Time.deltaTime;
        yaw = yawInput * turnSpeed * Time.deltaTime;
        roll = rollInput * rollSpeed * Time.deltaTime;
    }

    void Physics()
    {
        speed = (thrust / maxThrust) * maxSpeed;
        speed = Mathf.Max(speed, 0.0f);

        rb.velocity = transform.forward * speed;

        float liftMultiplier = speed / maxSpeed; // More speed = more lift
        Vector3 lift = transform.up * (liftForce * liftMultiplier);
        rb.AddForce(lift);
    }
}
