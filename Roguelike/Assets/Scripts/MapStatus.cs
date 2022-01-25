using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapStatus : ScriptableObject
{
    //�� ������ ���� �ѷ������� ���Ͱ� �����ɰ�����, �Ϲݹ��� �����, ������, ��� �ٸ� ����� ���� Ȯ���� ��� ����� 
    

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
