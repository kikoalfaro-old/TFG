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
        gameManager.UpdateCurrentAreaStatus(newStatus);
        //gameManager.SaveGame(); // Se guarda automáticamente cada vez que cambia el estado de un área
        Continue();
    }

    public override Color GetButtonColor()
    {
        return Color.cyan;
    }
}
