using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControl : MonoBehaviour
{
    public static CameraControl Instance;
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;

    private void Awake()
    {
        Instance = this;
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += CameraShakeEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= CameraShakeEvent;
    }

    private void CameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

    private void Start()
    {
        GetNewCameraBound();
    }

    private void GetNewCameraBound()
    {
        var obj = GameObject.FindGameObjectWithTag("DefaultBounds");
        if (obj == null) return;

        confiner2D.m_BoundingShape2D = obj.GetComponent<PolygonCollider2D>();
        confiner2D.InvalidateCache();
    }
}