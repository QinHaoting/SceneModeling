using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodingConnected.TraCI.NET;
using System;

public class Traci : MonoBehaviour {

	public TraCIClient client; // TraCI客户端

	public GameObject egoVehicle; // 主车

    // 辅车
	public GameObject sedanVehicle; // 普通车
    public GameObject policeVehicle; // 警车
    public GameObject busVehicle; // 大车
    public GameObject suvVehicle; // 城市越野

    /**
	 * 存场景内所有车辆的ID
	 */
    public List<String> vehicleIDList;
	/**
	 * 存场景内所有车辆
	 */
	public List<GameObject> carList;

    // 交通灯
    public Light ttLight; // 交通灯源
    public GameObject tLight; // 交通灯对象
    /**
     * 存场景内所有交通灯的ID
     */
    private List<string> tlightids;

    public Dictionary<string, List<trafficLights>> dicti;


    // Use this for initialization
    void Start () {
		// 建立连接
		client = new TraCIClient();
		client.Connect("127.0.0.1", 4001);
		Console.WriteLine("建立连接");

        // SUMO中所有交通灯
        tlightids = client.TrafficLight.GetIdList().Content;

        // 设置SUMO主车
        client.Gui.TrackVehicle("View #0", "0");
		client.Gui.SetZoom("View #0", 1200);

        // 设置红绿灯
        createTLS();
        
		// 加载车辆
        client.Control.SimStep();
        client.Control.SimStep();


        // 初始化车辆
        client.Vehicle.SetSpeed("0", 0);
        // 设置初始位置
        var shape = client.Vehicle.GetPosition("0").Content;
        var angle = client.Vehicle.GetAngle("0").Content;
        egoVehicle.transform.position = new Vector3((float)shape.X, 0.1f, (float)shape.Y);
        egoVehicle.transform.rotation = Quaternion.Euler(0, (float)angle, 0);

        carList.Add(egoVehicle);
    }


    private void FixedUpdate()
	{
		var newVehicles = client.Simulation.GetDepartedIDList("0").Content; // 当前的新车列表
		var vehiclesLeft = client.Simulation.GetArrivedIDList("0").Content; // 已失效的车辆列表

        // 检查车辆是否还在场景内，不在场景内的车直接移除
        for (int j = 0; j < vehiclesLeft.Count; j++)
        {
            GameObject toremove = GameObject.Find(vehiclesLeft[j]);
            if (toremove)
            {
                carList.Remove(toremove);
                Destroy(toremove);
            }
        }


		// 更新主车位置信息
        var road = client.Vehicle.GetRoadID(egoVehicle.name).Content;
		var lane = client.Vehicle.GetLaneIndex(egoVehicle.name).Content;
        /*
         * Updates the ego-vehicle's position in the SUMO scene
         * @params id: ego-vehicle's name in the SUMO simulation
         * @params road: current edge the vehicle is on in the SUMO simulation
         * @params lane: current lane number the ego vehicle is on
         * @params xPosition: X-axis position of the ego-vehicle in the Unity scene
         * @params yPosition: Z-axis position of the ego-vehicle in the Unity scene
         * @params angle: The angle that the ego vehicle is facing at
         * @params keepRoute: maps the ego-vehicle to the exact X-Y position in the SUMO simulation
         */
        client.Vehicle.MoveToXY("0", road, lane,
			(double)egoVehicle.transform.position.x,
			(double)egoVehicle.transform.position.z, // unity中竖轴为y, 纵轴为z
            (double)egoVehicle.transform.eulerAngles.y,
            2);
		// print("主车位置，x：" + egoVehicle.transform.position.x + ", y:" + egoVehicle.transform.position.y +  ", z:" + egoVehicle.transform.position.z);

        // 初始化辅车
        for (int carid = 1; carid < carList.Count; carid++)
        {
            var carPosition = client.Vehicle.GetPosition(carList[carid].name).Content; //gets position of NPC vehicle
            carList[carid].transform.position = new Vector3((float)carPosition.X, 0.59f, (float)carPosition.Y);
            var carAngle = client.Vehicle.GetAngle(carList[carid].name).Content; //gets angle of NPC vehicle
            carList[carid].transform.rotation = Quaternion.Euler(0f, (float)carAngle, 0f);

        }

        // 更新辅车位置
        for (int i = 0; i < newVehicles.Count; i++)
        {
            var type = client.Vehicle.GetTypeID(newVehicles[i]).Content;
            Debug.Log("type:" + type);
            if (type == "EgoCar") {
                continue;
            }
            var newCarPosition = client.Vehicle.GetPosition(newVehicles[i]).Content; // 获取新车的位置信息

            // 创建辅车实体
            GameObject newCar = null;

            switch (type)
            {
                case "Police":
                    {
                        newCar = GameObject.Instantiate(policeVehicle);
                        break;
                    }
                case "Bus":
                    {
                        newCar = GameObject.Instantiate(busVehicle);
                        break;
                    }
                case "SUV":
                    {
                        newCar = GameObject.Instantiate(suvVehicle);
                        break;
                    }
                case "Sedan":
                    {
                        newCar = GameObject.Instantiate(sedanVehicle);
                        break;
                    }
                default:
                    {
                        newCar = GameObject.Instantiate(sedanVehicle);
                        break;
                    }
            }
            float z = 0f;
            if (newCar.name != "0")
                z = 0.59f;
            newCar.transform.position = new Vector3((float)newCarPosition.X, z,
                (float)newCarPosition.Y);//maps its initial position

            var newAngle = client.Vehicle.GetAngle(newVehicles[i]).Content;
            newCar.transform.rotation = Quaternion.Euler(0f, (float)newAngle, 0f);

            newCar.name = newVehicles[i]; //object name the same as SUMO simulation version

            carList.Add(newCar);
        }


        // 交通灯同步
        var currentphase = client.TrafficLight.GetCurrentPhase("6790592423"); // 当前状态

        // 实时同步画面
        client.Control.SimStep();
        
        // 交通灯状态改变
        if (client.TrafficLight.GetCurrentPhase("6790592423").Content != currentphase.Content)
        {
            changeTrafficLights();
        }
    }


