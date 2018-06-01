using UnityEngine;
using Fungus;

[CommandInfo("Areas management", "Change area status", "Changes the area state (Unknown, visited, completed)")]
public class AreaStateCommand : Command {

    public AreaStatus newStatus;
    GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    public override void OnEnter()
    {
        Debug.Log("Instance: " + GameManager.Instance);
        Debug.Log("Start reference: " + gameManager);
        gameManager.UpdateCurrentAreaState(newStatus);
        Continue();
    }

    public override Color GetButtonColor()
    {
        return Color.cyan;
    }
}
