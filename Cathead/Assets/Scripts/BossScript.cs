using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    [Header("Characteristics")]
    Rigidbody2D rb2d;
    public int breakTime;

    [Header("Movement")]
    public float jumpForce;
    public float jumpTimer;
    public int jumpAmount;
    public float movementSpeed;
    private bool onGround;
    public float groundLength;
    public LayerMask groundLayer;
    public Vector3 colliderOffset;
    public float jumpDelay;
    public Vector2 direction;

    [Header("Physics")]
    //controls the maximum speed the player can move
    public float maxSpeed = 7f;
    //controls the amount of drag that the player has when in contact with a collider
    public float linearDrag = 4f;
    //used to control the pull downward on the player
    public float gravity = 0.4f;
    //used to accelerate the gravity
    public float fallMultiplier = 5f;


    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        int moveChooser = 1;

        if (moveChooser == 1)
        {
            JumpAttack();
            StartCoroutine(TakeABreath());
        }
        if (moveChooser == 2)
        {
            ProjectileAttack();
            StartCoroutine(TakeABreath());
        }
        if (moveChooser == 3)
        {
            DashAttack();
            StartCoroutine(TakeABreath());
        }
    }

    void JumpAttack()
    {
        for (int i = 0; i < jumpAmount; i++)
        {
            if (jumpTimer > Time.time && onGround)
            {
                characterJump();
            }
        }
    }

    void ProjectileAttack()
    {

    }

    void DashAttack()
    {

    }

    private IEnumerator TakeABreath()
    {
        yield return new WaitForSeconds(breakTime);
        jumpTimer = Time.time + jumpDelay;
    }

    private void characterJump()
    {
        rb2d.velocity = new Vector2(movementSpeed * transform.localScale.x, 0);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    /*
    private void modifyPhysics()
    {
        //checks if the player is trying to move the opposite way that it is currently moving
        bool changingDirections = (direction.x > 0 && rb2d.velocity.x < 0) || (direction.x < 0 && rb2d.velocity.x > 0);

        //if on the ground, increase the drag on the player to create friction on the ground
        //if not on the ground, move the character down based on the strength of gravity and the fallmultipler
        if (onGround)
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb2d.drag = linearDrag;
            }
            else
            {
                rb2d.drag = 0f;
            } 
        }
        else
        {
            rb2d.gravityScale = gravity;
            rb2d.drag = linearDrag * 0.15f;
            if (rb2d.velocity.y < 0)
            {
                rb2d.gravityScale = gravity * fallMultiplier;
            }
            else if (rb2d.velocity.y > 0 && (!isJumping))
            {
                rb2d.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
        
    }
    */
}
