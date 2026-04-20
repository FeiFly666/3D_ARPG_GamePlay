using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TargetSystem : MonoBehaviour
{
    public float maxDistance = 15f;
    public float fovAngle = 150f;

    public LayerMask enemyLayer;
    public LayerMask obstacleLayer;

    private List<Transform> _targets = new List<Transform>();

    private CharacterBase _owner;

    [SerializeField] private CharacterBase _currentTarget;

    private Collider[] _colliderBuffer = new Collider[100];

    private Camera _mainCam;

    //可选初始化
    public void Init(float maxDis, float angle, LayerMask enemy, LayerMask obstacle)
    {
        maxDistance = maxDis;
        fovAngle = angle;
        enemyLayer = enemy;
        obstacleLayer = obstacle;
    }
    //初始化
    public void Init(LayerMask enemy, LayerMask obstacle, CharacterBase owner)
    {
        enemyLayer = enemy;
        obstacleLayer = obstacle;

        _owner = owner;
    }
    public void ChangeMaxDistance(float maxDistance)
    {
        this.maxDistance = maxDistance;
    }

    public void ClearTarget()
    {
        _targets.Clear();
        _currentTarget = null;
    }

    public bool IsTargetVaild()
    {
        if (_currentTarget == null) return false;

        if(_currentTarget.currentState == CharacterState.Dead) return false;

        return (_currentTarget.transform.position - _owner.transform.position).sqrMagnitude <= maxDistance * maxDistance;
    }

    /// <summary>
    /// 寻找视野内最接近屏幕中心的敌人
    /// </summary>
    public CharacterBase FindBestTarget()
    {
        UpdateTargets();
        //if(_currentTarget != null) return _currentTarget;
        if (_targets.Count == 0) return null;

        CharacterBase bestTarget = null;
        float minDistance = float.MaxValue;

        for(int i = 0; i < _targets.Count; i++)
        {
            Transform target = _targets[i];
            if (target == null) continue;

            CharacterBase targetNow = target.GetComponent<CharacterBase>();
            if (targetNow == null) continue;

            Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.position);
            Vector2 screenPoint = new Vector2(viewportPos.x, viewportPos.y);

            float dis = (screenPoint - new Vector2(0.5f, 0.5f)).sqrMagnitude;

            if (dis < minDistance)
            {
                minDistance = dis;
                bestTarget = targetNow;
            }
        }
        _currentTarget = bestTarget;

        return bestTarget;
    }

    /// <summary>
    /// 根据方向在视野内切换锁定敌人
    /// </summary>
    public CharacterBase GetNextTarget(float dir)
    {
        UpdateTargets();
        if(_targets.Count <= 1 || _currentTarget == null) return null;

        Vector3 currentScreenPos = Camera.main.WorldToViewportPoint(_currentTarget.transform.position);

        CharacterBase nextTarget = null;
        float closestX = float.MaxValue;

        for (int i = 0; i < _targets.Count; i++)
        {
            Transform target = _targets[i];
            if (target == null) continue;

            CharacterBase targetNow = target.GetComponent<CharacterBase>();
            if (targetNow == null) continue;

            Vector3 viewportPos = Camera.main.WorldToViewportPoint(target.position);
            Vector2 screenPoint = new Vector2(viewportPos.x, viewportPos.y);

            float currentDir = screenPoint.x - currentScreenPos.x;

            if (currentDir * dir < 0)
            {
                if (Mathf.Abs(currentDir) < closestX)
                {
                    closestX = Mathf.Abs(currentDir);
                    nextTarget = targetNow;
                }
            }
        }
        if(nextTarget != null) _currentTarget = nextTarget;

        return nextTarget;
    }

    private void UpdateTargets()
    {
        _targets.Clear();

        if(_mainCam == null) _mainCam = Camera.main;

        int num = Physics.OverlapSphereNonAlloc(transform.position, maxDistance, _colliderBuffer, enemyLayer);

        for(int i = 0; i< num; i++)
        {
            Collider collider = _colliderBuffer[i];

            Transform t = collider.transform;

            if(t == this.transform) continue;

            Vector3 origin = _owner.characterType == CharacterType.Player?_mainCam.transform.position : _owner.transform.position;
            Vector3 forward = _owner.characterType == CharacterType.Player ? _mainCam.transform.forward : _owner.transform.forward;


            Vector3 dirToTarget = (t.position - origin).normalized;

            if (Vector3.Angle(forward, dirToTarget) > fovAngle / 2) continue;

            if(_owner.characterType == CharacterType.Player)
            {
                Vector3 viewportPos = _mainCam.WorldToViewportPoint(t.position);

                bool onScreen = viewportPos.z > 0 && viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1 && viewportPos.z > 0;
                if (!onScreen) continue;
            }

            bool blocked = Physics.Linecast(transform.position + Vector3.up, t.position + Vector3.up, obstacleLayer);

            if (!blocked)
            {
                _targets.Add(t);
            }
        }
    }

    //private Vector3 dirToAttacker(Vector3 targetPos) => (targetPos - transform.position).normalized;
}
