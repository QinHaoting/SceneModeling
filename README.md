# A V2X Simulator implementing via SUMO and Unity3D

A V2X project that uses **SUMO** and **Unity3D** to simulate real-time communication through the `Traci` interface.

### Project Structure

``````
|
|-- SumoProject // SUMO Project, implement V2X simulation of 2 dimension
|-- MyProject   // Unity3D Project，implement V2X simulation of 3 dimension
``````

### Run

1. run **SUMO**

   open command shell and run the command below

   ``sumo-gui -c map.sumocfg --start --remote-port 4001 --step-length 0.02``

2. run **Unity3D**

​	open Unity and import `MyProject`, and run the program.
