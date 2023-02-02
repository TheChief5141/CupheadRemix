using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    public int health { get { return currentHealth; }}
    int currentHealth;

    [Header("Characteristics")]
    //how much damage the character does
    public float damage;
    //how many hits it takes to get a super
    public float superAmount;

    [Header("Components")]
    //audio source to play sound
    public AudioSource audioSource;
    //used for functions from the gamemanager
    public GameManager gameController;
    //rigidbody for movement
    public Rigidbody2D rb2d;

    [Header("Horizontal Movement")]
    //controls how fast player moves on ground
    public float groundSpeed;
    //displays which direction the player is moving
    public Vector2 direction;
    //used for displaying which side the player is facing (used for dash)
    private bool facingRight = true;
    
    [Header("Vertical Movement")]
    //to check if the floor the player is touching is part of the ground layer
    public LayerMask groundLayer;
    //to check if the player is currently in the air
    private bool isJumping;
    //to check if the players velocity is below 0
    private bool isFalling;
    //used to control the height the player jumps to 
    public float jumpForce;
    //used to delay the amount of time that the player will be able to jump
    public float jumpDelay = 0.25f;
    //used to store the time of the delay to increment down to 0
    private float jumpTimer;

    [Header("Physics")]
    //controls the maximum speed the player can move
    public float maxSpeed = 7f;
    //controls the amount of drag that the player has when in contact with a collider
    public float linearDrag = 4f;
    //used to control the pull downward on the player
    public float gravity = 0.4f;
    //used to accelerate the gravity
    public float fallMultiplier = 5f;

    [Header("Collision")]
    //to store a boolean whether the player is touching the ground or not
    public bool onGround = false;
    //the distance the player checks to see whether or not it is touching the ground
    public float groundLength = 0.6f;
    //if the player needs to offset the line (to make it go at an angle)
    public Vector3 colliderOffset;

    [Header("Dashing")]
    //controls the speed of the dash
    public float dashSpeed;
    //controls the amount of time that the player will not be able to dash for
    public float dashTime = 0.5f;
    //shows the direction the player is dashing
    private Vector2 dashingDir;
    //checks if the player is currently dashing
    private bool isDashing;
    //checks if the player is able to currently dash
    private bool canDash = true;

    [Header("Invincible")]
    //the amount of time spent invicible after being hit
    public float timeInvincible = 2.0f;
    //to check if currently invincible or not
    bool isInvincible = false;
    //stores the time needed to be invincible when hit
    float invincibleTimer;

    [Header("Projectiles")]
    //creating the basic projectile from a prefab
    public GameObject basicProjectile;
    //creating the projectile from the spread shot
    public ParticleSystem spreadShot;
    //controls the speed of the projectiles
    public float projectileSpeed;
    //checks if the bullet has been fired or not
    private bool isFiring;
    //checks if player presses key to switch weapons
    private bool switchWeapons;
    //represents with currentweapon we are currently wielding, 0 for basic, 1 for spread shot
    public int currentWeapon;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GatherInputs();
    }

    void FixedUpdate()
    {
        moveCharacter(direction.x);
        if (jumpTimer > Time.time && onGround)
        {
            characterJump();
        }
        modifyPhysics();
    }

    private void GatherInputs()
    {
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        isJumping = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W);
        isFalling = Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.W);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isFiring = Input.GetKeyDown(KeyCode.C);
        switchWeapons = Input.GetKeyDown(KeyCode.X);

        if (isJumping)
        {
            jumpTimer = Time.time + jumpDelay;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (switchWeapons)
        {
            if (currentWeapon == 1)
            {
                currentWeapon = 0;
            }
            if (currentWeapon == 0)
            {
                currentWeapon = 1;
            }
        }
    }

    void moveCharacter(float horizontal)
    {

        var position = rb2d.position;

        var dashInput = Input.GetKey(KeyCode.Q);

        if (dashInput && canDash)
        {
            Debug.Log("Dashing");
            isDashing = true;
            canDash = false;
            dashingDir = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
        }
        Debug.Log(horizontal);
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        } 
        
        if (isDashing)
        {
            rb2d.AddForce(Vector2.right * horizontal * groundSpeed * dashSpeed, ForceMode2D.Force);
            StartCoroutine(StopDashing());
        }
        else
        {
            Debug.Log("moving and groovin");
            rb2d.AddForce(Vector2.right * horizontal * groundSpeed, ForceMode2D.Force);
            if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
            {
                rb2d.velocity = new Vector2((Mathf.Sign(rb2d.velocity.x)) * (maxSpeed), rb2d.velocity.y);
            }
            
        }

        if (isFiring && currentWeapon == 0)
        {
            switch (currentWeapon)
            {
                case 0: 
                    fireBasicProjectile(direction);
                    break;
                case 1: 
                    fireSpreadShot(direction);
                    break;
            }
        }

        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }
        

        /*
        if (isDashing)
        {
            position.x = position.x + groundSpeed * dashingDir.x * Time.deltaTime * dashSpeed;
            
        }
        else
        {
            position.x = position.x + groundSpeed * horizontal * Time.deltaTime;
        }
        rb2d.MovePosition(position);
        */


        //Animating
        //Once animations are set up, delete the paragrapgh comments on lines 106 and line 150
        /*
        if ((rb2d.velocity.y) > 0 && !onGround)
        {
            animator.SetBool("isRising", true);
            animator.SetBool("isFalling", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        if ((rb2d.velocity.y) < 0 && !onGround)
        {
            animator.SetBool("isRising", false);
            animator.SetBool("isFalling", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        if ((rb2d.velocity.y) == 0 && onGround)
        {
            animator.SetBool("isRising", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        //x direction
        if (Mathf.Abs(rb2d.velocity.x) > 0 && Mathf.Abs(rb2d.velocity.x) < 1 && onGround)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isRunning", false);
        }
        if (Mathf.Abs(rb2d.velocity.x) >= 1 && onGround)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
        }
        if (Mathf.Abs(rb2d.velocity.x) == 0 && onGround)
        {
            animator.SetBool("isRunning", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
    */
        

    }

    private void characterJump()
    {
        Debug.Log("Yahooo!");
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    private void Flip()
    {
        Debug.Log("flip");
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0: 180, 0);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
    }

    private void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb2d.velocity.x < 0) || (direction.x < 0 && rb2d.velocity.x > 0);

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

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        canDash = true;
    }

    private void fireBasicProjectile(Vector2 projectileDirection)
    {
        if (projectileDirection == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0);
        }
        GameObject projectileObject = Instantiate(basicProjectile, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);
        ProjectileScript projectile = projectileObject.GetComponent<ProjectileScript>();
        projectile.Launch(projectileDirection, projectileSpeed);
    }

    public void fireSpreadShot(Vector2 direction)
    {
        spreadShot.gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(direction.x, direction.y, 0));
        spreadShot.Emit(10);
    }

    public void ChangeHealth(int amount)
    {
        //GameObject particles = healthParticlesPrefab;
        if (amount < 0)
        {
            //animator.SetTrigger("Hit");
            if (isInvincible)
            {
                return;
            }
            isInvincible = true;
            invincibleTimer = timeInvincible;
            //particles = collisionParticlesPrefab;
            //audioSource.PlayOneShot(hurtSound);
        }
        //currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth); 
        //UIHealthScript.instance.SetValue(currentHealth/(float)maxHealth);
        //playParticleSystem(particles);

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
}
