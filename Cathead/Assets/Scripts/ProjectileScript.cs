using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

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
    public Vector2 direction;


    Rigidbody2D rb2d;
    GameObject playerControllerObject;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Awake()
    {

    }

    void Start()
    {

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

    void GatherInputs()
    {
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isFiring = Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0);
        Debug.Log("Projectile firing is " + isFiring);
        switchWeapons = Input.GetKeyDown(KeyCode.LeftShift);

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
    }

    private void fireBasicProjectile(Vector2 projectileDirection)
    {
        //if not moving in a direction, fires the projectile by the direction the player is facing
        if (projectileDirection == Vector2.zero)
        {
            projectileDirection = new Vector2(transform.localScale.x, 0);
        }
        //creates the projectile object
        GameObject projectileObject = Instantiate(basicProjectile, GetComponent<Rigidbody2D>().position + Vector2.up * 0.5f, Quaternion.identity);
        //launches the projectile based on the projectile speed
        projectileObject.GetComponent<Rigidbody2D>().AddForce(projectileDirection.normalized * projectileSpeed * 100f);
        Debug.Log("after");
    }

    //function used to fire the spread shot, from a prefab located in the prefabs folder
    public void fireSpreadShot(Vector2 projectileDirection, int spreadAmount, Vector3 startPoint)
    {
        float angleStep = 22.5f / spreadAmount;
        float angle = 75f * projectileDirection.x;

        if (projectileDirection == Vector2.zero)
        {
            projectileDirection = new Vector2(transform.localScale.x, 0);
        }

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
    

    // Update is called once per frame
    void Update()
    {
        GatherInputs();
        ProjectileCheck();
    }
}
