using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    private bool cursorLocked = false;

    void Update()
    {
        // Travar o cursor apenas no modo House
        if (GameStateManager.Instance.CurrentState == InputState.House)
        {
            if (!cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                cursorLocked = true;
            }

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            horizontalRotation += mouseX;
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

            transform.rotation = Quaternion.Euler(verticalRotation, horizontalRotation, 0f);
        }
        else
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                cursorLocked = false;
            }
        }
    }
}
