using Playroom;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    
    // Update is called once per frame
    void Update()
    {
        // Move();
    }

    public void Move()
    {
        var dirX = Input.GetAxisRaw("Horizontal");
        transform.Translate(new Vector3(dirX,0,0) * (moveSpeed * Time.deltaTime));
    }
}
