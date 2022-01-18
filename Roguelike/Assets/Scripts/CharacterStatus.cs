using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharacterData",menuName = "Scriptable Object/Character Data",order =int.MaxValue)]
public class CharacterStatus : ScriptableObject
{
    public enum Characters { Isaac, Cain, Mangdalene, Eve, Samson, Lazarus, Azazel, CMax };

    [SerializeField]
    private string myname;
    [SerializeField]
    private Characters type;
    [SerializeField]
    private int MaxHP;
    [SerializeField]
    private int CurHP;
    [SerializeField]
    private int SoulHeart;
    [SerializeField]
    private float Damage;
    [SerializeField]
    private float MoveSpeed;
    [SerializeField]
    private float AttackSpeed;
    [SerializeField]
    private float ShootSpeed;
    [SerializeField]
    private float ShootRange;
    [SerializeField]
    private bool UnLock;

}
