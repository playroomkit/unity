using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        var dirX = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        transform.Translate(dirX * (moveSpeed * Time.deltaTime));
    }
}
