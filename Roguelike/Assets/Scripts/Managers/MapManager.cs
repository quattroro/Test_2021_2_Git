using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//맵 컨트롤러, 다음층으로 넘어가기 등등
public class MapManager : Singleton<MapManager>
{
    public enum Floors { Floor1, Floor2, Floor3, Floor4, FloorMax };

    public enum RoomType { Nomal, Boss, Trap, Shop, Treasure, Devil, RoomTypeMax};

    public enum RoomSize { Single, Double_Vir, Double_Hor, Triple_1, Triple_2, Triple_3, Triple_4, Quard, SizeMax };

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

        public int MaxRoomNum;
        public int MinRoomNum;

        public int MaxBossRoom;
    }



    public GameObject LoadRoom(RoomType type, RoomSize size)
    {
        GameObject temp = new GameObject();

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
