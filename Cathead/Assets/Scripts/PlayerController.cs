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
    private bool dashInput;
    private TrailRenderer trailRenderer;

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
    public GameObject spreadShot;
    //controls the speed of the projectiles
    public float projectileSpeed;
    //checks if the bullet has been fired or not
    private bool isFiring;
    //checks if player presses key to switch weapons
    private bool switchWeapons;
    //represents with currentweapon we are currently wielding, 0 for basic, 1 for spread shot
    public int currentWeapon;
    //controls the amount of spread shots used
    public int spreadAmount;
    public int radius;

    [Header("Parry")]
    private bool parryActive;
    private bool parryInput;
    private bool canParry = true;
    public float parryActiveTime = 2.0f;
    private float parryActiveTimer;
    public int parryTime = 5;
    public AudioClip parrySoundEffect;



    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        GatherInputs();
        ProjectileCheck();
    }

    void ProjectileCheck()
    {
        //fires the current weapon that we are currently wielding
        if (isFiring)
        {
            Debug.Log("Where ya at");
            if (currentWeapon == 0)
            {
                Debug.Log("Before");
                fireBasicProjectile(direction);
            }else if (currentWeapon == 1){
                fireSpreadShot(direction, spreadAmount, transform.position);
            }
        }
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

    /*
    Used to check inputs from the player, and to check if we are currently jumping, and switch weapons between the two currently programmed
    */
    private void GatherInputs()
    {
        //using a raycast to check if our character is touching the ground or not
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        //controls for the game
        isJumping = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.W);
        isFalling = Input.GetButtonUp("Jump") || Input.GetKeyUp(KeyCode.W);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isFiring = Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0);
        Debug.Log("Projectile firing is " + isFiring);
        switchWeapons = Input.GetKeyDown(KeyCode.LeftShift);
        dashInput = Input.GetKeyDown(KeyCode.Q);
        parryInput = Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1);

        //if we are jumping, remove from the timer so we can jump again
        if (isJumping)
        {
            jumpTimer = Time.time + jumpDelay;
        }

        //restart the scene (for debugging)
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //switch weapons, 0 is basic projectile, 1 is spread shot
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

        if (parryInput && canParry)
        {
            parryActive = true;
            canParry = false;
            parryActiveTimer = parryActiveTime;
        }

        if (dashInput && canDash)
        {
            Debug.Log("Dashing");
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;
            dashingDir = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rb2d.velocity = new Vector2(dashingDir.x * dashSpeed, 0);
            //rb2d.AddForce(Vector2.right *, ForceMode2D.Impulse);
            return;
        }
        rb2d.velocity = new Vector2(direction.x * groundSpeed, rb2d.velocity.y);
        //rb2d.AddForce(Vector2.right * horizontal * groundSpeed, ForceMode2D.Force);
        //makes sure our character gets locked at a certain speed called maxSpeed
        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
        {
            rb2d.velocity = new Vector2((Mathf.Sign(rb2d.velocity.x)) * (maxSpeed), rb2d.velocity.y);
        }

    }

    /*
    Used to move the character around the map, make character dash if we press q, fire projectiles, and flip our character based on the direction we are moving
    */
    void moveCharacter(float horizontal)
    {
        //grabs the current position of the player
        var position = rb2d.position;

        //flips our character based on our key inputs and the direction that our character is currently facing
        Debug.Log(horizontal);
        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        } 
        
        //if we are invincible, decrease the invincible timer
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }

        if (parryActive)
        {
            parryActiveTimer -= Time.deltaTime;
            if (parryActiveTimer < 0)
            {
                parryActive = false;
                canParry = true;
            }
        }

        /*
        //if dashing, add extra dash speed to the players movement and stop player from falling
        //if not dashing, then move by the regular movement amount
        if (isDashing)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x * dashingDir.x * dashSpeed, 0);
            //rb2d.AddForce(Vector2.right *, ForceMode2D.Impulse);
            StartCoroutine(StopDashing());
            return;
        }
        */

        



        //Animating
        //Once animations are set up, delete the paragraph comments on lines 106 and line 150
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

    //used to make the character jump when we press the w or spacebar
    private void characterJump()
    {
        Debug.Log("Yahooo!");
        rb2d.velocity = new Vector2(rb2d.velocity.x, 0);
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    //changes the rotation of the player in the y, based on the current direction the player is facing
    private void Flip()
    {
        Debug.Log("flip");
        facingRight = !facingRight;
        //transform.rotation = Quaternion.Euler(0, facingRight ? 0: 180, 0);
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
    }

    //used to calculate physics for gravity, and movement while the player is in the air
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

    //used to make the character stop dashing after the total dash time
    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        trailRenderer.emitting = false;
        canDash = true;
    }

    //function used to fire the basic projectile, from a prefab located in the prefabs folder
    private void fireBasicProjectile(Vector2 projectileDirection)
    {
        //if not moving in a direction, fires the projectile by the direction the player is facing
        if (projectileDirection == Vector2.zero)
        {
            projectileDirection = new Vector2(transform.localScale.x, 0);
        }
        //creates the projectile object
        GameObject projectileObject = Instantiate(basicProjectile, rb2d.position + Vector2.up * 0.5f, Quaternion.identity);
        ProjectileScript projectile = projectileObject.GetComponent<ProjectileScript>();
        //launches the projectile based on the projectile speed
        projectile.Launch(projectileDirection.normalized, projectileSpeed * 100);
        Debug.Log("after");
    }

    //function used to fire the spread shot, from a prefab located in the prefabs folder
    public void fireSpreadShot(Vector2 projectileDirection, int spreadAmount, Vector3 startPoint)
    {
        float angleStep = 22.5f / spreadAmount;
        float angle = 75f * direction.x;

        for (int i = 0; i <= spreadAmount; i++)
        {
            float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
            float projectileDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

            Vector3 projectileVector = new Vector2(projectileDirXPosition, projectileDirYPosition);
            Vector2 projectileMoveDirection = (projectileVector - startPoint).normalized * projectileSpeed;

            GameObject tmpObj = Instantiate(spreadShot, startPoint, Quaternion.identity);
            tmpObj.GetComponent<Rigidbody2D>().velocity = new Vector2(projectileMoveDirection.x, projectileMoveDirection.y);

            angle += angleStep;
        }
    }

    //function to be run when hitting an enemy, and starts the invincible timer
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ParryEnemy")
        {
            if (parryActive == true)
            {
                ParryEffects();
                StartCoroutine(StopParrying());
                return;
            }
            else
            {
                //ChangeHealth();
            }
        }
    } 

    void ParryEffects()
    {
        Debug.Log("Parried");
        audioSource.PlayOneShot(parrySoundEffect);
    }

    private IEnumerator StopParrying()
    {
        yield return new WaitForSeconds(parryTime);
        canParry = true;
        parryActive = false;
    }
}
