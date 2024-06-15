using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SoldierType
{
    Hero,       //Ӣ��
    Warrior,    //սʿ
    Acher,      //����
    Magician,   //ħ��ʦ
    Long
}

public class SoldierObj : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private GameObject footEffect;
    //ʿ��������
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

    public void Move(Vector3 pos)  //�ƶ�����
    {
        agent.SetDestination(pos);
    }

    /// <summary>
    /// �Ƿ�uѡ���Լ�
    /// </summary>
    /// <param name="sel"></param>
    public void SetSelSelf(bool issel)
    {
        footEffect.SetActive(issel);
    }
}
