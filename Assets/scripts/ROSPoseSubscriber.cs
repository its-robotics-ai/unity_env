using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosSharp.Control;

public class ROSPoseSubscriber : MonoBehaviour
{
    public string messageName;

    ROSConnection ros;
    PoseMsg pose;
    
    Vector3 position;
    Quaternion rotation;
    
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PoseMsg>(messageName, ReceiveRosPose);
    }
    
    void ReceiveRosPose(PoseMsg msg)
    {
        position = new Vector3((float) msg.position.x, (float) msg.position.y, (float) msg.position.z);
        rotation = new Quaternion((float) msg.orientation.x, (float) msg.orientation.y,
            (float) msg.orientation.z, (float) msg.orientation.w);
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}