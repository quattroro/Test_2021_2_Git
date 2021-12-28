using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun Data", menuName = "New Scriptable Object/Gun Data", order = int.MaxValue)]
public class GunData : ScriptableObject
{
    public enum GunType { Pistol, Rifle, Shotgun, Grenade };

    public string GunName;
    public GunType type;
    [Range(0.0f, 10.0f), Tooltip("한번 발사 이후 재 발사 시간")]
    public float CoolDown;
    [Range(-1, 1000), Tooltip("최대 유효 거리(-1일때는 무한정 거리)")]
    public float MaxRange;
    [Range(1, 500), Tooltip("한탄창 탄의 개수")]
    public int OneClipBulletNum;
    [Range(1, 100), Tooltip("데미지")]
    public int Damage;
    [Range(1,10),Tooltip("한번 발사시 한번에 나갈 탄 개수")]
    public float MaxShot;
    [Range(0.0f, 1f), Tooltip("탄 정확도")]
    public float Variation;
    [Range(1.0f, 100.0f), Tooltip("탄착군 크기 반지름")]
    public float VariationRadius;

}
