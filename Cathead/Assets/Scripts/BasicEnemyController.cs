using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyController : MonoBehaviour
{
    public bool moveRight = true;
    public float speed;
    public float movementTime;

    Rigidbody2D rb2d;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

// Use this for initialization
    void Update()
    {
		if(moveRight) 
        {
            Debug.Log("Move Right");
			transform.Translate(2 * Time.deltaTime * speed, 0,0);
			transform.localScale = new Vector2 (1f,1f);
 		}
		else
        {
            Debug.Log("Move Left");
			transform.Translate(-2 * Time.deltaTime * speed, 0,0);
			transform.localScale= new Vector2 (-1f,1f);
		}
	}

    private bool IsFacingRight()
    {
        return transform.localScale.x > Mathf.Epsilon;
    }

	void OnTriggerEnter2D(Collider2D collision)
	{
        Debug.Log("poopy");
		if (collision.tag == "Marker")
        {
            Debug.Log("Made Contact");

			if (moveRight)
            {
				moveRight = false;
			}
			else
            {
				moveRight = true;
			}	
		}
	}

    private IEnumerator SwitchSides()
    {
        yield return new WaitForSeconds(movementTime);
        if (moveRight)
        {
			moveRight = false;
            yield return new WaitForSeconds(movementTime);
		}
		else
        {
			moveRight = true;
            yield return new WaitForSeconds(movementTime);
		}	
    }
}
