using UnityEngine;
using System.Collections;
using System;

public class UnitMover : MonoBehaviour
{
    private const float DistanceToStop = 0.2f;

    [SerializeField] private float _initialSpeed;

    private float _speed;
    private Transform _transform;
    private Coroutine _movingCoroutine;

    public event Action BaseReached;
    public event Action GoldReached;
    public event Action FlagReached;

    public void Start()
    {
        _transform = transform;
        _speed = _initialSpeed;
    }

    public void MoveToGold(IUnitTarget target)
    {
        _movingCoroutine = StartCoroutine(StartMoveToTarget(target));
    }

    public void Move(Vector3 direction)
    {
        Vector3 offset = direction.normalized * _speed * Time.deltaTime;

        _transform.Translate(offset);
    }

    public void BackToInitialPosition(Vector3 initialPosition)
    {
        StartCoroutine(StartMoveToBase(initialPosition));
    }

    public void MoveToFlag(IUnitTarget flag)
    {
        StartCoroutine(MoveToFlag(flag.Transform.position));
    }

    public void StopMoving()
    {
        _speed = 0;

        if (_movingCoroutine != null)
        {
            StopCoroutine(_movingCoroutine);
        }
    }

    private IEnumerator StartMoveToTarget(IUnitTarget target)
    {
        Vector3 direction = target.Transform.position - _transform.position;

        while (Vector3.SqrMagnitude(target.Transform.position - _transform.position) >= DistanceToStop)
        {
            Move(direction);
            yield return new WaitForEndOfFrame();
        }

        GoldReached?.Invoke();

        _movingCoroutine = null;
    }

    private IEnumerator MoveToFlag(Vector3 target)
    {
        Vector3 direction = target - _transform.position;

        while (Vector3.SqrMagnitude(target - _transform.position) >= DistanceToStop)
        {
            Move(direction);
            yield return new WaitForEndOfFrame();
        }

        FlagReached?.Invoke();

        _movingCoroutine = null;
    }

    private IEnumerator StartMoveToBase(Vector3 target)
    {
        _speed = _initialSpeed;
        Vector3 direction = target - _transform.position;

        while (Vector3.SqrMagnitude(target - _transform.position) >= DistanceToStop)
        {
            Move(direction);
            yield return new WaitForEndOfFrame();
        }

        BaseReached?.Invoke();
    }
}