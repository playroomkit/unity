using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class PlayerMovement : MonoBehaviour
{
    

    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 5f;

    Rigidbody2D rb2D;


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
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
