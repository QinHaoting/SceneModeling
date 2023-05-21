using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {
	// 输入
	private float horizontal; // 横轴输入
	private float vertical; // 纵轴输入
	private float angle; // 角度输入

	// 轮胎
	public WheelCollider lf, rf, lb, rb; // 轮胎碰撞器，控制轮胎旋转
	public Transform lfT, rfT, lbT, rbT; // 轮胎Transform组件

	// 速度
	public float angleSpeed; // 旋转速度
	public float speed; // 移动速度

	
	public void FixedUpdate()
	{	
		horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

		// print("horizontal:" + horizontal);
		// print("vertical:" + vertical);

		// 转向
        angle = angleSpeed * horizontal;
		// 前驱
		lf.steerAngle = angle;
		rf.steerAngle = angle;

		// 移动
		lf.motorTorque = vertical * speed;
		rf.motorTorque = vertical * speed;

		// 同步车轮
		UpdateWheel(lfT, lf);
		UpdateWheel(rfT, rf);
		UpdateWheel(lbT, lb);
		UpdateWheel(rbT, rb);

		if (Input.GetKey(KeyCode.Space)) // 刹车
		{
			lf.brakeTorque = 2500f;
			rf.brakeTorque = 2500f;
			lb.brakeTorque = 2500f;
			rf.brakeTorque = 2500f;
        }
		else 
		{
            lf.brakeTorque = 0;
            rf.brakeTorque = 0;
            lb.brakeTorque = 0;
            rf.brakeTorque = 0;
        }
    }

    /**
	 * 同步车轮
	 */
    void UpdateWheel(Transform trans, WheelCollider wheel)
	{
		Vector3 pos = trans.position; // 位置信息
		Quaternion rot = trans.rotation; // 旋转信息
		// 获取车轮的位置信息、旋转信息
		wheel.GetWorldPose(out pos, out rot);
		// 同步
		trans.position = pos;
		trans.rotation = rot;
	}
}
