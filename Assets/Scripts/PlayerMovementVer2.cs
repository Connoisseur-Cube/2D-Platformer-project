using System.Collections;
using UnityEngine;
public class PlayerMovementVer2 : MonoBehaviour
{
    public float speed = 8f;
    public float jumpforce = 17f;
    public float CoyoteTime = 0.1f;
    public float JumpBuffer = 0.1f;

    private float CoyoteTimeCounter;
    private float JumpBufferCounter;
    private bool canDash = true;
    private bool isDashing;
    private float DirX;

    [Range(-1,1)]private int Direction = 1;

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
        //AYAYAYA!!!! AWAKEN, MY MASTERS!
    }

    // Update is called once per frame
    void Update()
    {

        if (isDashing) {return;}

        DirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(DirX*speed,rb.velocity.y);

#region Coyote and Jump Buffer check        
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
#endregion

        
        if(JumpBufferCounter >0f && CoyoteTimeCounter > 0f)
        {
            if(!IsGrounded()){ Debug.Log("Coyote timed");}

            if(!Input.GetButtonDown("Jump")){ Debug.Log("Jump Buffered");}

            rb.velocity = new Vector2(rb.velocity.x, jumpforce);
            JumpBufferCounter = 0f;
        }

        if(Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x,rb.velocity.y *0.5f);
            CoyoteTimeCounter = 0f;
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

    private bool IsGrounded()
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
