using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public const float SPEED = 5f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(horizontal, vertical, 0f);
        transform.Translate(move * SPEED * Time.deltaTime, Space.World);
    }
}
