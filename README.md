# Space Invaders
![image](https://github.com/user-attachments/assets/64a0e30d-69f1-4fdc-94aa-95ceca76eebf)
![image](https://github.com/user-attachments/assets/13a23f2c-9178-416a-9bbf-4b543aaec773)
![image](https://github.com/user-attachments/assets/b168e574-6cec-42aa-a63c-320efaf0f131)

## Description
Big step up from Pong and Breakout. This project required a lot more work and took me a total of about 4 days of work to complete. With this being my 3rd game now, I'd say I'm getting much more comfortable with the engine and the "Unity" way of doing things. This one required some rudimentary AI behavior, sprite management, a lot more state management, and a lot more UI interaction. 

I would say the coding style in this one is a bit all over the place as I was trying to expose myself to different ways of doing things. 

## AI 
I went for a dynamic approach in spawning almost everything in. Enemies have a basic script attached to prefabs as well as a Factory class that handles spawning in the grid of enemies. Each enemy has a reference to the ```ProjectileFactory``` in order to request a new projectile. Each enemy has a randomized timer dictating how often they can shoot between 0.4s and 3s. They have a 3% chance to spawn a projectile when their timer is up. 

## Projectiles
There is a single projectile prefab and factory that both the enemies and player use. When either want's to spawn a projectile they request one from the ```ProjectileFactory``` along with a few parameters:

```
    public void requestNewProjectile(Vector3 startingPosition, ProjectileDirection direction, ProjectileType type) {
        if(!isEnabled)
            return;
        
        var newGameObject = Instantiate(prefab);
        var projectile = newGameObject.GetComponent<Projectile>();
        switch (direction) {
            case ProjectileDirection.UP:
                projectile.zRot = 180f;
                projectile.moveDirection = new Vector3(startingPosition.x, 1, 0);
                break;
            case ProjectileDirection.DOWN:
                projectile.zRot = 0f;
                projectile.moveDirection = new Vector3(startingPosition.x, -1, 0);
                break;
        }
        projectile.spawnPosition = startingPosition;
        projectile.isEnabled = true;
        projectile.projectileType = type;
    }
```
The projectile will spawn in the position given to it via this method and its direction is determined by the enum ```ProjectileDirection.UP ProjectileDirection.DOWN ``` and the type ```ProjectileType.Player, ProjectileType.Enemy```

The actual Projectile prefab is responsible for all the logic regarding collision and movement. The most complex of this section is the collision detection. Enemies shouldn't be able to damage other enemies, and the player shouldn't be able to damage itself. Collision between a projectile and an object emits one of 4 UnityEvents. 

```
    [SerializeField] private UnityEvent<GameObject> _onPlayerHit;
    [SerializeField] private UnityEvent<GameObject> _onEnemyHit;
    [SerializeField] private UnityEvent<GameObject> _onShieldHit;
    [SerializeField] private UnityEvent<GameObject> _onMothershipHit;
```
Some of the later code uses C# ```Action``` to emit events happening, but the advantage of using these UnityEvents is you're able to set which method of another class listens for them through the Engine's UI. 

Once a collision occurs, the projectile determines which of these events should be invoked:

```
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
```

The SpaceInvaders.cs class acts as an overall game manager and then determines what to do with the fact that a certain collision happened.  

```
    public void onPlayerHit(GameObject gameObj) {
        lives--;
        playerController.PlayerDeath();
        if (lives > 0) {
            isPlayerDeathTimerRunning = true;
            uiManager.removeLife();
        }
        else {
            gameOver();
        }
    }
    public void onEnemyHit(GameObject gameObj) {
        gameObj.GetComponent<Enemy>().onDeath();
        score += 100; 
        uiManager.setScore(score);
    }
    public void onShieldHit(GameObject gameObj) {
        gameObj.GetComponent<Shield>().TakeDamage();
    }
    public void onMothershipHit(GameObject gameObj) {
        gameObj.GetComponent<Mothership>();
        score += 300; 
        uiManager.setScore(score);
    }
```
## GameManager (SpaceInvaders.cs)

This class grew in scope to be much larger than was originally intended and I found myself throughout this development wanting to refactor it into a few different classes but ended up deciding against it due to the overall small scope of this project. The GameManager is responsible for: 

- Moving the enemy grid
- Telling the UI what to display
- Collision handling
- Starting the game
- Checking for game over cases
- Loading the next level when all enemies were defeated.
- Communicating to save/load high scores

I'll take the urge to refactor this as learnings for going forward and keeping in line with the standard programming doctrine of separation of concerns across classes. 

## Saving High Scores 

I added a rudimentary save system that will keep the top 5 high scores of all time similar to how an arcade machine would. The data model for a game like this was extremely simple: 

```
[Serializable]
public class HighScore {
    public int score;
    public string playerInitials;

    public HighScore(int sc, string inits) {
        score = sc;
        playerInitials = inits;
    }
}
```
When saving, I serialize this class into JSON for easy decoding. I figured even though not necessary for something as simple as a String and Int value, it would be good practice to learn for later, more complex projects.

## What I Would Have Done Differently

- I think the biggest thing I noticed was it would have been nice to have a centralized CollisionController class. Centralizing this I could've communicated collisions much easier to the game manager and it would've helped keep the code cleaner. 
- Event/Action bus. Another thing that would've helped is centralizing the communication between components. I imagine for more complex games with more concurrency, queuing events would be necessary to ensure the desired order of execution is maintained.

## Conclusion
Overall, I'm still happy with how this came out and I think it represents the original game fairly well.
