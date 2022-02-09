using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType { Nomal, Gold, Hidden, Locked, DoorTypeMax };
    public enum DoorDirection { Right, Left, Up, Down };

    public enum DoorState { Opend,Closed,Locked};

    public enum DoorElement { Body, Open, CloseLeft, CloseRight, Locked ,ElementMax };

    public GameObject[] elements;
    public GameObject NextRoom;

    public DoorType type;
    public DoorDirection direction;

    private DoorState nowstate;



    public DoorState NowState
    {
        get
        {
            return nowstate;
        }
        set
        {
            if(value==DoorState.Opend)
            {
                elements[(int)DoorElement.Open].SetActive(true);
                elements[(int)DoorElement.CloseLeft].SetActive(false);
                elements[(int)DoorElement.CloseRight].SetActive(false);
                elements[(int)DoorElement.Locked].SetActive(false);
            }
            else if(value == DoorState.Closed)
            {
                elements[(int)DoorElement.Open].SetActive(false);
                elements[(int)DoorElement.CloseLeft].SetActive(true);
                elements[(int)DoorElement.CloseRight].SetActive(true);
                elements[(int)DoorElement.Locked].SetActive(false);
            }
            else if (value == DoorState.Locked)
            {
                elements[(int)DoorElement.Open].SetActive(false);
                elements[(int)DoorElement.CloseLeft].SetActive(true);
                elements[(int)DoorElement.CloseRight].SetActive(false);
                elements[(int)DoorElement.Locked].SetActive(true);
            }
        }
    }



    public void InitSetting()
    {
        elements = new GameObject[(int)DoorElement.ElementMax];
        for(DoorElement i = DoorElement.Body; i<DoorElement.ElementMax;i++)
        {
            elements[(int)i] = transform.Find(i.ToString()).gameObject;
        }

        NowState = DoorState.Opend;
        //nextroom
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
