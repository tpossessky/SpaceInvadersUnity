using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldFactory : MonoBehaviour {
    private float[] xPositions = { -1.5f, 0.03f, 1.5f };
    private float yPos = -1.4f;

    [SerializeField] private GameObject prefab;

    public GameObject[] shields = new GameObject[3];

    public Action EnemyShieldCollision; 
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnShields() {
        for(var i = 0; i < xPositions.Length; i++) {
            var gamObj = Instantiate(prefab, new Vector3(xPositions[i], yPos, 0f), Quaternion.identity);
            shields[i] = gamObj;
            var shield = gamObj.GetComponent<Shield>();
            shield.OnEnemyCollision += EnemyCollision;
        }
    }

    public void EnemyCollision() {
        EnemyShieldCollision?.Invoke();
    }
}
