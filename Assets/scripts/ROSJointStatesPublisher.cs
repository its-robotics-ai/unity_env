using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosMessageTypes.Std;
using RosMessageTypes.Sensor;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.BuiltinInterfaces;

public class ROSJointStatesPublisher : MonoBehaviour
{
    const string k_JointStateTopic = "/joint_states";
    
    [SerializeField]
    double m_PublishRateHz = 20f;

    [SerializeField] 
    string frame_id = "";
    [SerializeField]
    GameObject rightJoint;
    [SerializeField]
    GameObject leftJoint;
    
    double m_LastPublishTimeSeconds;

    ROSConnection m_ROS;
    
    HeaderMsg header;
    uint sequence = 0;
    uint sec;
    uint nanosec;
    
    string[] name = new string[2] {"wheel_right_joint", "wheel_left_joint"};

    double[] position = new double[2];
    
    JointStateMsg joint_state;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => Clock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;
    // Start is called before the first frame update
    void Start()
    {
        if (rightJoint == null)
        {
            Debug.LogWarning($"No GameObject explicitly defined as {nameof(rightJoint)}, so using {name} as root.");
            rightJoint = gameObject;
        }
        else if (leftJoint == null)
        {
            Debug.LogWarning($"No GameObject explicitly defined as {nameof(leftJoint)}, so using {name} as root.");
            leftJoint = gameObject;
        }

        m_ROS = ROSConnection.GetOrCreateInstance();
        m_ROS.RegisterPublisher<JointStateMsg>(k_JointStateTopic);
        m_LastPublishTimeSeconds = Clock.time + PublishPeriodSeconds;
    }

    void PublishMessage()
    {
        // Header
        sec = (uint)Clock.time;
        nanosec = (uint)((Clock.time - Math.Floor(Clock.time)) * Clock.k_NanoSecondsInSeconds);
        header = new HeaderMsg(sequence, new TimeMsg(sec, nanosec), frame_id);
        sequence++;
        
        // position
        position[0] = rightJoint.transform.rotation.x;
        position[1] = leftJoint.transform.rotation.x;

        joint_state = new JointStateMsg(header, name, position, new double[2], new double[2]);
        
        m_ROS.Publish(k_JointStateTopic, joint_state);
        m_LastPublishTimeSeconds = Clock.FrameStartTimeInSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldPublishMessage)
        {
            PublishMessage();
        }
    }
}
