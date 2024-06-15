using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMgr
{
    // 地图的宽和高
    private int mapW;
    private int mapH;
    private static AStarMgr instance;
    public static AStarMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AStarMgr();
            }
            return instance;
        }
    }

    // 地图所有格子的对象容器
    public AStarNode[,] nodes;
    // 开启列表
    private List<AStarNode> openList = new List<AStarNode>();
    // 关闭列表
    private List<AStarNode> closeList = new List<AStarNode>();

    /// <summary>
    /// 初始化地图信息（一开始进入游戏，传值进去告诉该地图有多大
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitiMapInfo(int w, int h)
    {
        // 记录宽高
        this.mapW = w;
        this.mapH = h;
        // 声明容器一共可以装多少个格子
        nodes = new AStarNode[w, h];

        // 根据宽高创建地图的格子
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                // 随机地图的障碍点(实际是通过地图数据的配表  定义障碍点的
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
                nodes[i, j] = node;
            }
        }
    }

    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        // 首先判断传入的两个点是否合法（是否是地图范围内,是不是阻挡的点 ,不合法就要返回null
        // 1、判断是否在地图范围内
        if (startPos.x < 0 || startPos.x >= mapW || startPos.y < 0 || startPos.y >= mapH ||
            endPos.x < 0 || endPos.x >= mapW || endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("起点或者终点在地图范围外");
            return null;
        }

        // 2、判断起点和终点是不是障碍点
        AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode end = nodes[(int)endPos.x, (int)endPos.y];
        if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
        {
            Debug.Log("起点或者终点在障碍点内");
            return null;
        }

        // 寻路前，清空开表和闭包（为了下一次寻路
        closeList.Clear();
        openList.Clear();

        // 合法 (得到起点和终点的格子
        // 从起点开始找周围的 8个点 放入开表(进一步判断周围的点是否是边界或者墙壁障碍 还要判断是否在开闭表里面 ,如果都不是，就放入开表
        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        closeList.Add(start); // 把起点放入闭表

        while (true)
        {
            // 遍历周围的8个点
            //(左上)
            FindNearNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
            //（上）
            FindNearNodeToOpenList(start.x, start.y - 1, 1, start, end);
            //（右上）
            FindNearNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
            //左）
            FindNearNodeToOpenList(start.x - 1, start.y, 1f, start, end);
            //（右）
            FindNearNodeToOpenList(start.x + 1, start.y, 1f, start, end);
            //（左下）
            FindNearNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
            //（下）
            FindNearNodeToOpenList(start.x, start.y + 1, 1f, start, end);
            //（右下）
            FindNearNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);

            // 死路的判断（开表为空 
            if (openList.Count == 0)
            {
                Debug.Log("该点是死路");
                return null;
            }

            // 选出开表的寻路消耗最小的点，  放入闭表中,同时在开表移除
            openList.Sort(SortOpenList);
            // 放入闭表
            closeList.Add(openList[0]);  // 排序后最小的已经在左边了,所以下标是0
                                         // 以最小的这个点作为下一次寻路的起点
            start = openList[0];
            // 删除在开表的这个点
            openList.RemoveAt(0);

            // 如果这个点是终点，返回最终结果
            if (start == end) // 是终点
            {
                // 回溯  (找父对象的结点)
                List<AStarNode> path = new List<AStarNode>();
                path.Add(end);
                while (end.father != null)
                {
                    path.Add(end.father);
                    end = end.father; // 重新改变父对象
                }
                // father为空的时候  证明是起点
                path.Reverse();  // 列表翻转的API ,因为该列表的值是从 尾=>头  翻转后就是 头=>尾
                return path;
            }
        }
    }

    /// <summary>
    /// 排序开表里面的消耗值 (找到最小消耗值的点放入 闭表 用的
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int SortOpenList(AStarNode a, AStarNode b)
    {
        if (a.f > b.f)
            return 1;
        else
            return -1;
    }

    /// <summary>
    /// 把周围点放入 开表 的函数
    /// </summary>
    /// <param name="x">该点在地图的x坐标</param>
    /// <param name="y">该点在地图的y坐标</param>
    /// <param name="father">该节点的父对象</param>
    /// <param name="end">终点的位置</param>
    private void FindNearNodeToOpenList(int x, int y, float g, AStarNode father, AStarNode end)
    {
        // 边界判断
        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
            return;

        // 在地图范围内取点
        AStarNode node = nodes[x, y];

        // 判断周围的点是否是边界 障碍点 和在开表和闭表中，
        if (node == null || node.type == E_Node_Type.Stop || closeList.Contains(node))
            return;

        // 如果节点已经在开放列表中，则检查新的路径是否更优
        if (openList.Contains(node))
        {
            float newG = father.g + g;
            if (newG < node.g)
            {
                node.g = newG;
               
                node.f = node.g + node.h;
                node.father = father;
                return;
            }
            else
            {
                return;
            }

        }

        // 记录该点的父对象 
        node.father = father;

        // 计算寻路消耗 f(寻路消耗值） =  g(离起点的距离)+h（离终点的距离）
        node.g = father.g + g;
        // g =父的g+到自己的消耗值
        node.h = Mathf.Abs(node.x - end.x) + Mathf.Abs(node.y - end.y); //h = 横向距离 + 纵向距离
        node.f = node.g + node.h;

        // 最终把该节点放入 开表
        openList.Add(node);
    }
}

