using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

using RosMessageTypes.UnityRoboticsDemo;
using Unity.Robotics.ROSTCPConnector;

public class movingObstacle : MonoBehaviour
{
    public GameObject gameObject;
    public string messageName;
    
    ROSConnection ros;
    PointMsg point;
    Vector3 position;
    
    NavMeshAgent agent;

    private int count = 0;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PointMsg>(messageName, ReceiveRosPoint);
        
        agent = GetComponent<NavMeshAgent>();

    }
    void ReceiveRosPoint(PointMsg msg)
    {
        position = new Vector3((float) msg.x, gameObject.transform.position.y, (float) msg.z);
    }
    
    void Update()
    {
        count++;
        if (count % 600 == 0)
        {
            agent.SetDestination(position);
            count = 0;
        }
    }

}