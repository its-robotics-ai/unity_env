using RosMessageTypes.Geometry;
using RosMessageTypes.UnityRoboticsDemo;
using RosMessageTypes.UnityRosInterfaces;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class RosResetServiceServer : MonoBehaviour
{
    private string m_ServiceName;
    private string AmrName;
    
    private PoseMsg amr_start;
    private PointMsg amr_goal;
    private bool done = false;
    
    
    public RosResetServiceServer(string m_ServiceName, string AMRName)
    {
        this.m_ServiceName = m_ServiceName;
        this.AmrName = AMRName;
    }

    void Start()
    {
        // register the service with ROS
        ROSConnection.GetOrCreateInstance().ImplementService<ResetRequest, ResetResponse>(m_ServiceName, GetObjectPose);
    }

    /// <summary>
    ///  Callback to respond to the request
    /// </summary>
    /// <param name="request">service request containing the reset, amr_pose, amr_goal</param>
    /// <returns>service response containing the information about done (or 0 if object not found)</returns>
    private ResetResponse GetObjectPose(ResetRequest request)
    {
        amr_start = request.amr_start;
        amr_goal = request.amr_goal;
        
        // prepare a response
        ResetResponse resetResponse = new ResetResponse();
        // Find a game object with the requested name
        GameObject gameObject = GameObject.Find(AmrName);
        
        var x = (float)request.amr_start.position.x;
        var y = (float)request.amr_start.position.y;
        var z = (float)request.amr_start.position.z;

        if (Vector3.Distance(gameObject.transform.position, new Vector3(x, y, z)) < 1e2)
        {
            done = true;
        }
        
        // Fill-in the response
        resetResponse.done = done;

        return resetResponse;
    }
}