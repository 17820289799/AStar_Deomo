using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    //鼠标左键是否按下
    private bool isMouseDown = false;
    private LineRenderer line;
    //鼠标按下记录当前鼠标位置
    private Vector3 leftUpPoint;
    private Vector3 rightDownPoint;
    private Vector3 rightUpPoint;
    private Vector3 leftDownPoint;

    private RaycastHit hitInfo;
    private Vector3 beginWorldPos;
    //所有选择到的士兵对象，存储在该容器
    private List<SoldierObj> soldierObjs = new List<SoldierObj>();
    //上一次鼠标点击的位置
    private Vector3 frontPos = Vector3.zero;
    //士兵的间隔距离
    private float soldierOffset = 3f;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //选中士兵
        SelSoldiers();
        //控制士兵移动
        ControlSoldiersMove();
    }

    private void SelSoldiers()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //记录鼠标当前的位置
            leftUpPoint = Input.mousePosition;  //(左上角的点
            isMouseDown = true;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //通过射线检测，得到点击后 在该地面上的点  (用于范围检测
                beginWorldPos = hitInfo.point;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            //将线段点设置为0 相当于不绘制线段   取消绘制
            line.positionCount = 0;
            //每一次重新选择士兵，把记录的上一个位置清空
            frontPos = Vector3.zero;

            //清空上一次的选择 到的物体对象 ，防止保留上一次的选中记录
            for (int i = 0; i < soldierObjs.Count; i++)
            {
                soldierObjs[i].SetSelSelf(false);  //光圈消失
            }
            soldierObjs.Clear();

            //通过射线检测，得到松开鼠标后最后所在的点
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //范围的中心点   (在摄像机没有旋转的情况下可以这样写
                Vector3 center = new Vector3((hitInfo.point.x + beginWorldPos.x) / 2,
                    1, (hitInfo.point.z + beginWorldPos.z) / 2);
                //盒装范围检测的  长宽高的一半
                Vector3 halfExtents = new Vector3(Mathf.Abs(hitInfo.point.x - beginWorldPos.x) / 2, 1,
                    Mathf.Abs(hitInfo.point.z - beginWorldPos.z) / 2);

                //盒状范围检测  (得到范围内所有士兵的碰撞器
                Collider[] colliders = Physics.OverlapBox(center, halfExtents);
                //最多只能选择12个士兵
                for (int i = 0; i < colliders.Length && soldierObjs.Count < 12; i++)
                {
                    SoldierObj obj = colliders[i].GetComponent<SoldierObj>();
                    if (obj != null)
                    {
                        obj.SetSelSelf(true);  //亮起脚底光圈
                        soldierObjs.Add(obj);
                    }
                }

                //给士兵分类排序(站位的问题
                soldierObjs.Sort((a,b) =>
                {
                    if (a.type < b.type)
                    {  //类型小的在前面（比如英雄和战士
                        return -1;
                    }
                    else if(a.type == b.type) 
                    { 
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            }
        }
        //处理线款绘制的逻辑
        if (isMouseDown)
        {

            leftUpPoint.z = 5;
            //屏幕坐标系的位置  
            //（右下角的点
            rightDownPoint = Input.mousePosition;
            rightDownPoint.z = 5;

            //右上角的点
            rightUpPoint = new Vector3(rightDownPoint.x, leftUpPoint.y, 5);

            //左下角的点
            leftDownPoint = new Vector3(leftUpPoint.x, rightDownPoint.y, 5);

            //绘制4个世界坐标系的点
            line.positionCount = 4;
            line.SetPosition(0, Camera.main.ScreenToWorldPoint(leftUpPoint));
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(rightUpPoint));
            line.SetPosition(2, Camera.main.ScreenToWorldPoint(rightDownPoint));
            line.SetPosition(3, Camera.main.ScreenToWorldPoint(leftDownPoint));


        }
    }

    /// <summary>
    /// 控制士兵移动
    /// </summary>
