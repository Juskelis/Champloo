using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class ScaleGridToSpace : MonoBehaviour
{
    [SerializeField]
    private bool constrainHeight = false;
    [SerializeField]
    private bool constrainWidth = false;
    [SerializeField]
    private Vector2 minSize;
    [SerializeField]
    private Vector2 maxSize;

    [SerializeField]
    private bool shrinkHeight = false;
    [SerializeField]
    private bool shrinkWidth = false;

    private float aspectRatio;
    private GridLayoutGroup grid;
    private RectTransform rectTransform;

    private Vector3[] corners;

    private Vector2 initialCellSize;

    private Vector2 rectSize;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        initialCellSize = grid.cellSize;
        aspectRatio = initialCellSize.x / initialCellSize.y;
        rectTransform = GetComponent<RectTransform>();
        corners = new Vector3[4];
    }

    private void Update()
    {
        rectTransform.GetLocalCorners(corners);
        rectSize = new Vector2(
            Vector3.Distance(corners[1], corners[2]),
            Vector3.Distance(corners[0], corners[1]));
        ScaleToHeight(rectSize);
    }

    private void ScaleToHeight(Vector2 size)
    {
        float twoRowHeight = size.y / 2;
        if (twoRowHeight > initialCellSize.y + grid.spacing.y)
        {
            grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
            grid.constraintCount = 2;
            grid.cellSize = new Vector2(twoRowHeight * aspectRatio, twoRowHeight);
        }
        else
        {
            grid.constraint = GridLayoutGroup.Constraint.Flexible;
            grid.constraintCount = 1;
            grid.cellSize = initialCellSize;
        }

        if (shrinkHeight || shrinkWidth)
        {
            int rowCount = grid.constraintCount;
            int colCount = transform.childCount / rowCount;
            float shrunkWidth = shrinkWidth
                ? Mathf.Min(grid.cellSize.x, size.x / colCount)
                : grid.cellSize.x;
            float shrunkHeight = shrinkHeight
                ? Mathf.Min(grid.cellSize.y, size.y / rowCount)
                : grid.cellSize.y;

            //take the smaller shrink
            grid.cellSize = shrunkWidth < shrunkHeight * aspectRatio
                ? new Vector2(shrunkWidth,shrunkWidth/aspectRatio)
                : new Vector2(shrunkHeight*aspectRatio, shrunkHeight);
        }

        if (constrainWidth || constrainHeight)
        {
            grid.cellSize = new Vector2(
                constrainWidth
                    ? Mathf.Clamp(grid.cellSize.x, minSize.x, maxSize.x)
                    : grid.cellSize.x,
                constrainHeight
                    ? Mathf.Clamp(grid.cellSize.y, minSize.y, maxSize.y)
                    : grid.cellSize.y);
        }
    }
}
