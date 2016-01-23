using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGenerator : MonoBehaviour {

    public Transform cell;
    public Vector2 gridSize;

    
    
    public static Transform[,] gridArray;
    public GameObject player;
    Vector3 PlayerPos;
    public GameObject topDown;
    public GameObject playerCamera;
    public GameObject room;
    
    public List<GameObject> roomList;
    int succes;
    public int couldNotReachGoal;
    public int numberOfTries;
    public int tries;
    public Transform start;
    public bool roomMerge;
    public int spottedByCameras;
    public int cameraMaximum;

    public float buffer;
    int iter;
	// Use this for initialization
	void Start () {
        couldNotReachGoal = 0;
        Instantiate(player);
        CreateGrid();
        addNeighbours();
        iter = 0;
        createRooms(new Vector2(0, 0), new Vector2(gridSize.x - 1, 0), new Vector2(gridSize.x - 1, gridSize.y - 1), horizontal);
        paintRooms();
        player.transform.position = PlayerPos;
        playerCamera.gameObject.SetActive(false);
        resetVisited();
        createRooms();
        if(roomMerge)
            mergeRooms();
        placeCameras();
        clearCameras();

        if (start.GetComponent<Cell>().kind == Cell.Kind.Room)
        {
            foreach (Transform neigh in start.GetComponent<Cell>().Neighbours)
            {
                if (neigh.GetComponent<Cell>().kind == Cell.Kind.Hall)
                {
                    start = neigh;
                    break;
                }
            }

            PlayerPos = start.position + Vector3.up;
            player.transform.position = PlayerPos;
        }

       
	}

    public void CreateGrid()
    {
        int x = (int)gridSize.x;
        int z = (int)gridSize.y;

        int maxXZ = Mathf.Max(x, z);
        topDown.transform.position = new Vector3 (maxXZ / 2f, maxXZ * buffer, maxXZ / 8f);
        gridArray = new Transform[x, z];
        Transform newCell;
        for (int ix = 0; ix < x; ix++)
        {
            for (int iz = 0; iz < z; iz++)
            {
                newCell = (Transform)Instantiate(cell, new Vector3(ix, 0, iz) * buffer, Quaternion.identity);
                newCell.name = string.Format("({0},{1})", ix, iz);
                newCell.parent = transform;
                newCell.GetComponent<Cell>().Position = new Vector3(ix, iz);
                gridArray[ix, iz] = newCell;
            }
        }
    }
    public bool horizontal;

    public bool topDownCam;
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            topDownCam = !topDownCam;
        }

        if (topDownCam)
        {
            topDown.gameObject.SetActive(false);
            playerCamera.gameObject.SetActive(true);
        }
        else
        {
            topDown.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(false);
        }

        toggleRoof(topDownCam);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                resetFloors();
                //hit.collider.transform.parent.GetComponent<Cell>().paintFloor(Color.cyan);
                //aStar(start, hit.collider.transform.parent);
            }
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    GameObject grid = (GameObject) Instantiate(gridGenerator, Vector3.zero, Quaternion.identity);
        //    grid.name = gameObject.name;
        //    GameObject.Destroy(gameObject);
        //}
        if (Input.GetKeyDown(KeyCode.B))
        {
            checkVisibility();
            //mergeRooms();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            numberOfTries = 0;
            succes = 0;
            spottedByCameras = 0;
            couldNotReachGoal = 0;
            for (int i = 0; i < tries; i++)
            {
                aStar();
            }
    
            if((float)succes/(float)numberOfTries < .75)
                print("Potentially unsolvable, reroll level");

            float difficulty = (float)spottedByCameras/(float)numberOfTries; 
            
            if (difficulty > .80)
                print("Unsolvable, please reroll again");
            else if(difficulty > .60)
                print("Very difficult");
            else if(difficulty > .40)
                print("Difficult");
            else if(difficulty > .20)
                print("Medium");
            else
                print("Easy");
        }
        
        if (Input.GetKey(KeyCode.N))
        {
            aStar();
        }
	}

    public void toggleRoof(bool Cam)
    {
        foreach (Transform cell in gridArray)
        {
            foreach (Transform child in cell)
            {
                if (child.name == "Cube 5")
                {
                    child.gameObject.SetActive(Cam);
                }
                    
            }
        }
    }
    public Vector2 separations;

    public void createRooms(Vector2 firstCorner, Vector2 secondCorner, Vector2 thirdCorner, bool horizontal)
    {
        float length;
        Transform prev;
        Transform next;
        int division;
        if (horizontal)
        {
            division = (int)Random.Range(firstCorner.x + separations.x, secondCorner.x - separations.x);
            prev = gridArray[division, (int)firstCorner.y];
            next = gridArray[division, (int)thirdCorner.y];
            length = thirdCorner.y - firstCorner.y;

            
        }
        else
        {
            division = (int)Random.Range(firstCorner.y+separations.y, thirdCorner.y-separations.y);
            prev = gridArray[(int)firstCorner.x,division];
            next = gridArray[(int)secondCorner.x,division];
            length = thirdCorner.x - firstCorner.x;
        }

        if (iter == 0)
        {
            //Camera.main.enabled = false;
            PlayerPos = prev.position + Vector3.up;
            start = prev;
        }

        RaycastHit[] hitAll = Physics.RaycastAll(prev.position + Vector3.up, next.position - prev.position, length * buffer);

        foreach (RaycastHit hit in hitAll)
        {
            hit.transform.parent.GetComponent<Cell>().visited = true;
            //Destroy(hit.transform.gameObject);
            hit.transform.gameObject.SetActive(false);
        }
        iter++;

        if (thirdCorner.x-firstCorner.x > 4 && thirdCorner.y-firstCorner.y > 4)
        {
            if (horizontal)
            {
                createRooms(firstCorner, new Vector2(division, firstCorner.y), new Vector3(division, thirdCorner.y), !horizontal);
                createRooms(new Vector2(division, firstCorner.y), secondCorner, thirdCorner, !horizontal);
            }
            else
            {
                createRooms(new Vector2(firstCorner.x, division), new Vector2(secondCorner.x, division), thirdCorner, !horizontal);
                createRooms(firstCorner, secondCorner, new Vector3(thirdCorner.x, division), !horizontal);
            }

        }
    }

    public void paintRooms()
    {
        foreach (Transform cell in gridArray)
        {
            if (!cell.GetComponent<Cell>().visited)
            {
                cell.GetComponent<Cell>().updateKind(Cell.Kind.Room);


                cell.GetComponent<Cell>().paintFloor(Color.blue);

                foreach (Transform neighbor in cell.GetComponent<Cell>().Neighbours)
                {
                    if (!neighbor.GetComponent<Cell>().visited)
                    {
                        RaycastHit[] hitAll = Physics.RaycastAll(cell.position + Vector3.up, neighbor.position - cell.position, buffer);

                        foreach (RaycastHit hit in hitAll)
                        {
                            //Destroy(hit.transform.gameObject);
                            hit.transform.gameObject.SetActive(false);
                        }
                    }
                }
            }
            else
            {
                cell.GetComponent<Cell>().updateKind(Cell.Kind.Hall);
                foreach (Transform neighbor in cell.GetComponent<Cell>().Neighbours)
                {
                    if (neighbor.GetComponent<Cell>().visited)
                    {
                        RaycastHit[] hitAll = Physics.RaycastAll(cell.position + Vector3.up, neighbor.position - cell.position, buffer);

                        foreach (RaycastHit hit in hitAll)
                        {
                            //Destroy(hit.transform.gameObject);
                            hit.transform.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void addNeighbours()
    {
        foreach (Transform cell in gridArray)
        { 
            Vector2 pos = cell.GetComponent<Cell>().Position;
            List<Transform> neighbor = cell.GetComponent<Cell>().Neighbours;

            if (pos.x == 0 && pos.y == 0)
            {
                neighbor.Add(gridArray[(int)pos.x + 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y + 1]);
            }
            else if (pos.y == 0 && pos.x == gridSize.x-1)
            {
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y + 1]);
                neighbor.Add(gridArray[(int)pos.x - 1, (int)pos.y]);
            }
            else if (pos.x == gridSize.x - 1 && pos.y == gridSize.y - 1)
            {
                neighbor.Add(gridArray[(int)pos.x - 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y - 1]);
            }
            else if (pos.x == 0 && pos.y == gridSize.y - 1)
            {
                neighbor.Add(gridArray[(int)pos.x + 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y - 1]);
            }

            else if (pos.x == 0)
            {
                neighbor.Add(gridArray[(int)pos.x + 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y + 1]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y - 1]);
            }

            else if (pos.x == gridSize.x - 1)
            {
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y + 1]);
                neighbor.Add(gridArray[(int)pos.x - 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y - 1]);
            }

            else if (pos.y == 0)
            {
                neighbor.Add(gridArray[(int)pos.x + 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y + 1]);
                neighbor.Add(gridArray[(int)pos.x - 1, (int)pos.y]);
            }

            else if (pos.y == gridSize.y - 1)
            {
                neighbor.Add(gridArray[(int)pos.x + 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x - 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y - 1]);
            }
            else
            {
                neighbor.Add(gridArray[(int)pos.x + 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y + 1]);
                neighbor.Add(gridArray[(int)pos.x - 1, (int)pos.y]);
                neighbor.Add(gridArray[(int)pos.x, (int)pos.y - 1]);
            }
        }
    }

    public void resetVisited()
    {
        foreach (Transform cell in gridArray)
        {
            cell.GetComponent<Cell>().visited = false;
        }
    }

    public void createRooms()
    {
        int iter = 1;

        foreach(Transform cell in gridArray)
        {
            Cell cellClass = cell.GetComponent<Cell>();
            
            if (cellClass.kind == Cell.Kind.Room && !cell.GetComponent<Cell>().visited)
            {
                cell.GetComponent<Cell>().visited = true;
                GameObject newRoom = (GameObject)Instantiate(room, cell.transform.position, Quaternion.identity);
                newRoom.name = string.Format("Room{0}", iter);
                roomList.Add(newRoom);
                newRoom.GetComponent<Room>().cellList.Add(cell);
                cell.GetComponent<Cell>().parentRoom = newRoom.GetComponent<Room>();
                newRoom.GetComponent<Room>().size = 1;
                iter++;
                addCell(newRoom.GetComponent<Room>(), cellClass);
            }
        }
    }
    public void addCell(Room roomClass, Cell cellClass)
    {
        
        foreach (Transform cellOne in cellClass.Neighbours)
        {
            Cell cellNeighbour = cellOne.GetComponent<Cell>();
            if (cellNeighbour.kind == Cell.Kind.Room && !cellOne.GetComponent<Cell>().visited)
            {
                //print(cellClass.Position + " " + cellNeighbour.Position);
                cellOne.GetComponent<Cell>().visited = true;
                roomClass.cellList.Add(cellOne);
                cellOne.GetComponent<Cell>().parentRoom = roomClass;
                roomClass.size++;
                addCell(roomClass, cellNeighbour);
            }
        }
    }

    public void aStar()
    {

        Cell startCell = start.GetComponent<Cell>();
        Cell endCell = findGoal().GetComponent<Cell>();
        List<Cell> openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();

        openSet.Add(startCell);
        resetFloors();
        numberOfTries++;
       
        while (openSet.Count > 0)
        {
            Cell currentCell = openSet[0];
            //currentCell.paintFloor(Color.yellow);
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentCell.fCost || openSet[i].fCost == currentCell.fCost && openSet[i].hCost
                     < currentCell.hCost)
                    currentCell = openSet[i];
            }

            openSet.Remove(currentCell);
            closedSet.Add(currentCell);

            if (currentCell == endCell)
            {
                retracePath(startCell, endCell);
                succes++;
                couldNotReachGoal = numberOfTries - succes;
                return;
            }

            foreach (Transform t in currentCell.Neighbours)
            {
                Cell neighbourCell = t.GetComponent<Cell>();

                if (neighbourCell.kind == Cell.Kind.Room || closedSet.Contains(neighbourCell))
                    continue;

                int newMovementCostToNeighbour = currentCell.gCost + GetDistance(currentCell, neighbourCell);
                if (neighbourCell.camerasWatching > 0)
                    newMovementCostToNeighbour += 50;
                if (neighbourCell.camerasWatching >= cameraMaximum)
                    newMovementCostToNeighbour += 50;
                if (newMovementCostToNeighbour < neighbourCell.gCost || !openSet.Contains(neighbourCell))
                {
                    neighbourCell.gCost = newMovementCostToNeighbour;
                    neighbourCell.hCost = GetDistance(neighbourCell, endCell);
                    neighbourCell.parent = currentCell;

                    if (!openSet.Contains(neighbourCell))
                        openSet.Add(neighbourCell);
                }
            }
        }
       
    }
    void retracePath(Cell start, Cell end)
    {
        List<Cell> path = new List<Cell>();
        Cell currentCell = end;
        while (currentCell != start)
        {
            path.Add(currentCell);
            currentCell = currentCell.parent;
        }
        
        path.Reverse();
        start.paintFloor(Color.cyan);
        foreach (Cell cell in path)
        {
            if (!cell.isVisible)
                cell.paintFloor(Color.green);
            else if (cell.camerasWatching < cameraMaximum)
                cell.paintFloor(Color.yellow);
            else
            {
                spottedByCameras++;
                cell.paintFloor(Color.red);
                break;
            }
        }
    }
    int GetDistance(Cell cellA, Cell cellB)
    {
       return (int)(Mathf.Abs(cellA.transform.position.x - cellB.transform.position.x) + Mathf.Abs(cellA.transform.position.z - cellB.transform.position.z)) / (int)buffer;
    }

    public void resetFloors()
    {
        foreach (Transform t in gridArray)
        {
            if(t.GetComponent<Cell>().kind == Cell.Kind.Hall)
                t.GetComponent<Cell>().paintFloor(new Color(0.009f,0.009f,0.009f));
            else
                t.GetComponent<Cell>().paintFloor(Color.blue);
        }
    }

    public float probabilityOfCamera;

    void placeCameras()
    {
        foreach (Transform t in gridArray)
        {
            Cell currentCell = t.GetComponent<Cell>();
            if (currentCell.kind == Cell.Kind.Hall)
            {
                int x = 0;
                Vector3 roomPos = new Vector3(0,0,0);
                foreach (Transform n in currentCell.Neighbours)
                {
                    Cell neighbour = n.GetComponent<Cell>();
                    if (neighbour.kind == Cell.Kind.Hall)
                        x++;
                    else
                        roomPos = neighbour.transform.position;
                }

                if (x == 3 && Random.value < probabilityOfCamera)
                {
                    currentCell.securityCamera.SetActive(true);
                    currentCell.hasCamera = true;
                    if (currentCell.transform.position.x > roomPos.x)
                    {
                        currentCell.securityCamera.transform.Translate(new Vector3(0, 0, -2.3f));
                        currentCell.securityCamera.transform.Rotate(new Vector3(0, -90, 0));
                        currentCell.cameraDir = new Vector3(1, 0, 0);
                    }
                    else if (currentCell.transform.position.x < roomPos.x)
                    {
                        currentCell.securityCamera.transform.Translate(new Vector3(0, 0, 2.3f));
                        currentCell.securityCamera.transform.Rotate(new Vector3(0, 90, 0));
                        currentCell.cameraDir = new Vector3(-1, 0, 0);
                    }

                    else if (currentCell.transform.position.z > roomPos.z)
                    {
                        currentCell.securityCamera.transform.Translate(new Vector3(2.3f, 0, 0));
                        currentCell.securityCamera.transform.Rotate(new Vector3(0, 180, 0));
                        currentCell.cameraDir = new Vector3(0, 0, 1);
                    }
                    else if (currentCell.transform.position.z < roomPos.z)
                    {
                        currentCell.securityCamera.transform.Translate(new Vector3(-2.3f, 0, 0));
                        currentCell.cameraDir = new Vector3(0, 0, -1);
                    }
                    
                }
            }
        }
    }

    void clearCameras()
    {
        foreach (Transform t in gridArray)
        {
            Cell currentCell = t.GetComponent<Cell>();
            if (currentCell.kind == Cell.Kind.Hall && currentCell.hasCamera)
            {
                //currentCell.paintFloor(Color.green);
                foreach (Transform n in currentCell.Neighbours)
                {
                    Cell neighbour = n.GetComponent<Cell>();
                    if (neighbour.kind == Cell.Kind.Hall && neighbour.hasCamera)
                    {
                        neighbour.securityCamera.SetActive(false);
                        neighbour.hasCamera = false;
                    }
                   
                }
            }
        }

        foreach (Transform t in gridArray)
        {
            Cell currentCell = t.GetComponent<Cell>();
            if (currentCell.kind == Cell.Kind.Hall && currentCell.hasCamera)
            {
                currentCell.isVisible = true;
                currentCell.camerasWatching++;
                //currentCell.camerasWatching++;
                foreach (Transform n in currentCell.Neighbours)
                {
                    Cell neighbour = n.GetComponent<Cell>();
                    if (neighbour.kind == Cell.Kind.Hall)
                    {
                        neighbour.isVisible = true;
                        neighbour.camerasWatching++;
                    }
                }

                if (currentCell.cameraDir.x != 0)
                {
                    for (int i = 0; i < cameraRange; i++)
                    {
                        if (currentCell.Position.x + i * currentCell.cameraDir.x < gridSize.x && currentCell.Position.x + i * currentCell.cameraDir.x >= 0)
                        {
                            Cell nextCell = gridArray[(int)(currentCell.Position.x + i * currentCell.cameraDir.x), (int)currentCell.Position.y].GetComponent<Cell>();
                            if (nextCell.kind == Cell.Kind.Room)
                                break;
                            if (!currentCell.Neighbours.Contains(nextCell.transform))
                            {
                                nextCell.isVisible = true;
                                nextCell.camerasWatching++;
                            }
                        }
                    }
                }
                else if (currentCell.cameraDir.z != 0)
                {
                    for (int i = 0; i < cameraRange; i++)
                    {
                        if (currentCell.Position.y + i * currentCell.cameraDir.z < gridSize.y && currentCell.Position.y + i * currentCell.cameraDir.z >= 0)
                        {
                            Cell nextCell = gridArray[(int)currentCell.Position.x, (int)(currentCell.Position.y + i * currentCell.cameraDir.z)].GetComponent<Cell>();
                            if (nextCell.kind == Cell.Kind.Room)
                                break;
                            if (!currentCell.Neighbours.Contains(nextCell.transform))
                            {
                                nextCell.isVisible = true;
                                nextCell.camerasWatching++;
                            }
                        }
                    }
                }
            }
        }
    }
    public void checkVisibility()
    {
        foreach (Transform t in gridArray)
        {
            Cell currentCell = t.GetComponent<Cell>();
            if (currentCell.kind == Cell.Kind.Hall && !currentCell.isVisible)
                currentCell.paintFloor(Color.green);
            else if (currentCell.kind == Cell.Kind.Hall && currentCell.camerasWatching <= 2)
                currentCell.paintFloor(Color.yellow);
            else if(currentCell.kind == Cell.Kind.Hall && currentCell.camerasWatching > 2)
                currentCell.paintFloor(Color.red);
        }
    }
    public float cameraRange;

   
    public void mergeRooms()
    {
        foreach (GameObject room in roomList)
        {
            Room currentRoom = room.GetComponent<Room>();

            if (currentRoom.size < gridSize.x / 6)
            {
                List<Transform> tempList = new List<Transform>();
                bool added = false;
                foreach (Transform cell in currentRoom.cellList)
                {
                    if (!added)
                    {
                        Cell currentCell = cell.GetComponent<Cell>();
                        foreach (Transform neighbour in currentCell.Neighbours)
                        {
                            Cell currentNeighbour = neighbour.GetComponent<Cell>();
                            if (currentNeighbour.kind == Cell.Kind.Hall)
                            {
                                Vector2 dir = new Vector2(currentNeighbour.Position.x - currentCell.Position.x, currentNeighbour.Position.y - currentCell.Position.y);
                                Transform nextNeighbour = gridArray[(int)currentNeighbour.Position.x + (int)dir.x, (int)currentNeighbour.Position.y + (int)dir.y];
                                if (nextNeighbour.GetComponent<Cell>().kind == Cell.Kind.Room)
                                {
                                   
                                    currentNeighbour.paintFloor(Color.blue);


                                    try
                                    {
                                        nextNeighbour.GetComponent<Cell>().parentRoom.cellList.Remove(nextNeighbour);
                                    }

                                    catch
                                    {
                                        print("ERROR: AT CELL " + nextNeighbour.name);
                                    }
                                    foreach (Transform child in neighbour)
                                    {
                                        if (child.name != "SecurityCamera ")
                                            child.gameObject.SetActive(true);
                                    }

                                    RaycastHit[] hitAll = Physics.RaycastAll(cell.position + Vector3.up, nextNeighbour.position - cell.position, 2*buffer);

                                    foreach (RaycastHit hit in hitAll)
                                    {
                                        //Destroy(hit.transform.gameObject);
                                        hit.transform.gameObject.SetActive(false);
                                    }

                                    neighbour.GetComponent<Cell>().kind = Cell.Kind.Room;
                                    tempList.Add(neighbour);
                                    neighbour.GetComponent<Cell>().parentRoom = currentRoom;
                                    nextNeighbour.GetComponent<Cell>().parentRoom = currentRoom;
                                    tempList.Add(nextNeighbour);
                                    added = true;
                                    break;
                                }
                            }
                        }
                    }

                }

                currentRoom.cellList.AddRange(tempList);
            }
        }
        //foreach (GameObject room in roomList)
        //{
        //    Color col = new Color(Random.Range(0, 0.255f), Random.Range(0, 0.255f), Random.Range(0, 0.255f));
        //    Room currentRoom = room.GetComponent<Room>();
        //    foreach (Transform cell in currentRoom.cellList)
        //    { 
        //        cell.GetComponent<Cell>().paintFloor(col);
        //    }
        //}
    }

    public Transform findGoal()
    {
        Transform goal = null;
        while (goal == null)
        {
            foreach (Transform cell in gridArray)
            {
                Cell currentCell = cell.GetComponent<Cell>();
                if (currentCell.kind == Cell.Kind.Hall)
                {
                    if (GetDistance(start.GetComponent<Cell>(), currentCell) > gridSize.x - (Random.value * 10) && Random.value < 0.01)
                        goal = cell;
                }
            }
        }
        return goal;
    }

}
