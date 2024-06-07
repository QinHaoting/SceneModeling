# SceneModeling
## 基于SUMO与Unity3D的车联网场景建模与设计

### Project Structure

``````
|
|-- SumoProject // SUMO Project, implement V2X simulation of 2 dimension
|-- MyProject   // Unity3D Project，implement V2X simulation of 3 dimension
``````



仿真步骤：

Step1. 开启SUMO仿真：
``sumo-gui -c map.sumocfg --start --remote-port 4001 --step-length 0.02``

Step2. 运行Unity3D

open Unity and import the `MyProject`, and run the program.
