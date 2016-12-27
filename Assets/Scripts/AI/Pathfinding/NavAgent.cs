using UnityEngine;
using System.Collections;
using Entity;
using LightSensing;

public class NavAgent : MonoBehaviour
{
    [SerializeField, HideInInspector, AssignEntityAutomaticly]
    ThightAIMovementEntity actor;

    [SerializeField]
    Transform pathStartPoint;

    PathPlaner pathPlaner;
    OnPathComputationFinished cPathFoundCallback;
    NavPathIterator cPath;
    LightMarker[] emptyMarkerArray;

    public bool IsFollowingAPath { get { return cPath.path != null; } }
    public bool IsOnEdgeNode { get { return IsFollowingAPath && cPath.CurrentSegement.GetType() == typeof(PathSegment); } }

    void Awake()
    {
        cPath = new NavPathIterator();
        emptyMarkerArray = new LightMarker[] { };
        pathPlaner = PathPlaner.Instance;
    }

    public delegate void OnPathComputationFinished(bool foundPath);

    public void SetDestination(Vector2 dest, LightMarker[] lightMarker, LightSkin lightSkin, OnPathComputationFinished onPathFinished)
    {
        Debug.Assert(cPathFoundCallback == null);
        cPathFoundCallback = onPathFinished;
        if (!actor.CharacterController2D.isGrounded)
        {
            OnPathCompleted(null);
            return;
        }
        pathPlaner.FindRequestedPath(new PathRequest(pathStartPoint.position, dest, OnPathCompleted, lightSkin, lightMarker));
    }

    public void SetDestination(Vector2 dest, LightSkin lightSkin, OnPathComputationFinished onPathFinished)
    {
        SetDestination(dest, emptyMarkerArray, lightSkin, onPathFinished);
    }

    public void UpdateDestination(Vector2 dest, LightSkin lightSkin, OnPathComputationFinished onPathFinished)
    {
        if (cPath.path == null)
        {
            SetDestination(dest, emptyMarkerArray, lightSkin, onPathFinished);
            return;
        }
        if (cPath.path.goal == dest)
            return;
        Debug.Assert(cPathFoundCallback == null);
        cPathFoundCallback = onPathFinished;
        if (!actor.CharacterController2D.isGrounded)
        {
            OnUpdatePathCompleted(null);
            return;
        }
        pathPlaner.FindRequestedPath(new PathRequest(pathStartPoint.position, dest, OnUpdatePathCompleted, lightSkin, emptyMarkerArray));
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
            cPath.CurrentSegement.InitTravers(actor.CC2DThightAIMotor, cPath.GetNextSegment());
            cPathFoundCallback(true);
        }

        cPathFoundCallback = null;
    }

    void OnUpdatePathCompleted(NavPath path)
    {
        Debug.Assert(cPathFoundCallback != null);
        if (path == null)
            cPathFoundCallback(false);
        else
        {
            cPath.AssignNewPath(path);
            cPath.CurrentSegement.InitTravers(actor.CC2DThightAIMotor, cPath.GetNextSegment());
            cPathFoundCallback(true);
        }

        cPathFoundCallback = null;
    }

    void FixedUpdate()
    {
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
            cPath.CurrentSegement.InitTravers(actor.CC2DThightAIMotor, cPath.GetNextSegment());
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
            cPath.path = null;
            StopMoving();
            return;
        }
        // else
        // {
        cPath.CurrentSegement.UpdateMovementInput(actor.CC2DThightAIMotor.CurrentMovementInput, actor.CC2DThightAIMotor);
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

        public IPathSegment GetNextSegment()
        {
            if (currentSegment + 1 >= path.pathSegments.Length)
                return null;
            return path.pathSegments[currentSegment];
        }

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
