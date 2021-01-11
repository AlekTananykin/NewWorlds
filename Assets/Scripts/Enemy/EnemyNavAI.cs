using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNavAI : MonoBehaviour, IReactToHit
{    
    private NavMeshAgent _navMeshAgent = null;
    private Animator _animator = null;

    private PlayerController _playerController;
    private Transform _playerPos;

    private float _speedRotation = 50f;

    [SerializeField] private float _attackDistance = 5f;
    [SerializeField] private float _seeDistance = 20;

    [SerializeField] List<Vector3> _wayPoints = new List<Vector3>();
    private int _wayPointIndex = 0;

    bool _isAlive = true;

    float _rootMotionOffsetWalk = 5.2f;
    float _rootMotionOffsetRun = 9.98176f;

    private int IncrementWaypointIndex()
    {
        _wayPointIndex = (++_wayPointIndex) % _wayPoints.Count;
        return _wayPointIndex;
    }

    private const float _rayRechargeTimeMs = 1.5f;
    private float _rayTime = 0f;

    public void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.stoppingDistance = _attackDistance;
        _navMeshAgent.speed = _rootMotionOffsetWalk;
        
        _animator = GetComponent<Animator>();

        _playerController = FindObjectOfType<PlayerController>();
        _playerPos = _playerController.transform;
    }

    void Start()
    {
        _navMeshAgent.SetDestination(_wayPoints[IncrementWaypointIndex()]);
    }

    void Update()
    {
        if (!_isAlive)
        {
            _navMeshAgent.ResetPath();

            Destroy(this.gameObject, 3f);
            return;
        }

        if (_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance)
        {
            _animator.SetBool("move", true);
        }
        else 
        {
            _animator.SetBool("move", false);
        }

        Ray rayToPlayer = GetRayToPlayer();
        if (!IsPlayerSeen(rayToPlayer))
        {
            if (_navMeshAgent.stoppingDistance >=
                _navMeshAgent.remainingDistance)
            {
                _navMeshAgent.SetDestination(
                    _wayPoints[IncrementWaypointIndex()]);
            }
            return;
        }

        ToFollowThePlayer(rayToPlayer);

        _navMeshAgent.stoppingDistance = _attackDistance;
        _navMeshAgent.SetDestination(_playerPos.position);
        
        ShootThePlayerByRay();
    }

    private void ShootThePlayerByRay()
    {
        _rayTime += Time.deltaTime;
        if (_rayTime < _rayRechargeTimeMs)
            return;

        _rayTime = 0;
        _playerController.ReactToHit(5);
    }

    private Ray GetRayToPlayer()
    {
        Vector3 enemyCenter = new Vector3(transform.position.x, 
            transform.position.y + 0.7f, 
            transform.position.z);

        return new Ray(enemyCenter,
            _playerPos.position - enemyCenter);
    }

    bool IsPlayerSeen(Ray rayToPlayer)
    {
        if (0.5f > Vector3.Dot(transform.forward.normalized, 
            rayToPlayer.direction.normalized))
            return false;

        RaycastHit hit;
        if (Physics.Raycast(rayToPlayer, out hit))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hit.distance > _seeDistance || !hitObject.CompareTag("Player"))
                return false;
        }
        else return false;

        return true;
    }

    private void ToFollowThePlayer(Ray rayToPlayer)
    {
        Quaternion lookRotation = Quaternion.LookRotation(
            rayToPlayer.direction.normalized);
        lookRotation.x = 0;
        lookRotation.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            lookRotation, Time.deltaTime * _speedRotation);
    }

    private int _health = 100;
    public void ReactToHit(int hitCount)
    {
        _health -= hitCount;

        if (_health <= 0)
        {
            _isAlive = false;
            _animator.SetBool("die", true);
        }
    }
}
