using System;
using UnityEngine;
using UnityEngine.Rendering;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update

    
    public Action OnEnemyCollision;
    private bool enabled;

    public int health = 3;
    private int healthModifier = 5;
    private int totalHP;
    private SpriteRenderer renderer;
    private int curSprite;
    
    //0 - default
    //4 - max damage
    [SerializeField] private Sprite[] shieldSprites;
    void Start() {
        totalHP = health * healthModifier;
        renderer = GetComponent<SpriteRenderer>();
    }
    
    public void TakeDamage() {
        totalHP--;
        if (totalHP % 3 == 0) {
            ShowDamage();    
        }
        if (totalHP == 0) {
            Destroy(gameObject);
        }
    }

    private void ShowDamage() {
        curSprite++;
        if(curSprite < 5)
            renderer.sprite = shieldSprites[curSprite];
    }
}
