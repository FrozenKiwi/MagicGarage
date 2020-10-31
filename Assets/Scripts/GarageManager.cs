using Apt.Unity.Projection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GarageManager : MonoBehaviour
{
    public GarageDoor GarageDoor;

    public GameObject[] Scenes;

    private TrackerBase Tracker;
    private ProjectionPlaneCamera projectionCamera;
    private Vector3 initialLocalPosition;

    private int _lastScene = 0;


    private float _sceneRunTime = 0;
    private bool _sceneRunning = false;
    private Coroutine _deactivation;

    public bool SceneRunning
    {
        get => _sceneRunning;
        set {
            if (_sceneRunning == value)
                return;

            Debug.Log($"Toggling Scene: {_sceneRunning}");

            _sceneRunning = value;
            _sceneRunTime = Time.time;
            if (_sceneRunning) SceneShow();
            else SceneHide();
        }
    }

    void Start()
    {
        projectionCamera = FindObjectOfType<ProjectionPlaneCamera>();
        initialLocalPosition = projectionCamera.transform.localPosition;
        Tracker = GetFirstConnectedTracker();

        //GarageDoor.InitPanelXforms(projectionCamera.ProjectionScreen);
    }

    // Update is called once per frame
    void Update()
    {
        var tracking = Tracker.IsTracking;
        if (tracking)
            UpdateCameraXform();

        SceneRunning = Tracker.IsTracking;
    }

    private void UpdateCameraXform()
        => projectionCamera.transform.localPosition = initialLocalPosition + Tracker.Translation;

    private void SceneShow()
    {
        GarageDoor.TriggerOpen();

        Scenes[_lastScene].SetActive(false);
        _lastScene = (_lastScene + 1) % Scenes.Length;
        Scenes[_lastScene].SetActive(true);
    }

    private void SceneHide()
    {
        GarageDoor.TriggerClose();

        if (_deactivation != null)
            StopCoroutine(_deactivation);
        _deactivation = StartCoroutine(TemporarilyDeactivate(2.1f, Scenes[_lastScene]));
        
    }
    private IEnumerator TemporarilyDeactivate(float duration, GameObject go)
    {
        yield return new WaitForSeconds(duration);
        go.SetActive(false);
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