    /**
	 * 关闭TraCI接口
	 */
    private void OnApplicationQuit()
	{
		client.Control.Close();
	}


    // 将所有的交通灯状态都进行改变
    void changeTrafficLights()
    {
        for (int i = 0; i < tlightids.Count; i++)
        {
            // 按组进行同步
            for (int k = 0; k < dicti[tlightids[i]].Count; k++)
            {

                var newstate = client.TrafficLight.GetState(tlightids[i]).Content;
                var lightchange = dicti[tlightids[i]][k]; // retrieves traffic light object from list

                var chartochange = newstate[lightchange.index].ToString(); // traffic lights new state based on its index
                if (lightchange.isdual == false)
                {
                    lightchange.changeState(chartochange.ToLower()); // 单信号灯
                }
                else
                {
                    lightchange.changeStateDual(chartochange.ToLower()); // 复合信号灯
                }

            }
        }
    }

    // 给每个路口创建交通灯
    void createTLS()
    {
        dicti = new Dictionary<string, List<trafficLights>>(); // key:value = 路口:交通灯组
        for (int ids = 0; ids < tlightids.Count; ids++)
        {
            List<trafficLights> traLightslist = new List<trafficLights>();
            // 交通灯状态值
            int numconnections = 0;  //The index that represents the traffic light's state value
            var newjunction = GameObject.Find(tlightids[ids]); //the traffic light junction
            

            for (int i = 0; i < newjunction.transform.childCount; i++)
            {
                bool isdouble = false;
                var trafficlight = newjunction.transform.GetChild(i);//the next traffic light in the junction
                //Checks if the traffic light has more than 3 lights
                if (trafficlight.childCount > 3)
                {
                    isdouble = true;
                }
                Light[] newlights = trafficlight.GetComponentsInChildren<Light>();//list of light objects belonging to
                                                                                  //the traffic light
                                                                                  //Creation of the traffic light object, with its junction name, list of lights, index in the junction
                                                                                  //and if it is a single or dual traffic light
                trafficLights newtraLights = new trafficLights(newjunction.name, newlights, numconnections, isdouble);
                traLightslist.Add(newtraLights);
                var linkcount = client.TrafficLight.GetControlledLinks(newjunction.name).Content.NumberOfSignals;
                var laneconnections = client.TrafficLight.GetControlledLinks(newjunction.name).Content.Links;
                if (numconnections + 1 < linkcount - 1)
                {
                    numconnections++;//index increases
                    //increases index value until the next lane is reached
                    while ((laneconnections[numconnections][0] == laneconnections[numconnections - 1][0] || isdouble) &&
                           numconnections < linkcount - 1)
                    {
                        //if the next lane is reached but the traffic light is a dual lane, continue until the
                        //lane after is reached
                        if (laneconnections[numconnections][0] != laneconnections[numconnections - 1][0] && isdouble)
                        {
                            isdouble = false;
                        }
                        numconnections++;
                    }
                }
            }
            dicti.Add(newjunction.name, traLightslist);
        }
        changeTrafficLights(); // 显示初始化后的红绿灯
    }
}
