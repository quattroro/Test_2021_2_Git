using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//맵 컨트롤러, 다음층으로 넘어가기 등등
public class MapManager : Singleton<MapManager>
{
    public enum Floors { Floor1, Floor2, Floor3, Floor4, FloorMax };

    public enum RoomType { Nomal, Boss, Trap, Shop, Treasure, Devil, RoomTypeMax};

    public enum RoomSize { Single, Double_Vir, Double_Hor, Triple_1_1, Triple_1_2, Triple_2_1, Triple_2_2, Triple_3_1, Triple_3_2, Triple_4_1, Triple_4_2, Quard, SizeMax };

    public Floors NowFloor;

    [System.Serializable]
    public class CurrentStats
    {
        public float TreasureRoomVal;
        public int MaxTreasureRoom;

        public float DevilRoomVal;
        public int MaxDevilRoom;

        public float ShopRoomVal;
        public int MaxShopRoom;

        [Range(1, 20)]
        public int MaxRoomNum;
        [Range(1, 20)]
        public int MinRoomNum;

        public int MaxBossRoom;
    }

    public CurrentStats stats;

    public GameObject LoadRoom(RoomType type, RoomSize size)
    {
        GameObject temp = null;
        if (type==RoomType.Nomal)
        {
            temp = (GameObject)Resources.Load($"Prefabs/MapPrefabs/{NowFloor.ToString()}/{type.ToString()}/Room_{size.ToString()}");
        }
        else
        {
            temp = (GameObject)Resources.Load($"Prefabs/MapPrefabs/{NowFloor.ToString()}/{type.ToString()}");
        }
        return temp;
    }

    public void NextFloor()
    {

    }

    public void RoomSpawn()
    {



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
