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
    public float speed = 1f;
    public TMP_Text scoreText;
    public TMP_Text winText;
    public TMP_Text timerText;
    public TMP_Text confusedText;
    public GameObject Wall;
    private Rigidbody rb;

    private bool confused = false;

    private int score = 0;
    private float timer = 10f;
    private float waitTime = 2f;
    private float confusedTimer = 0f;

    private float confuseChance = 10f;

    void setScoreText()
    {
        scoreText.text = "Score: " + score.ToString();

        if (score >= 10)
        {
            WinText();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = 0;
        setScoreText();
        winText.text = "";
        confusedText.text = "";
        timerText.text = "Time: " + timer.ToString();
    }

    void addScore()
    {
        score = score + 1;
        timer += 5f;
        setScoreText();
    }

    void WinText()
    {
        winText.text = "You Win! Press R to restart or ESC to exit";
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);

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

        //Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        //Quit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //Movement
        if (confused ? Input.GetKey(KeyCode.A) : Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        if (confused ? Input.GetKey(KeyCode.D) : Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        if (confused ? Input.GetKey(KeyCode.W) : Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if(confused) {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
            } else {
                transform.Translate(Vector3.back * speed * Time.deltaTime);
            }
            
        }

        if (UnityEngine.Random.Range(1f, 10000f) <= confuseChance && !confused)
        {
            confused = true;
            confusedTimer = 5f;
        }

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