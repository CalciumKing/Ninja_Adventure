using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager i;
    [SerializeField] CinemachineVirtualCamera[] allCameras;
    Vector2 trackedObjectOffset;
    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer transposer;
    Coroutine panCameraCoroutine;
    private void Awake()
    {
        if (i == null) i = this;
        else Destroy(i);
    }
    private void Start()
    {
        for(int i = 0; i < allCameras.Length; i++)
        {
            if (allCameras[i].enabled)
            {
                currentCamera = allCameras[i];
                transposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
        trackedObjectOffset = transposer.m_TrackedObjectOffset;
    }
    #region Pan Camera
    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPosition)
    {
        panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPosition));
    }
    IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPosition)
    {
        Vector2 startPos = Vector2.zero;
        Vector2 endPos = Vector2.zero;
        
        if (!panToStartingPosition)
        {
            switch (panDirection)
            {
                case PanDirection.UP:
                    endPos = Vector2.up;
                break;
                case PanDirection.DOWN:
                    endPos = Vector2.down;
                break;
                case PanDirection.LEFT:
                    endPos = Vector2.left;
                break;
                case PanDirection.RIGHT:
                    endPos = Vector2.right;
                break;
            }
            endPos *= panDistance;
            startPos = trackedObjectOffset;
            endPos += startPos;
        }
        else
        {
            startPos = transposer.m_TrackedObjectOffset;
            endPos = trackedObjectOffset;
        }
        var elapsedTime = 0f;
        while(elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;
            Vector3 panLerp = Vector3.Lerp(startPos, endPos, (elapsedTime / panTime));
            transposer.m_TrackedObjectOffset = panLerp;
            yield return null;
        }
    }
    #endregion
}