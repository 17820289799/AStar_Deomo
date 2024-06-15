using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    //左上角第一个立方体的位置
    public int beginX = -1;
    public int beginY = 5;
    //之后么一个立方体的偏移位置
    public int offsetX = 2;
    public int offsetY = 2;
    //地图格子的宽高
    public int mapW = 5;
    public int mapH = 5;
    //开始点给他一个-负的坐标点
    private Vector2 beginPos = Vector2.right *-1;
    //用来存放立方体
    private Dictionary<string,GameObject> cubes = new Dictionary<string,GameObject>();

    public Material redMaterial; 
    public Material yellowMaterial; 
    public Material greenMaterial;
    public Material normalMaterial;

    List<AStarNode> list;

    // Start is called before the first frame update
    void Start()
    {
        //初始化地图
        AStarMgr.Instance.InitiMapInfo(mapW, mapH);
        for (int i = 0; i < mapW; i++)
        {
            for(int j = 0; j < mapH; j++)
            {
                //创建立方体
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);
                //名字
                obj.name = i + "_" + j;
                //存储立方体到字典容器（目的是给鼠标点击确定起点终点做铺垫
                cubes.Add(obj.name, obj);

                AStarNode node = AStarMgr.Instance.nodes[i,j];
                //得到格子的类型
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
            //得到鼠标位置发出去的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out hit,1000))
            {
                //记录开始点  (得到点击的立方体
                    
                if(beginPos ==Vector2.right * -1) //没有起点
                {
                    //清理上一次的路径(绿色立方体变成白色
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = normalMaterial;
                        }
                    }

                    string[] strs = hit.collider.gameObject.name.Split('_');
                    //得到行列坐标（起点
                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    //点击到的立方体变成黄色
                    hit.collider.gameObject.GetComponent<MeshRenderer>().material = yellowMaterial;
                }
                else //有起点
                {
                    //得到终点
                    string[] strs = hit.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));

                    //寻路
                    list =  AStarMgr.Instance.FindPath(beginPos, endPos);
                    //避免死路的时候，已点击的黄色不变成白色  ,先清除一次
                    cubes[(int)beginPos.x + "_" + (int)beginPos.y].GetComponent<MeshRenderer>().material = normalMaterial;

                    if (list != null)
                    {
                        for(int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = greenMaterial;
                        }
                    }
                    beginPos = Vector2.right * -1;  //设置为新的起点

                }

                //记录终点
            }
        }
    }
}
