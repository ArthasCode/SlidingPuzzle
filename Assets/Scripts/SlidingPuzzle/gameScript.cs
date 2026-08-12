using UnityEngine;
public class gameScript : MonoBehaviour
{
    private Camera _camera = null;
    [SerializeField] private Transform emptySpace = null;
    [SerializeField] private tileScript[] tiles;
    int emptySpaceIndex = 8;
    

    void Start() {
        _camera = Camera.main;
        
        Shuffle();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)){
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if(hit)
            {
                if(Vector2.Distance(emptySpace.position, hit.transform.position)<=2)
                {
                    
                    Vector2 lastEmptySpacePosition = emptySpace.position;
                    tileScript thisTile = hit.transform.GetComponent<tileScript>();
                    emptySpace.position = thisTile.targetPosition;
                    thisTile.targetPosition = lastEmptySpacePosition;
                    int tileIndex = findIndex(thisTile);
                    tiles[emptySpaceIndex] = tiles[tileIndex];
                    tiles[tileIndex] = null;
                    emptySpaceIndex = tileIndex;
                }

            }
        }
    }

    public void Shuffle()
{
    if(emptySpaceIndex != 8)
        {
            var tileOn8LastPos = tiles[8].targetPosition;
            tiles[8].targetPosition = emptySpace.position;
            emptySpace.position = tileOn8LastPos;
            tiles[emptySpaceIndex] = tiles[8];
            tiles[8] = null;
            emptySpaceIndex = 8;
        }
    int invertion;
    do{
    for(int i = 0; i <= 7; i++)
        {
          
                var lastPos = tiles[i].targetPosition;
                int randomIndex = Random.Range(0, 7);
                tiles[i].targetPosition = tiles[randomIndex].targetPosition;
                tiles[randomIndex].targetPosition = lastPos;
                var tile = tiles[i];
                tiles[i] = tiles[randomIndex];
                tiles[randomIndex] = tile;
            
        }

        invertion = GetInversion();
        Debug.Log("Puzze shuffled!");
    }while (invertion%2 != 0);
        
}

public int findIndex(tileScript ts)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i] != null)
            {
                if(tiles[i] == ts)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    int GetInversion()
    {
        int inversionsSum = 0;
        for (int i = 0; i < tiles.Length; i++)
        {
            int thisTileInvertion = 0;
            for(int j = i; j < tiles.Length; j++)
            {
                if(tiles[j] != null)
                {
                    if(tiles[i].number > tiles[j].number)
                    {
                        thisTileInvertion++;
                    }
                }
            }
            inversionsSum += thisTileInvertion;
        }
        return inversionsSum;
    }

}

