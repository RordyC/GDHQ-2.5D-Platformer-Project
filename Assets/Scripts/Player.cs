using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private enum MovementState {Default,GrabbingLedge, ClimbingLadder}

    [SerializeField]
    private MovementState _movementState = MovementState.Default;

    [SerializeField]
    private float _gravity = 9.81f;

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _climbSpeed = 2.5f;

    [SerializeField]
    private float _jumpHeight = 5f;

    [SerializeField]
    private Vector3 _direction;

    [SerializeField]
    private bool _rolling = false;

    [SerializeField]
    private float _rollDirection = 0;

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
    private bool _climbedLadder = false;

    [SerializeField]
    private Vector3 _grabPos;

    [SerializeField]
    private Vector3 _ladderClimbPos;

    [SerializeField]
    private float _maxYPos, _minYPos;

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

        if (Input.GetKeyDown(KeyCode.LeftShift) && _controller.isGrounded== true && _movementState == MovementState.Default && _rolling == false)
        {
            _playerAnimator.SetTrigger("Roll");
            StartCoroutine(RollRoutine());
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
        else if (_movementState == MovementState.ClimbingLadder)
        {
            CalculateLadderMovement();
        }
    }

    private void CalculateMovement()
    {
        if (_controller.isGrounded == true)
        {
            //_controller.Move(Vector3.down * _gravity * Time.deltaTime);
            float horizontal = Input.GetAxis("Horizontal");
            if (_rolling == true)
                horizontal = _rollDirection;

            _direction = new Vector3(0, 0, horizontal);
            _playerAnimator.SetFloat("Speed", Mathf.Abs(horizontal));

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_direction.z != 0)
                {
                    _playerAnimator.SetTrigger("RunningJump");
                    _direction.y += _jumpHeight;
                }
                else
                {
                    _playerAnimator.SetTrigger("Jump");
                    _direction.y += _jumpHeight;
                }
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

    private void CalculateLadderMovement()
    {
        if (_ladderClimbPos != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, _ladderClimbPos, (_climbSpeed/2) * Time.deltaTime);

            if (transform.position == _ladderClimbPos)
            {
                _ladderClimbPos = Vector3.zero;
            }
            _playerAnimator.SetFloat("ClimbSpeed", 1);
        }
        else
        {
            float vertical = Input.GetAxis("Vertical");
            if (transform.position.y >= _minYPos && transform.position.y <= _maxYPos)
            {
                Vector3 direction = new Vector3(0, vertical * _climbSpeed * Time.deltaTime, 0);
                transform.position += direction;

                if (transform.position.y > _maxYPos)
                    transform.position = new Vector3(transform.position.x, _maxYPos, transform.position.z);

                if (transform.position.y < _minYPos)
                    transform.position = new Vector3(transform.position.x, _minYPos, transform.position.z);
            }

            if (transform.position.y > _minYPos && transform.position.y < _maxYPos)
            {
                _playerAnimator.SetFloat("ClimbSpeed", vertical);
            }
            else
            {
                _playerAnimator.SetFloat("ClimbSpeed", 0);
            }

            if (Input.GetKeyDown(KeyCode.E) && _climbedLadder == false && transform.position.y == _maxYPos)
            {
                _playerAnimator.SetTrigger("ClimbLadder");
                transform.position += new Vector3(0, 2, 0);
                _climbedLadder = true;
            }
        }
    }

    IEnumerator RollRoutine()
    {
        if (_playerModel.transform.rotation.y == 0)
        {
            _rollDirection = 1;
        }
        else
        {
            _rollDirection = -1;
        }
        yield return new WaitForSeconds(0.11f);
        _rolling = true;
        yield return new WaitForSeconds(1f);
        _rollDirection = 0;
        _rolling = false;
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

    public void GrabbingLadder(float maxY, float minY)
    {
        if (_movementState == MovementState.ClimbingLadder
            || _direction.z < 0
            || _controller.isGrounded == false
            || _playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Running Jump")
            || _rolling == true)
        {
            return;
        }

        _playerAnimator.SetTrigger("GrabLadder");

        _maxYPos = maxY;
        _minYPos = minY;
        _controller.enabled = false;

        _ladderClimbPos = transform.position + new Vector3(0, 2.4f, 0);

        _movementState = MovementState.ClimbingLadder;
    }

    public void GrabbedLadder()
    {
        transform.position -= new Vector3(0, 2.4f, 0);
    }

    public void FinishedClimbingLadder()
    {
        transform.position += new Vector3(0, 4.75f, 2.7f);
        _climbedLadder = false;     
        _playerAnimator.SetFloat("Speed", 0);
        _controller.enabled = true;
        _gravity = 9.81f;
        _movementState = MovementState.Default;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collectable")
        {
            Destroy(other.gameObject);
        }
    }
}
