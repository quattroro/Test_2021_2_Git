using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//2차원 배열을 만들고 
//램덤으로 방 프리셋을 뽑아서 방들을 병합한다.
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
        new Vector3Int( 1, 0,  0),       // right
        new Vector3Int(-1, 0,  0),       // left
        new Vector3Int( 0, 1,  0),       // down
        new Vector3Int( 0, -1, 0)        // up
    };

    //public Dictionary<int, List<Vector3Int>> downPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0),   new Vector3Int(0, 2, 0),   new Vector3Int(-1, 2, 0) } }, // ㅁ
    //    {  1, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0),    new Vector3Int(-1, 2, 0)    } }, // ┓
    //    {  2, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0),    new Vector3Int(1, 2, 0)     } }, // ┏
    //    {  3, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(0, 2, 0)                                 } }, // 아래 |
    //    {  4, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0)                                } }, // 아래 |
    //    {  5, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(1, 1, 0)                                 } }, // 아래 |
    //    {  6, new List<Vector3Int>      { new Vector3Int(0, 1, 0)                                                            } }, // 아래 |
    //};
    //public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),  new Vector3Int(-1, -1, 0),  new Vector3Int(-1, -2, 0) } }, // ㅁ
    //    {  1, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),   new Vector3Int(1, -2, 0)    } }, // ┏
    //    {  2, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0),   new Vector3Int(-1, -2, 0)   } }, // ┐
    //    {  3, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(0, -2, 0)                                } }, // 위 |
    //    {  4, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)                                } }, // 위 |
    //    {  5, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)                               } }, // 위 |
    //    {  6, new List<Vector3Int>      { new Vector3Int(0, -1, 0)                                                            } }, // 위 |
    //};
    //public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-1, -1, 0),  new Vector3Int(-2, -1, 0) } }, // ㅁ
    //    {  1, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, -1, 0)   } }, // └ 
    //    {  2, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 1, 0)    } }, // ┌
    //    {  3, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0)                                } }, // 왼쪽  --
    //    {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, -1, 0)                               } }, // 왼쪽  --
    //    {  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 1, 0)                                } }, // 왼쪽  --
    //    {  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                           } }, // 왼쪽  --
    //};
    //public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    //{
    //    {  0, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(1, 1, 0) ,   new Vector3Int(2, 1, 0) } }, // ㅁ
    //    {  1, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, 1, 0)     } }, // ┐
    //    {  2, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, -1, 0)    } }, // ┛ 
    //    {  3, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0)                                 } }, // 오른쪽  --
    //    {  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 1, 0)                                 } }, // 오른쪽  --
    //    {  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, -1, 0)                                } }, // 오른쪽  --
    //    {  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                            } },
    //};


    public Dictionary<int, List<Vector3Int>> downPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(-1, 0, 0),   new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, 1, 0),    new Vector3Int(-1, 1, 0)    } }, // ┓
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, 1, 0),    new Vector3Int(1, 1, 0)     } }, // ┏
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, 1, 0)                                 } }, // 아래 |
       // {  4, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0)                                } }, // 아래 |
        //{  5, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(1, 1, 0)                                 } }, // 아래 |
        //{  6, new List<Vector3Int>      { new Vector3Int(0, 1, 0)                                                            } }, // 아래 |
    };
    public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0),  new Vector3Int(1, -1, 0),  new Vector3Int(1, 0, 0) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)    } }, // ┏
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)   } }, // ┐
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0)                                } }, // 위 |
        //{  4, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)                                } }, // 위 |
        //{  5, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)                               } }, // 위 |
        //{  6, new List<Vector3Int>      { new Vector3Int(0, -1, 0)                                                            } }, // 위 |
    };
    public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0),   new Vector3Int(-1, -1, 0),  new Vector3Int(0, -1, 0) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0),   new Vector3Int(-1, -1, 0)   } }, // └ 
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0),   new Vector3Int(-1, 1, 0)    } }, // ┌
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0)                                } }, // 왼쪽  --
       // {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, -1, 0)                               } }, // 왼쪽  --
        //{  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 1, 0)                                } }, // 왼쪽  --
        //{  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                           } }, // 왼쪽  --
    };
    public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0),    new Vector3Int(1, 1, 0) ,   new Vector3Int(0, 1, 0) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0),    new Vector3Int(1, 1, 0)     } }, // ┐
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0),    new Vector3Int(1, -1, 0)    } }, // ┛ 
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0)                                 } }, // 오른쪽  --
        //{  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 1, 0)                                 } }, // 오른쪽  --
        //{  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, -1, 0)                                } }, // 오른쪽  --
        //{  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                            } },
    };


    public void RoomSpawnProc()
    {
        
        Initsetting();
        CreateRoomInfo(listsize / 2, listsize / 2);
        MargeRoom();


    }

    public void CreateRoomArr()
    {





    }


    //만들어진 방들을 찾아다니면서 주위에 존재하는 방의 개수에 따라 확률을 적용해서 방들을 병합해준다. 병합이 끝난방들은 active를 true 로 바꿔 중복으로 변경하지 않도록 한다.
    public void MargeRoom()
    {

        List<Dictionary<int, List<Vector3Int>>> move = new List<Dictionary<int, List<Vector3Int>>>() { downPatten, upPatten, rightPatten, leftPatten };
        //double Percent{ 0.02,0.1,0.1,0.3,0.3,0.3,0.8}
        double[] persent = { 0.2, 0.3, 0.3, 0.5};

        for (int y=0;y<listsize;y++)
        {
            for(int x=0;x<listsize;x++)
            {
                Vector3Int start = new Vector3Int(x, y, 0);
                if (current.Roomarr[x+(y*listsize)]!=null)
                {
                    int direction = UnityEngine.Random.Range(0, direction4.Count);
                    int pattern = choose(persent);
                    Debug.Log($"{pattern}번 패턴 뽑힘");
                    //해당 위치에서 해당 패턴이 생성 가능하면 패턴대로 만들어져있는 방들을 병합한다.
                    if (PosiblePattern(start, move[direction][pattern]))
                    {
                        Vector3Int next;
                        int count = move[direction][pattern].Count;
                        for (int i = 0; i < count; i++)
                        {
                            next = start + move[direction][pattern][i];
                            switch (count)
                            {
                                case 1:
                                    Debug.Log($"{next.x},{next.y}번방 싱글룸 만듬");
                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    current.Roomarr[next.x + (next.y * listsize)].roomType = "Single";
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = start;
                                    break;

                                case 2:
                                    Debug.Log($"{next.x},{next.y}번방 더블룸으로 변경");
                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    current.Roomarr[next.x + (next.y * listsize)].roomType = "Double";
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = next;
                                    current.Roomarr[next.x + (next.y * listsize)].parentPos = start + move[direction][pattern][0];
                                    break;

                                case 3:
                                    Debug.Log($"{next.x},{next.y}번방 Triple룸으로 변경");
                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple";
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = next;
                                    current.Roomarr[next.x + (next.y * listsize)].parentPos = start + move[direction][pattern][0];

                                    break;

                                case 4:
                                    Debug.Log($"{next.x},{next.y}번방 Quard룸으로 변경");
                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    current.Roomarr[next.x + (next.y * listsize)].roomType = "Quard";
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = next;
                                    current.Roomarr[next.x + (next.y * listsize)].parentPos = start + move[direction][pattern][0];
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (current.Roomarr[start.x + (start.y * listsize)].roomState == false)
                        {
                            Debug.Log($"{start.x},{start.y}번방 싱글룸 만듬");
                            current.Roomarr[start.x + (start.y * listsize)].roomState = true;
                            current.Roomarr[start.x + (start.y * listsize)].roomType = "Single";
                            current.Roomarr[start.x + (start.y * listsize)].CenterPos = start;
                        }
                        
                    }
                }
            }
        }
    }

    public bool PosiblePattern(Vector3Int start, List<Vector3Int> pattern)
    {
        Vector3Int next;
        for (int i = 0; i < pattern.Count; i++)
        {
            next = start + pattern[i];
            //배열의 범위를 벋어나면 false
            if (!IsMoveabe(next.x, next.y))
            {
                return false;
            }
            //해당 위치에 룸정보가 없거나, 이미 작업이 완료된 방이면 false를 리턴
            if (current.Roomarr[next.x + (next.y * listsize)] == null || current.Roomarr[next.x + (next.y * listsize)].roomState)
            {
                return false;
            }
        }
        return true;
    }



    public int choose(double[] probs)
    {
        //double total = 0;
        //double[] percent = new double[probs.Length];
        //foreach(var a in probs)
        //{
        //    total += a;
        //}

        //for(int i=0;i<probs.Length;i++)
        //{
        //    percent[i] = probs[i] / total;
        //}

        double rnd = UnityEngine.Random.value;

        for(int i=0;i<probs.Length;i++)
        {
            if(rnd< probs[i])
            {
                return i;
            }
        }

        return probs.Length - 1;

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

    //재귀함수로 배열들을 돌면서 방들을 활성화 시킨다. 이후에 할성화된 방들을 돌면서 방들을 병합시킨다.
    public void CreateRoomInfo(int x, int y)
    {
        Vector3Int nowpos = new Vector3Int(x, y, 0);
        Vector3Int nextpos;

        if (current.NowCount >= stats.MaxRoomNum)
        {
            //Debug.Log($"개수 다참");
            return;
        }

        //해당 위치가 비어있으면 방정보를 만들어 준다.
        if (current.Roomarr[x + (y * listsize)] == null)
        {
            Debug.Log($"{current.NowCount}번방 {x},{y} 생성");
            current.Roomarr[x + (y * listsize)] = GetNewRoomInfo(nowpos, current.NowCount);
            current.NowCount++;
        }
        else
        {
            //Debug.Log($"널이아님");
            return;
        }


        //네 방향중 하나를 선택해서 이동 해당 함수가 리턴했는데 아직방 개수개 채워지지 않았으면 다른방향으로 간다 

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
                //Debug.Log($"개수 다참2");
                return;
            }

        }
       //Debug.Log($"그냥나감");
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
