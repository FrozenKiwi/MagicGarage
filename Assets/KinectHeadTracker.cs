using Apt.Unity.Projection;
using UnityEngine;

public class KinectHeadTracker : TrackerBase, SpeechRecognitionInterface
{
    KinectManager Kinect;

    public KinectInterop.JointType TrackedJoint = KinectInterop.JointType.Head;
    private int _joint => (int)TrackedJoint;

    public GameObject KinectCameraOffset;

    // Callibration details
    public ProjectionPlane ProjectionPlane;
    public Vector3 CallibrateOffset = Vector3.zero;
    public Vector3 CallibrateLeft = Vector3.left;
    public Vector3 CallibrateRight = -Vector3.left;

    private Matrix4x4 _offset = Matrix4x4.identity;
    // Start is called before the first frame update
    void Start()
    {
        Kinect = KinectManager.Instance;
    }

    // Each update, see if we have a head position we can track
    void Update()
    {
        // Can we see anyone?
        if (UpdateIsTracking())
        {
            SecondsHasBeenTracked += Time.deltaTime;

            // Get our tracked head position
            var headPosition = Kinect.GetJointPosition(TrackedId, _joint);

            // Move from Kinect space into world space
            translation = _offset.MultiplyPoint(headPosition) + KinectCameraOffset.transform.position;
        }
    }

    bool UpdateIsTracking()
    {
        // Do we have anyone?
        if (TrackedId == 0)
        {
            TrackedId = Kinect.GetUserIdByIndex(0);
            IsTracking = TrackedId != 0;
        }
        else if (!Kinect.IsUserTracked(TrackedId))
        {
            // How long do we wait before we give up on the tracked user?
            TrackedId = 0;
            IsTracking = !IsTracking;
            SecondsHasBeenTracked = 0;
        }

        return IsTracking;
    }

    #region SpeechRecognitionInterface

    public bool SpeechPhraseRecognized(string phraseTag, float condidence)
    {
        if (phraseTag == "CALL_LEFT")
        {
            Debug.Log("Callibrating Left...");

            // Take the current head position
            CallibrateLeft = Kinect.GetJointPosition(TrackedId, _joint);
            UpdateCallibration();
        }

        if (phraseTag == "CALL_RIGHT")
        {
            Debug.Log("Callibrating Right...");

            // Take the current head position
            CallibrateRight = Kinect.GetJointPosition(TrackedId, _joint);
            UpdateCallibration();
        }

        return true;
    }

    void UpdateCallibration()
    {
        // Don't forget the kinect mirrors things
        var left = CallibrateLeft - CallibrateRight;

        // remove inadvertent up/down
        left.y = 0;
        var distance = left.magnitude;
        left /= distance;
        var fwd = Vector3.Cross(left, Vector3.up).normalized;

        // Our offset is scaled up so that L -> R traverses LHS of the plane to the RHS
        var scale = ProjectionPlane.Size.x / distance;

        Vector4 position = CallibrateOffset;
        // Remove height offset
        position.y = -scale * (CallibrateRight.y + CallibrateLeft.y) / 2;
        position.w = 1;

        _offset = new Matrix4x4(
            -left * scale, // For some reason, this needs to be negated (mirroring from Kinect?)
            Vector3.up * scale,
            fwd * scale,
            position
            );
    }

    #endregion
}