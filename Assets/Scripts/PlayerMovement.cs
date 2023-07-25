using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 5f;

    Rigidbody2D rb2D;


    // runs before start
    void Awake()
    {


        if (PlayroomKit.IsRunningInBrowser())
        {
            PlayroomKit.LoadPlayroom();
        }
        else
        {
            Debug.LogWarning("Editor game start");
        }

    }



    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
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

            if (PlayroomKit.IsRunningInBrowser())
            {
                // PlayroomKit.InsertCoin();
                PlayroomKit.Jump("JUMP!");
            }
            else
            {
                Debug.LogWarning("Editor jump");
            }
        }
    }

    public void StartGame()
    {
        PlayroomKit.InsertCoin();
    }



}
