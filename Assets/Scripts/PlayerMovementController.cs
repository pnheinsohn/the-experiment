using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementController : NetworkBehaviour
{
    private bool moved;
    private Vector3 prevPosition;
    private Vector2 previousInput;

    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }

    private GameObject[] players;
    private GameObject[] Players
    {
        get
        {
            if (players != null) { return players; }
            return players = GameObject.FindGameObjectsWithTag("Player");
        }
    }

    public override void OnStartAuthority()
    {
        moved = true;
        enabled = true;
        prevPosition = gameObject.GetComponent<Transform>().position;
        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]
    private void OnEnable() => Controls.Enable();

    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    [Client]
    private void SetMovement(Vector2 movement) {
        if (moved) { return; }
        moved = true;

        Vector3 playerPos = transform.position;
        Vector3 newPlayerPos = new Vector3(playerPos.x + 50 * movement.x, playerPos.y + 50 * movement.y, 0);

        if (Vector3.Distance(prevPosition, newPlayerPos) > 1) { return; }

        foreach (GameObject player in Players)
        {
            if (gameObject == player) { continue; }
            if (Vector3.Distance(player.transform.position, newPlayerPos) == 0) { return; }
        }

        transform.position = newPlayerPos;
    }

    [Client]
    private void ResetMovement() => moved = false;
}

