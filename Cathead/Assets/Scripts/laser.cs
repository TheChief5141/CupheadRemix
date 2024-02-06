using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{

   public GameObject playerObject;
    Rigidbody2D rb2d;
    private bool canSuper;
    AudioSource audioSource;
    public AudioClip laserShot;
 

    // Start is called before the first frame update
    void Start()
    {
        m_lineRenderer.enabled = false;
        canSuper = playerObject.GetComponent<PlayerController>().canSuper;
        rb2d = playerObject.GetComponent<Rigidbody2D>();
        audioSource = playerObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        canSuper = playerObject.GetComponent<PlayerController>().canSuper;
        if(canSuper && Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("LASERBEAM");
            ShootLaser();
            Debug.Log("LASERWORKS");
        }
    }

    [SerializeField] private float defDistanceRay = 100;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    Transform m_transform;

    private void Awake() 
    {
        m_transform = GetComponent<Transform>();
    }

    void ShootLaser()
    {
        m_lineRenderer.enabled = true;
        rb2d.simulated = false;
           
        Debug.Log("HIT SOMETHING");

        audioSource.PlayOneShot(laserShot);
        Draw2DRay(laserFirePoint.position, laserFirePoint.transform.right * defDistanceRay);

        StartCoroutine(StopLaser());
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    private IEnumerator StopLaser()
    {
        yield return new WaitForSeconds(4);
        m_lineRenderer.enabled = false;
        rb2d.simulated = true;
        playerObject.GetComponent<PlayerController>().canPlayerSuper = false;
    }
}
