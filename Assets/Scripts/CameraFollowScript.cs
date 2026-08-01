using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void CameraFollowPlayer()
    {
        transform.position = new Vector3 (player.position.x + 0, 0, -10);
    }
    
    // Update is called once per frame
    void Update()
    {
        CameraFollowPlayer();
    }
}