/*    private void ControlSoldiersMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //没有士兵，就不移动
            if(soldierObjs.Count == 0)
                return;

            //获取 鼠标右键点击的 目标点

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //判断队伍新朝向（点击的下一个目标点） 和 队伍老朝向(之前阵型第一个士兵的面朝向)之间的夹角
                if(Vector3.Angle((hitInfo.point - soldierObjs[0].transform.position).normalized,
                    soldierObjs[0].transform.forward) > 60) //它们之间的夹角是否大于60°
                {
                    //Debug.Log("大于60度了！！");
                    //重新进行排序  （先兵种排序，后距离排序
                    soldierObjs.Sort((a, b) =>
                    {
                        if (a.type < b.type)
                        {
                            //类型小的在前面（比如英雄和战士
                            return -1;
                        }
                        else if (a.type == b.type)  //兵种相同，用距离排序
                        { 
                            if(Vector3.Distance(a.transform.position,hitInfo.point) <=
                            Vector3.Distance(b.transform.position, hitInfo.point))
                                return -1;
                            else
                                return 1;
                        }

                        else
                            return 1;
                    });


                }


                //计算每个士兵的目标点(在阵型对应的位置
                List<Vector3> targetsPosList = GetTargetPos(hitInfo.point);
                //命令士兵向着阵型的目标点移动
                for (int i = 0; i < soldierObjs.Count; i++)
                {
                    //soldierObjs[i].Move(hitInfo.point);
                    //通过目标点，分析阵型目标
                    soldierObjs[i].Move(targetsPosList[i]);
                }
            }
        }

    }*/
    private void ControlSoldiersMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //没有士兵，就不移动
            if (soldierObjs.Count == 0)
                return;

            //获取 鼠标右键点击的 目标点
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //判断队伍新朝向（点击的下一个目标点） 和 队伍老朝向(之前阵型第一个士兵的面朝向)之间的夹角
                if (Vector3.Angle((hitInfo.point - soldierObjs[0].transform.position).normalized,
                    soldierObjs[0].transform.forward) > 60) //它们之间的夹角是否大于60°
                {
                    //重新进行排序  （先兵种排序，后距离排序
                    soldierObjs.Sort((a, b) =>
                    {
                        if (a.type < b.type)
                        {
                            //类型小的在前面（比如英雄和战士
                            return -1;
                        }
                        else if (a.type == b.type)  //兵种相同，用距离排序
                        {
                            if (Vector3.Distance(a.transform.position, hitInfo.point) <=
                            Vector3.Distance(b.transform.position, hitInfo.point))
                                return -1;
                            else
                                return 1;
                        }
                        else
                            return 1;
                    });
                }

                //计算每个士兵的目标点(在阵型对应的位置
                List<Vector3> targetsPosList = GetTargetPos(hitInfo.point);
                //命令士兵向着阵型的目标点移动
                for (int i = 0; i < soldierObjs.Count; i++)
                {
                    float distance = Vector3.Distance(soldierObjs[i].transform.position, hitInfo.point);
                    // 如果士兵与目标点之间的距离大于一个小的阈值，则移动
                    if (distance > 0.1f)
                    {
                        soldierObjs[i].Move(targetsPosList[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 根据鼠标点击的目标点，计算出 阵型的 点位
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private List<Vector3> GetTargetPos(Vector3 targetPos)
    {
        Vector3 nowForward = Vector3.zero;
        Vector3 nowRight = Vector3.zero;
        //表示上一次已经点击过了位置，移动过士兵了
        if(frontPos != Vector3.zero)
        {
            //有上一次的点，就直接求两个点的向量
            nowForward = (targetPos - frontPos).normalized;
        }
        else  //没有上一次的点（d第一次移动士兵） 就用 第一个士兵的点 进行 向量计算
        {
            nowForward = (targetPos - soldierObjs[0].transform.position).normalized;
        }
        //根据面朝向得到 右朝向 旋转y轴90°
        nowRight = Quaternion.Euler(0, 90, 0) * nowForward;
        //多个士兵的位置容器
        List<Vector3> targetsPos = new List<Vector3>();
        //根据士兵的数量，进行不同的阵型分列
        switch (soldierObjs.Count)
        {
            case 1:
                targetsPos.Add(targetPos);
                break;
            case 2://两个士兵的间隔为soldierOffset ，也就是分别距离中心的距离一样S
                targetsPos.Add(targetPos + nowRight * (soldierOffset / 2));  
                targetsPos.Add(targetPos - nowRight * (soldierOffset / 2));
                break;
            case 3:
                targetsPos.Add(targetPos);
                targetsPos.Add(targetPos + nowRight * (soldierOffset));
                targetsPos.Add(targetPos - nowRight * (soldierOffset));
                break;
            case 4:
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 - nowRight * (soldierOffset / 2)); //左上的点
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 + nowRight * (soldierOffset / 2)); //右上的点
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 - nowRight * (soldierOffset / 2)); //左下的点
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 + nowRight * (soldierOffset / 2)); //右下的点
                break;
            case 5:
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2); //上中
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 - nowRight * soldierOffset); //左上的点
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 + nowRight * soldierOffset); //右上的点
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 - nowRight * soldierOffset); //左下的点
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 + nowRight * soldierOffset); //右下的点
                break;
            case 6:
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2);
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 - nowRight * soldierOffset); 
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 + nowRight * soldierOffset); 
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 - nowRight * soldierOffset); 
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 + nowRight * soldierOffset); 
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2); 
                break;
            case 7:
                targetsPos.Add(targetPos + nowForward * soldierOffset);
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset);

                targetsPos.Add(targetPos);
                targetsPos.Add(targetPos - nowRight * soldierOffset);
                targetsPos.Add(targetPos + nowRight * soldierOffset);

                targetsPos.Add(targetPos - nowForward *soldierOffset);
                break;
            case 8:
                targetsPos.Add(targetPos + nowForward * soldierOffset);
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset);

                targetsPos.Add(targetPos);
                targetsPos.Add(targetPos - nowRight * soldierOffset);
                targetsPos.Add(targetPos + nowRight * soldierOffset);

                targetsPos.Add(targetPos - nowForward * soldierOffset - nowRight*soldierOffset);
                targetsPos.Add(targetPos - nowForward * soldierOffset + nowRight*soldierOffset);

                break;
            case 9:
                targetsPos.Add(targetPos + nowForward * soldierOffset);
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset);

                targetsPos.Add(targetPos);
                targetsPos.Add(targetPos - nowRight * soldierOffset);
                targetsPos.Add(targetPos + nowRight * soldierOffset);

                targetsPos.Add(targetPos - nowForward*soldierOffset);
                targetsPos.Add(targetPos - nowForward * soldierOffset - nowRight * soldierOffset);
                targetsPos.Add(targetPos - nowForward * soldierOffset + nowRight * soldierOffset);
                break;
            case 10:
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset /2);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset /2);
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset*1.5f);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset*1.5f);

                targetsPos.Add(targetPos - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos + nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowRight * soldierOffset / 2);

                targetsPos.Add(targetPos - nowForward * soldierOffset - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowForward * soldierOffset + nowRight * soldierOffset * 1.5f);
                break;
            case 11:
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset * 1.5f);

                targetsPos.Add(targetPos - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos + nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowRight * soldierOffset / 2);

                targetsPos.Add(targetPos - nowForward * soldierOffset - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowForward * soldierOffset + nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowForward * soldierOffset);
                break;
            case 12:
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowForward * soldierOffset - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos + nowForward * soldierOffset + nowRight * soldierOffset * 1.5f);

                targetsPos.Add(targetPos - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos + nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos + nowRight * soldierOffset / 2);

                targetsPos.Add(targetPos - nowForward * soldierOffset - nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowForward * soldierOffset + nowRight * soldierOffset * 1.5f);
                targetsPos.Add(targetPos - nowForward * soldierOffset - nowRight * soldierOffset / 2);
                targetsPos.Add(targetPos - nowForward * soldierOffset + nowRight * soldierOffset / 2);

                break;

        }

        //计算完毕后，记录当前的位置
        frontPos = targetPos;
        //取出所有目标点
        return targetsPos;
    }
}
