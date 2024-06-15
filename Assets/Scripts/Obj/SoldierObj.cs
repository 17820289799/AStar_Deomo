using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SoldierType
{
    Hero,       //英雄
    Warrior,    //战士
    Acher,      //猎人
    Magician,   //魔法师
    Long
}

public class SoldierObj : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private GameObject footEffect;
    //士兵的类型
    public SoldierType type;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        footEffect = transform.Find("FootEffect").gameObject;
        SetSelSelf(false);
    }


    void Update()
    {
        animator.SetBool("IsMove", agent.velocity.magnitude > 0);
    }

    public void Move(Vector3 pos)  //移动方法
    {
        agent.SetDestination(pos);
    }

    /// <summary>
    /// 是佛u选中自己
    /// </summary>
    /// <param name="sel"></param>
    public void SetSelSelf(bool issel)
    {
        footEffect.SetActive(issel);
    }
}
