using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{
   

 

    // Start is called before the first frame update
    void Start()
    {
        m_lineRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
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
           
                Debug.Log("HIT SOMETHING");
             
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
        
    }
}
