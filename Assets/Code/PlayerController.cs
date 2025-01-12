using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speedMovement = 10;
    private Rigidbody2D _rb2d;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        Vector2 direction = new Vector2(x, y);
        Walk(direction);
    }

    private void Walk(Vector2 direction)
    {
        _rb2d.linearVelocity = new Vector2(direction.x * speedMovement, _rb2d.linearVelocity.y);
    }
}
