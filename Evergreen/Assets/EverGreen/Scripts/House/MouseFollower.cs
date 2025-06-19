using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public float fixedDepth = 5f; // Distância da câmera até o plano do mouse no mundo

    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("Camera.main é nula");
            return;
        }

        // Cria a posição do mouse como 3D (screen X/Y + profundidade Z)
        Vector3 screenMousePos = Input.mousePosition;
        screenMousePos.z = fixedDepth; // Z é a profundidade a partir da câmera

        // Linha visual do centro da câmera até o ponto
        Debug.DrawLine(Camera.main.transform.position, screenMousePos, Color.red);

        Debug.Log("Mouse 2D convertido para mundo: " + screenMousePos);
    }
}
