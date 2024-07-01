using UnityEngine;
using TMPro;

public class PlayerController2d : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] private float jumpAmount = 15f;

    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private bool isJumping;

    [SerializeField] private bool isMoving;

    [SerializeField] private GameObject bulletPrefab;
    public TextMeshProUGUI scoreText;


    public float dirX;

    public void Move()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        transform.Translate(new Vector3(dirX, 0, 0) * (moveSpeed * Time.deltaTime));
        isMoving = Mathf.Abs(dirX) > 0;
    }


    public int ShootBullet(Vector3 position, float speed, int score)
    {
        if (isMoving)
        {
            score += 10;
            GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);

            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(new Vector3(dirX, 0, 0) * speed, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("Bullet prefab does not have a Rigidbody2D component.");
            }
            Destroy(bullet, 2f);

            return score;
        }
        return score;   
    }

    public void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isJumping)
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
