using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Animator))]
public class Enemy : MonoBehaviour {
    [SerializeField] private Sprite death;
    [FormerlySerializedAs("renderer")] [SerializeField] private SpriteRenderer enemyRenderer;
    [SerializeField] private Animator animator;

    [SerializeField] private ProjectileFactory projectileFactory;

    private Collider2D enemyCollider;
    private bool isDying;
    public float deathTimer = 0.5f;
    private float curDeathTimer;
    public int shootingChance = 25;
    public float shootTimer = 3f;
    private float curShootTimer; 
    
    void Start() {
        var randShootOffset = Random.Range(0f, 2.6f);
        curShootTimer = randShootOffset; 
        enemyRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    public void onDeath() {
        isDying = true;
        enemyRenderer.sprite = death;
        enemyCollider.enabled = false;
        animator.enabled = false;
    }

    private void handleDeath() {
        if (isDying) {
            curDeathTimer += Time.deltaTime;
        }

        if (curDeathTimer >= deathTimer) {
            Destroy(gameObject);
        }        
    }

    // Update is called once per frame
    void Update() {
        handleDeath();

        // Increment timer
        curShootTimer += Time.deltaTime;

        // Check if cooldown is over
        if (curShootTimer >= shootTimer) {
            // Random chance to shoot, with a lower chanceToShoot percentage reducing frequency
            if (Random.Range(0, 100) < shootingChance) {
                projectileFactory.requestNewProjectile(
                    gameObject.transform.position, 
                    ProjectileDirection.DOWN, 
                    ProjectileType.ENEMY
                );
            }
            // Reset the timer after shooting chance is checked
            curShootTimer = 0;
        }
    }
}