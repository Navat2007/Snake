using System;
using UnityEngine;

public class Apple : MonoBehaviour
{
    
    public event Action<int> OnCollectApple;
    public int Index { get; set; }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectApple();
    }

    private void CollectApple()
    {
        OnCollectApple?.Invoke(Index);
    }
}
