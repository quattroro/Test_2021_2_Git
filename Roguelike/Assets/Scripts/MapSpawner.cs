using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2���� �迭�� ����� 
//�������� �� �������� �̾Ƽ� ����� �����Ѵ�.
//

public class MapSpawner : Singleton<MapSpawner>
{
    public RoomInfo[] Roomarr;
    public int listsize;


    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int( 0, 1,  0),       // down
        new Vector3Int( 1, 0,  0),       // right
        new Vector3Int(-1, 0,  0),       // left
        new Vector3Int( 0, -1, 0)        // up
    };



    public void Initsetting()
    {
        Roomarr = new RoomInfo[listsize * listsize];



    }

    public void RoomClear(RoomInfo room)
    {

    }

    public void InitRoomInfo(RoomInfo roominfo, Vector2Int index)
    {
        roominfo.CenterPos = new Vector3(0, 0, 0);
        //roominfo.currPos = index;




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
