using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float speed = 1f;
    public float jumpForce = 1.0f;
    public TMP_Text scoreText;
    public TMP_Text winText;
    public GameObject Wall;
    private Rigidbody rb;

    private int score = 0;

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
    }

    void addScore()
    {
        score = score + 1;
        setScoreText();
    }

    void removeScore()
    {
        score = score - 1;
        setScoreText();
    }

    void WinText()
    {
        winText.text = "You Win my nigga! Press R to restart or ESC to exit";
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);

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
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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