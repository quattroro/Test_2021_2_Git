using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance = null;

    public static MapManager Instance
    {
        get
        {
                
            return _instance;
        }
    }

    public enum STAGE { Stage1, Stage1Boss, Stage2, Stage2Boss, Stage3, Stage3Boss, StageMax };
    public enum ROOMTYPE { START, RESTAURANT, SHOP, END, NOMAL, MAX };
    public enum ROOMCLASS { SMALL, MEDIUM, LARGE };

    //public enum 
    public StageData[] LinkedData = null;
    //public int arrsize;

    public GameObject[] StageArr = null;
    public int arrsize;

   
    public STAGE nowstage;


    public GameObject[] SpecialRoom;
    public GameObject startRoom;
    public GameObject[] largeroom;
    public GameObject[] mediumroom;
    public GameObject[] smallroom;
    public GameObject EndRoom;



    public STAGE NowStage
    {
        get 
        {
            return nowstage;
        }
        set
        {
            //현재 스테이지가 설정되면 해당 스테이지의 
            GameObject[] stages = Resources.LoadAll<GameObject>($"Prefabs/MapPrefabs/{value.ToString()}");
            foreach(var a in stages)
            {
                //a.GetComponent<BaseStage>().
            }

        }
    }    





    public void LoadStagesToPrefabs(STAGE stage)
    {
        largeroom = Resources.LoadAll<GameObject>($"Prefabs/MapPrefabs/{stage.ToString()}/Large");
        mediumroom = Resources.LoadAll<GameObject>($"Prefabs/MapPrefabs/{stage.ToString()}/Medium");
        smallroom = Resources.LoadAll<GameObject>($"Prefabs/MapPrefabs/{stage.ToString()}/Small");

        SpecialRoom[(int)ROOMTYPE.START]= Resources.Load<GameObject>($"Prefabs/MapPrefabs/{stage.ToString()}/Stage1_Start");
        SpecialRoom[(int)ROOMTYPE.SHOP] = Resources.Load<GameObject>($"Prefabs/MapPrefabs/{stage.ToString()}/Stage1_Shop");
        SpecialRoom[(int)ROOMTYPE.RESTAURANT] = Resources.Load<GameObject>($"Prefabs/MapPrefabs/{stage.ToString()}/Stage1_Restaurant");
    }


    public void StageSetting(GameObject[] rooms, int size)
    {
        StageArr = rooms;
        arrsize = size;
    }

    //스테이지 세팅이 끝나면 시작
    public void StageStart()
    {
        //0째 방을 찾아서 해당방을 활성화 시킨다.
    }

    public void SetBossRoom()
    {

    }

    

    public GameObject StageLoad(ROOMTYPE type)
    {
        return SpecialRoom[(int)type];
    }

    public GameObject StageLoad(ROOMTYPE type, ROOMCLASS roomclass)
    {
        //랜덤으로 뽑아서 하나를 넘겨준다.
        int count = 0;
        if (roomclass == ROOMCLASS.SMALL) count = smallroom.Length;
        else if (roomclass == ROOMCLASS.MEDIUM) count = mediumroom.Length;
        else if (roomclass == ROOMCLASS.LARGE) count = largeroom.Length;

        int rnd = Random.Range(0, count);


        if (roomclass == ROOMCLASS.SMALL) return smallroom[rnd];
        else if (roomclass == ROOMCLASS.MEDIUM) return mediumroom[rnd];
        else return largeroom[rnd];

    }


    //public void StageLoad(StageData[] arr, int size)
    //{
    //    for (int i = 0; i < size * size; i++)
    //    {
    //        int count = 0;
    //        if (arr[i] != null)
    //        {
    //            if (arr[i].RightMap != null) count++;
    //            if (arr[i].LeftMap != null) count++;
    //            if (arr[i].UpMap != null) count++;
    //            if (arr[i].DownMap != null) count++;
    //            //주변에 존재하는 방의 개수를 가지고 방의 크기를 결정한다.

    //            //100% 큰방
    //            if (count >= 4)
    //            {

    //            }
    //            else if (count >= 3)//70%중간방,30%큰방
    //            {

    //            }
    //            else if (count >= 2)//길이 두개있는방도 레스토랑과 상점방이 없으면 번호가 빠른 방이 레스토랑과 상점방으로 뽑힌다.
    //            {

    //            }
    //            else//길이 하나있는방은 레스토랑 또는 상점방이 된다.
    //            {

    //            }

    //        }

    //    }
    //}

    public void InitSetting()
    {
        SpecialRoom = new GameObject[(int)ROOMTYPE.MAX];
    }
    public void InitSetting(StageData[] arr, int size)
    {
        LinkedData = arr;
        arrsize = size;
        for(int i=0;i<size*size;i++)
        {
            if (arr[i] != null)
            {

            }
        }





    }




    //public T Load<T>(string name)
    //{

    //}

    private void Awake()
    {
        if(_instance==null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        InitSetting();
        LoadStagesToPrefabs(STAGE.Stage1);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
