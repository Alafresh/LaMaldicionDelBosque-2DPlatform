using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 direction;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _virtualCamera;

    [Header("Stadistics")]
    [SerializeField] float speedMovement = 10;
    [SerializeField] float forceJump = 10;
    [SerializeField] float dashSpeed = 20;


    [Header("Components")]
    [SerializeField] private Rigidbody2D _rb2d;
    [SerializeField] private Animator _animator;

    [Header("Colisions")]
    [SerializeField] LayerMask ground;
    [SerializeField] Vector2 down;
    [SerializeField] private float radiusDetection;
    
    [Header("Booleans")]
    [SerializeField] bool canMove = true;
    [SerializeField] bool isGrounded = true;
    [SerializeField] bool canDash;
    [SerializeField] bool isDashing;
    [SerializeField] bool touchGround;
    [SerializeField] bool shakingCamera;

    private void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Movement();
        Grip();
    }

    private IEnumerator ShakeCamera()
    {
        shakingCamera = true;;
        CinemachineBasicMultiChannelPerlin noise = _virtualCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
        noise.AmplitudeGain = 5;
        yield return new WaitForSeconds(0.5f);
        noise.AmplitudeGain = 0;
        yield return new WaitForSeconds(0.3f);
        shakingCamera = false;
    }

    private void Dash(float x, float y)
    {
        _animator.SetBool("dashing", true);
        Vector3 playerPosition = Camera.main.WorldToViewportPoint(transform.position);
        StartCoroutine(ShakeCamera());
        canDash = true;
        _rb2d.linearVelocity = Vector2.zero;
        _rb2d.linearVelocity += new Vector2(x, y).normalized * dashSpeed;
        StartCoroutine(SetUpDash());
    }

    private IEnumerator SetUpDash()
    {
        StartCoroutine(GroundDash());
        _rb2d.gravityScale = 0;
        isDashing = true;
        
        yield return new WaitForSeconds(0.8f);

        _rb2d.gravityScale = 1;
        isDashing = false;
        FinishDash();
    }

    private IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(0.15f);
        if(isGrounded)
        {
            canDash = false;
        }
    }

    public void FinishDash()
    {
        _animator.SetBool("dashing", false);
    }

    private void TouchGround()
    {
        canDash = false;
        isDashing = false;
        _animator.SetBool("jumping", false);
    }

    private void Movement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direction = new Vector2(x, y);
        Walk();
        BetterJump();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(isGrounded)
            {
                _animator.SetBool("jumping", true);
                Jump();
            }
        }

        if(Input.GetKeyDown(KeyCode.X) && !isDashing)
        {
            if(xRaw != 0 || yRaw != 0)
            {
                Dash(xRaw, yRaw);
                Debug.Log("Dash");
            }
        }

        if (isGrounded && !touchGround)
        {
            TouchGround();
            touchGround = true;
        }

        if (!isGrounded && touchGround)
        {
            touchGround = false;
        }

        float speed;
        if (_rb2d.linearVelocity.y > 0)
        {
            speed = 1;
        }
        else
        {
            speed = -1f;
        }

        if (!isGrounded)
        {
            
            _animator.SetFloat("verticalSpeed", speed);
        }
        else
        {
            if(speed == -1)
            {
                _animator.SetBool("jumping", false);
            }
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
        if(canMove && !isDashing)
        {
            _rb2d.linearVelocity = new Vector2(direction.x * speedMovement, _rb2d.linearVelocity.y);

            if (direction != Vector2.zero)
            {
                if(!isGrounded)
                {
                    _animator.SetBool("jumping", true);
                }
                else
                {
                    _animator.SetBool("walking", true);
                }

                if (direction.x < 0 && transform.localScale.x > 0)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (direction.x > 0 && transform.localScale.x < 0)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                _animator.SetBool("walking", false);
            }
        }
    }
    private void Grip()
    {
        isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + down, radiusDetection, ground);
    }
}
