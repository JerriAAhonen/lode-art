using UnityEngine;

public class MouseLook : MonoBehaviour
{
	[SerializeField] private float sensitivity;
	
	private InputManager inputManager;
	private PlayerMovement movement;
	private Transform tm;

	private float xRot;
	private float yRot;

	private bool IsReady { get; set; }
	
	public void Init(PlayerMovement movement)
	{
		inputManager = InputManager.I;
		tm = transform;

		this.movement = movement;

		IsReady = true;
	}

	private void Update()
	{
		if (!IsReady) return;

		var delta = inputManager.LookInput;
		delta *= sensitivity;
		var yaw = delta.x;
		var pitch = -delta.y;

		xRot += pitch;
		xRot = Mathf.Clamp(xRot, -89, 89);
		
		var targetRot = tm.eulerAngles;
		targetRot.x = xRot;
		tm.eulerAngles = targetRot;
		
		movement.Rotate(yaw);
	}
}