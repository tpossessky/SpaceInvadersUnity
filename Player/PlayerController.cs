using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    
    [SerializeField] private GameObject player;
    private Rigidbody2D controller;
    private SpriteRenderer playerRenderer;
    [SerializeField] private Sprite playerDeathSprite;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private GameObject factoryGameObject;
    private ProjectileFactory projectileFactory;
    
    private Vector2 playerStartingPosition = new(-1.5f, -1.9f);
    private Vector2 playerMoveDirection; // Use Vector2 for 2D movement

    public float maxShotTimer = 0.5f;
    private float curShotTimer;

    [SerializeField] private float speed = 2f;
    
    private bool isEnabled = true;
    
    // Start is called before the first frame update
    private void Start() {
        projectileFactory = factoryGameObject.GetComponent<ProjectileFactory>();
        playerRenderer = gameObject.GetComponent<SpriteRenderer>();
        controller = player.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update() {
        updateShotTimer();
        if(isEnabled)
            playerMovement();
    }

    public void PlayerDeath() {
        playerRenderer.sprite = playerDeathSprite;
        controller.velocity = Vector3.zero;
        isEnabled = false;
    }

    public void PlayerRevive() {
        playerRenderer.sprite = defaultSprite;
        gameObject.transform.position = playerStartingPosition; 
        curShotTimer = 0f;
        isEnabled = true; 
    }
    

    private void playerMovement() {
        var position = player.transform.position;
        var clampedX = Mathf.Clamp(position.x + playerMoveDirection.x * speed * Time.deltaTime, -2.3f, 2.1f);

        controller.velocity = new Vector2(playerMoveDirection.x * speed, controller.velocity.y);
        
        position = new Vector3(clampedX, position.y, position.z);
        player.transform.position = position;
    }

    private void updateShotTimer() {
        if (curShotTimer > 0) {
            curShotTimer -= Time.deltaTime;
        }
    }

    public void OnPlayerShoot(InputAction.CallbackContext callbackContext) {
        if (curShotTimer > 0 || !isEnabled) 
            return;
        
        projectileFactory.requestNewProjectile(player.transform.position, ProjectileDirection.UP, ProjectileType.PLAYER);
        curShotTimer = maxShotTimer;
    }

    public void MovePlayer(InputAction.CallbackContext callbackContext) {
        var input = callbackContext.ReadValue<Vector2>().x;
        playerMoveDirection = new Vector2(input, 0); // Set movement direction as Vector2
    }

    public void OnPause() {
        isEnabled = false;
        controller.velocity = Vector3.zero;
    }

    public void OnResume() {
        isEnabled = true;
    }
    
}