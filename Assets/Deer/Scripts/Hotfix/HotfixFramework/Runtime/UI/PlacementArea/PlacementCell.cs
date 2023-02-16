using System;
using System.Collections;
using System.Collections.Generic;
using Flower;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum PlacementCellState
{
    Filled,
    Empty
}

public enum CarFitStatus
{
    Fits,
    Overlaps,
    OutOfBounds
}

public class PlacementCell : MonoBehaviour
{
    public IntVector2 CellIndex { get;private set; }
    private RectTransform mCellTransform;
    private Image mCellImage;
    public TextMeshProUGUI mIndexText;
    public Color EmptyColor = Color.gray;
    public Color FillColor = Color.red;

    private void Awake()
    {
        mCellImage = transform.GetComponent<Image>();
        mCellTransform = mCellImage.transform as RectTransform;
    }

    public void SetState(PlacementCellState tempState)
    {
        switch (tempState)
        {
            case PlacementCellState.Empty:
            {
                mCellImage.color = EmptyColor;
                break;
            }
            case PlacementCellState.Filled:
            {
                mCellImage.color = FillColor;
                break;
            }
        }
    }

    public void SetCellSize(float tempSize)
    {
        mCellTransform.sizeDelta = new Vector2(tempSize, tempSize);
    }

    public void SetCellIndex(IntVector2 tempIndex)
    {
        CellIndex = tempIndex;
        mIndexText.text = $"{tempIndex.x},{tempIndex.y}";
    }
}
