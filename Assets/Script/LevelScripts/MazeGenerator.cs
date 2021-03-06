﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generiert zufaellige Irrgarten und arbeitet diese fuer den LevelGenerator auf
public class MazeGenerator : MonoBehaviour
{

    const int breite = 29; // Entspricht der Breite der LevelTextdatein. Darf nicht veraerndert werden
    const int hoehe = 20; // Nur gerade Werte zulaessig!
    private MazeCell[,] Maze;
    private MazeCell current;
    private int[,] mazeDataMap;

    private Stack<MazeCell> backtrack;
    public string[][] mazeLevelData;
    public bool mazeCalculated;

    void Awake()
    {
        mazeCalculated = false;
        Maze = new MazeCell[breite, hoehe];
        backtrack = new Stack<MazeCell>();
    }

    void Start()
    {
        initializeMaze();
        StartCoroutine(generateMaze());
    }

    // Berechnet ein neues Maze
    // mazeCalculated wird am Ende durch generateMazeLevelData() wieder auf true gesetzt um zu signalisieren das es fertig ist
    public void generateNewMaze()
    {
        mazeCalculated = false;
        StopCoroutine(generateMaze());

        initializeMaze();
        StartCoroutine(generateMaze());
    }

    // Initialisiert das Array in dem der Maze berechner wird.
    public void initializeMaze()
    {
        for (int j = 1; j < hoehe; j += 2)
            for (int i = 1; i < breite; i += 2)
            {
                Maze[i, j] = new MazeCell(i, j);
            }
    }

    // Fuehrt die Wegfindung mit Backtracking durch
    // Basiert auf: https://en.wikipedia.org/wiki/Maze_generation_algorithm#Recursive_backtracker
    public IEnumerator generateMaze()
    {
        bool done = false;
        MazeCell next = null;
        current = Maze[15, 1];
        current.visited = true;

        while (!done)
        {
            next = checkNeighbors(current);

            if (next != null)
            {
                backtrack.Push(current);
                removeWalls(current, next);
                current = next;
                current.visited = true;

            }
            else if (backtrack.Count > 0)
            {

                current = backtrack.Pop();
            }
            else
            {
                done = true;
            }
            yield return null;
        }

        generateMazeBinaerMap();
        generateMazeLevelData();
    }

    // Wird von generateMaze() aufgerufen um die Waende der einzelnen Cellen auf false zu stellen (wenn keine Wand dort sein darf)
    private void removeWalls(MazeCell current, MazeCell next)
    {
        int x = current.x - next.x;
        if (x == 2)
        {
            current.wall[3] = false;    //left
            next.wall[1] = false;       //right
        }
        else if (x == -2)
        {
            current.wall[1] = false;    //right
            next.wall[3] = false;       //left
        }

        int y = current.y - next.y;
        if (y == 2)
        {
            current.wall[2] = false;    //bottom
            next.wall[0] = false;       //top
        }
        else if (y == -2)
        {
            current.wall[0] = false;    //top
            next.wall[2] = false;       //bottom
        }

        //Entfernt die Waende in der ersten und letzten Y-Reihe, damit in das Maze hinein und rausgelaufen werden kann
        if (current.y == 1)
        {
            current.wall[2] = false;
        }
        else if (current.y == hoehe - 1)
        {
            current.wall[0] = false;
        }

    }

    private MazeCell checkNeighbors(MazeCell MazeCell)
    {
        List<MazeCell> neighbors = new List<MazeCell>();

        MazeCell top = null;
        MazeCell right = null;
        MazeCell bottom = null;
        MazeCell left = null;

        if (current.y + 2 < hoehe)
            top = Maze[current.x, current.y + 2];

        if (current.x + 2 < breite)
            right = Maze[current.x + 2, current.y];

        if (current.y - 2 > 0)
            bottom = Maze[current.x, current.y - 2];

        if (current.x - 2 > 0)
            left = Maze[current.x - 2, current.y];

        if (top != null && !top.visited)
            neighbors.Add(top);

        if (right != null && !right.visited)
            neighbors.Add(right);

        if (bottom != null && !bottom.visited)
            neighbors.Add(bottom);

        if (left != null && !left.visited)
            neighbors.Add(left);

        if (neighbors.Count > 0)
        {
            int pick = (int)Random.Range(0f, neighbors.Count);
            return neighbors[pick];
        }
        else
        {
            return null;
        }
    }


    // Das Maze-Array wird in eine mazeDataMap "umgerechnet" damit diese in das gleiche Format
    // welches die Textdatein aufweisen umgewandelt werden kann (wird von generateMazeLevelData() gemacht)
    // Die "Umrechnung" defeniert an welchen Stellen im Array final Waende generiert werden muessen und wo nicht
    private void generateMazeBinaerMap()
    {
        MazeCell current;
        mazeDataMap = new int[breite + 1, hoehe + 1];

        for (int j = 0; j < hoehe; j++)
        {
            for (int i = 0; i < breite; i++)
            {
                current = Maze[i, j];

                if (current == null)
                {
                    mazeDataMap[i, j] = 0;

                }
                else if (current.visited)
                {

                    mazeDataMap[i, j] = 1;

                    for (int w = 0; w < 4; w++)
                    {
                        if (current.wall[w])
                        {
                            // Generiere eine Wand wenn wall = true ergibt
                            switch (w)
                            {
                                case 0: mazeDataMap[i, j + 1] = 2; break; // top
                                case 1: mazeDataMap[i + 1, j] = 2; break; // right
                                case 2: mazeDataMap[i, j - 1] = 2; break; // bottom
                                case 3: mazeDataMap[i - 1, j] = 2; break; // left
                            }
                        }
                        else
                        {
                            // Generiere einen Gang wenn wall = false ergibt
                            switch (w)
                            {
                                case 0: mazeDataMap[i, j + 1] = 1; break; // top
                                case 1: mazeDataMap[i + 1, j] = 1; break; // right
                                case 2: mazeDataMap[i, j - 1] = 1; break; // bottom
                                case 3: mazeDataMap[i - 1, j] = 1; break; // left
                            }
                        }
                    }
                }
            }
        }
    }

    // Umwandlung der mazeDataMap in das gleiche "Textformat" welches der LevelGenerator benoetig
    // um den Irrgarten Zeilenweise generieren zu koennen
    private void generateMazeLevelData()
    {
        mazeLevelData = new string[hoehe][];
        for (int j = 0; j < hoehe; j++)
        {
            string[] mazeLine = new string[breite + 2];
            for (int i = 0; i < breite; i++)
            {
                switch (mazeDataMap[i, j])
                {
                    case 0: mazeLine[i + 1] = "x"; break;
                    case 1: mazeLine[i + 1] = "o"; break;
                    case 2: mazeLine[i + 1] = "x"; break;
                    default: break;
                }
            }
            mazeLevelData[j] = mazeLine;
        }
        mazeCalculated = true;
    }

}
