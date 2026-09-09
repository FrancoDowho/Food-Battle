using Fusion;
using UnityEngine;

public class SpaceRocket : NetworkBehaviour
{
    [Header("Speed Range")]
    [SerializeField] float _minSpeed = 2f;
    [SerializeField] float _maxSpeed = 5f;

    [Header("Respawn Time Range")]
    [SerializeField] float _minFlightSeconds = 2f;
    [SerializeField] float _maxFlightSeconds = 4f;

    [Networked] float _currentSpeed { get; set; }
    [Networked] TickTimer _flightTimer { get; set; }
    [Networked] Vector2 _originPosition { get; set; }

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            _originPosition = transform.position;
            StartNewFlightCycle();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        transform.position += Vector3.up * (_currentSpeed * Runner.DeltaTime);

        if (_flightTimer.Expired(Runner))
        {
            transform.position = _originPosition;
            StartNewFlightCycle();
        }
    }

    void StartNewFlightCycle()
    {
        _currentSpeed = Random.Range(_minSpeed, _maxSpeed);
        float randomFlightSeconds = Random.Range(_minFlightSeconds, _maxFlightSeconds);
        _flightTimer = TickTimer.CreateFromSeconds(Runner, randomFlightSeconds);
    }
}