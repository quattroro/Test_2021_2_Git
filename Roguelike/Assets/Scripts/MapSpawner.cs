using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2차원 배열을 만들고 
//램덤으로 방 프리셋을 뽑아서 방들을 병합한다.
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
