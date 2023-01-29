using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    [SerializeField] private Vector2 _inputDirection = new Vector2(0, 0);
    [SerializeField] private Vector2 _currentDirection = new Vector2(0, 0);
    [SerializeField] private bool _isSnakeMove = true;
    [SerializeField] private float _snakeSpeed = 0.5f;

    [SerializeField] private GameObject _snakeBodyPrefab;
    [SerializeField] private List<GameObject> _snakeBodyList = new();
    [SerializeField] private int _startSnakeBodyCount = 2;

    [SerializeField] private LayerMask _crushLayers;

    private void Awake()
    {
        ServiceLocator.Snake = this;
    }

    private void Start()
    {
        EventBus.AppleCollectEvent += () => AddSnakeBody();
        
        SetStartPosition();
        AddSnakeHead();
        AddSnakeBody(_startSnakeBodyCount);
        StartCoroutine(SnakeMover());
    }

    private void Update()
    {
        InputPlayer();
    }

    private void SetStartPosition()
    {
        transform.position = new Vector2(
            UnityEngine.Random.Range(3, Board.XBlocks - 2), 
            UnityEngine.Random.Range(3, Board.YBlocks - 2)
            );
    }

    private void InputPlayer()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && _currentDirection.x != -1)
        {
            _inputDirection = Vector2.zero;
            _inputDirection.x = 1;
        }

        if (Input.GetAxisRaw("Horizontal") < 0 && _currentDirection.x != 1)
        {
            _inputDirection = Vector2.zero;
            _inputDirection.x = -1;
        }

        if (Input.GetAxisRaw("Vertical") < 0 && _currentDirection.y != 1)
        {
            _inputDirection = Vector2.zero;
            _inputDirection.y = -1;
        }

        if (Input.GetAxisRaw("Vertical") > 0 && _currentDirection.y != -1)
        {
            _inputDirection = Vector2.zero;
            _inputDirection.y = 1;
        }
    }

    private void SnakeMove()
    {
        var newBlockPosition = Vector2.zero;
        var previousBlockPosition = Vector2.zero;
        var currentPosition = transform.position;
        _currentDirection = _inputDirection;

        for (int i = 0; i < _snakeBodyList.Count; i++)
        {
            if (i == 0)
            {
                newBlockPosition = new Vector2(currentPosition.x + _inputDirection.x,
                    currentPosition.y + _inputDirection.y);
            }
            else if (i > 0)
            {
                newBlockPosition = previousBlockPosition;
            }

            previousBlockPosition = _snakeBodyList[i].transform.position;
            _snakeBodyList[i].transform.position = newBlockPosition;
        }
    }

    private IEnumerator SnakeMover()
    {
        while (_isSnakeMove)
        {
            if (_inputDirection != Vector2.zero)
                SnakeMove();

            yield return new WaitForSeconds(_snakeSpeed);
        }
    }

    private void AddSnakeHead()
    {
        if (_snakeBodyList.Count == 0)
            _snakeBodyList.Add(gameObject);
    }

    private void AddSnakeBody(int value = 1)
    {
        for (int i = 0; i < value; i++)
        {
            GameObject newSnakeBodyBlock = Instantiate(_snakeBodyPrefab, new Vector2(-1, -1), Quaternion.identity);
            _snakeBodyList.Add(newSnakeBodyBlock);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((_crushLayers.value & 1 << collision.gameObject.layer) != 0)
        {
            EventBus.GameOverEvent?.Invoke();
            _isSnakeMove = false;
        }
    }

    public List<Vector2> GetSnakeBodyPositionList()
    {
        List<Vector2> tempList = new ();

        foreach (var body in _snakeBodyList)
        {
            tempList.Add(body.transform.position);
        }

        return tempList;
    }
}