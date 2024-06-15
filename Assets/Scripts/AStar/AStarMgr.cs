using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarMgr
{
    // ��ͼ�Ŀ�͸�
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

    // ��ͼ���и��ӵĶ�������
    public AStarNode[,] nodes;
    // �����б�
    private List<AStarNode> openList = new List<AStarNode>();
    // �ر��б�
    private List<AStarNode> closeList = new List<AStarNode>();

    /// <summary>
    /// ��ʼ����ͼ��Ϣ��һ��ʼ������Ϸ����ֵ��ȥ���߸õ�ͼ�ж��
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitiMapInfo(int w, int h)
    {
        // ��¼���
        this.mapW = w;
        this.mapH = h;
        // ��������һ������װ���ٸ�����
        nodes = new AStarNode[w, h];

        // ���ݿ�ߴ�����ͼ�ĸ���
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                // �����ͼ���ϰ���(ʵ����ͨ����ͼ���ݵ����  �����ϰ����
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
                nodes[i, j] = node;
            }
        }
    }

    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        // �����жϴ�����������Ƿ�Ϸ����Ƿ��ǵ�ͼ��Χ��,�ǲ����赲�ĵ� ,���Ϸ���Ҫ����null
        // 1���ж��Ƿ��ڵ�ͼ��Χ��
        if (startPos.x < 0 || startPos.x >= mapW || startPos.y < 0 || startPos.y >= mapH ||
            endPos.x < 0 || endPos.x >= mapW || endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("�������յ��ڵ�ͼ��Χ��");
            return null;
        }

        // 2���ж������յ��ǲ����ϰ���
        AStarNode start = nodes[(int)startPos.x, (int)startPos.y];
        AStarNode end = nodes[(int)endPos.x, (int)endPos.y];
        if (start.type == E_Node_Type.Stop || end.type == E_Node_Type.Stop)
        {
            Debug.Log("�������յ����ϰ�����");
            return null;
        }

        // Ѱ·ǰ����տ���ͱհ���Ϊ����һ��Ѱ·
        closeList.Clear();
        openList.Clear();

        // �Ϸ� (�õ������յ�ĸ���
        // ����㿪ʼ����Χ�� 8���� ���뿪��(��һ���ж���Χ�ĵ��Ƿ��Ǳ߽����ǽ���ϰ� ��Ҫ�ж��Ƿ��ڿ��ձ����� ,��������ǣ��ͷ��뿪��
        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        closeList.Add(start); // ��������ձ�

        while (true)
        {
            // ������Χ��8����
            //(����)
            FindNearNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
            //���ϣ�
            FindNearNodeToOpenList(start.x, start.y - 1, 1, start, end);
            //�����ϣ�
            FindNearNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
            //��
            FindNearNodeToOpenList(start.x - 1, start.y, 1f, start, end);
            //���ң�
            FindNearNodeToOpenList(start.x + 1, start.y, 1f, start, end);
            //�����£�
            FindNearNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
            //���£�
            FindNearNodeToOpenList(start.x, start.y + 1, 1f, start, end);
            //�����£�
            FindNearNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);

            // ��·���жϣ�����Ϊ�� 
            if (openList.Count == 0)
            {
                Debug.Log("�õ�����·");
                return null;
            }

            // ѡ�������Ѱ·������С�ĵ㣬  ����ձ���,ͬʱ�ڿ����Ƴ�
            openList.Sort(SortOpenList);
            // ����ձ�
            closeList.Add(openList[0]);  // �������С���Ѿ��������,�����±���0
                                         // ����С���������Ϊ��һ��Ѱ·�����
            start = openList[0];
            // ɾ���ڿ���������
            openList.RemoveAt(0);

            // �����������յ㣬�������ս��
            if (start == end) // ���յ�
            {
                // ����  (�Ҹ�����Ľ��)
                List<AStarNode> path = new List<AStarNode>();
                path.Add(end);
                while (end.father != null)
                {
                    path.Add(end.father);
                    end = end.father; // ���¸ı丸����
                }
                // fatherΪ�յ�ʱ��  ֤�������
                path.Reverse();  // �б�ת��API ,��Ϊ���б��ֵ�Ǵ� β=>ͷ  ��ת����� ͷ=>β
                return path;
            }
        }
    }

    /// <summary>
    /// ���򿪱����������ֵ (�ҵ���С����ֵ�ĵ���� �ձ� �õ�
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
    /// ����Χ����� ���� �ĺ���
    /// </summary>
    /// <param name="x">�õ��ڵ�ͼ��x����</param>
    /// <param name="y">�õ��ڵ�ͼ��y����</param>
    /// <param name="father">�ýڵ�ĸ�����</param>
    /// <param name="end">�յ��λ��</param>
    private void FindNearNodeToOpenList(int x, int y, float g, AStarNode father, AStarNode end)
    {
        // �߽��ж�
        if (x < 0 || x >= mapW || y < 0 || y >= mapH)
            return;

        // �ڵ�ͼ��Χ��ȡ��
        AStarNode node = nodes[x, y];

        // �ж���Χ�ĵ��Ƿ��Ǳ߽� �ϰ��� ���ڿ���ͱձ��У�
        if (node == null || node.type == E_Node_Type.Stop || closeList.Contains(node))
            return;

        // ����ڵ��Ѿ��ڿ����б��У������µ�·���Ƿ����
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

        // ��¼�õ�ĸ����� 
        node.father = father;

        // ����Ѱ·���� f(Ѱ·����ֵ�� =  g(�����ľ���)+h�����յ�ľ��룩
        node.g = father.g + g;
        // g =����g+���Լ�������ֵ
        node.h = Mathf.Abs(node.x - end.x) + Mathf.Abs(node.y - end.y); //h = ������� + �������
        node.f = node.g + node.h;

        // ���հѸýڵ���� ����
        openList.Add(node);
    }
}

