using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{

    public Transform CrossHairObj;

    public Vector3 WorldAimingPos;

    public Vector2 CrossHairScreenPos;

    public Transform WorldAimingDir;

    public Camera NowCamera;


    public void InitSetting()
    {

    }


    public void MoveCrossHair()
    {
        RaycastHit hit;
        bool cast;
        Ray ray = new Ray(WorldAimingDir.position, WorldAimingDir.forward);
        cast = Physics.Raycast(ray, out hit);

        if(cast)
        {
            CrossHairScreenPos = NowCamera.WorldToScreenPoint(hit.point);
            CrossHairObj.position = CrossHairScreenPos;
        }
        else
        {

        }
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCrossHair();
    }
}
