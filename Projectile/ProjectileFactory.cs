using UnityEngine;

public class ProjectileFactory : MonoBehaviour {
    
    [SerializeField] private GameObject prefab;
    private bool isEnabled = true;
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

    public void OnPause() {
        isEnabled = false;
    }

    public void OnResume() {
        isEnabled = true;
    }
}

public enum ProjectileDirection {
    UP,
    DOWN
}