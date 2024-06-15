using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Node_Type  //���ӵ�����
{
    Walk,
    Stop,
}

/// <summary>
/// ������
/// </summary>
public class AStarNode
{
    //���Ӷ����ڵ�ͼ������
    public int x;
    public int y;
    
    public float f;  //Ѱ·����ֵ
    public float g;  //�����ľ���
    public float h;  //���յ�ľ���

    public AStarNode father; //���Ӷ���ĸ�����
    //���ӵ�����
    public E_Node_Type type; 

    public AStarNode(int x,int y,E_Node_Type type)
    {
        this.x = x; 
        this.y = y;
        this.type = type;
    }

}
