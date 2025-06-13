using UnityEngine;

public class PlayerReturn : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] BoxCollider[] leafs;

    private bool isPlayerInArea;

    private GameObject player;

    void Start()
    {
        player = FindFirstObjectByType<CharacterMovement>().gameObject;
        startPosition = player.transform.position;
        timer.OnTimerEnded += ReturnToStart;
    }

    void Update()
    {
        bool _isPlayerInArea = false;

        foreach (var leaf in leafs)
        {
            if (leaf != null)
            {
                BoxCollider leafCollider = leaf.GetComponent<BoxCollider>();
                Collider[] colls = Physics.OverlapBox(leaf.transform.position, leafCollider.size, Quaternion.identity, LayerMask.GetMask("Player"));

                foreach (var coll in colls)
                {
                    if (coll.gameObject == player)
                    {
                        _isPlayerInArea = true;
                    }
                }
            }
        }

        isPlayerInArea = _isPlayerInArea;
    }

    void ReturnToStart()
    {
        if (!isPlayerInArea)
        {
            player.GetComponent<CharacterMovement>().motor.SetPosition(startPosition);
        }
    }

    void OnDestroy()
    {
        timer.OnTimerEnded -= ReturnToStart;
    }
}