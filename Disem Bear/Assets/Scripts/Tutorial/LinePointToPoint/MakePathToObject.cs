using System.Collections;
using System.Collections.Generic;
using External.DI;
using Game.LPlayer;
using UnityEngine;
using UnityEngine.AI;

public class MakePathToObject : MonoBehaviour
{
    private Transform Player;
    private Transform Target;
    private LineRenderer lineRenderer;
    public GameBootstrap gameBootstrap;
    public AudioClip water;

    public bool On = false;

    public float maxHeightObject = 5f;

    private Vector3 TargetNavMeshPosition = new();

    private NavMeshPath path;

    public void Init(Player player)
    {
        lineRenderer = GetComponent<LineRenderer>();

        Player = player.transform;
        Target = Player;

        path = new();
    }

    public void OnUpdate(float deltaTime)
    {
        if (On)
        {
            NavMesh.SamplePosition(Player.position, out var hit, maxHeightObject, NavMesh.AllAreas);
            Vector3 PlayerNavMeshPosition = hit.position;
            NavMesh.CalculatePath(PlayerNavMeshPosition, TargetNavMeshPosition, NavMesh.AllAreas, path);
            RendererPath(path.corners);
        }
        else if (lineRenderer.positionCount > 0)
        {
            lineRenderer.positionCount = 0;
        }
    }


    private void RendererPath(Vector3[] points)
    {
        lineRenderer.positionCount = points.Length + 2;
        for (int i = 0; i < points.Length; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
        Vector3 TargetPositionZeroY = Target.position;
        if (points.Length > 0)
            TargetPositionZeroY.y = points[0].y;
        lineRenderer.SetPosition(points.Length, TargetPositionZeroY);
        lineRenderer.SetPosition(points.Length + 1, Target.position);
    }

    public void SetTarget(Transform target)
    {
        NavMesh.SamplePosition(target.position, out var hit, maxHeightObject, NavMesh.AllAreas);
        TargetNavMeshPosition = hit.position;
        Target = target;
        On = true;
    }

    public void ClearTarget()
    {
        On = false;
    }
}
