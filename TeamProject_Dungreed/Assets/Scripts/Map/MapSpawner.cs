using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapSpawner : MonoBehaviour
{
    public enum DIRECTION { NODATA = -1, UP, RIGHT, DOWN, LEFT, MAX };

    [Serializable]
    public class SpawnOption
    {
        public int MaxNum;
        public int MaxRestaurant;
        public int MaxShop;


        public List<GameObject> MapPrefabs;

        [Range(0.00f, 1.00f), Tooltip("각 방의 텔레포터 스폰 확률")]
        public float teleporterPercent;
        [Range(0.00f, 1.00f), Tooltip("각 방의 브론즈박스 스폰 확률")]
        public float BronzeChestPercent;
        [Range(0.00f, 1.00f), Tooltip("각 방의 실버박스 스폰 확률")]
        public float SilverChestPercent;
        [Range(0.00f, 1.00f), Tooltip("각 방의 골드박스 스폰 확률")]
        public float GoldChestPercent;

        public int MaxListSize = 5;
    }

    [Serializable]
    public class CurrentValue
    {
        public int NowCount;
        public int RestaurantCount;
        public int ShopCount;
        [SerializeField]
        public StageData[] Maplist;
        public GameObject[] MapPrefabsList;

    }

    [SerializeField]
    public SpawnOption option = new SpawnOption();
    [SerializeField]
    public CurrentValue current = new CurrentValue();

    public MapManager mapmanager;

    public void InitSetting()
    {
        current.NowCount = 0;
        current.Maplist = new StageData[option.MaxListSize * option.MaxListSize];
        current.MapPrefabsList = new GameObject[option.MaxListSize * option.MaxListSize];
        for (int i = 0; i < option.MaxListSize * option.MaxListSize - 1; i++)
        {
            current.Maplist[i] = null;
            current.MapPrefabsList[i] = null;
        }


    }

    //맵매니저에 정보를 보내서 해당하는 맵을 받아온다.
    public void StageLoad(StageData[] arr, int size)
    {

    }

    //시작위치, 레스토랑, 상점, 제단, 보스방 등의 방을 만들어 준다.
    public void StageSetting()
    {
        int size = option.MaxListSize;
        for (int i = 0; i < size * size; i++)
        {
            int count = 0;

            if(current.Maplist[i]!=null)
            {
                if (current.Maplist[i].Num == 0)
                {

                    //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.START);
                    current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.Start);
                    Debug.Log($"{current.Maplist[i].Num}번째방 시작방");
                    //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                }
                else if (current.Maplist[i].Num == option.MaxNum - 1)//마지막 방은 끝방으로 한다.
                {
                    //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.END);
                    current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.End);
                    Debug.Log($"{current.Maplist[i].Num}번째방 끝방");
                    //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                }
                else
                {
                    if (current.Maplist[i].RightMap != null) count++;
                    if (current.Maplist[i].LeftMap != null) count++;
                    if (current.Maplist[i].UpMap != null) count++;
                    if (current.Maplist[i].DownMap != null) count++;
                    //주변에 존재하는 방의 개수를 가지고 방의 크기를 결정한다.

                    //100% 큰방
                    if (count <= 2)//길이 하나 또는 두개인 방은 무조건 작은방 이면서 상점과, 음식점이 없으면 상점과 음식점이 된다.
                    {
                        int rnd = UnityEngine.Random.Range(0, 100);
                        if (current.RestaurantCount < option.MaxRestaurant)
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.RESTAURANT);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.Restaurant);
                            current.RestaurantCount++;
                            Debug.Log($"{current.Maplist[i].Num}번째방 레스토랑");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                        }
                        else if (current.ShopCount < option.MaxShop)
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.SHOP);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.Shop);
                            current.ShopCount++;
                            Debug.Log($"{current.Maplist[i].Num}번째방 상점");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                        }
                        else
                        {

                            if (rnd < 70)
                            {
                                //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                                current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.SMALL);
                                Debug.Log($"{current.Maplist[i].Num}번째방 작은방");
                                //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                            }
                            else
                            {
                                //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                                current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                                Debug.Log($"{current.Maplist[i].Num}번째방 중간방");
                                //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                            }

                        }
                    }
                    else if (count <= 3)//70%중간방,30%큰방
                    {
                        int rnd = UnityEngine.Random.Range(0, 100);
                        if (rnd < 70)
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                            Debug.Log($"{current.Maplist[i].Num}번째방 중간방");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                        }
                        else
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                            Debug.Log($"{current.Maplist[i].Num}번째방 큰방");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                        }
                    }
                    else if (count <= 4)//길이 두개있는방도 레스토랑과 상점방이 없으면 번호가 빠른 방이 레스토랑과 상점방으로 뽑힌다.
                    {
                        //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                        current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                        Debug.Log($"{current.Maplist[i].Num}번째방 큰방");
                        // current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//방들의 링크정보를 각각에게 들려준다.
                    }
                }
            }
            
            ////프리팹 리스트에 값이 들어갔으면 주변에 있는 방들을 검사해서 링크데이터를 넣어준다.
            //if (current.MapPrefabsList[i] != null)
            //{
            //    LinkedData data = SetLinkingData(i);
            //    current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = data;
            //    Debug.Log($"{i}번방 링크세팅");
            //}
        }
    }

    public LinkedData SetLinkingData(int index)
    {
        LinkedData linkeddata = new LinkedData();
        int yval = option.MaxListSize;
        int x = index % yval;
        int y = index / yval;
        linkeddata.Num = current.Maplist[index].Num;
        //linkeddata에서 어느방향이 연결되어 있는 지 확인하고
        //prefabs에서 해당방향에 방이 현재 만들어져 있는지 확인하고
        //서로 연결한다.

        if (current.Maplist[index].LeftMap!=null)
        {
            if(current.MapPrefabsList[(x - 1) + (y * yval)] != null)
            {
                linkeddata.LeftMap = current.MapPrefabsList[(x - 1) + (y * yval)];
                if (current.MapPrefabsList[(x - 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapPrefabsList[(x - 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData.RightMap = current.MapPrefabsList[(x) + (y * yval)];
                }
            }
        }
        if(current.Maplist[index].RightMap!=null)
        {
            if (current.MapPrefabsList[(x + 1) + (y * yval)] != null)
            {
                linkeddata.RightMap = current.MapPrefabsList[(x + 1) + (y * yval)];

                if (current.MapPrefabsList[(x + 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapPrefabsList[(x + 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData.LeftMap = current.MapPrefabsList[(x) + (y * yval)];
                }
            }
        }
        if(current.Maplist[index].UpMap!=null)
        {
            if (current.MapPrefabsList[x + ((y - 1) * yval)] != null)
            {
                linkeddata.UpMap = current.MapPrefabsList[x + ((y - 1) * yval)];

                if (current.MapPrefabsList[x + ((y - 1) * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapPrefabsList[x + ((y - 1) * yval)].GetComponent<BaseStage>().StageLinkedData.DownMap = current.MapPrefabsList[(x) + (y * yval)];
                }
                //current.Maplist[x + (y * yval)].UpMap = current.Maplist[x + ((y - 1) * yval)];
                //current.Maplist[x + ((y - 1) * yval)].DownMap = current.Maplist[x + (y * yval)];
            }
        }
        if(current.Maplist[index].DownMap!=null)
        {
            //아래쪽
            if (current.MapPrefabsList[x + ((y + 1) * yval)] != null)
            {

                linkeddata.DownMap = current.MapPrefabsList[x + ((y + 1) * yval)];

                if (current.MapPrefabsList[x + ((y + 1) * yval)].GetComponent<BaseStage>().StageLinkedData != null)
                {
                    current.MapPrefabsList[x + ((y + 1) * yval)].GetComponent<BaseStage>().StageLinkedData.UpMap = current.MapPrefabsList[(x) + (y * yval)];
                }
                //current.Maplist[x + (y * yval)].DownMap = current.Maplist[x + ((y + 1) * yval)];
                //current.Maplist[x + ((y + 1) * yval)].UpMap = current.Maplist[x + (y * yval)];
            }
        }
              


        ////왼쪽
        //if (x - 1 >= 0 && current.MapPrefabsList[(x - 1) + (y * yval)] != null)
        //{
        //    linkeddata.LeftMap = current.MapPrefabsList[(x - 1) + (y * yval)];
        //    if (current.MapPrefabsList[(x - 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData != null)
        //    {
        //        current.MapPrefabsList[(x - 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData.RightMap = current.MapPrefabsList[(x) + (y * yval)];
        //    }
        //    //current.Maplist[x + (y * yval)].LeftMap = current.Maplist[(x - 1) + (y * yval)];
        //    //current.Maplist[(x - 1) + (y * yval)].RightMap = current.Maplist[x + (y * yval)];
        //}

        ////오른쪽
        //if (x + 1 <= option.MaxListSize - 1 && current.MapPrefabsList[(x + 1) + (y * yval)] != null)
        //{
        //    linkeddata.RightMap = current.MapPrefabsList[(x + 1) + (y * yval)];

        //    if (current.MapPrefabsList[(x + 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData != null)
        //    {
        //        current.MapPrefabsList[(x + 1) + (y * yval)].GetComponent<BaseStage>().StageLinkedData.LeftMap = current.MapPrefabsList[(x) + (y * yval)];
        //    }

        //    //current.Maplist[x + (y * yval)].RightMap = current.Maplist[(x + 1) + (y * yval)];
        //    //current.Maplist[(x + 1) + (y * yval)].LeftMap = current.Maplist[x + (y * yval)];
        //}

        ////위쪽
        //if (y - 1 >= 0 && current.MapPrefabsList[x + ((y - 1) * yval)] != null)
        //{
        //    linkeddata.UpMap = current.MapPrefabsList[x + ((y - 1) * yval)];

        //    if (current.MapPrefabsList[x + ((y - 1) * yval)].GetComponent<BaseStage>().StageLinkedData != null)
        //    {
        //        current.MapPrefabsList[x + ((y - 1) * yval)].GetComponent<BaseStage>().StageLinkedData.DownMap = current.MapPrefabsList[(x) + (y * yval)];
        //    }
        //    //current.Maplist[x + (y * yval)].UpMap = current.Maplist[x + ((y - 1) * yval)];
        //    //current.Maplist[x + ((y - 1) * yval)].DownMap = current.Maplist[x + (y * yval)];
        //}

        ////아래쪽
        //if (y + 1 <= option.MaxListSize - 1 && current.MapPrefabsList[x + ((y + 1) * yval)] != null)
        //{

        //    linkeddata.DownMap = current.MapPrefabsList[x + ((y + 1) * yval)];

        //    if (current.MapPrefabsList[x + ((y + 1) * yval)].GetComponent<BaseStage>().StageLinkedData != null)
        //    {
        //        current.MapPrefabsList[x + ((y + 1) * yval)].GetComponent<BaseStage>().StageLinkedData.UpMap = current.MapPrefabsList[(x) + (y * yval)];
        //    }
        //    //current.Maplist[x + (y * yval)].DownMap = current.Maplist[x + ((y + 1) * yval)];
        //    //current.Maplist[x + ((y + 1) * yval)].UpMap = current.Maplist[x + (y * yval)];
        //}

        return linkeddata;
    }

    //스포너에서 맵을 생성을 해서 맵 배열을 만들어 주고 해당 배열을 맵 매니저에 넘겨준다.
    public int MapSpawn(int x, int y, StageData Parent)
    {
        if (current.NowCount >= option.MaxNum)
        {
            //Debug.Log($"전부 끝났을때 리턴");
            return option.MaxNum;
        }

        int RandNum;
        int yval = option.MaxListSize;
        //들어오면 해당 위치에 방을 만들고 
        //음식점, 상점, 방 크기, 상자 스폰 등을 결정한다.
        //보스전이 있어야 하면 보스방도 스폰
        if (current.Maplist[x + (y * yval)] == null)
        {

            current.Maplist[x + (y * yval)] = new StageData();
            Debug.Log($"[{x},{y}] {x + (y * yval)}생성");
            current.Maplist[x + (y * yval)].InitSttting(current.NowCount, x, y);
            current.NowCount++;

            if(Parent!=null)
            {
                if (Parent.indexX == x)
                {
                    if(Parent.indexY>y)//아래쪽과 연결
                    {
                        current.Maplist[x + (y * yval)].DownMap = Parent;
                        Parent.UpMap = current.Maplist[x + (y * yval)];
                    }
                    else//위쪽과 연결
                    {
                        current.Maplist[x + (y * yval)].UpMap = Parent;
                        Parent.DownMap = current.Maplist[x + (y * yval)];

                        
                    }
                }
                else if (Parent.indexY == y)
                {
                    if(Parent.indexX>x)//오른쪽과 연결
                    {
                        current.Maplist[x + (y * yval)].RightMap = Parent;
                        Parent.LeftMap = current.Maplist[x + (y * yval)];
                    }
                    else//왼쪽과 연결
                    {
                        current.Maplist[x + (y * yval)].LeftMap = Parent;
                        Parent.RightMap = current.Maplist[x + (y * yval)];
                    }
                }
            }

            ////왼쪽
            //if (x - 1 >= 0 && current.Maplist[(x - 1) + (y * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].LeftMap = current.Maplist[(x - 1) + (y * yval)];
            //    current.Maplist[(x - 1) + (y * yval)].RightMap = current.Maplist[x + (y * yval)];
            //}

            ////오른쪽
            //if (x + 1 <= option.MaxListSize - 1 && current.Maplist[(x + 1) + (y * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].RightMap = current.Maplist[(x + 1) + (y * yval)];
            //    current.Maplist[(x + 1) + (y * yval)].LeftMap = current.Maplist[x + (y * yval)];
            //}

            ////위쪽
            //if (y - 1 >= 0 && current.Maplist[x + ((y - 1) * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].UpMap = current.Maplist[x + ((y - 1) * yval)];
            //    current.Maplist[x + ((y - 1) * yval)].DownMap = current.Maplist[x + (y * yval)];
            //}

            ////아래쪽
            //if (y + 1 <= option.MaxListSize - 1 && current.Maplist[x + ((y + 1) * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].DownMap = current.Maplist[x + ((y + 1) * yval)];
            //    current.Maplist[x + ((y + 1) * yval)].UpMap = current.Maplist[x + (y * yval)];
            //}

        }
        else
        {
            return option.MaxNum;
        }

        //천번째 방은 항상 오른쪽으로간다.
        if (current.NowCount == 1)
        {
            MapSpawn(x + 1, y, current.Maplist[(x) + (y * yval)]);
            return current.NowCount;
        }

        //왼쪽
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"랜덤{RandNum}");
        if (RandNum <= 70)
        {
            if (x - 1 >= 1 && current.Maplist[(x - 1) + (y * yval)] == null)
            {
                MapSpawn(x - 1, y, current.Maplist[(x) + (y * yval)]);
            }
        }

        //오른쪽
        RandNum = UnityEngine.Random.Range(0, 100);

        //Debug.Log($"랜덤1=>{RandNum}");
        if (RandNum <= 70)
        {
            //Debug.Log($"조건1=>{x + 1 >= option.MaxListSize - 1}");
            //Debug.Log($"조건2=>{current.Maplist[x + 1, y] == null}");
            if (x + 1 <= option.MaxListSize - 1 && current.Maplist[(x + 1) + (y * yval)] == null)
            {
                //Debug.Log($"랜덤3=>{RandNum}");
                MapSpawn(x + 1, y, current.Maplist[(x) + (y * yval)]);
            }
        }

        //위쪽
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"랜덤{RandNum}");
        if (RandNum <= 70)
        {
            if (y - 1 >= 0 && current.Maplist[x + ((y - 1) * yval)] == null)
            {
                MapSpawn(x, y - 1, current.Maplist[(x) + (y * yval)]);
            }
        }

        //아래쪽
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"랜덤{RandNum}");
        if (RandNum <= 70)
        {
            if (y + 1 <= option.MaxListSize - 1 && current.Maplist[x + ((y + 1) * yval)] == null)
            {
                MapSpawn(x, y + 1, current.Maplist[(x) + (y * yval)]);
            }
        }
        //Debug.Log($"만들곳이 없을때 리턴");
        return current.NowCount;
    }




    //맵스폰하고 스폰된 맵을 맵 매니저한테 넘겨준다.
    public void SpawnStart()
    {
        int val = MapSpawn(0, 2,null);
        int num = val;

        StageSetting();

        MapManager.Instance.StageSetting(current.MapPrefabsList, option.MaxListSize);
        Debug.Log($"만들어진 방의 개수{num}");
    }

    public void ShowMaps()
    {
        int interval = 50;
        int yval = option.MaxListSize;

        for (int y = 0; y < option.MaxListSize; y++)
        {
            for (int x = 0; x < option.MaxListSize; x++)
            {
                if(current.MapPrefabsList[x + (y * yval)]!=null)
                {
                    GameObject obj = GameObject.Instantiate(current.MapPrefabsList[x + (y * yval)]);
                    
                    obj.transform.position = new Vector3(transform.position.x + (x * interval), transform.position.y + ((y * interval) * -1));
                    LinkedData data = SetLinkingData(x + (y * yval));
                    obj.GetComponent<BaseStage>().StageLinkedData = data;
                    obj.GetComponent<BaseStage>().Initsetting();
                    int num= obj.GetComponent<BaseStage>().StageLinkedData.Num;

                    obj.name = current.MapPrefabsList[x + (y * yval)].name /*+ $"_{num}"*/;
                    obj.GetComponent<BaseStage>().StageNum = num;

                    if (obj.GetComponent<BaseStage>().type==MapManager.ROOMTYPE.Start)
                    {
                        //플레이어를 시작 위치로 이동
                        //GameObject playerobj = GameObject.Find("Player");
                        //playerobj.transform.position = obj.GetComponent<BaseStage>().StartPos.position;
                        //playerobj=obj.GetComponent<BaseStage>().start
                    }

                }
            }
        }
    }

    //끝방을 클리어 하고 e버튼으로 상호작용을 했을때
    public void EndMaps()
    {

    }
    //public void 


    // Start is called before the first frame update
    void Start()
    {
        InitSetting();
        SpawnStart();
        ShowMaps();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
