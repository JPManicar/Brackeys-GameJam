using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// Can be used to check what type the tile it is, then change vfx / sound / physics based on it (E.g. grass vs ice vs sand, etc)
public class TileChecker : MonoBehaviour
{
    public LayerMask LayerMask;

    public bool IsTouchingTile => tileCollider.IsTouchingLayers(LayerMask);

    public event Action OnHitTile;
    public event Action OnExitTile;

    private Collider2D tileCollider;

    private void Awake()
    {
        tileCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnHitTile?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExitTile?.Invoke();
    }
}
