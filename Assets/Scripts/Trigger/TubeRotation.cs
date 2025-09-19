using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class TubeRotation : MonoBehaviour
{
    public int positionId = 0;
    public Transform tubeTransform;
    public UnityEvent OnRotate;

    public void rotateTube() {
        positionId = (positionId + 1) % 4;
        tubeTransform.DORotate(new Vector3(0, 0, -90), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.InOutSine).OnComplete(() => OnRotate.Invoke());
    }

    public void rotateTubeBack()
    {
        positionId = (positionId + 1) % 4;
        tubeTransform.DORotate(new Vector3(0, 0, 90), 0.5f, RotateMode.WorldAxisAdd).SetEase(Ease.InOutSine).OnComplete(() => OnRotate.Invoke());
    }
}
