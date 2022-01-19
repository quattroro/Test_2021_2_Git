using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseStage : MonoBehaviour
{
    //public enum Direction { Left, Right, Up, Down,DIREC };

    //public enum MapType { NOMAL, RESTAURANT, SHOP, MAPTYPEMAX};
    public Door[] door;

    [SerializeField]
    public LinkedData StageLinkedData;

    //링크드데이터에 따라서 문이 2개 이상이면 일정 확률로 중간크기, 3개이상이면 일정확률로 큰방, 4개 이상이면 무조건 큰방 등등 의 처리를 해서 최종적으로 방을 생성해준다.
    public enum STAGECLASS { SMALL, MEDIUM, LARGE };

    public MapManager.ROOMTYPE type;

    public Transform StartPos;
    public Transform EndPos;

    public MapManager.STAGE NowStage;

    public int StageNum = -1;

    [Serializable]
    public class Values
    {
        STAGECLASS size;
        //Vector2Int Size;
        string Name;


    }

    //자신의 아래에 있는 문들을 받아온다.
    public void Initsetting()
    {
        door = new Door[(int)Door.DoorType.DoorMax];
        for (Door.DoorType i = Door.DoorType.Up; i < Door.DoorType.DoorMax; i++)
        {
            door[(int)i] = null;
        }

        Door[] temp = GetComponentsInChildren<Door>();
        foreach(var i in temp)
        {
            door[(int)i.type] = i;
            i.gameObject.SetActive(false);
        }

        //링크정보가 있는데 해당 위치에 문이 없으면 
        //해당 방을 다른 방이랑 교체 한다.
        //링크정보가 있고 해당 방향에 문도 존재하면 문을 만들어 준다.
        if (StageLinkedData.RightMap!=null)
        {
            if(door[(int)Door.DoorType.Right]!=null)
            {
                door[(int)Door.DoorType.Right].gameObject.SetActive(true);
                door[(int)Door.DoorType.Right].CreateDoor(this.gameObject);
            }
        }
        if(StageLinkedData.LeftMap!=null)
        {
            if (door[(int)Door.DoorType.Left] != null)
            {
                door[(int)Door.DoorType.Left].gameObject.SetActive(true);
                door[(int)Door.DoorType.Left].CreateDoor(this.gameObject);
            }
        }
        if(StageLinkedData.UpMap!=null)
        {
            if (door[(int)Door.DoorType.Up] != null)
            {
                door[(int)Door.DoorType.Up].gameObject.SetActive(true);
                door[(int)Door.DoorType.Up].CreateDoor(this.gameObject);
            }
        }
        if(StageLinkedData.DownMap!=null)
        {
            if (door[(int)Door.DoorType.Down] != null)
            {
                door[(int)Door.DoorType.Down].gameObject.SetActive(true);
                door[(int)Door.DoorType.Down].CreateDoor(this.gameObject);
            }
        }

        if(type==MapManager.ROOMTYPE.Start)
        {
            StartPos = transform.Find($"{MapManager.Instance.nowstage}StartGate");
        }
        else if(type == MapManager.ROOMTYPE.End)
        {
            EndPos = transform.Find($"{MapManager.Instance.nowstage}EndGate");
        }

    }

    public void SetLinkedData(LinkedData linkeddata)
    {

        

        //int count = 0;
        //if(linkeddata.LeftMap!=null)
        //{
        //    door[(int)Direction.Left] = new Door();

        //}
    }

    

    //[Serializable]
    //public class MapValues
    //{
    //    public MapType type;
    //    public BaseStage RightMap = null;
    //    public BaseStage LeftMap = null;
    //    public BaseStage UpMap = null;
    //    public BaseStage DownMap = null;

    //    public int Num;
    //    public int indexX;
    //    public int indexY;


    //}

    //public MapValues mapvalues = new MapValues();

    
    //public void InitSttting(int num, int x, int y, StageData.MapType type )
    //{
    //    StageLinkedData.Num = num;
    //    StageLinkedData.indexX = x;
    //    StageLinkedData.indexY = y;
    //    StageLinkedData.type = type;

    //}
    //public void InitSttting(int num, int x, int y)
    //{
    //    StageLinkedData.Num = num;
    //    StageLinkedData.indexX = x;
    //    StageLinkedData.indexY = y;
    //    StageLinkedData.type = StageData.MapType.NOMAL;

    //}


    public void SetRestaurant()
    {

    }


    public void SetShop()
    {

    }

    private void Start()
    {
        //Initsetting();
    }

    private void Update()
    {
        
    }
}
