using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Stadistics")]
    [SerializeField] float speedMovement = 10;
    [SerializeField] float forceJump = 10;

    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb2d;
    private Vector2 direction;
    
    [Header("Booleans")]
    [SerializeField] bool canMove = true;

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
        
        direction = new Vector2(x, y);
        Walk();
        BetterJump();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        //_rb2d.AddForce(Vector2.up * forceJump, ForceMode2D.Impulse);
        _rb2d.linearVelocity = new Vector2(_rb2d.linearVelocity.x, 0);
        _rb2d.linearVelocity += Vector2.up * forceJump;
    }

    private void BetterJump()
    {
        if(_rb2d.linearVelocity.y < 0)
        {
            _rb2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (_rb2d.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rb2d.linearVelocity += Vector2.up * Physics2D.gravity.y * (2f - 1) * Time.deltaTime;
        }
    }


    private void Walk()
    {
        if(canMove)
        {
            _rb2d.linearVelocity = new Vector2(direction.x * speedMovement, _rb2d.linearVelocity.y);

            if (direction != Vector2.zero)
            {
                if (direction.x < 0 && transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (direction.x > 0 && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
        }
    }
}
