using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    //���Ͻǵ�һ���������λ��
    public int beginX = -1;
    public int beginY = 5;
    //֮��ôһ���������ƫ��λ��
    public int offsetX = 2;
    public int offsetY = 2;
    //��ͼ���ӵĿ��
    public int mapW = 5;
    public int mapH = 5;
    //��ʼ�����һ��-���������
    private Vector2 beginPos = Vector2.right *-1;
    //�������������
    private Dictionary<string,GameObject> cubes = new Dictionary<string,GameObject>();

    public Material redMaterial; 
    public Material yellowMaterial; 
    public Material greenMaterial;
    public Material normalMaterial;

    List<AStarNode> list;

    // Start is called before the first frame update
    void Start()
    {
        //��ʼ����ͼ
        AStarMgr.Instance.InitiMapInfo(mapW, mapH);
        for (int i = 0; i < mapW; i++)
        {
            for(int j = 0; j < mapH; j++)
            {
                //����������
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);
                //����
                obj.name = i + "_" + j;
                //�洢�����嵽�ֵ�������Ŀ���Ǹ������ȷ������յ����̵�
                cubes.Add(obj.name, obj);

                AStarNode node = AStarMgr.Instance.nodes[i,j];
                //�õ����ӵ�����
                if(node.type == E_Node_Type.Stop)
                {
                    obj.GetComponent<MeshRenderer>().material = redMaterial;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            //�õ����λ�÷���ȥ������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit,1000))
            {
                //��¼��ʼ��  (�õ������������
                    
                if(beginPos ==Vector2.right * -1) //û�����
                {
                    //������һ�ε�·��(��ɫ�������ɰ�ɫ
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = normalMaterial;
                        }
                    }

                    string[] strs = hit.collider.gameObject.name.Split('_');
                    //�õ��������꣨���
                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    //��������������ɻ�ɫ
                    hit.collider.gameObject.GetComponent<MeshRenderer>().material = yellowMaterial;
                }
                else //�����
                {
                    //�õ��յ�
                    string[] strs = hit.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));

                    //Ѱ·
                    list =  AStarMgr.Instance.FindPath(beginPos, endPos);
                    //������·��ʱ���ѵ���Ļ�ɫ����ɰ�ɫ  ,�����һ��
                    cubes[(int)beginPos.x + "_" + (int)beginPos.y].GetComponent<MeshRenderer>().material = normalMaterial;

                    if (list != null)
                    {
                        for(int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = greenMaterial;
                        }
                    }
                    beginPos = Vector2.right * -1;  //����Ϊ�µ����

                }

                //��¼�յ�
            }
        }
    }
}
