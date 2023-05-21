using CodingConnected.TraCI.NET.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class NPCController : MonoBehaviour {

	public NavMeshAgent agent;
	public Animator animator; // 动画

	public GameObject Path;
	private Transform[] PathPoints;

	public int index = 0;

	public float minDistance = 10;




    // Use this for initialization
    void Start () {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();

		PathPoints = new Transform[Path.transform.childCount];
		for (int i = 0; i < PathPoints.Length; i++) {
			PathPoints[i] = Path.transform.GetChild(i);
		}

		// 定期删除
        // 获取需要删除的GameObject
        GameObject objToDelete = GameObject.Find("Male");

        // 定义等待时间
        float waitTime = 22.0f;

        // 开始协程
        StartCoroutine(DestroyObjectAfterTime(objToDelete, waitTime));
    }
	
	// Update is called once per frame
	void Update () {
		roam();
	}

	void roam ()
	{
        if (Vector3.Distance(transform.position, PathPoints[index].position) < minDistance) {
            if (index > 0 && index < PathPoints.Length)
            {
				index += 1;
			}
			else { 
				index = 0;
			}
		}
		agent.SetDestination(PathPoints[index].position);
	}

    // 定义协程函数
    IEnumerator DestroyObjectAfterTime(GameObject obj, float waitTime)
    {
        // 等待指定时间
        yield return new WaitForSeconds(waitTime);

        // 删除GameObject
        Destroy(obj);
    }
}
