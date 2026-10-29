using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : EnemyState
{
    private readonly List<Vector3> searchPoints =
        new List<Vector3>();

    private float searchTimer;
    private int currentSearchPointIndex;

    public SearchState(EnemyStates enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        searchTimer = 0f;
        currentSearchPointIndex = 0;
        searchPoints.Clear();

        enemy.Movement.SetSpeed(enemy.PatrolSpeed);

        if (!enemy.HasLastSeenPosition)
        {
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        GenerateSearchPoints(enemy.LastSeenPosition);

        if (searchPoints.Count == 0)
        {
            enemy.ChangeState(enemy.PatrolState);
            return;
        }

        MoveToCurrentSearchPoint();
    }

    public override void Tick()
    {
        searchTimer += Time.deltaTime;

        if (TargetCanBeSeenAgain())
        {
            enemy.UpdateLastSeenPosition();
            enemy.ChangeState(enemy.ChaseState);
            return;
        }

        if (searchTimer >= enemy.SearchDuration)
        {
            FinishSearch();
            return;
        }

        if (!enemy.Movement.HasReachedTarget)
            return;

        currentSearchPointIndex++;

        if (currentSearchPointIndex >= searchPoints.Count)
        {
            FinishSearch();
            return;
        }

        MoveToCurrentSearchPoint();
    }

    private bool TargetCanBeSeenAgain()
    {
        if (enemy.CurrentTarget == null)
            return false;

        return enemy.DetectPlayer.CanSee(
            enemy.CurrentTarget);
    }

    private void GenerateSearchPoints(Vector3 center)
    {
        // Første punkt er altid den sidste kendte position,
        // hvis den stadig ligger på NavMesh.
        if (NavMesh.SamplePosition(
                center,
                out NavMeshHit centerHit,
                enemy.SearchRadius,
                enemy.SearchNavMeshAreaMask))
        {
            searchPoints.Add(centerHit.position);
        }

        int desiredPointCount = enemy.SearchPointCount;
        int attemptsPerPoint =
            enemy.SearchSampleAttemptsPerPoint;

        for (int pointIndex = searchPoints.Count;
             pointIndex < desiredPointCount;
             pointIndex++)
        {
            bool pointFound = false;

            for (int attempt = 0;
                 attempt < attemptsPerPoint;
                 attempt++)
            {
                Vector3 randomOffset =
                    Random.insideUnitSphere * enemy.SearchRadius;

                Vector3 candidate =
                    center + randomOffset;

                if (!NavMesh.SamplePosition(
                        candidate,
                        out NavMeshHit hit,
                        enemy.SearchRadius,
                        enemy.SearchNavMeshAreaMask))
                {
                    continue;
                }

                if (IsTooCloseToExistingPoint(hit.position))
                    continue;

                searchPoints.Add(hit.position);
                pointFound = true;
                break;
            }

            if (!pointFound)
                continue;
        }
    }

    private bool IsTooCloseToExistingPoint(Vector3 point)
    {
        const float minimumPointDistance = 1f;

        foreach (Vector3 existingPoint in searchPoints)
        {
            if (Vector3.Distance(existingPoint, point)
                < minimumPointDistance)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveToCurrentSearchPoint()
    {
        if (currentSearchPointIndex < 0 ||
            currentSearchPointIndex >= searchPoints.Count)
        {
            return;
        }

        enemy.Movement.MoveTo(
            searchPoints[currentSearchPointIndex]);
    }

    private void FinishSearch()
    {
        enemy.ClearCurrentTarget();
        enemy.ClearLastSeenPosition();
        enemy.ChangeState(enemy.PatrolState);
    }
}