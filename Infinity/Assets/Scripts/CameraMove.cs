using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class CameraMove : MonoBehaviour
{
    public Transform Target;
    public Vector3 DisFromTarget;

    public Tilemap Map;
    public float CamSpeed;

    public void CamMove()
    {
        Vector3 dis = Target.position + DisFromTarget;
        this.transform.position = Vector3.Lerp(transform.position, dis, CamSpeed);
    }

    // Start is called before the first frame update
    void Start()
    {
        DisFromTarget = transform.position - Target.transform.position;
    }

    
    // Update is called once per frame
    void Update()
    {
        CamMove();
    }
}
