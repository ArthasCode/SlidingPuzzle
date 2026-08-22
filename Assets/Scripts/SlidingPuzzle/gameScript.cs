using UnityEngine;
using Unity.MLAgents;
using UnityEditor.Actions;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using System.Linq;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class gameScript : Agent
{
    private Camera _camera = null;

    [SerializeField] private tileScript[] tiles;

    private int emptySpaceIndex;
    int minDistanceInEpisode;
    
    public override void Initialize() {
        _camera = Camera.main;
    }

    public override void OnEpisodeBegin()
    {
        emptySpaceIndex = 8;

        int max_inversions = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("puzzle_difficulty", 10f);
        
        PlaceOnStartPositions();
        Shuffle(max_inversions);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       for (int i = 0; i < tiles.Length; i++)
    {
        float value = (float)tiles[i].number / 8.0f;
        sensor.AddObservation(value);
    }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int act = actions.DiscreteActions[0];   
        int targetIndex = -1;

        switch (act)
            {
                // Penalty for do nothing
                case 0:
                    AddReward(-0.01f);
                    return;

                // Move right
                case 1:
                    if (emptySpaceIndex % 3 != 2) 
                        targetIndex = emptySpaceIndex + 1;

                    break;

                // Move up
                case 2:
                    if (emptySpaceIndex >= 3) 
                        targetIndex = emptySpaceIndex - 3;
                    break;

                // Move left
                case 3:
                    if (emptySpaceIndex % 3 != 0)
                        targetIndex = emptySpaceIndex - 1;
                    break;
                
                // Move down
                case 4: 
                    if(emptySpaceIndex < 6)
                        targetIndex = emptySpaceIndex + 3;
                    break;
            }

        if (targetIndex != -1)
        {
            SwapTiles(emptySpaceIndex, targetIndex);

            emptySpaceIndex = targetIndex;
            // Penalty for step
            AddReward(-0.01f);

            if (PuzzleComplete())
            {
                AddReward(1f);
                EndEpisode();
            }
        }
        else AddReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> action = actionsOut.DiscreteActions;

        action[0] = 0;
        if (Input.GetKeyDown(KeyCode.D))
        {
            action[0] = 1;
        }
         if (Input.GetKeyDown(KeyCode.W))
        {
            action[0] = 2;
        }
         if (Input.GetKeyDown(KeyCode.A))
        {
            action[0] = 3;
        }
         if (Input.GetKeyDown(KeyCode.S))
        {
            action[0] = 4;
        }
    }

    // void Update() {
    //     if(Input.GetMouseButtonDown(0)){
    //         Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
    //         RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
    //         if(hit)
    //         {
    //             if(Vector2.Distance(emptySpace.position, hit.transform.position)<=2)
    //             {
                    
    //                 Vector2 lastEmptySpacePosition = emptySpace.position;
    //                 tileScript thisTile = hit.transform.GetComponent<tileScript>();
    //                 emptySpace.position = thisTile.targetPosition;
    //                 thisTile.targetPosition = lastEmptySpacePosition;
    //                 int tileIndex = findIndex(thisTile);
    //                 tiles[emptySpaceIndex] = tiles[tileIndex];
    //                 tiles[tileIndex] = null;
    //                 emptySpaceIndex = tileIndex;
    //             }

    //         }
    //     }
    // }
    private void PlaceOnStartPositions()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i].number == 0)
            {
                tileScript emptySpace = tiles[i];
                Vector3 emptySpacePosition = emptySpace.targetPosition;

                tiles[i].targetPosition = tiles[emptySpaceIndex].targetPosition; 
                tiles[i] = tiles[emptySpaceIndex];

                tiles[emptySpaceIndex].targetPosition = emptySpacePosition;
                tiles[emptySpaceIndex] = emptySpace;
                
            }

            if (tiles[i].number == i + 1)
                continue;

            for (int j = i + 1; j < tiles.Length; j++)
            {
                if(tiles[j].number == i + 1)
                {
                    tileScript tile_i = tiles[i];
                    Vector3 tile_iPosition = tile_i.targetPosition;        

                    tiles[i].targetPosition = tiles[j].targetPosition;
                    tiles[i] = tiles[j];

                    tiles[j].targetPosition = tile_iPosition;
                    tiles[j] = tile_i;
                    
                    break;
                }
            }
        }
    }
    private void Shuffle(int max_steps)
    {
        int lastPosIndex = -1;
    
        for(int i = 0; i < max_steps; i++)
        {
            List<int> validNeighbors = GetValidNeighbors(emptySpaceIndex);

            if(lastPosIndex != -1)
                validNeighbors.Remove(lastPosIndex);
            lastPosIndex = emptySpaceIndex;

            int tileIndexToMove = validNeighbors[UnityEngine.Random.Range(0, validNeighbors.Count)];
            
            SwapTiles(emptySpaceIndex, tileIndexToMove);

            emptySpaceIndex = tileIndexToMove; 
        }
    }

    private void SwapTiles(int emptySpaceIndex, int tileIndexToMove)
    {
        
        tileScript tile = tiles[tileIndexToMove];
        Vector3 tilePosition = tile.targetPosition;
        tile.targetPosition = tiles[emptySpaceIndex].targetPosition;
        tiles[emptySpaceIndex].targetPosition = tilePosition;

        tiles[tileIndexToMove] = tiles[emptySpaceIndex];
        tiles[emptySpaceIndex] = tile;
    }

    private static List<int> GetValidNeighbors(int emptySpaceIndex)
    {
        List<int> neighbors = new List<int>();
        
        if(emptySpaceIndex % 3 != 2)
            neighbors.Add(emptySpaceIndex + 1);
    
        if (emptySpaceIndex > 2) 
            neighbors.Add(emptySpaceIndex - 3);

        if (emptySpaceIndex % 3 != 0)
            neighbors.Add(emptySpaceIndex - 1);
    
        if(emptySpaceIndex < 6)
            neighbors.Add(emptySpaceIndex + 3);

        return neighbors;
    }

    private bool PuzzleComplete()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            // Never i + 1 == 9
            if(tiles[i].number != i + 1 && tiles[i].number != 0)
                return false;

        }
        FindFirstObjectByType<ShowMessageUI>()?.ShowMessage();
        return true;
    }
}


