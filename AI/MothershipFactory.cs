using UnityEngine;

public class MothershipFactory : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int percentChance = 5;
    private Mothership curShip;

    private bool isEnabled = true;
    private readonly float spawnCheckCooldown = 5f; 
    private float lastCheckTime;

    void Update()
    {
        if (!isEnabled || curShip != null) 
            return;

        if (lastCheckTime >= spawnCheckCooldown)
        {
            if (Random.Range(0f, 100f) < percentChance) {
                lastCheckTime = 0f;
                curShip = Instantiate(prefab).GetComponent<Mothership>();
                curShip.OnCreate();
                curShip.OnMothershipDestroyed += HandleMothershipDestroyed;
            }
            else {
                lastCheckTime = 0f;
            }
        }
        lastCheckTime += Time.deltaTime; 

    }

    private void HandleMothershipDestroyed() {
        curShip = null;
    }

    public void OnPause() {
        isEnabled = false;

        if (curShip != null) {
            Destroy(curShip.gameObject);
            curShip = null;
        }
    }

    public void OnResume() {
        isEnabled = true;
    }
}