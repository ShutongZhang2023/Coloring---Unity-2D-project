using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TubeRequirement
{
    public TubeRotation tube;
    public List<int> allowedPositions;
}

public class DropDrip : MonoBehaviour
{
    public List<TubeRequirement> requirements;
    public bool isCorrect;

    public GameObject dripPrefab;
    public float interval = 0.2f;
    private Transform spawnPoint;

    private void Awake()
    {
        spawnPoint = GetComponent<Transform>();
    }

    public void checkTubeMatch()
    {
        foreach (var req in requirements)
        {
            var tube = req.tube;
            var allowed = req.allowedPositions;

            if (allowed.Count == 0) continue;

            if (!allowed.Contains(tube.positionId))
            {
                isCorrect = false;
                return;
            }
        }
        isCorrect = true;
    }

    public void Drop() {
        if (isCorrect)
        {
            StartCoroutine(DripSequence());
        }
    }

    private IEnumerator DripSequence()
    {
        for (int i = 0; i < 5; i++)
        {
            // 生成水滴
            Instantiate(dripPrefab, spawnPoint.position, Quaternion.identity);

            // 等待间隔
            yield return new WaitForSeconds(interval);
        }
    }

}
