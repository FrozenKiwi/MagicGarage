using Apt.Unity.Projection;
using UnityEngine;

public class KinectHeadTracker : TrackerBase
{
    KinectManager Kinect;

    public KinectInterop.JointType TrackedJoint = KinectInterop.JointType.Head;

    public GameObject KinectWorldOffset;

    // Start is called before the first frame update
    void Start()
    {
        Kinect = KinectManager.Instance;
    }

    // Each update, see if we have a head position we can track
    void Update()
    {
        var joint = (int)TrackedJoint;
        // Can we see anyone?
        if (UpdateIsTracking())
        {
            SecondsHasBeenTracked += Time.deltaTime;

            // Get our tracked head position
            var headPosition = Kinect.GetJointPosition(TrackedId, joint);

            // Move from Kinect space into world space
            translation = KinectWorldOffset.transform.TransformPoint(headPosition);
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
}