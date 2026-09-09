using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class MoveUpAndDown : NetworkBehaviour
{
    [SerializeField] float _distance = 2f;
    [SerializeField] float _speed = 1f;
    [SerializeField] bool _useLocalPosition = true;

    Vector3 _startPosition;

    public override void Spawned()
    {
        _startPosition = _useLocalPosition ? transform.localPosition : transform.position;
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        float time = Runner.SimulationTime;
        float offsetY = -(1f - Mathf.Cos(time * _speed)) * 0.5f * _distance;

        Vector3 targetPosition = _startPosition + new Vector3(0f, offsetY, 0f);

        if (_useLocalPosition)
            transform.localPosition = targetPosition;
        else
            transform.position = targetPosition;
    }

    void OnDrawGizmos()
    {
        Vector3 startPosition = _useLocalPosition && transform.parent != null
            ? transform.parent.TransformPoint(transform.localPosition)
            : transform.position;

        Vector3 endPosition = startPosition + Vector3.down * _distance;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(startPosition, endPosition);

        Gizmos.DrawSphere(startPosition, 0.08f);
        Gizmos.DrawSphere(endPosition, 0.08f);
    }
}
