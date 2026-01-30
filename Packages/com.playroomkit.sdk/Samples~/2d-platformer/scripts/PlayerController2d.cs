using UnityEngine;
using TMPro;

public class PlayerController2d : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float shootSpeed = 50f;
    [SerializeField] private float jumpAmount = 15f;

    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private bool isGrounded;


    [SerializeField] private GameObject bulletPrefab;
    public TextMeshProUGUI scoreText;


    public float dirX;
    private float lastNonZeroDirX = 1f;
    public float FacingDir => lastNonZeroDirX;

    public bool Move()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(dirX) > 0.01f)
        {
            lastNonZeroDirX = Mathf.Sign(dirX);
            ApplyFacing(lastNonZeroDirX);
        }

        if (rb2D != null)
        {
            var velocity = rb2D.velocity;
            rb2D.velocity = new Vector2(dirX * moveSpeed, velocity.y);
        }
        return Mathf.Abs(dirX) > 0;
    }

    public void ShootBullet()
    {
        ShootBullet(transform.position, lastNonZeroDirX);
    }

    public void ShootBullet(Vector3 position, float direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, Quaternion.identity);

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        if (bulletRb != null)
        {
            var shootDir = Mathf.Abs(direction) > 0.01f ? Mathf.Sign(direction) : lastNonZeroDirX;
            bulletRb.AddForce(new Vector2(shootDir, 0) * shootSpeed, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning("Bullet prefab does not have a Rigidbody2D component.");
        }
        Destroy(bullet, 2f);

    }

    public bool Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isGrounded && rb2D != null)
        {
            rb2D.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            isGrounded = false;
            return true;
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    public void ApplyFacing(float direction)
    {
        if (Mathf.Abs(direction) <= 0.01f)
        {
            return;
        }

        lastNonZeroDirX = Mathf.Sign(direction);
        var localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * lastNonZeroDirX;
        transform.localScale = localScale;
    }

    public void ApplyRemoteState(Vector3 position, float facing)
    {
        transform.position = position;
        ApplyFacing(facing);
    }
}
