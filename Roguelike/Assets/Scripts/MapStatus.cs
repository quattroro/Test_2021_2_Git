using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapStatusData", menuName = "Scriptable Object/MapStatusData", order = int.MaxValue)]
public class MapStatus : ScriptableObject
{
    //각 층마다 무슨 총류까지의 몬스터가 스폰될것인지, 일반방은 몇개인지, 상점방, 등등 다른 방들의 스폰 확률은 어떤지 등등의 
    enum Floors { Floor1, Floor2, Floor3, Floor4, FloorMax };

    Floors floor;




}
