using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapStatus : ScriptableObject
{
    //각 층마다 무슨 총류까지의 몬스터가 스폰될것인지, 일반방은 몇개인지, 상점방, 등등 다른 방들의 스폰 확률은 어떤지 등등의 
    

    public float TreasureRoomVal;
    public int MaxTreasureRoom;

    public float DevilRoomVal;
    public int MaxDevilRoom;

    public float ShopRoomVal;
    public int MaxShopRoom;

    [Range(1,20)]
    public int MaxRoomNum;
    [Range(1,20)]
    public int MinRoomNum;

    public int MaxBossRoom;


}
