# 基于SUMO与Unity3D的车联网场景建模与设计

通过`Traci`接口实现**SUMO**和**Unity3D**实时通信，从而对V2X的通信进行模拟

### Project Structure

``````
|
|-- SumoProject // SUMO Project, 实现V2X 2维仿真
|-- MyProject   // Unity3D Project，实现V2X 3维仿真
``````

### Run

1. 运行**SUMO**
   ``sumo-gui -c map.sumocfg --start --remote-port 4001 --step-length 0.02``

2. 运行**Unity3D**

​	open Unity and import the `MyProject`, and run the program.
