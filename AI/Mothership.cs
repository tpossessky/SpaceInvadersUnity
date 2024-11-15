using System;
using UnityEngine;

public class Mothership : MonoBehaviour
{
    public event Action OnMothershipDestroyed;

    [SerializeField] private float speed = 0.3f;
    
    private Vector2 startingCoords = new(-2.3f, 2.17f);
    private float xMax = 2.37f;
    private bool isEnabled;

    public void OnCreate() {
        gameObject.transform.position = new Vector3(startingCoords.x, startingCoords.y, 0f);
        isEnabled = true;
    }
    void Update()
    {
        if(!isEnabled)
            return;
        
        var xAmt = speed * Time.deltaTime;
        var curPos = gameObject.transform.position;
        var newPos = curPos.x + xAmt;

        if (newPos >= xMax)
        {
            Destroy(gameObject);
            OnMothershipDestroyed?.Invoke();
        }
        else
        {
            gameObject.transform.position = new Vector3(newPos, curPos.y, curPos.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.gameObject.CompareTag("projectile")) 
            return;
        Destroy(gameObject);
        OnMothershipDestroyed?.Invoke();
    }
}