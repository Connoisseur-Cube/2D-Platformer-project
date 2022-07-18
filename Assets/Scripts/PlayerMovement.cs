using UnityEngine;
using System.Collections;
public class PlayerMovement : MonoBehaviour
{
    public float speed = 8f;
    public float jumpforce = 18f;
    public float FallMultiplier = 5f;
    public float LowJump = 20f;
    public float CoyoteTime = 0.1f;
    public float JumpBuffer = 0.1f;

    private float CoyoteTimeCounter;
    private float JumpBufferCounter;
    private bool canDash = true;
    private bool isDashing;
    private float DirX;
    private bool JumpPressed = false;

    [Range(-1,1)]int Direction = 1;

    [SerializeField] private float dashingPower = 35f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private BoxCollider2D collide;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private LayerMask Ground;

    private enum MoveState {idle, running, jumping, falling};

    // Start is called before the first frame update
    void Start()
    {
        //nothing yet
    }

    // Update is called once per frame
    void Update()
    {

        if (isDashing) {return;}

        DirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(DirX*speed,rb.velocity.y);

        
        if(IsGrounded())
        {
            CoyoteTimeCounter = CoyoteTime;
            canDash = true;
        } 
        else
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }
        if(Input.GetButtonDown("Jump"))
        {
            JumpBufferCounter = JumpBuffer;
        } 
        else
        {
            JumpBufferCounter -= Time.deltaTime;
        }


        if(JumpBufferCounter >0f && CoyoteTimeCounter > 0f)
        {
            if(!IsGrounded()) { Debug.Log("Coyote timed");}

            if(!Input.GetButtonDown("Jump")) { Debug.Log("Jump Buffered");}

            rb.velocity = new Vector2(rb.velocity.x,jumpforce);
            JumpBufferCounter = 0f;
        }

         if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            CoyoteTimeCounter = 0f;
        }

        if(Input.GetButton("Jump")){
            JumpPressed = true;
        } else {
            JumpPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.J) && canDash)
        {
            StartCoroutine(Dash());
        }

        UpdateAnimation();

    }

#region animation
    void UpdateAnimation()
    {
        MoveState state;

        if(DirX > 0f)
        {
            state = MoveState.running;
            sprite.flipX = false;
            Direction = 1;
        }
        else if(DirX < 0f )
        {
            state = MoveState.running;
            sprite.flipX = true;
            Direction = -1;
        }
        else 
        {
            state = MoveState.idle;
        }

        if(rb.velocity.y > 0.1f)
        {
            state = MoveState.jumping;
        }

        if(rb.velocity.y < -0.1f)
        {
            state = MoveState.falling;
            
        }

        anim.SetInteger("State",(int)state);
    }
#endregion

    private void FixedUpdate()
    {
        if (isDashing) {return;}

        if(rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (FallMultiplier - 1) * Time.fixedDeltaTime;
        } else if (rb.velocity.y > 0f && !JumpPressed)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (LowJump - 1) * Time.fixedDeltaTime;
        }
        
    }

    public bool IsGrounded()
    {
        return Physics2D.BoxCast(collide.bounds.center,collide.bounds.size,0f,Vector2.down,0.1f,Ground);
    }

    private IEnumerator Dash()
    {
        CoyoteTimeCounter = 0;
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        anim.SetTrigger("Dash");
        rb.velocity = new Vector2(Direction * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        anim.ResetTrigger("Dash");
    }
}
