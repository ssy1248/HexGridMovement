using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField]
    private HexGrid hexGrid;

    [SerializeField]
    private MovementSystem movementSystem;

    public bool PlayersTurn { get; private set; } = true;

    [SerializeField]
    private Unit selectedUnit;
    private Hex previouslySelectedHex;

    public void HandleUnitSelected(GameObject unit)
    {
        if (PlayersTurn == false)
            return;

        Unit unitReference = unit.GetComponent<Unit>();

        if (CheckIfTheSameUnitSelected(unitReference))
            return;

        PrepareUnitForMovement(unitReference);
    }

    public void HandleTerrainSelected(GameObject hexGo)
    {
        if(selectedUnit == null || PlayersTurn == false)
        {
            return;
        }

        Hex selectedHex = hexGo.GetComponent<Hex>();

        if (HandleHexOutOfRange(selectedHex.HexCoords) || HandleSelectedHexIsUnitHex(selectedHex.HexCoords))
            return;
        
        HandleTargetHexSelected(selectedHex);
    }

    private void HandleTargetHexSelected(Hex selectedHex)
    {
        if(previouslySelectedHex == null || previouslySelectedHex != selectedHex)
        {
            previouslySelectedHex = selectedHex;
            movementSystem.ShowPath(selectedHex.HexCoords, this.hexGrid);
        }
        else
        {
            movementSystem.MoveUnit(selectedUnit, this.hexGrid);
            PlayersTurn = false;
            selectedUnit.MovementFinished += ResetTurn;
            ClearOldSelection();
        }
    }

    private void ResetTurn(Unit selectedUnit)
    {
        selectedUnit.MovementFinished -= ResetTurn;
        PlayersTurn = true;
    }

    private bool HandleSelectedHexIsUnitHex(Vector3Int hexPosition)
    {
        if(hexPosition == hexGrid.GetClosestHex(selectedUnit.transform.position))
        {
            selectedUnit.Deselect();
            ClearOldSelection();
            return true;
        }
        return false;
    }

    private bool HandleHexOutOfRange(Vector3Int hexPosition)
    {
        if(movementSystem.IsHexInRange(hexPosition) == false)
        {
            Debug.Log("Hex Out of renge!");
            return true;
        }
        return false;
    }

    private void PrepareUnitForMovement(Unit unitReference)
    {
        if(this.selectedUnit != null)
        {
            ClearOldSelection();
        }

        this.selectedUnit = unitReference;
        this.selectedUnit.Select();
        movementSystem.ShowRange(this.selectedUnit, this.hexGrid);
    }

    private bool CheckIfTheSameUnitSelected(Unit unitReference)
    {
        if(this.selectedUnit == unitReference)
        {
            ClearOldSelection();
            return true;
        }
        return false;
    }

    private void ClearOldSelection()
    {
        previouslySelectedHex = null;
        this.selectedUnit.Deselect();
        movementSystem.HideRange(this.hexGrid);
        this.selectedUnit = null;
    }
}
