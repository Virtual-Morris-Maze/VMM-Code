using UnityEngine;

public class SimpleBillboardSprite : MonoBehaviour
{
	void Start ()
	{
		
	}
	
	void Update ()
	{
        transform.LookAt(Camera.main.transform.position, -Vector3.up);
    }
}
