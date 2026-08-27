using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI jumpKey;
    [SerializeField] private TextMeshProUGUI dashKey;
    [SerializeField] private TextMeshProUGUI crouchKey;

    [SerializeField] private PlayerActions player;

    private void Update()
    {
        jumpKey.text = "Jump Key: " + player.GetJumpKey();
        dashKey.text = "Dash Key: " + player.GetDashKey();
        crouchKey.text = "Crouch Key: " + player.GetCrouchKey();
    }
}
