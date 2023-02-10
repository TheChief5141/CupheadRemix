using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("LASERBEAM");
            if (Physics2D.Raycast(m_transform.position, m_transform.right))
            {
                RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, m_transform.right);
                 Draw2DRay(laserFirePoint.position, _hit.point);
             }
            else
            {
                 Draw2DRay(laserFirePoint.position, laserFirePoint.transform.right * defDistanceRay);
            }
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }
}
