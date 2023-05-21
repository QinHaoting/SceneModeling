using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 摄像机跟随脚本
 * 实现三维场景漫游
 * @author Haoting
 */
public class CameraFollower : MonoBehaviour {
	public Transform obj;
	public Vector3 offset;

	// 速度
	public float fSpeed = 10; // 跟随速度
	public float lSpeed = 10; // 移动速度

	/**
	 * 调整视角转向
	 */
	public void LookAtTrager()
	{
		Vector3 rot = obj.position - this.transform.position;
		Quaternion _rot = Quaternion.LookRotation(rot, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lSpeed * Time.deltaTime);
	}

	/**
	 * 调整视角位置
	 */
	public void MoveToTrager()
	{
		Vector3 pos = obj.position +
			obj.forward * offset.z +
			obj.right * offset.x +
			obj.up * offset.y;
		transform.position = Vector3.Lerp(transform.position, pos, fSpeed * Time.deltaTime);
	}

	public void FixedUpdate()
	{
        LookAtTrager();
		MoveToTrager();
	}
}	
