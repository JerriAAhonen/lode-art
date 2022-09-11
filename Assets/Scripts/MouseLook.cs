using UnityEngine;

public class MouseLook : MonoBehaviour
{
	[SerializeField] private float sensitivity;
	
	private InputManager inputManager;
	private Player player;
	private PlayerMovement movement;
	private Transform tm;

	private float xRot;
	private float yRot;
	
	public bool IsReady { get; private set; }
	
	public void Init(Player player, PlayerMovement movement)
	{
		inputManager = InputManager.I;
		tm = transform;

		this.player = player;
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