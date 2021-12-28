using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Data", menuName = "New Scriptable Object/Gun Data", order = int.MaxValue)]
public class GunData : ScriptableObject
{
    public enum GunType { Pistol, Rifle, Shotgun, Grenade };

    public string GunName;
    public GunType type;
    [Range(0.0f, 10.0f), Tooltip("�ѹ� �߻� ���� �� �߻� �ð�")]
    public float CoolDown;
    [Range(-1, 1000), Tooltip("�ִ� ��ȿ �Ÿ�(-1�϶��� ������ �Ÿ�)")]
    public float MaxRange;
    [Range(1, 500), Tooltip("��źâ ź�� ����")]
    public int OneClipBulletNum;
    [Range(1, 100), Tooltip("������")]
    public int Damage;
    [Range(1,10),Tooltip("�ѹ� �߻�� �ѹ��� ���� ź ����")]
    public float MaxShot;
    [Range(0.0f, 1f), Tooltip("ź ��Ȯ��")]
    public float Variation;
    [Range(1.0f, 100.0f), Tooltip("ź���� ũ�� ������")]
    public float VariationRadius;

}
