using UnityEngine;

public class PlayerPCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "TargetPlatform")
        {
            EventManager.TriggerEvent("platformFound");
        }
    }
}
