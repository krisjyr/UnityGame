using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 10f;
    public TMP_Text scoreText;
    public TMP_Text winText;
    public TMP_Text timerText;
    public TMP_Text confusedText;
    public GameObject Wall;
    private Rigidbody rb;

    private bool confused = false;

    private int score = 0;
    private float timer = 60f;
    private float waitTime = 2f;
    private float confusedTimer = 0f;

    private float confuseChance = 5f;

    void setScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        if (score >= 10)
        {
            WinText();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            rb.drag = 0f;
            rb.angularDrag = 0.05f;
        }

        score = 0;
        setScoreText();
        winText.text = "";
        confusedText.text = "";
        timerText.text = "Time: " + timer.ToString();
    }

    void addScore()
    {
        score = score + 1;
        timer += 10f;
        setScoreText();
    }

    void WinText()
    {
        winText.text = "You Win! Press R to restart or ESC to exit";
    }

    void Update()
    {
        HandleTimer();
        HandleConfusion();
        HandleRestartAndQuit();
    }

    void FixedUpdate()
    {
        // Physics-based movement should be in FixedUpdate
        HandlePhysicsMovement();
    }

    void HandleTimer()
    {
        if (timer <= 0)
        {
            winText.text = "Time's up!";
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }
        else
        {
            timer -= Time.deltaTime;
            timerText.text = "Time: " + Math.Round(timer).ToString();
        }
    }

    void HandlePhysicsMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (confused)
        {
            moveHorizontal = -moveHorizontal;
            moveVertical = -moveVertical;
        }

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        rb.AddForce(movement * speed);
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

    void HandleConfusion()
    {
        // Check for new confusion
        if (!confused && UnityEngine.Random.Range(1f, 50000f) <= confuseChance)
        {
            confused = true;
            confusedTimer = 5f;
            confusedText.text = "You are confused!";
        }

        // Update confusion timer
        if (confused)
        {
            confusedTimer -= Time.deltaTime;
            confusedText.text = "You are confused for " + Math.Round(confusedTimer).ToString() + " seconds";
            
            if (confusedTimer <= 0f)
            {
                confusedText.text = "";
                confused = false;
                confusedTimer = 0f;
            }
        }
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
            addScore();
            if (score >= 5)
            {
                Wall.gameObject.SetActive(false);
            }
        }
    }
}