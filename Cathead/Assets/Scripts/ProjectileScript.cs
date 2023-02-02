using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    Rigidbody2D rb2d;
    GameObject playerControllerObject;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerControllerObject = GameObject.FindWithTag("PlayerController");
    }

    public void Launch(Vector2 direction, float force)
    {
        rb2d.AddForce(direction * force);
    }


    /*
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyController e = other.collider.GetComponent<enemyController>();
            if (e != null)
            {
                Destroy(other.gameObject);
                if (playerControllerObject != null)
                {
                    gameManager.changeScore(1);
                }
            } 
        }
        if (other.gameObject.CompareTag("PlayerController"))
        {
            return;
        }
        
        Destroy(gameObject);
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude > 100.0f)
        {
            Destroy(gameObject);
        }
    }
    
}
