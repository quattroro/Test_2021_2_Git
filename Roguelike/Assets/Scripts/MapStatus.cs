using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapStatusData", menuName = "Scriptable Object/MapStatusData", order = int.MaxValue)]
public class MapStatus : ScriptableObject
{
    //�� ������ ���� �ѷ������� ���Ͱ� �����ɰ�����, �Ϲݹ��� �����, ������, ��� �ٸ� ����� ���� Ȯ���� ��� ����� 
    enum Floors { Floor1, Floor2, Floor3, Floor4, FloorMax };

    Floors floor;




}
