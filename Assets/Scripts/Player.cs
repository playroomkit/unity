using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5;
    
    public void Move()
    {
        var xInput = Input.GetAxisRaw("Horizontal");
        var yInput = Input.GetAxisRaw("Vertical");

        Vector3 moveVector = new(xInput, yInput, transform.position.z);
        transform.Translate(moveVector.normalized * (speed * Time.deltaTime));
    }
}
