using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor;
using UnityEditor.TerrainTools;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspector;
    Collider2D _collider;
    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (customInspector.panCamera)
            {
                CameraManager.i.PanCameraOnContact(customInspector.panDistance, customInspector.panTime, customInspector.panDirection, false);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (customInspector.panCamera)
            {
                CameraManager.i.PanCameraOnContact(customInspector.panDistance, customInspector.panTime, customInspector.panDirection, true);
            }
        }
    }
}

[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCamera = false;
    public bool panCamera = false;

    [HideInInspector] public CinemachineVirtualCamera leftCamera;
    [HideInInspector] public CinemachineVirtualCamera rightCamera;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = .35f;
}
public enum PanDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    private CameraControlTrigger ccTrigger;
    private void OnEnable()
    {
        ccTrigger = (CameraControlTrigger) target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (ccTrigger.customInspector.swapCamera)
        {
            ccTrigger.customInspector.leftCamera = EditorGUILayout.ObjectField("Camera on Left", ccTrigger.customInspector.leftCamera,
                                                   typeof(CameraControlTrigger), true) as CinemachineVirtualCamera;
            ccTrigger.customInspector.rightCamera = EditorGUILayout.ObjectField("Camera on Right", ccTrigger.customInspector.rightCamera,
                                                   typeof(CameraControlTrigger), true) as CinemachineVirtualCamera;
        }
        if (ccTrigger.customInspector.panCamera)
        {
            ccTrigger.customInspector.panDistance = EditorGUILayout.FloatField("Pan Distance", ccTrigger.customInspector.panDistance);
            ccTrigger.customInspector.panTime = EditorGUILayout.FloatField("Pan Time", ccTrigger.customInspector.panTime);
            ccTrigger.customInspector.panDirection = (PanDirection) EditorGUILayout.EnumPopup("Camera Pan Direction", ccTrigger.customInspector.panDirection);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(ccTrigger);
        }
    }
}