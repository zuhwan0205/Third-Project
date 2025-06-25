using Fusion;
using UnityEngine;

public class PlayerInputHandler : NetworkBehaviour
{
    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority) return;

        // 인트로가 끝나야 입력 허용
        if (!GameManager.Instance || !GameManager.Instance.IsInputAllowed) return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Q키 누름");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E키 누름");
        }
    }
}