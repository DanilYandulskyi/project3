using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UnitMover))]
public class Unit : MonoBehaviour
{
    [SerializeField] private BaseSpawner _baseSpawner;
    [SerializeField] private int _price;
    [SerializeField] private Gold _gold;
    [SerializeField] private Flag _flag;

    private Vector3 _initialPosition;
    private UnitMover _mover;

    public event Action<Gold> CollectedGold;

    public bool IsResourceCollected { get; private set; } = false;
    public Vector3 InitialPosition => _initialPosition;
    public bool IsStanding { get; private set; } = true;
    public int Price => _price;

    private void OnEnable()
    {
        _mover = GetComponent<UnitMover>();

        _mover.GoldReached += TakeGold;
        _mover.BaseReached += CollectGold;
        _mover.FlagReached += TakeFlag;
    }

    private void OnDisable()
    {
        _mover.GoldReached -= TakeGold;
        _mover.BaseReached -= CollectGold;
        _mover.FlagReached -= TakeFlag;
    }

    public void Initialize(BaseSpawner baseSpawner)
    {
        _baseSpawner = baseSpawner;
    }

    public void SetFlag(Flag flag)
    {
        _flag = flag;
        
        if (_flag.isActiveAndEnabled)
        {
            IsStanding = false;
            _mover.MoveToFlag(_flag);
        }
    }

    public void SetInitialPosition(Vector3 position)
    {
        _initialPosition = position;
    }

    public void Stop()
    {
        StartCoroutine(StopForSeconds(3));
    }

    public void SetGold(Gold gold)
    {
        if (IsStanding)
        {
            if (gold.IsActiveAndEnabled)
            {
                _gold = gold;
                IsStanding = false;
                _mover.MoveToGold(_gold);
            }
        }
    }

    private void TakeFlag()
    {
        if (_flag.isActiveAndEnabled)
        {
            _baseSpawner.SpawnBase(_flag.transform.position).Assign(this);
            _flag.Disable();
            _flag = null;
            IsStanding = true;
        }
    }

    private void TakeGold()
    {
        _gold.StartFollow(transform);
        _mover.StopMoving();
        _mover.BackToInitialPosition(_initialPosition);
    }

    private void CollectGold()
    {
        CollectedGold?.Invoke(_gold);
        IsResourceCollected = false;
        IsStanding = true;
        _gold.StopFollow();
        _gold.Disable();
        _gold = null;
    }

    private IEnumerator StopForSeconds(float time)
    {
        _mover.StopMoving();
        IsStanding = false;
        _gold = null;

        yield return new WaitForSeconds(time);

        IsStanding = true;
    }
}