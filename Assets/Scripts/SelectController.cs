using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectController : MonoBehaviour
{
    //�������Ƿ���
    private bool isMouseDown = false;
    private LineRenderer line;
    //��갴�¼�¼��ǰ���λ��
    private Vector3 leftUpPoint;
    private Vector3 rightDownPoint;
    private Vector3 rightUpPoint;
    private Vector3 leftDownPoint;

    private RaycastHit hitInfo;
    private Vector3 beginWorldPos;
    //����ѡ�񵽵�ʿ�����󣬴洢�ڸ�����
    private List<SoldierObj> soldierObjs = new List<SoldierObj>();
    //��һ���������λ��
    private Vector3 frontPos = Vector3.zero;
    //ʿ���ļ������
    private float soldierOffset = 3f;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //ѡ��ʿ��
        SelSoldiers();
        //����ʿ���ƶ�
        ControlSoldiersMove();
    }

    private void SelSoldiers()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //��¼��굱ǰ��λ��
            leftUpPoint = Input.mousePosition;  //(���Ͻǵĵ�
            isMouseDown = true;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //ͨ�����߼�⣬�õ������ �ڸõ����ϵĵ�  (���ڷ�Χ���
                beginWorldPos = hitInfo.point;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            //���߶ε�����Ϊ0 �൱�ڲ������߶�   ȡ������
            line.positionCount = 0;
            //ÿһ������ѡ��ʿ�����Ѽ�¼����һ��λ�����
            frontPos = Vector3.zero;

            //�����һ�ε�ѡ�� ����������� ����ֹ������һ�ε�ѡ�м�¼
            for (int i = 0; i < soldierObjs.Count; i++)
            {
                soldierObjs[i].SetSelSelf(false);  //��Ȧ��ʧ
            }
            soldierObjs.Clear();

            //ͨ�����߼�⣬�õ��ɿ�����������ڵĵ�
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //��Χ�����ĵ�   (�������û����ת������¿�������д
                Vector3 center = new Vector3((hitInfo.point.x + beginWorldPos.x) / 2,
                    1, (hitInfo.point.z + beginWorldPos.z) / 2);
                //��װ��Χ����  ����ߵ�һ��
                Vector3 halfExtents = new Vector3(Mathf.Abs(hitInfo.point.x - beginWorldPos.x) / 2, 1,
                    Mathf.Abs(hitInfo.point.z - beginWorldPos.z) / 2);

                //��״��Χ���  (�õ���Χ������ʿ������ײ��
                Collider[] colliders = Physics.OverlapBox(center, halfExtents);
                //���ֻ��ѡ��12��ʿ��
                for (int i = 0; i < colliders.Length && soldierObjs.Count < 12; i++)
                {
                    SoldierObj obj = colliders[i].GetComponent<SoldierObj>();
                    if (obj != null)
                    {
                        obj.SetSelSelf(true);  //����ŵ׹�Ȧ
                        soldierObjs.Add(obj);
                    }
                }

                //��ʿ����������(վλ������
                soldierObjs.Sort((a,b) =>
                {
                    if (a.type < b.type)
                    {  //����С����ǰ�棨����Ӣ�ۺ�սʿ
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
        //�����߿���Ƶ��߼�
        if (isMouseDown)
        {

            leftUpPoint.z = 5;
            //��Ļ����ϵ��λ��  
            //�����½ǵĵ�
            rightDownPoint = Input.mousePosition;
            rightDownPoint.z = 5;

            //���Ͻǵĵ�
            rightUpPoint = new Vector3(rightDownPoint.x, leftUpPoint.y, 5);

            //���½ǵĵ�
            leftDownPoint = new Vector3(leftUpPoint.x, rightDownPoint.y, 5);

            //����4����������ϵ�ĵ�
            line.positionCount = 4;
            line.SetPosition(0, Camera.main.ScreenToWorldPoint(leftUpPoint));
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(rightUpPoint));
            line.SetPosition(2, Camera.main.ScreenToWorldPoint(rightDownPoint));
            line.SetPosition(3, Camera.main.ScreenToWorldPoint(leftDownPoint));


        }
    }

    /// <summary>
    /// ����ʿ���ƶ�
    /// </summary>
/*    private void ControlSoldiersMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //û��ʿ�����Ͳ��ƶ�
            if(soldierObjs.Count == 0)
                return;

            //��ȡ ����Ҽ������ Ŀ���

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //�ж϶����³��򣨵������һ��Ŀ��㣩 �� �����ϳ���(֮ǰ���͵�һ��ʿ�����泯��)֮��ļн�
                if(Vector3.Angle((hitInfo.point - soldierObjs[0].transform.position).normalized,
                    soldierObjs[0].transform.forward) > 60) //����֮��ļн��Ƿ����60��
                {
                    //Debug.Log("����60���ˣ���");
                    //���½�������  ���ȱ������򣬺��������
                    soldierObjs.Sort((a, b) =>
                    {
                        if (a.type < b.type)
                        {
                            //����С����ǰ�棨����Ӣ�ۺ�սʿ
                            return -1;
                        }
                        else if (a.type == b.type)  //������ͬ���þ�������
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


                //����ÿ��ʿ����Ŀ���(�����Ͷ�Ӧ��λ��
                List<Vector3> targetsPosList = GetTargetPos(hitInfo.point);
                //����ʿ���������͵�Ŀ����ƶ�
                for (int i = 0; i < soldierObjs.Count; i++)
                {
                    //soldierObjs[i].Move(hitInfo.point);
                    //ͨ��Ŀ��㣬��������Ŀ��
                    soldierObjs[i].Move(targetsPosList[i]);
                }
            }
        }

    }*/
    private void ControlSoldiersMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            //û��ʿ�����Ͳ��ƶ�
            if (soldierObjs.Count == 0)
                return;

            //��ȡ ����Ҽ������ Ŀ���
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1000,
                1 << LayerMask.NameToLayer("Ground")))
            {
                //�ж϶����³��򣨵������һ��Ŀ��㣩 �� �����ϳ���(֮ǰ���͵�һ��ʿ�����泯��)֮��ļн�
                if (Vector3.Angle((hitInfo.point - soldierObjs[0].transform.position).normalized,
                    soldierObjs[0].transform.forward) > 60) //����֮��ļн��Ƿ����60��
                {
                    //���½�������  ���ȱ������򣬺��������
                    soldierObjs.Sort((a, b) =>
                    {
                        if (a.type < b.type)
                        {
                            //����С����ǰ�棨����Ӣ�ۺ�սʿ
                            return -1;
                        }
                        else if (a.type == b.type)  //������ͬ���þ�������
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

                //����ÿ��ʿ����Ŀ���(�����Ͷ�Ӧ��λ��
                List<Vector3> targetsPosList = GetTargetPos(hitInfo.point);
                //����ʿ���������͵�Ŀ����ƶ�
                for (int i = 0; i < soldierObjs.Count; i++)
                {
                    float distance = Vector3.Distance(soldierObjs[i].transform.position, hitInfo.point);
                    // ���ʿ����Ŀ���֮��ľ������һ��С����ֵ�����ƶ�
                    if (distance > 0.1f)
                    {
                        soldierObjs[i].Move(targetsPosList[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �����������Ŀ��㣬����� ���͵� ��λ
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private List<Vector3> GetTargetPos(Vector3 targetPos)
    {
        Vector3 nowForward = Vector3.zero;
        Vector3 nowRight = Vector3.zero;
        //��ʾ��һ���Ѿ��������λ�ã��ƶ���ʿ����
        if(frontPos != Vector3.zero)
        {
            //����һ�εĵ㣬��ֱ���������������
            nowForward = (targetPos - frontPos).normalized;
        }
        else  //û����һ�εĵ㣨d��һ���ƶ�ʿ���� ���� ��һ��ʿ���ĵ� ���� ��������
        {
            nowForward = (targetPos - soldierObjs[0].transform.position).normalized;
        }
        //�����泯��õ� �ҳ��� ��תy��90��
        nowRight = Quaternion.Euler(0, 90, 0) * nowForward;
        //���ʿ����λ������
        List<Vector3> targetsPos = new List<Vector3>();
        //����ʿ�������������в�ͬ�����ͷ���
        switch (soldierObjs.Count)
        {
            case 1:
                targetsPos.Add(targetPos);
                break;
            case 2://����ʿ���ļ��ΪsoldierOffset ��Ҳ���Ƿֱ�������ĵľ���һ��S
                targetsPos.Add(targetPos + nowRight * (soldierOffset / 2));  
                targetsPos.Add(targetPos - nowRight * (soldierOffset / 2));
                break;
            case 3:
                targetsPos.Add(targetPos);
                targetsPos.Add(targetPos + nowRight * (soldierOffset));
                targetsPos.Add(targetPos - nowRight * (soldierOffset));
                break;
            case 4:
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 - nowRight * (soldierOffset / 2)); //���ϵĵ�
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 + nowRight * (soldierOffset / 2)); //���ϵĵ�
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 - nowRight * (soldierOffset / 2)); //���µĵ�
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 + nowRight * (soldierOffset / 2)); //���µĵ�
                break;
            case 5:
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2); //����
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 - nowRight * soldierOffset); //���ϵĵ�
                targetsPos.Add(targetPos + nowForward * soldierOffset / 2 + nowRight * soldierOffset); //���ϵĵ�
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 - nowRight * soldierOffset); //���µĵ�
                targetsPos.Add(targetPos - nowForward * soldierOffset / 2 + nowRight * soldierOffset); //���µĵ�
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

        //������Ϻ󣬼�¼��ǰ��λ��
        frontPos = targetPos;
        //ȡ������Ŀ���
        return targetsPos;
    }
}
