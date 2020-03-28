using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    //private BackgroundTile[,] allTiles;

    public GameObject[] dots;
    public GameObject[,] allDots;
    public GameObject tilePrefab;
     
    void Start()
    {
        //allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }

    /// <summary>
    /// Sets up the grid and instantiates the color in the grid box
    /// </summary>
    private void Setup()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Grids
                Vector2 gridPosition = new Vector2(i, j);
                GameObject backGroundTile = Instantiate(tilePrefab, gridPosition, Quaternion.identity) as GameObject;
                backGroundTile.transform.parent = this.transform;
                backGroundTile.transform.name = $"( {i}, {j} )";

                // Dots
                int randomDots = Random.Range(0, dots.Length);

                int maxIterations = 0;

                // On game start, ensure that there are no 3 or more matching column or rows
                while (FindMatchesAt(i, j, dots[randomDots]) && maxIterations < 100)
                {
                    randomDots = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dotToInstantiate = Instantiate(dots[randomDots], gridPosition, Quaternion.identity) as GameObject;
                dotToInstantiate.transform.parent = this.transform;
                dotToInstantiate.name = $"( {i}, {j} )";

                // Populate the 2D array
                allDots[i, j] = dotToInstantiate;
            }
        }
    }

    /// <summary>
    /// Return true if the game objects in the first and second column or row  have the same tag with gameobject
    /// to be instantiated
    /// </summary>
    /// <param name="column">Represents the horizontal grid</param>
    /// <param name="row">Represents the vertical grid</param>
    /// <param name="currentDot">The game object to be instantiated</param>
    /// <returns></returns>
    private bool FindMatchesAt(int column, int row, GameObject currentDot)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == currentDot.tag && allDots[column - 2, row].tag == currentDot.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == currentDot.tag && allDots[column, row - 2].tag == currentDot.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == currentDot.tag && allDots[column, row - 2].tag == currentDot.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == currentDot.tag && allDots[column - 2, row].tag == currentDot.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// If there is a match, destroy the gameObject at this position and set it to null
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
   private void DestroyMatchesAt(int column, int row)
   {
        if (allDots[column, row].GetComponent<DotTracker>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
   }

    /// <summary>
    /// Loop through the board to find a gameObject to destroy
    /// </summary>
    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRows());
    }

    public IEnumerator DecreaseRows()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<DotTracker>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoard());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    int random = Random.Range(0, dots.Length);
                    GameObject instantiatedDot = Instantiate(dots[random], tempPosition, Quaternion.identity);
                    allDots[i, j] = instantiatedDot;
                }
            }
        }
    }

    private bool isDotsMatched()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<DotTracker>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoard()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        while (isDotsMatched())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        
    }
}