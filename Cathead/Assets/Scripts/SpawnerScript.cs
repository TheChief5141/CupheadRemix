using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public float timerStart;
    private float currentTime;
    private bool isSpawning;
    public GameObject parryGhost;


    // Start is called before the first frame update
    void Start()
    {
        currentTime = timerStart;
        isSpawning = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpawning)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                Instantiate(parryGhost, transform.position, Quaternion.identity);
                currentTime = timerStart;
            }
        }
    }
}
