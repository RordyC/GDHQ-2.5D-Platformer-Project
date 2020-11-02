using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabChecker : MonoBehaviour
{
    private Player _player;

    [SerializeField]
    private Vector3 _handPos;

    [SerializeField]
    private Transform _handPosition;

    private void Start()
    {
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is NULL!");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LedgeGrabChecker")
        {
            _player.GrabbedLedge(_handPosition.position);
        }
    }
}
