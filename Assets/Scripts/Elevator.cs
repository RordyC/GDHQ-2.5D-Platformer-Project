using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private Transform _pointA, _pointB;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private bool _elevatorStarted = false;

    [SerializeField]
    private bool _canMove = false;

    private WaitForSeconds _elevatorDelay = new WaitForSeconds(5f);
    private WaitForSeconds _elevatorStartDelay = new WaitForSeconds(1f);

    private Transform _target = null;
    // Update is called once per frame
    void Update()
    {
        if (_elevatorStarted && _canMove == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target.position, Time.deltaTime * _speed);
            if (transform.position == _target.position)
            {
                _canMove = false;
                StartCoroutine(ElevatorRoutine());
            }
        }

    }

    public void StartElevator()
    {
        if (_elevatorStarted == false)
        {
            _elevatorStarted = true;
            StartCoroutine(ElevatorRoutine());
        }
    }

    IEnumerator StartElevatorRoutine()
    {
        yield return _elevatorStartDelay;
        _canMove = true;
        _target = _pointB;
    }

    IEnumerator ElevatorRoutine()
    {
        yield return _elevatorDelay;

        if (_target == _pointB)
        {
            _target = _pointA;
        }
        else
        {
            _target = _pointB;
        }
        _canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (_elevatorStarted == false)
                StartElevator();
            other.transform.parent = transform;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            other.transform.parent = null;
    }
}
