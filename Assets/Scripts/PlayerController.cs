using Playroom;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private Rigidbody2D rb;
    
    public void Move()
    {
        var dirX = Input.GetAxisRaw("Horizontal");
        transform.Translate(new Vector3(dirX,0,0) * (moveSpeed * Time.deltaTime));
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
        }
    }
    
}
