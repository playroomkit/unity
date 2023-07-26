using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 5f;

    [SerializeField] private Text text;

    Rigidbody2D rb2D;

    int a = 0;
    int b = 0;

    // runs before start
    void Awake()
    {

        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.LoadPlayroom();
        }
        else
        {
            // Todo: mock api call ? 
            Debug.LogWarning("PlayroomKit only works in the browser (for now)!");
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.InsertCoin();
        }
        else
        {
            Debug.LogWarning("InsertCoin Mock");
        }


        PlayroomKit.SetState("valueGameString", "Game Started State");



        rb2D = GetComponent<Rigidbody2D>();
        text.text = "a = " + a + " b = " + b;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Jump();
    }

    void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector3.right * (horizontal * speed * Time.deltaTime));
    }


    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // test: Works!
            // Debug.Log("this player is host: " + PlayroomKit.IsHost());

            Debug.Log("valueGameString: " + PlayroomKit.GetState<string>("valueGameString"));

        }
    }



    // functions for buttons
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);
        PlayroomKit.SetState<int>("valX", a);
        text.text = "a = " + a + " b = " + b;
    }

    public void TestGetState()
    {
        Debug.Log("b = " + b);
        b = PlayroomKit.GetState<int>("valX");
        Debug.Log("b after getState:  " + b);
        text.text = "new b = " + b;
    }


}
