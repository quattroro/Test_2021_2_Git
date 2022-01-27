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
    public List<Vector3Int> ExistMap;

    public enum Direction { Right, Left, Down, Up };
    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int( 1, 0,  0),       // right
        new Vector3Int(-1, 0,  0),       // left
        new Vector3Int( 0, 1,  0),       // down
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
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(-1, 0, 0),   new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, 1, 0),    new Vector3Int(1, 1, 0)    } }, // �� �� 
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, 1, 0),    new Vector3Int(-1, 1, 0)     } }, // ��  �� 
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, 1, 0)                                 } }, // �Ʒ� |
       // {  4, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(-1, 1, 0)                                } }, // �Ʒ� |
        //{  5, new List<Vector3Int>      { new Vector3Int(0, 1, 0),   new Vector3Int(1, 1, 0)                                 } }, // �Ʒ� |
        //{  6, new List<Vector3Int>      { new Vector3Int(0, 1, 0)                                                            } }, // �Ʒ� |
    };
    public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0),  new Vector3Int(1, -1, 0),  new Vector3Int(1, 0, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)    } }, // ��
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)   } }, // ��
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(0, -1, 0)                                } }, // �� |
        //{  4, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(1, -1, 0)                                } }, // �� |
        //{  5, new List<Vector3Int>      { new Vector3Int(0, -1, 0),   new Vector3Int(-1, -1, 0)                               } }, // �� |
        //{  6, new List<Vector3Int>      { new Vector3Int(0, -1, 0)                                                            } }, // �� |
    };
    public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0),   new Vector3Int(-1, -1, 0),  new Vector3Int(0, -1, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0),   new Vector3Int(-1, -1, 0)   } }, // ��
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0),   new Vector3Int(-1, 1, 0)    } }, // 
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),  new Vector3Int(-1, 0, 0)                                } }, // ����  --
       // {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, -1, 0)                               } }, // ����  --
        //{  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 1, 0)                                } }, // ����  --
        //{  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                           } }, // ����  --
    };
    public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0),    new Vector3Int(1, 1, 0) ,   new Vector3Int(0, 1, 0) } }, // ��
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0),    new Vector3Int(1, 1, 0)     } }, // ��
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0),    new Vector3Int(1, -1, 0)    } }, // 
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 0),   new Vector3Int(1, 0, 0)                                 } }, // ������  --
        //{  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 1, 0)                                 } }, // ������  --
        //{  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, -1, 0)                                } }, // ������  --
        //{  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                            } },
    };


    public void RoomSpawnProc()
    {
        
        Initsetting();
        CreateRoomInfo(listsize / 2, listsize / 2);
        MargeRoom();
        CreateRoomArr();

    }

    public void CreateRoomArr()
    {
        int count = 0;
        for(int y=0;y<listsize;y++)
        {
            for(int x=0;x<listsize;x++)
            {
                //�������迭�� ������ ������ ���� ��ġ�� ���� ����� �ش�.
                if(current.Roomarr[x + (y * listsize)] != null&& current.Roomarr[x + (y * listsize)].roomState)
                {
                    //�� Ÿ�Կ� ���� ���� ����� �ش�. ���� �� ��Ʈ��Ʈ���� �θ� �����ϸ� ���յ� ���������� ���� ������ �ʴ´�.
                    //���� �̹� ����� ���� �ʾ����� 
                    if (current.Roomarr[x + (y * listsize)].roomType == "Single")
                    {
                        if (!ExistMap.Contains(new Vector3Int(x, y, 0)))
                        {
                            GameObject prefabobj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, MapManager.RoomSize.Single);
                            current.MapObjList[x + (y * listsize)] = GameObject.Instantiate(prefabobj);
                            current.MapObjList[x + (y * listsize)].transform.position = this.transform.position + new Vector3(x * 30, y * 30, 0);
                            ExistMap.Add(new Vector3Int(x, y, 0));
                        }
                    }
                    else
                    {
                        Vector3Int parent = current.Roomarr[x + (y * listsize)].parentPos;
                        if (!ExistMap.Contains(new Vector3Int(parent.x, parent.y, 0)))
                        {
                            MapManager.RoomSize size = StringToEnum(current.Roomarr[x + (y * listsize)].roomType);

                            GameObject prefabobj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, size);
                            current.MapObjList[parent.x + (parent.y * listsize)] = GameObject.Instantiate(prefabobj);
                            current.MapObjList[x + (y * listsize)].transform.position = this.transform.position + new Vector3(x * 30, y * 30, 0);
                            ExistMap.Add(new Vector3Int(parent.x, parent.y, 0));
                        }
                    }


                    //else if (current.Roomarr[x + (y * listsize)].roomType == "Double_Hor")
                    //{
                    //    //MapManager.RoomSize size = current.Roomarr[x + (y * listsize)].roomType.ToString().ToEnum<MapManager.RoomSize>;

                    //    MapManager.RoomSize size = StringToEnum(current.Roomarr[x + (y * listsize)].roomType);
                    //    GameObject obj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, size);
                    //    ExistMap.Add(new Vector3Int(x, y, 0));
                    //    //GameObject obj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, MapManager.RoomSize.);
                    //}

                    //else if (current.Roomarr[x + (y * listsize)].roomType == "Triple")
                    //{
                    //    MapManager.RoomSize size = StringToEnum(current.Roomarr[x + (y * listsize)].roomType);
                    //    GameObject obj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, size);
                    //    ExistMap.Add(new Vector3Int(x, y, 0));
                    //    //GameObject obj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, MapManager.RoomSize.Single);
                    //}
                    //else if (current.Roomarr[x + (y * listsize)].roomType == "Quard")
                    //{
                    //    MapManager.RoomSize size = StringToEnum(current.Roomarr[x + (y * listsize)].roomType);
                    //    GameObject obj = MapManager.Instance.LoadRoom(MapManager.RoomType.Nomal, size);
                    //    ExistMap.Add(new Vector3Int(x, y, 0));
                    //}

                    //
                    
                }


            }
        }

    }

    MapManager.RoomSize StringToEnum(string str)
    {
        return (MapManager.RoomSize)Enum.Parse(typeof(MapManager.RoomSize), str);
    }


    public void ShowRoom()
    {

    }

    //�ֺ��� �ִ� ����� ã�Ƽ� ���������� �Ѱ��ش�.
    public LinkedData GetLinkedData(int x, int y)
    {
        LinkedData link = new LinkedData();
        Vector3Int nowpos = new Vector3Int(x, y, 0);

        //right
        Vector3Int next = nowpos + direction4[(int)Direction.Right];
        if (current.MapObjList[next.x + (next.y * listsize)] != null)
        {
            link.RightRoom = current.MapObjList[next.x + (next.y * listsize)];
            //�ش� ��ġ�� �����ϴ� �濡 ��ũ������ �̹� ���� �Ǿ������� �ش� ���� ��ũ������ ����
            if (current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata != null)
            {
                current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata.LeftRoom = current.MapObjList[nowpos.x + (nowpos.y * listsize)];
            }
        }

        //left
        next = nowpos + direction4[(int)Direction.Left];
        if (current.MapObjList[next.x + (next.y * listsize)] != null)
        {
            link.LeftRoom = current.MapObjList[next.x + (next.y * listsize)];
            //�ش� ��ġ�� �����ϴ� �濡 ��ũ������ �̹� ���� �Ǿ������� �ش� ���� ��ũ������ ����
            if (current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata != null)
            {
                current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata.RightRoom = current.MapObjList[nowpos.x + (nowpos.y * listsize)];
            }
        }

        //up
        next = nowpos + direction4[(int)Direction.Up];
        if (current.MapObjList[next.x + (next.y * listsize)] != null)
        {
            link.UpRoom = current.MapObjList[next.x + (next.y * listsize)];
            //�ش� ��ġ�� �����ϴ� �濡 ��ũ������ �̹� ���� �Ǿ������� �ش� ���� ��ũ������ ����
            if (current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata != null)
            {
                current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata.DownRoom = current.MapObjList[nowpos.x + (nowpos.y * listsize)];
            }
        }

        //down
        next = nowpos + direction4[(int)Direction.Down];
        if (current.MapObjList[next.x + (next.y * listsize)] != null)
        {
            link.DownRoom = current.MapObjList[next.x + (next.y * listsize)];
            //�ش� ��ġ�� �����ϴ� �濡 ��ũ������ �̹� ���� �Ǿ������� �ش� ���� ��ũ������ ����
            if (current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata != null)
            {
                current.MapObjList[next.x + (next.y * listsize)].GetComponentInChildren<MyRoom>().linkeddata.UpRoom = current.MapObjList[nowpos.x + (nowpos.y * listsize)];
            }
        }

        return link;
    }


    //�ֺ��� �����ϴ� ���� ������ �Ѱ��ش�.
    public int RoomCount(int x,int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        Vector3Int temp;
        int count = 0;
        for (int i = 0; i < direction4.Count; i++)
        {
            temp = pos + direction4[i];
            if (current.Roomarr[temp.x + (temp.y * listsize)] != null)
                count++;
        }
        return count;
    }


    //������� ����� ã�ƴٴϸ鼭 ������ �����ϴ� ���� ������ ���� Ȯ���� �����ؼ� ����� �������ش�. ������ ��������� active�� true �� �ٲ� �ߺ����� �������� �ʵ��� �Ѵ�.
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
                    Debug.Log($"{pattern}�� ���� ����");
                    //�ش� ��ġ���� �ش� ������ ���� �����ϸ� ���ϴ�� ��������ִ� ����� �����Ѵ�.
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
                                    Debug.Log($"{next.x},{next.y}���� �̱۷� ����");
                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    current.Roomarr[next.x + (next.y * listsize)].roomType = "Single";
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = start;
                                    break;

                                case 2:
                                    Debug.Log($"{next.x},{next.y}���� ��������� ����");
                                    if (direction == (int)Direction.Right || direction == (int)Direction.Left)
                                    {
                                        current.Roomarr[next.x + (next.y * listsize)].roomType = "Double_Hor";
                                    }
                                    else
                                    {
                                        current.Roomarr[next.x + (next.y * listsize)].roomType = "Double_Vir";
                                    }
                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = next;
                                    current.Roomarr[next.x + (next.y * listsize)].parentPos = start + move[direction][pattern][0];
                                    break;

                                case 3:
                                    Debug.Log($"{next.x},{next.y}���� Triple������ ����");

                                    current.Roomarr[next.x + (next.y * listsize)].roomState = true;
                                    if(direction == (int)Direction.Right)
                                    {
                                        if (pattern == 1)
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_3_2";
                                        }
                                        else
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_3_1";
                                        }
                                    }

                                    if (direction == (int)Direction.Left)
                                    {
                                        if (pattern == 1)
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_1_1";
                                        }
                                        else
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_1_2";
                                        }
                                    }

                                    if (direction == (int)Direction.Up)
                                    {
                                        if (pattern == 1)
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_2_2";
                                        }
                                        else
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_2_1";
                                        }
                                    }

                                    if (direction == (int)Direction.Down)
                                    {
                                        if (pattern == 1)
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_4_1";
                                        }
                                        else
                                        {
                                            current.Roomarr[next.x + (next.y * listsize)].roomType = "Triple_4_2";
                                        }
                                    }
                                    
                                    current.Roomarr[next.x + (next.y * listsize)].CenterPos = next;
                                    current.Roomarr[next.x + (next.y * listsize)].parentPos = start + move[direction][pattern][0];

                                    break;

                                case 4:
                                    Debug.Log($"{next.x},{next.y}���� Quard������ ����");
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
                            Debug.Log($"{start.x},{start.y}���� �̱۷� ����");
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
            //�迭�� ������ ����� false
            if (!IsMoveabe(next.x, next.y))
            {
                return false;
            }
            //�ش� ��ġ�� �������� ���ų�, �̹� �۾��� �Ϸ�� ���̸� false�� ����
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
        current.MapObjList = new GameObject[listsize * listsize];


        for(int i=0;i<listsize*listsize;i++)
        {
            current.Roomarr[i] = null;
            current.MapObjList[i] = null;
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
            //Debug.Log($"{current.NowCount}���� {x},{y} ����");
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
