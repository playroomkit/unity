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

    Dictionary<string, int> myDictionary;



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


        // int 
        myDictionary = new Dictionary<string, int>
        {
            { "x", 100 },
            { "y", 200 },
            { "z", 300 }
        };

        // // flaot
        // Dictionary<string, float> myDictionary2 = new Dictionary<string, float>
        // {
        //     { "x", 69.5f },
        //     { "y", 25.1f },
        //     { "z", 15.5f }
        // };

        // // bool
        // Dictionary<string, bool> myDictionary3 = new Dictionary<string, bool>
        // {
        //     { "x", true },
        //     { "y", false },
        //     { "z", true }
        // };

        // // string
        // Dictionary<string, string> myDictionary4 = new Dictionary<string, string>
        // {
        //     { "x", "hello" },
        //     { "y", "world" },
        //     { "z", "!" }
        // };

        // PlayroomKit.SetState("myDictionary2", myDictionary2);
        // PlayroomKit.SetState("myDictionary3", myDictionary3);
        // PlayroomKit.SetState("myDictionary4", myDictionary4);



    }


    // Start is called before the first frame update
    void Start()
    {
        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.InsertCoin(PlayroomKit.CallBackInsertCoin);
        }
        else
        {
            Debug.LogWarning("InsertCoin Mock");
        }

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
            PlayroomKit.GETFloat(3.14f);

        }
    }



    // functions for buttons
    public void TestSetState()
    {
        a++;
        Debug.Log("a = " + a);

        PlayroomKit.SetState("myDictionary", myDictionary);

        PlayroomKit.SetState("valX", a);

        // PlayroomKit.SetState("floatKey", 3.14f);

        PlayroomKit.SetStateString("stringKey", "hello");

        PlayroomKit.SetState("boolKey", true);



        text.text = "a = " + a + " b = " + b;
    }

    public void TestGetState()
    {
        Debug.Log("b = " + b);

        b = PlayroomKit.GetStateInt("valX");

        Debug.Log("Getting a float: " + PlayroomKit.GetStateFloat("floatKey"));

        Debug.Log("Getting a bool: " + PlayroomKit.GetStateBool("boolKey"));

        Debug.Log("Getting a String: " + PlayroomKit.GetStateString("stringKey"));




        text.text = "new b = " + b;
    }


}
