using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARLessonPlacer : MonoBehaviour
{
    [SerializeField] ARRaycastManager raycastManager;
    [SerializeField] ARPlaneManager planeManager;
    [SerializeField] Camera arCamera;
    [SerializeField] StepController stepController;
    [SerializeField] LessonExtras extras;
    [SerializeField] GameObject stepPanel;
    [SerializeField] GameObject reticle;
    [SerializeField] GameObject scanPrompt;
    [SerializeField] GameObject trackingLostPrompt;
    [SerializeField] LessonData editorFallbackLesson;

    static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    GameObject spawned;
    LessonData lesson;

    void Start()
    {
        lesson = AppState.SelectedLesson != null ? AppState.SelectedLesson : editorFallbackLesson;
        stepPanel.SetActive(false);
        reticle.SetActive(false);
        trackingLostPrompt.SetActive(false);
        scanPrompt.SetActive(true);

        // In the Unity Editor there is no AR camera feed, so place the model in front of the camera
        if (Application.isEditor)
        {
            Transform c = arCamera.transform;
            Spawn(c.position + c.forward * 0.9f + Vector3.down * 0.3f, Quaternion.identity);
        }
    }

    void Update()
    {
        if (spawned != null)
        {
            if (!Application.isEditor)
                trackingLostPrompt.SetActive(ARSession.state != ARSessionState.SessionTracking);
            return;
        }

        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        bool found = raycastManager.Raycast(center, hits, TrackableType.PlaneWithinPolygon);
        reticle.SetActive(found);
        scanPrompt.SetActive(!found);
        if (!found) return;

        Pose pose = hits[0].pose;
        reticle.transform.SetPositionAndRotation(pose.position, pose.rotation);

        Touchscreen ts = Touchscreen.current;
        if (ts != null && ts.primaryTouch.press.wasPressedThisFrame)
            Spawn(pose.position, pose.rotation);
    }

    void Spawn(Vector3 position, Quaternion fallbackRotation)
    {
        if (lesson == null || lesson.modelPrefab == null)
        {
            Debug.LogError("No lesson / model prefab selected.");
            return;
        }

        // Face the model toward the user (labels are readable from the -Z side)
        Vector3 away = position - arCamera.transform.position;
        away.y = 0f;
        Quaternion look = away.sqrMagnitude > 0.0001f ? Quaternion.LookRotation(away) : fallbackRotation;

        spawned = Instantiate(lesson.modelPrefab, position, look);
        reticle.SetActive(false);
        scanPrompt.SetActive(false);
        planeManager.SetTrackablesActive(false);

        stepPanel.SetActive(true);
        stepController.Begin(lesson, spawned.transform);
        if (extras != null) extras.Setup(lesson, spawned.transform);
    }

    public void Replace()
    {
        if (spawned != null) Destroy(spawned);
        spawned = null;
        stepPanel.SetActive(false);
        if (extras != null) extras.Hide();
        planeManager.SetTrackablesActive(true);
    }

    public void BackToMenu() { SceneManager.LoadScene("MainMenu"); }
}