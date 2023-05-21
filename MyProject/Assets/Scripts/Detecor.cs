using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// using System.Numerics;
using UnityEngine;

public class Detecor : MonoBehaviour {

    private float detectionRadius = 100f;  // 检测半径：100m

    Ray ray; // 射线对象

    private StreamWriter writer;
    private string path; // 日志路径


    // Use this for initialization
    void Start () {
        path = Application.dataPath + "/log.txt"; // 日志路径
        writer = new StreamWriter(path, true);
        Log("LogToFile started.");
    }
	
	// Update is called once per frame
	void Update () {
        // 射线检测
        // 定义起点和方向
        //ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.forward); // (起点, 方向)
        //RaycastHit hitInfo; // 射线检测的信息
        //if (Physics.Raycast(ray, out hitInfo, detectionRadius)) // 最大检测距离100m
        //{
        //    GameObject gameObject = hitInfo.collider.gameObject;
        //    if (gameObject != null)
        //    {
        //        char[] tempName = gameObject.name.ToCharArray();
        //        if (gameObject.name != "Plane" && !(tempName[0] >= '0' && tempName[0] <= '9'))
        //        {
        //            if (gameObject.transform.GetComponent<Renderer>() != null)
        //            {
        //                Debug.Log("检测到建筑物：" + hitInfo.collider.gameObject.name);
        //                var size = hitInfo.collider.gameObject.transform.GetComponent<Renderer>().bounds.size;
        //                Debug.Log("坐标为" + "x: " + size.x + ",y: " + size.y + ",z:" + size.z);
        //                Debug.DrawLine(transform.position, hitInfo.point, Color.yellow); // 绘制出检测线
        //            }
        //        }
        //    }

        //}



        // 获取车辆的位置
        Vector3 vehiclePosition = transform.position;

        // 使用OverlapSphere函数创建一个球形区域，并返回球形区域内与车辆碰撞的Collider组件数组
        Collider[] colliders = Physics.OverlapSphere(vehiclePosition, detectionRadius);

        // 遍历碰撞器数组
        foreach (Collider collider in colliders)
        {
            GameObject gameObject = collider.gameObject;
            if (gameObject != null)
            {
                char[] tempName = gameObject.name.ToCharArray();
                if (gameObject.name != "Plane") // 检测建筑物
                {
                    if (!(tempName[0] >= '0' && tempName[0] <= '9'))
                    {
                        Vector3 length = gameObject.GetComponent<BoxCollider>().size;
                        float xlength = length.x;
                        float ylength = length.y;
                        float zlength = length.z;
                        Debug.Log("检测到建筑物: " + gameObject.name + ". 长：" + xlength + ", 宽：" + zlength + ", 高：" + ylength);
                        Log("检测到建筑物: " + gameObject.name + ". 长：" + xlength + ", 宽：" + zlength + ", 高：" + ylength);

                        string str = new string(tempName, 0, 3);
                        int type = 0;
                        if (str == "重庆大")
                        {
                            type = 0;
                        }
                        else if (str == "富力城") {
                            type = 1;
                        }
                        // 反射点数量
                        double nums_VRS = V2XModel.NumsOfVRS((double)ylength, type);
                        Debug.Log("检测到建筑物: " + gameObject.name + ".反射点数量为：" + nums_VRS);
                        // 复信号衰落
                        V2XModel.H_NLOS(nums_VRS, type);

                    }
                    else if (gameObject.name != "")
                    {
                        GameObject parent = null;
                        try
                        {
                            parent = gameObject.transform.parent.gameObject;
                        }
                        catch
                        { }

                        if (parent != null)  // 检测路载单元
                        {
                            char[] parentName = parent.name.ToCharArray();
                            if (parentName[0] >= '0' && parentName[0] <= '9')
                            {
                                MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
                                Vector3 length;
                                if (meshFilter != null) // 组件存在，可以使用
                                {
                                    length = meshFilter.mesh.bounds.size;
                                }
                                else // 组件不存在，使用Box Collider检测
                                {
                                    length = gameObject.GetComponent<BoxCollider>().size;
                                }
                                float xlength = length.x;
                                float ylength = length.y;
                                float zlength = length.z;
                                // Debug.Log("检测到交通灯: " + parent.name + "-" + gameObject.name + ". 长：" + xlength + ", 宽：" + ylength + ", 高：" + zlength);
                                Log("检测到交通灯: " + parent.name + "-" + gameObject.name + ". 长：" + xlength + ", 宽：" + ylength + ", 高：" + zlength); // 交通灯和建筑物模型的坐标轴不一样，交通灯纵轴为z，建筑物为y
                            }
                        }
                        if (gameObject.name != "0") // 检测车辆
                        {
                            // 使用Collider组件的bounds属性来获取车辆的长、宽和高
                            // Vector3 = gameObject.GetComponent<BoxCollider>().size
                            Vector3 position =  gameObject.transform.position;
                           // Vector3 carSize = collider.bounds.size;
                            Debug.Log("检测到车辆: car-" + gameObject.name + ". 它的坐标为: " + position);
                            //Debug.Log("检测到车辆: car-" + gameObject.name + ". 它的坐标为: " + position + ". 尺寸为:" + carSize);
                        }
                    }
                }
            }        
        }

        
    }

    /**
     * 绘制出检测域
     */
    void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        Gizmos.color = new Color(1f, 0f, 0f, 0.1f);
        Gizmos.DrawSphere(transform.position, detectionRadius);
    }

    /**
     * 退出关闭流
     */
    void OnApplicationQuit()
    {
        Log("LogToFile end.");
        writer.Close();
    }

    /**
     * 写入日志
     */
    void Log(string message)
    {
        writer.WriteLine("[" + System.DateTime.Now + "]" + message);
    }
}
