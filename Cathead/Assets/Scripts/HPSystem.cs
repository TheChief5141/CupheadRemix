using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HPSystem : MonoBehaviour
{
    public GameObject[] hearts;
    public int life;

    void Update()
    {
        if (life < 1)
        {
            Destroy(hearts[0].gameObject);
        }
        else if (life < 2) 
        {
            Destroy(hearts[1].gameObject);
        }
        else if (life < 3)
        {
            Destroy(hearts[2].gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("poopy");
        life += damage;
        if (life <= 0)
        {
            SceneManager.LoadScene("Game Over");
        }
    }
}
