using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeManager : MonoBehaviour
{
    [Header(" Settings ")]
    [SerializeField] private LayerMask _gridCellMask;

    [Header(" Elements ")]
    private List<GridCell> _updatedCells = new List<GridCell>();

    public static Action<int> OnHexagonsVanished;

    private void Awake()
    {
        StackController.OnStackPlaced += StackPlacedCallback;
    }

    private void OnDestroy()
    {
        StackController.OnStackPlaced -= StackPlacedCallback;
    }

    private void StackPlacedCallback(GridCell gridCell)
    {
        StartCoroutine(StackPlacedCoroutine(gridCell));

    }

    IEnumerator StackPlacedCoroutine(GridCell gridCell)
    {
        _updatedCells.Add(gridCell);
        
        while(_updatedCells.Count > 0)
            yield return CheckForMerge(_updatedCells[0]);
    }

    IEnumerator CheckForMerge(GridCell gridCell)
    {
        _updatedCells.Remove(gridCell);
        if (!gridCell.IsOccupied)
        {
            yield break;
        }

        List<GridCell> neighborGridCells = GetNeighborGridCells(gridCell);

        if (neighborGridCells.Count <= 0)
        {
            Debug.Log("No neighbors for this cell");
            yield break;
        }
        Color gridCellTopHexagonColor = gridCell.Stack.GetTopHexagonColor();
        List<GridCell> similarNeighborGridCells = GetSimilarNeighborGridCells(gridCellTopHexagonColor, neighborGridCells);

        if (similarNeighborGridCells.Count <= 0)
        {
            Debug.Log("No similar neighbor for this cell");
            yield break;
        }

        _updatedCells.AddRange(similarNeighborGridCells);
        List<Hexagon> hexagonsToAdd = GetHexagonsToAdd(similarNeighborGridCells, gridCellTopHexagonColor);
        RemoveHexagonsFromStacks(similarNeighborGridCells, hexagonsToAdd);
        MoveHexagons(gridCell, hexagonsToAdd);

        yield return new WaitForSeconds(0.25f + (hexagonsToAdd.Count + 1) * 0.04f);

        yield return CheckForCompleteStack(gridCell, gridCellTopHexagonColor);
    }
    private List<GridCell> GetNeighborGridCells(GridCell gridCell)
    {
        List<GridCell> neighborGridCells = new List<GridCell>();
        Collider[] neighborGridCellColliders = Physics.OverlapSphere(gridCell.transform.position, 2, _gridCellMask);
        foreach (Collider gridCellCollider in neighborGridCellColliders)
        {
            GridCell neighborGridCell = gridCellCollider.GetComponent<GridCell>();

            if (!neighborGridCell.IsOccupied)
            {
                continue;
            }
            if (neighborGridCell == gridCell)
            {
                continue;
            }
            neighborGridCells.Add(neighborGridCell);
        }

        return neighborGridCells;
    }

    private List<GridCell> GetSimilarNeighborGridCells(Color gridCellTopHexagonColor, List<GridCell> neighborGridCells)
    {
        List<GridCell> similarNeighborGridCells = new List<GridCell>();

        foreach (GridCell neighborGridCell in neighborGridCells)
        {
            Color neighborGridCellTopHexagonColor = neighborGridCell.Stack.GetTopHexagonColor();

            if (gridCellTopHexagonColor == neighborGridCellTopHexagonColor)
            {
                similarNeighborGridCells.Add(neighborGridCell);
            }
        }

        return similarNeighborGridCells;
    }

    private List<Hexagon> GetHexagonsToAdd(List<GridCell> similarNeighborGridCells, Color gridCellTopHexagonColor)
    {
        List<Hexagon> hexagonsToAdd = new List<Hexagon>();
        foreach (GridCell neighborCell in similarNeighborGridCells)
        {
            HexStack neighborCellHexStack = neighborCell.Stack;

            for (int i = neighborCellHexStack.Hexagons.Count - 1; i >= 0; i--)
            {
                Hexagon hexagon = neighborCellHexStack.Hexagons[i];
                if (hexagon.Color != gridCellTopHexagonColor)
                {
                    break;
                }
                hexagonsToAdd.Add(hexagon);
                hexagon.SetParent(null);
            }

        }
        return hexagonsToAdd;
    }

    private void RemoveHexagonsFromStacks(List<GridCell> similarNeighborGridCells, List<Hexagon> hexagonsToAdd)
    {
        foreach (GridCell neighborCell in similarNeighborGridCells)
        {
            HexStack stack = neighborCell.Stack;
            foreach (Hexagon hexagon in hexagonsToAdd)
            {
                if (stack.ContainsHexagon(hexagon))
                {
                    stack.RemoveHexagon(hexagon);
                }
            }

        }
    }

    private void MoveHexagons(GridCell gridCell, List<Hexagon> hexagonsToAdd)
    {
        float initialY = gridCell.Stack.Hexagons.Count * .2f;

        for (int i = 0; i < hexagonsToAdd.Count; i++)
        {
            Hexagon hexagon = hexagonsToAdd[i];

            float targetY = initialY + i * .2f;
            Vector3 targetLocalPosition = Vector3.up * targetY;
            gridCell.Stack.AddHexagon(hexagon);

            float soundDelay = i * 0.03f;
            
            if (AudioService.Instance != null)
            {
                StartCoroutine(PlayMergeSoundWithDelay(soundDelay));
            }

            hexagon.MoveToLocal(targetLocalPosition);
        }
    }

    private IEnumerator PlayMergeSoundWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (AudioService.Instance != null)
        {
            AudioService.Instance.PlayHexagonMerge();
        }
    }

    private IEnumerator CheckForCompleteStack(GridCell gridCell, Color gridCellTopHexagonColor)
    {
        if (gridCell.Stack.Hexagons.Count < 10)
        {
            yield break;
        }

        List<Hexagon> similarHexagons = new List<Hexagon>();

        for (int i = gridCell.Stack.Hexagons.Count - 1; i >= 0; i--)
        {
            Hexagon hexagon = gridCell.Stack.Hexagons[i];

            if (hexagon.Color != gridCellTopHexagonColor)
            {
                break;
            }

            similarHexagons.Add(hexagon);
        }

        int similarHexagonCount = similarHexagons.Count;

        if (similarHexagons.Count < 10)
        {
            yield break;
        }

        if (AudioService.Instance != null)
            AudioService.Instance.PlayHexagonVanish();

        float delay = 0;
        int hexagonCount = similarHexagons.Count;

        while (similarHexagons.Count > 0)
        {
            similarHexagons[0].SetParent(null);
            similarHexagons[0].Vanish(delay);
            //DestroyImmediate(similarHexagons[0].gameObject);


            delay += 0.01f;

            gridCell.Stack.RemoveHexagon(similarHexagons[0]);
            similarHexagons.RemoveAt(0);
        }

        OnHexagonsVanished?.Invoke(hexagonCount);

        _updatedCells.Add(gridCell);
        yield return new WaitForSeconds(0.25f + (similarHexagonCount + 1) * 0.02f);
    }
}
