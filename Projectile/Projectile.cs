using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour {

    public Vector3 spawnPosition;
    public float zRot;
    public Vector3 moveDirection;
    public float speed = .5f;

    public bool isEnabled;
    public float maxTimer = .6f;
    private float curTimer;
    public ProjectileType projectileType; 
    private Rigidbody2D projectileRigidBody;

    [SerializeField] private UnityEvent<GameObject> _onPlayerHit;
    [SerializeField] private UnityEvent<GameObject> _onEnemyHit;
    [SerializeField] private UnityEvent<GameObject> _onShieldHit;
    [SerializeField] private UnityEvent<GameObject> _onMothershipHit;

    void Start() {
        projectileRigidBody = GetComponent<Rigidbody2D>();
        transform.position = spawnPosition;
        transform.rotation = Quaternion.Euler(0, 0, zRot);
        moveDirection = new Vector3(0, moveDirection.y, 0);
        projectileRigidBody.velocity = moveDirection * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isEnabled)
            updateTimer();
    }

    private void updateTimer() {
        curTimer += Time.deltaTime;
        
        if(curTimer >= maxTimer )
            Destroy(gameObject);
    }



    private void OnTriggerEnter2D(Collider2D other) {
        var gameObj = other.gameObject;

        //if projectile hits an enemy
        if (gameObj.CompareTag("enemy")) {
            //only care if its from the player
            if (projectileType != ProjectileType.PLAYER) 
                return;
            _onEnemyHit?.Invoke(gameObj);
            Destroy(gameObject);
        }
        else if (gameObj.CompareTag("player")) {
            if (projectileType != ProjectileType.ENEMY) 
                return;
            _onPlayerHit?.Invoke(gameObj);
            Destroy(gameObject);
        }
        else if (gameObj.CompareTag("shield")) {
            _onShieldHit?.Invoke(gameObj);
            Destroy(gameObject);
        }
        else if(gameObj.CompareTag("mothership")) {
            _onMothershipHit?.Invoke(gameObj);
            Destroy(gameObject);
        }
        else if(gameObj.CompareTag("boundary")) {
            Destroy(gameObject);
        }
    }
}

public enum ProjectileType {
    PLAYER,
    ENEMY
}