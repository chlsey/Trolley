using UnityEngine;
using UnityEngine.Events;

public class CatWalkPath : MonoBehaviour
{
    [Header("Waypoints (assign in Inspector)")]
    public Transform[] waypoints;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;

    public UnityEvent onFirstWaypointReached;

    private int _currentWaypoint;
    private bool _walking;
    private bool _firedFirstWaypoint;
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void StartWalking()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        _currentWaypoint = 0;
        _walking = true;
        _firedFirstWaypoint = false;

        // Force the walk animation state
        if (_anim != null)
            _anim.Play("Cat Armature|Cat Walk Loop", 0, 0f);
    }

    private void Update()
    {
        if (!_walking) return;

        float remaining = moveSpeed * Time.deltaTime;

        while (remaining > 0f && _currentWaypoint < waypoints.Length)
        {
            Transform target = waypoints[_currentWaypoint];
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;

            float dist = direction.magnitude;
            if (dist < 0.05f)
            {
                _currentWaypoint++;
                if (!_firedFirstWaypoint)
                {
                    _firedFirstWaypoint = true;
                    onFirstWaypointReached?.Invoke();
                }
                continue;
            }

            // Rotate to face movement direction
            Quaternion look = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotateSpeed * Time.deltaTime);

            // Move toward waypoint, consuming only the distance needed
            float step = Mathf.Min(remaining, dist);
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
            remaining -= step;
        }

        if (_currentWaypoint >= waypoints.Length)
            _walking = false;
    }
}
