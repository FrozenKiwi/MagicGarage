using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apt.Unity.Projection
{
    [RequireComponent(typeof(ProjectionPlaneCamera))]
    public class BasicMovement : MonoBehaviour
    {
        private TrackerBase Tracker;
        private ProjectionPlaneCamera projectionCamera;
        private Vector3 initialLocalPosition;

        void Start()
        {
            projectionCamera = GetComponent<ProjectionPlaneCamera>();
            initialLocalPosition = projectionCamera.transform.localPosition;
            Tracker = GetFirstConnectedTracker();
        }

        void Update()
        {
            if (Tracker == null)
                return;

            if(Tracker.IsTracking)
            {
                projectionCamera.transform.localPosition = initialLocalPosition + Tracker.Translation;
            }
        }

        TrackerBase GetFirstConnectedTracker()
        {
            var allTrackers = FindObjectsOfType<TrackerBase>();
            foreach (var tracker in allTrackers.OrderByDescending(t => t.Priority))
            {
                if (tracker.enabled && tracker.IsConnected)
                {
                    return tracker;
                }
            }
            return null;
        }
    }
}
