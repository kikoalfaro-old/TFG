using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

[CommandInfo("Interaction", "Interact", "Triggers an interaction")]

public class DoInteraction : Command {

    public Interactable interactable;

    public override void OnEnter()
    {
        interactable.Interact();

        Continue();
    }

    public override Color GetButtonColor()
    {
        return new Color32(216, 228, 170, 255);
    }
}
