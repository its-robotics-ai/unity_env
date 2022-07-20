using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class ROSPointSubscriber : MonoBehaviour
{
    [SerializeField]
    public GameObject gameObject;

    [SerializeField] public string messageName;
    
    ROSConnection ros;
    PointMsg point;
    Vector3 position;
    
    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();
        ros.Subscribe<PointMsg>(messageName, ReceiveRosPoint);
    }
    
    void ReceiveRosPoint(PointMsg msg)
    {
        position = new Vector3((float) msg.x, (float) msg.y, (float) msg.z);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = position;
    }
}
