using UnityEngine;
using System.Collections;
using Actors;

public class NavAgent : MonoBehaviour
{
    [SerializeField, HideInInspector, AssignActorAutomaticly]
    ThightAIMovementActor actor;

    [SerializeField]
    Transform pathStartPoint;

    [SerializeField]
    LightSkin lightSkin;

    //only for debug!!
    [SerializeField]
    Transform goal;
    //only for debug!!
    [SerializeField]
    bool updatePath = true;

    PathPlaner pathPlaner;
    OnPathComputationFinished cPathFoundCallback;
    NavPathIterator cPath;

    void Awake()
    {
        cPath = new NavPathIterator();
    }

    void Start()
    {
        pathPlaner = PathPlaner.Instance;
    }

    public delegate void OnPathComputationFinished(bool foundPath);

    public void SetDestination(Vector2 dest, OnPathComputationFinished onPathFinished)
    {
        Debug.Assert(cPathFoundCallback == null);
        cPathFoundCallback = onPathFinished;
        pathPlaner.FindRequestedPath(new PathRequest(pathStartPoint.position, dest, OnPathCompleted, lightSkin));
    }

    void OnPathCompleted(NavPath path)
    {
        Debug.Assert(cPathFoundCallback != null);
        if (path == null)
            cPathFoundCallback(false);
        else
        {
            EnableMovementAgain();
            cPath.AssignNewPath(path);
            cPath.CurrentSegement.InitTravers(actor.CC2DThightAIMotor);
            cPathFoundCallback(true);
        }

        cPathFoundCallback = null;
    }

    //Only For Debug!!
    public void PathComputationFinished(bool foundPath)
    {

    }

    void FixedUpdate()
    {
        if (updatePath)
        {
            SetDestination(goal.position, PathComputationFinished);
            updatePath = false;
        }

        if (cPath.path == null)
            return;

        cPath.path.Visualize();

        if (cPath.CurrentSegement.ReachedTarget(pathStartPoint.position))
        {
            cPath.CurrentSegement.StopTravers(actor.CC2DThightAIMotor);
            if (!cPath.NextSegment())
            {
                cPath.path = null;
                Debug.Log("Finished Path!");
                StopMoving();
                return;
            }
            Debug.Log("Next Segment!");
            cPath.CurrentSegement.InitTravers(actor.CC2DThightAIMotor);
        }
        /*else if (cPath.IsSegmentTimedOut)
        {
            /*cPath.path = null;
            Debug.Log("Time out. Abort!");
            StopMoving();
            //Time out has problems! Usage not recommended.
        }*/
        else if (!cPath.CurrentSegement.IsOnTrack(pathStartPoint.position))
        {
            //cPath.path = null;
            Debug.Log("Lost Path. Abort!");
            StopMoving();
            return;
        }
        // else
        // {
        cPath.CurrentSegement.UpdateMovementInput(actor.CC2DThightAIMotor.CurrentMovementInput);
        // }        
    }

    void StopMoving()
    {
        actor.CC2DThightAIMotor.CurrentMovementInput.ResetToNeutral();
        actor.CC2DThightAIMotor.SetManualXSpeed(0);
    }

    void EnableMovementAgain()
    {
        actor.CC2DThightAIMotor.StopUsingManualSpeed();
    }

    class NavPathIterator
    {
        public IPathSegment CurrentSegement { get { return path.pathSegments[currentSegment]; } }
        public bool IsSegmentTimedOut { get { return Time.time - segmentStartTime > CurrentSegement.TimeOut; } }

        public NavPath path;
        int currentSegment;
        float segmentStartTime;

        public bool NextSegment()
        {
            currentSegment++;
            if (currentSegment >= path.pathSegments.Length)
                return false;
            return true;
        }

        public void AssignNewPath(NavPath path)
        {
            this.path = path;
            currentSegment = 0;
            segmentStartTime = Time.time;
        }
    }
}
