using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseStage : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down };

    //public enum MapType { NOMAL, RESTAURANT, SHOP, MAPTYPEMAX};
    public Door[] door;

    [SerializeField]
    public LinkedData StageLinkedData;

    //��ũ�嵥���Ϳ� ���� ���� 2�� �̻��̸� ���� Ȯ���� �߰�ũ��, 3���̻��̸� ����Ȯ���� ū��, 4�� �̻��̸� ������ ū�� ��� �� ó���� �ؼ� ���������� ���� �������ش�.
    public enum STAGECLASS { SMALL, MEDIUM, LARGE };

    [Serializable]
    public class Values
    {
        STAGECLASS size;
        //Vector2Int Size;
        string Name;


    }

    public void SetLinkedData(LinkedData linkeddata)
    {

        

        //int count = 0;
        //if(linkeddata.LeftMap!=null)
        //{
        //    door[(int)Direction.Left] = new Door();

        //}
    }

    public void RoomStart(Direction direction)
    {

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

    
}
