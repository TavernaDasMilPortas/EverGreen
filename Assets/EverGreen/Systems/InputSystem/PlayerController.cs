using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private MoveChanPhisical mover;

    private void Awake()
    {
        Instance = this;
        mover = GetComponent<MoveChanPhisical>();
    }

    public void Move(float h, float v)
    {
        mover.SetMoveInput(h, v);
    }

    public void Jump()
    {
        mover.TriggerJump();
    }

    public void Attack()
    {
        mover.TriggerAttack();
    }
}

