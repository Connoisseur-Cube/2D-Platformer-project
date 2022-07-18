using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]private Transform player;
    [SerializeField]private float offsetx = 0.5f;
    [SerializeField]private float offsety = 0.5f;


    private void Update()
    {
        transform.position = new Vector3(player.position.x +offsetx, player.position.y + offsety, transform.position.z);
    }
}
