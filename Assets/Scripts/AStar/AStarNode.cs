using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Node_Type  //格子的类型
{
    Walk,
    Stop,
}

/// <summary>
/// 格子类
/// </summary>
public class AStarNode
{
    //格子对象在地图的坐标
    public int x;
    public int y;
    
    public float f;  //寻路消耗值
    public float g;  //离起点的距离
    public float h;  //离终点的距离

    public AStarNode father; //格子对象的父对象
    //格子的类型
    public E_Node_Type type; 

    public AStarNode(int x,int y,E_Node_Type type)
    {
        this.x = x; 
        this.y = y;
        this.type = type;
    }

}
