using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected float Hp; // 체력 
    protected float Damage; // 공격력
    protected float Speed; // 스피드 
    protected float Attack_Delay;

    protected abstract void Move(); // 이동

    protected abstract void Attack(); // 공격

    protected abstract void Damaged(); // 피격

    protected virtual void Jump() // 점프
    {

    }
}
