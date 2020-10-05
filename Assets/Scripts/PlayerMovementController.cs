using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private GameObject playerVisuals = null;

    private bool moved;
    private float scale;
    private Vector3 prevPosition;
    private Vector3 lastPosition;

    private readonly int moveUnits = 100;

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
        scale = playerVisuals.GetComponent<Transform>().localScale.x / 4;
        prevPosition = gameObject.GetComponent<Transform>().position;
        lastPosition = prevPosition;
        Controls.Player.Move.performed += ctx => SetMovement(ctx.ReadValue<Vector2>());
        Controls.Player.Move.canceled += ctx => ResetMovement();
    }

    [ClientCallback]
    private void OnEnable() => Controls.Enable();

    [ClientCallback]
    private void OnDisable() => Controls.Disable();

    [Client]
    private void SetMovement(Vector2 movement)
    {
        if (moved) { return; }
        moved = true;

        Vector3 playerPos = transform.position;
        Vector3 newPlayerPos = new Vector3(playerPos.x + scale * movement.x, playerPos.y + scale * movement.y, 0);

        float moveDistance = moveUnits * scale;
        if (Math.Abs(prevPosition.x - newPlayerPos.x) > moveDistance) { return; }
        if (Math.Abs(prevPosition.y - newPlayerPos.y) > moveDistance) { return; }

        bool checkBox = Physics2D.BoxCast(new Vector2(newPlayerPos.x, newPlayerPos.y), new Vector2(scale / 2, scale / 2), 0f, new Vector2(transform.forward.x, transform.forward.y));
        if (checkBox) { return; }

        foreach (GameObject player in Players)
        {
            if (gameObject == player) { continue; }
            if (Vector3.Distance(player.transform.position, newPlayerPos) == 0) { return; }
        }
        lastPosition = transform.position;
        transform.position = newPlayerPos;
    }

    [Client]
    private void ResetMovement() => moved = false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        transform.position = lastPosition;
    }
}

