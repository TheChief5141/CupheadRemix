using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.magnitude >= 100f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "ParryEnemy")
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "BasicEnemy")
        {
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
