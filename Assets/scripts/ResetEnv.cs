using RosMessageTypes.Geometry;
using RosMessageTypes.UnityRosInterfaces;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using UnityEditor;

public class ResetEnv : MonoBehaviour
{
    ROSConnection ros;
    
    public GameObject AMRPrefab;
    public GameObject goalObject;
    
    public string AmrName;
    public string ResetServiceName;

    private PoseMsg amr_start;
    private PointMsg amr_goal;
    private bool done = false;


    void Start()
    {
        // register the service with ROS
        ROSConnection.GetOrCreateInstance().ImplementService<ResetRequest, ResetResponse>(ResetServiceName, ResetEnvironment);
    }

    /// <summary>
    ///  Callback to respond to the request
    /// </summary>
    /// <param name="request">service request containing the reset, amr_pose, amr_goal</param>
    /// <returns>service response containing the information about done (or 0 if object not found)</returns>
    private ResetResponse ResetEnvironment(ResetRequest request)
    {
        amr_start = request.amr_start;
        amr_goal = request.amr_goal;
        
        DestroyAMR();
        SetAMR(amr_start);
        SetGoal(amr_goal);
        
        // prepare a response
        ResetResponse resetResponse = new ResetResponse();
        resetResponse.done = is_done(request);

        return resetResponse;
    }

    private void SetAMR(PoseMsg pose)
    {
        Vector3 position = new Vector3((float)pose.position.x, (float)pose.position.y, (float)pose.position.z);
        Quaternion orientation = new Quaternion((float)pose.orientation.x ,(float)pose.orientation.y,
            (float)pose.orientation.z, (float)pose.orientation.w);
        
        Instantiate(AMRPrefab, position, orientation);
    }
    
    private void DestroyAMR()
    {
        Destroy(GameObject.Find(AmrName));
    }

    private void SetGoal(PointMsg point)
    {
        goalObject.transform.position = new Vector3((float)point.x, (float)point.y, (float)point.z);
    }

    private bool is_done(ResetRequest request)
    {
        var start_x = (float)request.amr_start.position.x;
        var start_y = (float)request.amr_start.position.y;
        var start_z = (float)request.amr_start.position.z;

        var goal_x = (float)request.amr_goal.x;
        var goal_y = (float)request.amr_goal.y;
        var goal_z = (float)request.amr_goal.z;

        bool done = (Vector3.Distance(GameObject.Find(AmrName).transform.position, new Vector3(start_x, start_y, start_z)) < 1e3)
            && (Vector3.Distance(goalObject.transform.position, new Vector3(goal_x, goal_y, goal_z)) < 1e3);

        return done;
    }
}