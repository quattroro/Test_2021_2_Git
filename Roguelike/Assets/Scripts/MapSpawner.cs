using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2���� �迭�� ����� 
//�������� �� �������� �̾Ƽ� ����� �����Ѵ�.
//

public class MapSpawner : MonoBehaviour
{
    public RoomInfo[] maparr;
    public int listsize;

    public void Initsetting()
    {
        maparr = new RoomInfo[listsize * listsize];



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
