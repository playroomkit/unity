using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] private float jumpAmount = 15f;

    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private bool isJumping;

    [SerializeField] private bool isMoving;

    [SerializeField] private GameObject bulletPrefab;
    public Text scoreText;


    public float dirX;

    public void Move()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        transform.Translate(new Vector3(dirX, 0, 0) * (moveSpeed * Time.deltaTime));
        isMoving = Mathf.Abs(dirX) > 0;
    }


    public void ShootBullet(Vector3 position, float speed)
    {
        if (isMoving)
        {
            // Instantiate bullet prefab at specified position with the specified rotation
            GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);

            // Get the bullet's Rigidbody component if it has one
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            // If bullet has Rigidbody2D component, apply force in the specified direction
            if (bulletRb != null)
            {
                bulletRb.AddForce(new Vector3(dirX, 0, 0) * speed, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("Bullet prefab does not have a Rigidbody2D component.");
            }
            Destroy(bullet, 2f);
        }
    }

    public void Jump()
    {
        if (/*Input.GetButton("Jump") && */ isJumping)
        {
            rb2D.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            isJumping = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
        }
    }
}
