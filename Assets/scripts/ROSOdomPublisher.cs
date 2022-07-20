using System;
using RosMessageTypes.BuiltinInterfaces;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.Nav;
using Unity.Robotics.Core;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class ROSOdomPublisher : MonoBehaviour
{
    const string k_OdomTopic = "/odom";
    
    [SerializeField]
    double m_PublishRateHz = 20f;
    [SerializeField]
    string frame_id = "odom";
    [SerializeField]
    string child_frame_id = "base_footprint";
    [SerializeField]
    GameObject rootGameObject;
    
    double m_LastPublishTimeSeconds;

    HeaderMsg header;
    PoseMsg pose;
    PoseWithCovarianceMsg pose_with_covariance;
    TwistMsg twist;
    TwistWithCovarianceMsg twist_with_covariance;
    OdometryMsg odometry_msg;
    ROSConnection m_ROS;
    
    uint sequence = 0;
    uint sec;
    uint nanosec;

    double PublishPeriodSeconds => 1.0f / m_PublishRateHz;

    bool ShouldPublishMessage => Clock.NowTimeInSeconds > m_LastPublishTimeSeconds + PublishPeriodSeconds;

    // Start is called before the first frame update
    void Start()
    {
        if (rootGameObject == null)
        {
            Debug.LogWarning($"No GameObject explicitly defined as {nameof(rootGameObject)}, so using {name} as root.");
            rootGameObject = gameObject;
        }

        m_ROS = ROSConnection.GetOrCreateInstance();
        m_ROS.RegisterPublisher<OdometryMsg>(k_OdomTopic);
        m_LastPublishTimeSeconds = Clock.time + PublishPeriodSeconds;
    }
    void PublishMessage()
    {
        // Header
        sec = (uint)Clock.time;
        nanosec = (uint)((Clock.time - Math.Floor(Clock.time)) * Clock.k_NanoSecondsInSeconds);
        header = new HeaderMsg(sequence, new TimeMsg(sec, nanosec), frame_id);
        sequence++;
        
        //pose
        var position = rootGameObject.transform.position;
        var quaternion = rootGameObject.transform.rotation;
        PointMsg point = new PointMsg(position.x, position.y, position.z);
        QuaternionMsg orientation = new QuaternionMsg(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        pose = new PoseMsg(point, orientation);
        var cov = new double[36];
        pose_with_covariance = new PoseWithCovarianceMsg(pose, cov);
        
        //twist
        //Todo: change twist msg ( linear velocity ,angular velocity )
        var rotation = rootGameObject.transform.rotation;
        Vector3Msg linear = new Vector3Msg(position.x, position.y, position.z);
        Vector3Msg angular = new Vector3Msg(rotation.x, rotation.y, rotation.z);
        twist = new TwistMsg(linear, angular);
        twist_with_covariance = new TwistWithCovarianceMsg(twist, cov);

        odometry_msg = new OdometryMsg(header, child_frame_id, pose_with_covariance, twist_with_covariance);
        
        m_ROS.Publish(k_OdomTopic, odometry_msg);
        m_LastPublishTimeSeconds = Clock.FrameStartTimeInSeconds;
    }

    void Update()
    {
        if (ShouldPublishMessage)
        {
            PublishMessage();
        }

    }
}
