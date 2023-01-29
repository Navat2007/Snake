using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AppleSpawner : MonoBehaviour
{
    [SerializeField] private int _maxAppleSpawn = 2;
    [SerializeField] private float _appleSpawnRate = 4f;
    [SerializeField] private List<Apple> _apples = new ();
    [SerializeField] private Apple _applePrefab;

    private void Awake()
    {
        ServiceLocator.AppleSpawner = this;
    }

    private void Start()
    {
        FillApples();
        StartCoroutine(Spawner());
    }

    private void FillApples()
    {
        for (int i = 0; i < _maxAppleSpawn; i++)
        {
            var newApple = Instantiate(_applePrefab, new Vector3(-1, -1, 0), Quaternion.identity, transform);
            newApple.Index = _apples.Count;
            newApple.OnCollectApple += OnCollectApple;
            newApple.gameObject.SetActive(false);
            _apples.Add(newApple);
        }
    }

    private Apple GetApple()
    {
        return _apples.FirstOrDefault(apple => !apple.gameObject.activeSelf);
    }

    private void ReturnApple(int index)
    {
        _apples[index].gameObject.SetActive(false);
    }

    private IEnumerator Spawner()
    {
        while (true)
        {
            if (_apples.FindAll(apple => apple.gameObject.activeSelf).Count < _maxAppleSpawn)
            {
                yield return new WaitForSeconds(_appleSpawnRate);
                Spawn();
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Spawn()
    {
        var apple = GetApple();
        apple.transform.position = GetPositionForApple();
        apple.gameObject.SetActive(true);
    }

    private Vector3 GetPositionForApple()
    {
        var cleanBlocks = new List<Vector2>();
        var appleBlocks = _apples
            .FindAll(apple => apple.gameObject.activeSelf)
            .Select(apple => new Vector2(apple.transform.position.x, apple.transform.position.y)).ToList();
        var snakeBodyList = ServiceLocator.Snake.GetSnakeBodyPositionList();

        for (int i = 1; i < Board.XBlocks; i++)
        {
            for (int j = 1; j < Board.YBlocks; j++)
            {
                if (!snakeBodyList.Contains(new Vector2(i, j)) && !appleBlocks.Contains(new Vector2(i, j)))
                {
                    cleanBlocks.Add(new Vector2(i, j));
                }
            }
        }

        if (cleanBlocks.Count != 0)
        {
            return cleanBlocks[UnityEngine.Random.Range(0, cleanBlocks.Count)];
        }
        else
        {
            Debug.Log("ПОБЕДА!");
            return new Vector2(-1, -1);
        }
    }

    private void OnCollectApple(int index)
    {
        ReturnApple(index);
        EventBus.AppleCollectEvent?.Invoke();
    }
}
