using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Hand : MonoBehaviour
{
    private Animator anim;
    private BoxCollider2D boxCollider;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject target; // 타겟, 플레이어

    void Start()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        MoveToTarget();
    }

    bool attack_Start = false;
    void MoveToTarget()
    {
        if (!attack_Start)
            return;

        Vector3 targetPos = new Vector3(transform.position.x, target.transform.position.y, 0);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.05f);

        if (transform.position == targetPos)
            attack_Start = false;
    }

    public IEnumerator Anim_Coroutine()
    {
        attack_Start = true;

        yield return new WaitForSeconds(0.7f);

        Attack_Start();
    }

    public void Attack_Start()
    {
        attack_Start = false;
        boxCollider.enabled = true;
        laser.SetActive(true);
    }

    public void Attack_Finish()
    {
        boxCollider.enabled = false;
        laser.SetActive(false);
        anim.SetBool("isAttack", false);
    }
}
