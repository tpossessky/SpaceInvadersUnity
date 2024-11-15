# Space Invaders
![image](https://github.com/user-attachments/assets/64a0e30d-69f1-4fdc-94aa-95ceca76eebf)
![image](https://github.com/user-attachments/assets/13a23f2c-9178-416a-9bbf-4b543aaec773)
![image](https://github.com/user-attachments/assets/b168e574-6cec-42aa-a63c-320efaf0f131)

## Description
Big step up from Pong and Breakout. This project required a lot more work and took me a total of about 4 days of work to complete. With this being my 3rd game now, I'd say I'm getting much more comfortable with the engine and the "Unity" way of doing things. This one required some rudimentary AI behavior, sprite management, a lot more state management, and a lot more UI interaction. 

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
