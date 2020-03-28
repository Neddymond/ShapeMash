using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTracker : MonoBehaviour
{
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    private int swipeResist = 1;

    public bool isMatched = false;

    private Board board;

    private GameObject otherDot;

    public float swipeAngle = 0;
    void Start()
    {
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;

        column = targetX;
        row = targetY;

        previousColumn = column;
        previousRow = row;

        board = FindObjectOfType<Board>();
    }
    
    void Update()
    {
        FindMatches(); 
        if (isMatched)
        {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            sprite.color = new Color(0, 0, 0, 2f);
        }

        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the target in the horizontal direction
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);

            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move towards the target in the vertical direction
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);

            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    /// <summary>
    /// This is called when a mouse button is pressed while over the GUI element or collider
    /// </summary>
    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        print(firstTouchPosition);
    }

    /// <summary>
    /// This is called when the mouse button is released
    /// </summary>
    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x -
            firstTouchPosition.x) * 180 / Mathf.PI;
            print(swipeAngle);
            MovePieces();
        }
    }

    /// <summary>
    /// Moves the Dots in up, right, down or left direction depending on the swipe angle
    /// </summary>
    private void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            // Right swipe
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<DotTracker>().column -= 1;
            column += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            // Up swipe
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<DotTracker>().row -= 1;
            row += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            // Left swipe
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<DotTracker>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            // Down swipe
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<DotTracker>().row += 1;
            row -= 1;
        }

        StartCoroutine(ResetPositionIfNoMatchIsDetected());
    }

    /// <summary>
    /// Checks if the surrounding gameObjects have the same tag with THIS gameObject, 
    /// and if it does, set isMatched to true.
    /// </summary>
    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot = board.allDots[column - 1, row];
            GameObject rightDot = board.allDots[column + 1, row];

            if (leftDot != null && rightDot != null)
            {
                if (leftDot.tag == this.gameObject.tag && rightDot.tag == this.gameObject.tag)
                {
                    leftDot.GetComponent<DotTracker>().isMatched = true;
                    rightDot.GetComponent<DotTracker>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot = board.allDots[column, row + 1];
            GameObject downDot = board.allDots[column, row - 1];

            if (upDot != null && downDot != null )
            {
                if (upDot.tag == gameObject.tag && downDot.tag == gameObject.tag)
                {
                    upDot.GetComponent<DotTracker>().isMatched = true;
                    downDot.GetComponent<DotTracker>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    /// <summary>
    /// If no match is detected between swiped Dots, reset their position to their previous position
    /// </summary>
    /// <returns></returns>
    public IEnumerator ResetPositionIfNoMatchIsDetected()
    {
        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<DotTracker>().isMatched)
            {
                otherDot.GetComponent<DotTracker>().row = row;
                otherDot.GetComponent<DotTracker>().column = column;

                row = previousRow;
                column = previousColumn;
            }
                        board.DestroyMatches();
            otherDot = null;
        }
    }
}
