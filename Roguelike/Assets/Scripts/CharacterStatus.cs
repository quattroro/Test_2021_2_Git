using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CharacterData",menuName = "Scriptable Object/Character Data",order =int.MaxValue)]
public class CharacterStatus : ScriptableObject
{
    public enum Characters { Isaac, Cain, Mangdalene, Eve, Samson, Lazarus, Azazel, CMax };

    [SerializeField]
    public string myname;
    [SerializeField]
    public Characters type;
    [SerializeField]
    public int MaxHP;
    [SerializeField]
    public int CurHP;
    [SerializeField]
    public int SoulHeart;
    [SerializeField]
    public float Damage;
    [SerializeField]
    public float MoveSpeed;
    [SerializeField]
    public float AttackSpeed;
    [SerializeField]
    public float ShootSpeed;
    [SerializeField]
    public float ShootRange;
    [SerializeField]
    public bool UnLock;

}
