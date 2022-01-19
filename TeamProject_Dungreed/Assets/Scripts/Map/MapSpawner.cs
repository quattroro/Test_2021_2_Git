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

        [Range(0.00f, 1.00f), Tooltip("�� ���� �ڷ����� ���� Ȯ��")]
        public float teleporterPercent;
        [Range(0.00f, 1.00f), Tooltip("�� ���� �����ڽ� ���� Ȯ��")]
        public float BronzeChestPercent;
        [Range(0.00f, 1.00f), Tooltip("�� ���� �ǹ��ڽ� ���� Ȯ��")]
        public float SilverChestPercent;
        [Range(0.00f, 1.00f), Tooltip("�� ���� ���ڽ� ���� Ȯ��")]
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

    //�ʸŴ����� ������ ������ �ش��ϴ� ���� �޾ƿ´�.
    public void StageLoad(StageData[] arr, int size)
    {

    }

    //������ġ, �������, ����, ����, ������ ���� ���� ����� �ش�.
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
                    Debug.Log($"{current.Maplist[i].Num}��°�� ���۹�");
                    //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                }
                else if (current.Maplist[i].Num == option.MaxNum - 1)//������ ���� �������� �Ѵ�.
                {
                    //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.END);
                    current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.End);
                    Debug.Log($"{current.Maplist[i].Num}��°�� ����");
                    //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                }
                else
                {
                    if (current.Maplist[i].RightMap != null) count++;
                    if (current.Maplist[i].LeftMap != null) count++;
                    if (current.Maplist[i].UpMap != null) count++;
                    if (current.Maplist[i].DownMap != null) count++;
                    //�ֺ��� �����ϴ� ���� ������ ������ ���� ũ�⸦ �����Ѵ�.

                    //100% ū��
                    if (count <= 2)//���� �ϳ� �Ǵ� �ΰ��� ���� ������ ������ �̸鼭 ������, �������� ������ ������ �������� �ȴ�.
                    {
                        int rnd = UnityEngine.Random.Range(0, 100);
                        if (current.RestaurantCount < option.MaxRestaurant)
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.RESTAURANT);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.Restaurant);
                            current.RestaurantCount++;
                            Debug.Log($"{current.Maplist[i].Num}��°�� �������");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                        }
                        else if (current.ShopCount < option.MaxShop)
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.SHOP);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.Shop);
                            current.ShopCount++;
                            Debug.Log($"{current.Maplist[i].Num}��°�� ����");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                        }
                        else
                        {

                            if (rnd < 70)
                            {
                                //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                                current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.SMALL);
                                Debug.Log($"{current.Maplist[i].Num}��°�� ������");
                                //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                            }
                            else
                            {
                                //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                                current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                                Debug.Log($"{current.Maplist[i].Num}��°�� �߰���");
                                //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                            }

                        }
                    }
                    else if (count <= 3)//70%�߰���,30%ū��
                    {
                        int rnd = UnityEngine.Random.Range(0, 100);
                        if (rnd < 70)
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.MEDIUM);
                            Debug.Log($"{current.Maplist[i].Num}��°�� �߰���");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                        }
                        else
                        {
                            //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                            current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                            Debug.Log($"{current.Maplist[i].Num}��°�� ū��");
                            //current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                        }
                    }
                    else if (count <= 4)//���� �ΰ��ִ¹浵 ��������� �������� ������ ��ȣ�� ���� ���� ��������� ���������� ������.
                    {
                        //current.MapPrefabsList[i] = MapManager.Instance.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                        current.MapPrefabsList[i] = mapmanager.StageLoad(MapManager.ROOMTYPE.NOMAL, MapManager.ROOMCLASS.LARGE);
                        Debug.Log($"{current.Maplist[i].Num}��°�� ū��");
                        // current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = current.Maplist[i];//����� ��ũ������ �������� ����ش�.
                    }
                }
            }
            
            ////������ ����Ʈ�� ���� ������ �ֺ��� �ִ� ����� �˻��ؼ� ��ũ�����͸� �־��ش�.
            //if (current.MapPrefabsList[i] != null)
            //{
            //    LinkedData data = SetLinkingData(i);
            //    current.MapPrefabsList[i].GetComponent<BaseStage>().StageLinkedData = data;
            //    Debug.Log($"{i}���� ��ũ����");
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
        //linkeddata���� ��������� ����Ǿ� �ִ� �� Ȯ���ϰ�
        //prefabs���� �ش���⿡ ���� ���� ������� �ִ��� Ȯ���ϰ�
        //���� �����Ѵ�.

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
            //�Ʒ���
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
              


        ////����
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

        ////������
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

        ////����
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

        ////�Ʒ���
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

    //�����ʿ��� ���� ������ �ؼ� �� �迭�� ����� �ְ� �ش� �迭�� �� �Ŵ����� �Ѱ��ش�.
    public int MapSpawn(int x, int y, StageData Parent)
    {
        if (current.NowCount >= option.MaxNum)
        {
            //Debug.Log($"���� �������� ����");
            return option.MaxNum;
        }

        int RandNum;
        int yval = option.MaxListSize;
        //������ �ش� ��ġ�� ���� ����� 
        //������, ����, �� ũ��, ���� ���� ���� �����Ѵ�.
        //�������� �־�� �ϸ� �����浵 ����
        if (current.Maplist[x + (y * yval)] == null)
        {

            current.Maplist[x + (y * yval)] = new StageData();
            Debug.Log($"[{x},{y}] {x + (y * yval)}����");
            current.Maplist[x + (y * yval)].InitSttting(current.NowCount, x, y);
            current.NowCount++;

            if(Parent!=null)
            {
                if (Parent.indexX == x)
                {
                    if(Parent.indexY>y)//�Ʒ��ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].DownMap = Parent;
                        Parent.UpMap = current.Maplist[x + (y * yval)];
                    }
                    else//���ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].UpMap = Parent;
                        Parent.DownMap = current.Maplist[x + (y * yval)];

                        
                    }
                }
                else if (Parent.indexY == y)
                {
                    if(Parent.indexX>x)//�����ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].RightMap = Parent;
                        Parent.LeftMap = current.Maplist[x + (y * yval)];
                    }
                    else//���ʰ� ����
                    {
                        current.Maplist[x + (y * yval)].LeftMap = Parent;
                        Parent.RightMap = current.Maplist[x + (y * yval)];
                    }
                }
            }

            ////����
            //if (x - 1 >= 0 && current.Maplist[(x - 1) + (y * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].LeftMap = current.Maplist[(x - 1) + (y * yval)];
            //    current.Maplist[(x - 1) + (y * yval)].RightMap = current.Maplist[x + (y * yval)];
            //}

            ////������
            //if (x + 1 <= option.MaxListSize - 1 && current.Maplist[(x + 1) + (y * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].RightMap = current.Maplist[(x + 1) + (y * yval)];
            //    current.Maplist[(x + 1) + (y * yval)].LeftMap = current.Maplist[x + (y * yval)];
            //}

            ////����
            //if (y - 1 >= 0 && current.Maplist[x + ((y - 1) * yval)] != null)
            //{
            //    current.Maplist[x + (y * yval)].UpMap = current.Maplist[x + ((y - 1) * yval)];
            //    current.Maplist[x + ((y - 1) * yval)].DownMap = current.Maplist[x + (y * yval)];
            //}

            ////�Ʒ���
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

        //õ��° ���� �׻� ���������ΰ���.
        if (current.NowCount == 1)
        {
            MapSpawn(x + 1, y, current.Maplist[(x) + (y * yval)]);
            return current.NowCount;
        }

        //����
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"����{RandNum}");
        if (RandNum <= 70)
        {
            if (x - 1 >= 1 && current.Maplist[(x - 1) + (y * yval)] == null)
            {
                MapSpawn(x - 1, y, current.Maplist[(x) + (y * yval)]);
            }
        }

        //������
        RandNum = UnityEngine.Random.Range(0, 100);

        //Debug.Log($"����1=>{RandNum}");
        if (RandNum <= 70)
        {
            //Debug.Log($"����1=>{x + 1 >= option.MaxListSize - 1}");
            //Debug.Log($"����2=>{current.Maplist[x + 1, y] == null}");
            if (x + 1 <= option.MaxListSize - 1 && current.Maplist[(x + 1) + (y * yval)] == null)
            {
                //Debug.Log($"����3=>{RandNum}");
                MapSpawn(x + 1, y, current.Maplist[(x) + (y * yval)]);
            }
        }

        //����
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"����{RandNum}");
        if (RandNum <= 70)
        {
            if (y - 1 >= 0 && current.Maplist[x + ((y - 1) * yval)] == null)
            {
                MapSpawn(x, y - 1, current.Maplist[(x) + (y * yval)]);
            }
        }

        //�Ʒ���
        RandNum = UnityEngine.Random.Range(0, 100);
        //Debug.Log($"����{RandNum}");
        if (RandNum <= 70)
        {
            if (y + 1 <= option.MaxListSize - 1 && current.Maplist[x + ((y + 1) * yval)] == null)
            {
                MapSpawn(x, y + 1, current.Maplist[(x) + (y * yval)]);
            }
        }
        //Debug.Log($"������� ������ ����");
        return current.NowCount;
    }




    //�ʽ����ϰ� ������ ���� �� �Ŵ������� �Ѱ��ش�.
    public void SpawnStart()
    {
        int val = MapSpawn(0, 2,null);
        int num = val;

        StageSetting();

        MapManager.Instance.StageSetting(current.MapPrefabsList, option.MaxListSize);
        Debug.Log($"������� ���� ����{num}");
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
                        //�÷��̾ ���� ��ġ�� �̵�
                        //GameObject playerobj = GameObject.Find("Player");
                        //playerobj.transform.position = obj.GetComponent<BaseStage>().StartPos.position;
                        //playerobj=obj.GetComponent<BaseStage>().start
                    }

                }
            }
        }
    }

    //������ Ŭ���� �ϰ� e��ư���� ��ȣ�ۿ��� ������
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