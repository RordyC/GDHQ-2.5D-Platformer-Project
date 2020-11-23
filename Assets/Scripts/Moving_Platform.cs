using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _positions = null;

    private Transform _targetPos;

    private int _targetPosIndex = 0;

    [SerializeField]
    private float _speed = 5f;

    private WaitForSeconds _platformDelay = new WaitForSeconds(1f);

    private void Start()
    {
        _targetPos = _positions[0];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_targetPos != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos.position, Time.deltaTime * _speed);

            if (transform.position == _targetPos.position)
            {
                _targetPos = null;
                StartCoroutine(ArrivedAtTargetRoutine());
            }
        }
    }

    IEnumerator ArrivedAtTargetRoutine()
    {
        yield return _platformDelay;
        _targetPosIndex++;

        if (_targetPosIndex == _positions.Count)
        {
            _positions.Reverse();
            _targetPosIndex = 1;
        }

        _targetPos = _positions[_targetPosIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.transform.parent = this.transform;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            other.transform.parent = null;
    }
}
