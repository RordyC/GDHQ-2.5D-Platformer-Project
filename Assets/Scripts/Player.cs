using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum MovementState {Default,GrabbingLedge}

    [SerializeField]
    private MovementState _movementState = MovementState.Default;

    [SerializeField]
    private float _gravity = 9.81f;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _jumpHeight = 5f;

    [SerializeField]
    private Vector3 _direction;

    private CharacterController _controller;

    private Animator _playerAnimator;

    [SerializeField]
    private GameObject _playerModel = null;

    private WaitForSeconds _oneSecondDelay = new WaitForSeconds(1f);
    private WaitForSeconds _pointSevenDelay = new WaitForSeconds(0.7f);

    //[SerializeField]
    //private Player_Animation _playAnimation = null;

    private float _moveTowardsSpeed = 30f;
    [SerializeField]
    private bool _grabbedLedge = false;

    [SerializeField]
    private Vector3 _grabPos;

    // Start is called before the first frame update
    void Start()
    {
        _controller = transform.GetComponent<CharacterController>();
        _playerAnimator = transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_grabbedLedge && Input.GetKeyDown(KeyCode.E))
        {
            _grabbedLedge = false;
            _moveTowardsSpeed = 0;
            transform.position = transform.position + new Vector3(0, 1.33f, 0);
            _playerAnimator.SetTrigger("Climb");
        }

        if (_movementState == MovementState.GrabbingLedge && _grabPos != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, _grabPos, Time.deltaTime * _moveTowardsSpeed);
            if (transform.position == _grabPos)
            {
                _grabPos = Vector3.zero;
            }
        }
        else if (_movementState == MovementState.Default)
        {
            CalculateMovement();
        }
    }

    private void CalculateMovement()
    {
        if (_controller.isGrounded == true)
        {
            //_controller.Move(Vector3.down * _gravity * Time.deltaTime);
            float horizontal = Input.GetAxis("Horizontal");

            _direction = new Vector3(0, 0, horizontal);
            _playerAnimator.SetFloat("Speed", Mathf.Abs(horizontal));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _playerAnimator.SetTrigger("Jump");
                _direction.y += _jumpHeight;
            }

            Vector3 facing = _playerModel.transform.localEulerAngles;

            if (horizontal > 0)
            {
                facing.y = 0;
            }
            else if (horizontal < 0)
            {
                facing.y = 180;
            }
            _playerModel.transform.localEulerAngles = facing;
        }

        _direction.y -= _gravity * Time.deltaTime;

        _controller.Move(_direction * _speed * Time.deltaTime);
    }

    public void GrabbedLedge(Vector3 snapPos)
    {
        if (_direction.z <= 0)
            return;

        _controller.enabled = false;
        _direction = Vector3.zero;
        _gravity = 0;
        _movementState = MovementState.GrabbingLedge;

        _grabPos = snapPos;
        _playerAnimator.SetTrigger("Grab");

        StartCoroutine(GrabLedgeRoutine(snapPos));
    }

    IEnumerator GrabLedgeRoutine(Vector3 snapPos)
    {
        _moveTowardsSpeed = 30f;
        yield return _oneSecondDelay;
        _moveTowardsSpeed = 4.1f;
        _grabPos = transform.position + new Vector3(0, -1.5f, 0);
        yield return _pointSevenDelay;
        _grabbedLedge = true;
    }

    public void ClimbLedgeComplete()
    {
        transform.position += new Vector3(-0.25f, 6.7f, 1.67f);
        _controller.enabled = true;
        _playerAnimator.SetFloat("Speed", 0);
        _movementState = MovementState.Default;
        _gravity = 9.81f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            Debug.Log("Collected collectable!");
            Destroy(other.gameObject);
        }
    }
}
