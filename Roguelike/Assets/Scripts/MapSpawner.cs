using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//2���� �迭�� ����� 
//�������� �� �������� �̾Ƽ� ����� �����Ѵ�.
//

public class MapSpawner : Singleton<MapSpawner>
{
    
    public int listsize;

    MapManager.CurrentStats stats;

    [Serializable]
    public class CurrentValue
    {
        public int NowCount;

        public int ShopCount;
        [SerializeField]
        public RoomInfo[] Roomarr;
        //public RoomInfo[] Maplist;
        //public GameObject[] MapPrefabsList;
        public List<GameObject> CurrentRoomObjs;
        public GameObject[] MapObjList;
    }
    public CurrentValue current;

    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int( 0, 1,  0),       // down
        new Vector3Int( 1, 0,  0),       // right
        new Vector3Int(-1, 0,  0),       // left
        new Vector3Int( 0, -1, 0)        // up
    };

    //public Dictionary<int, List<Vector3Int>> downPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0),   new Vector3Int(0, 2, 0),   new Vector3Int(-1, 2, 0) } }, // ��
    //    {  1, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0),    new Vector3Int(-1, 2, 0)    } }, // ��
    //    {  2, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0),    new Vector3Int(1, 2, 0)     } }, // ��
    //    {  3, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0)                                 } }, // �Ʒ� |
    //    {  4, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0)                                } }, // �Ʒ� |
    //    {  5, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(1, 1, 0)                                 } }, // �Ʒ� |
    //    {  6, new List<Vector3Int>      { new Vector3Int(0, 1, 0)                                                            } }, // �Ʒ� |
    //};
    //public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),  new Vector3Int(-1, -1, 0),  new Vector3Int(-1, -2, 0) } }, // ��
    //    {  1, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),   new Vector3Int(1, -2, 0)    } }, // ��
    //    {  2, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),   new Vector3Int(-1, -2, 0)   } }, // ��
    //    {  3, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0)                                } }, // �� |
    //    {  4, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)                                } }, // �� |
    //    {  5, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)                               } }, // �� |
    //    {  6, new List<Vector3Int>      { new Vector3Int(0, -1, 0)                                                            } }, // �� |
    //};
    //public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-1, -1, 0),  new Vector3Int(-2, -1, 0) } }, // ��
    //    {  1, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, -1, 0)   } }, // �� 
    //    {  2, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 1, 0)    } }, // ��
    //    {  3, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0)                                } }, // ����  --
    //    {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, -1, 0)                               } }, // ����  --
    //    {  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 1, 0)                                } }, // ����  --
    //    {  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                           } }, // ����  --
    //};
    //public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(1, 1, 0) ,   new Vector3Int(2, 1, 0) } }, // ��
    //    {  1, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, 1, 0)     } }, // ��
    //    {  2, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, -1, 0)    } }, // �� 
    //    {  3, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0)                                 } }, // ������  --
    //    {  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 1, 0)                                 } }, // ������  --
    //    {  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, -1, 0)                                } }, // ������  --
    //    {  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                            } },
    //};


    public Dictionary<int, List<Vector3Int>> downPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0),   new Vector3Int(0, 2, 0),   new Vector3Int(-1, 2, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0),    new Vector3Int(-1, 2, 0)    } }, // ��
        {  2, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0),    new Vector3Int(1, 2, 0)     } }, // ��
        {  3, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0)                                 } }, // �Ʒ� |
        {  4, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0)                                } }, // �Ʒ� |
        {  5, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(1, 1, 0)                                 } }, // �Ʒ� |
        {  6, new List<Vector3Int>      { new Vector3Int(0, 1, 0)                                                            } }, // �Ʒ� |
    };
    public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),  new Vector3Int(-1, -1, 0),  new Vector3Int(-1, -2, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),   new Vector3Int(1, -2, 0)    } }, // ��
        {  2, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),   new Vector3Int(-1, -2, 0)   } }, // ��
        {  3, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0)                                } }, // �� |
        {  4, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)                                } }, // �� |
        {  5, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)                               } }, // �� |
        {  6, new List<Vector3Int>      { new Vector3Int(0, -1, 0)                                                            } }, // �� |
    };
    public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-1, -1, 0),  new Vector3Int(-2, -1, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, -1, 0)   } }, // �� 
        {  2, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 1, 0)    } }, // ��
        {  3, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0)                                } }, // ����  --
        {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, -1, 0)                               } }, // ����  --
        {  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 1, 0)                                } }, // ����  --
        {  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                           } }, // ����  --
    };
    public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(1, 1, 0) ,   new Vector3Int(2, 1, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, 1, 0)     } }, // ��
        {  2, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, -1, 0)    } }, // �� 
        {  3, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0)                                 } }, // ������  --
        {  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 1, 0)                                 } }, // ������  --
        {  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, -1, 0)                                } }, // ������  --
        {  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                            } },
    };


    public void RoomSpawnProc()
    {
        
        Initsetting();
        CreateRoomInfo(listsize / 2, listsize / 2);



    }

    //������� ����� ã�ƴٴϸ鼭 ������ �����ϴ� ���� ������ ���� Ȯ���� �����ؼ� ����� �������ش�. ������ ��������� active�� true �� �ٲ� �ߺ����� �������� �ʵ��� �Ѵ�.
    public void MargeRoom()
    {

    }

    public void SelectSpecialRoom()
    {

    }    


    public void Initsetting()
    {
        current.NowCount = 0;
        
        stats = MapManager.Instance.stats;
        current.Roomarr = new RoomInfo[listsize * listsize];

        for(int i=0;i<listsize*listsize;i++)
        {
            current.Roomarr[i] = null;
        }

    }

    //����Լ��� �迭���� ���鼭 ����� Ȱ��ȭ ��Ų��. ���Ŀ� �Ҽ�ȭ�� ����� ���鼭 ����� ���ս�Ų��.
    public void CreateRoomInfo(int x, int y)
    {
        Vector3Int nowpos = new Vector3Int(x, y, 0);
        Vector3Int nextpos;

        if (current.NowCount >= stats.MaxRoomNum)
        {
            //Debug.Log($"���� ����");
            return;
        }

        //�ش� ��ġ�� ��������� �������� ����� �ش�.
        if (current.Roomarr[x + (y * listsize)] == null)
        {
            Debug.Log($"{current.NowCount}���� {x},{y} ����");
            current.Roomarr[x + (y * listsize)] = GetNewRoomInfo(nowpos, current.NowCount);
            current.NowCount++;
        }
        else
        {
            //Debug.Log($"���̾ƴ�");
            return;
        }


        //�� ������ �ϳ��� �����ؼ� �̵� �ش� �Լ��� �����ߴµ� ������ ������ ä������ �ʾ����� �ٸ��������� ���� 

        //int dir = UnityEngine.Random.Range(0, direction4.Count);
        //nextpos = nowpos + direction4[dir];

        for (int i = 0; i < direction4.Count; i++)
        {
            int dir = UnityEngine.Random.Range(0, direction4.Count);
            nextpos = nowpos + direction4[dir];
            if (IsMoveabe(nextpos.x, nextpos.y))
            {
                CreateRoomInfo(nextpos.x, nextpos.y);
            }
            if (current.NowCount >= stats.MaxRoomNum)
            {
                //Debug.Log($"���� ����2");
                return;
            }

        }
       //Debug.Log($"�׳ɳ���");
    }




    public bool IsMoveabe(int x, int y)
    {
        if ((x >= 0 && x < listsize) && (y >= 0 && y < listsize))
        {
            return true;
        }
        else
        {
            return false;
        }


    }

    public RoomInfo GetNewRoomInfo(Vector3Int pos,  int num)
    {
        RoomInfo info = new RoomInfo();
        info.currPos = pos;
        info.CenterPos = new Vector3Int(-1, -1, 0);
        info.parentPos = new Vector3Int(-1, -1, 0);
        info.roomState = false;
        info.weightCnt = -1;
        info.num = num;
        return info;
    }

    public void RoomClear(RoomInfo room)
    {
        room.currPos = new Vector3Int(-1, -1, 0);
        room.CenterPos = new Vector3Int(-1, -1, 0);
        room.parentPos = new Vector3Int(-1, -1, 0);
        room.roomState = false;
        room.weightCnt = -1;
    }

    public void InitRoomInfo(RoomInfo roominfo, Vector2Int index)
    {
        roominfo.CenterPos = new Vector3(0, 0, 0);
        //roominfo.currPos = index;




    }





    // Start is called before the first frame update
    void Start()
    {
        RoomSpawnProc();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
