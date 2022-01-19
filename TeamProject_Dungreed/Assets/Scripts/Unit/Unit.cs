using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    protected float Hp; // ü�� 
    protected float Damage; // ���ݷ�
    protected float Speed; // ���ǵ� 
    protected float Attack_Delay;

    protected abstract void Move(); // �̵�

    protected abstract void Attack(); // ����

    protected abstract void Damaged(); // �ǰ�

    protected virtual void Jump() // ����
    {

    }
}
