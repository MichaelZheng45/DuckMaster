using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{
	public static List<Vector3> getTilePathDuck(Vector3 from, Vector3 to, DuckTileMap tileMap, int duckDirection)
	{
		//Create list data to keep track of
		List<DuckTile> openList = new List<DuckTile>();
		HashSet<DuckTile> closedList = new HashSet<DuckTile>();
		List<Vector3> path = new List<Vector3>();
		int openListCount = 0;
		int nodesProcessed = 0; //processed is when it is thrown into the closed list

		//Allocate positions
		Vector3 startingPos = from;
		Vector3 targetPos = to;

		//Get the first node
		DuckTile firstNode = GameManager.Instance.GetTileMap().getTileFromPosition(from);
        if (firstNode == null)
        {
            return path;
        }
        firstNode.mCostSoFar = 0;
		firstNode.mHeuristicCostSoFar = (targetPos - startingPos).magnitude;

		//Add Node
		openList.Add(firstNode);
		openListCount++;

		//getTargetNode
		DuckTile targetNode = GameManager.Instance.GetTileMap().getTileFromPosition(to);
		if (targetNode.mType == DuckTile.TileType.UnpassableBoth || targetNode.mType == DuckTile.TileType.UnpasssableDuck || targetNode == firstNode)
		{
			return path;
		}

		//create tile search order
		// instead of just looping through any nodes, create a list of the order of searching like if duck is facing right, it will look right, bot, left then top
		// how it works is there will be a list of 4 members of the directions 0 starting top clockwise. this is the index to access which adj tile to look at first
		List<int> searchOrder = new List<int>();
		for (int i = duckDirection; i < duckDirection + 4; i++)
		{
			float direction = i % 4;
			searchOrder.Add(i);
		}

		//conditions for other stuff like limiting the range etc.
		bool loop = true;

		//condition when tile is found
		bool foundToTile = false;
		while (loop && openListCount > 0 && !foundToTile) //&& if node is not found, count should be fine but in case
		{
			DuckTile curNode = openList[0];
			Vector3 curPos = curNode.mPosition;

			//to create the index is i = (row * Colsize) + column where col is x
			//maybe a check if the nodes processed is too much then stop or something
			if (targetNode == curNode)
			{
				foundToTile = true;
			}
			else
			{
				//iterate through all adjacents
				if (!closedList.Contains(curNode))
				{
					for (int count = 0; count < 4; count++)
					{
						int adjNodeDirection = searchOrder[count];
						Connection adjConnection = curNode.GetConnectionDirection((DuckTile.ConnectionDirection)adjNodeDirection);

						if (adjConnection != null)
						{
							Vector3 adjIndex = adjConnection.mToIndex;
							DuckTile adjTile = tileMap.GetTile((int)adjIndex.x, (int)adjIndex.y, (int)adjIndex.z);

							//if it is same height, cannot ignore walkable and the tile is not walkable, then it cannot travel to adj tile
							if (adjTile.mHeight <= curNode.mHeight && !closedList.Contains(adjTile) && curNode != adjTile
								 && (adjTile.mType == DuckTile.TileType.PassableBoth || adjTile.mType == DuckTile.TileType.UnpassableMaster))
							{
								adjTile.mCostSoFar = curNode.mCostSoFar + adjConnection.mDuckCost;

								Vector2 manhattanDis = (targetPos - adjTile.mPosition);
								adjTile.mHeuristicCostSoFar = adjTile.mCostSoFar + Mathf.Abs(manhattanDis.x) + Mathf.Abs(manhattanDis.y);
								adjTile.mPreviousTile = curNode;

								//place the node into a queue 
								bool placed = false;
								for (int i = 0; i < openListCount; i++)
								{
									if (openList[i].mHeuristicCostSoFar > adjTile.mHeuristicCostSoFar)
									{
										placed = true;
										openList.Insert(i, adjTile);
										openListCount++;
										i = openListCount;
									}
								}

								if (placed == false)
								{
									openList.Insert(openListCount, adjTile);
									openListCount++;
								}
							}
						}
					}
				}

				nodesProcessed++;
				closedList.Add(curNode);
			}
			openList.RemoveAt(0);
			openListCount--;
		}

		if (foundToTile)
		{
			path.Add(targetNode.mPosition + new Vector3(0, 1, 0));
			bool finishPath = false;
			DuckTile curTile = targetNode;

			while (!finishPath)
			{
				curTile = curTile.mPreviousTile;
				path.Add(curTile.mPosition + new Vector3(0, 1, 0));
				if (curTile == firstNode)
				{
					finishPath = true;
				}
			}
		}

		return path;
	}

	public static List<Vector3> getTilePathPlayer(Vector3 from, Vector3 to, DuckTileMap tileMap)
	{
		//Create list data to keep track of
		List<DuckTile> openList = new List<DuckTile>();
		HashSet<DuckTile> closedList = new HashSet<DuckTile>();
		List<Vector3> path = new List<Vector3>();
		int openListCount = 0;
		int nodesProcessed = 0; //processed is when it is thrown into the closed list

		//Allocate positions
		Vector3 startingPos = from;
		Vector3 targetPos = to;

		//Get the first node
		DuckTile firstNode = GameManager.Instance.GetTileMap().getTileFromPosition(from);
        if(firstNode == null)
        {
            return path;
        }

		firstNode.mCostSoFar = 0;
		firstNode.mHeuristicCostSoFar = (targetPos - startingPos).magnitude;

		//Add Node
		openList.Add(firstNode);
		openListCount++;

		//getTargetNode
		DuckTile targetNode = GameManager.Instance.GetTileMap().getTileFromPosition(to);
		if (targetNode.mType == DuckTile.TileType.UnpassableBoth || targetNode.mType == DuckTile.TileType.UnpassableMaster || targetNode == firstNode)
		{
			return path;
		}

		//conditions for other stuff like limiting the range etc.
		bool loop = true;

		//condition when tile is found
		bool foundToTile = false;
		while (loop && openListCount > 0 && !foundToTile) //&& if node is not found, count should be fine but in case
		{
			DuckTile curNode = openList[0];
			Vector3 curPos = curNode.mPosition;

            //to create the index is i = (row * Colsize) + column where col is x
            //maybe a check if the nodes processed is too much then stop or something
            if (targetNode == curNode)
            {
				foundToTile = true;
			}
			else
			{
				if (!closedList.Contains(curNode))
				{
					for (int count = 0; count < 4; count++)
					{
						int adjNodeDirection = count;
						Connection adjConnection = curNode.GetConnectionDirection((DuckTile.ConnectionDirection)adjNodeDirection);
						if(adjConnection != null)
						{
							Vector3 adjIndex = adjConnection.mToIndex;
							DuckTile adjTile = tileMap.GetTile((int)adjIndex.x, (int)adjIndex.y, (int)adjIndex.z);
							//if it is same height, cannot ignore walkable and the tile is not walkable, then it cannot travel to adj tile
							if (adjTile.mHeight <= curNode.mHeight && !closedList.Contains(adjTile) && curNode != adjTile
								 && (adjTile.mType == DuckTile.TileType.PassableBoth || adjTile.mType == DuckTile.TileType.UnpasssableDuck))
							{
								adjTile.mCostSoFar = curNode.mCostSoFar + adjConnection.mMasterCost;

								Vector2 manhattanDis = (targetPos - adjTile.mPosition);
								adjTile.mHeuristicCostSoFar = adjTile.mCostSoFar + Mathf.Abs(manhattanDis.x) + Mathf.Abs(manhattanDis.y);
								adjTile.mPreviousTile = curNode;

								//place the node into a queue 
								bool placed = false;
								for (int i = 0; i < openListCount; i++)
								{
									if (openList[i].mHeuristicCostSoFar > adjTile.mHeuristicCostSoFar)
									{
										placed = true;
										openList.Insert(i, adjTile);
										openListCount++;
										i = openListCount;
									}
								}

								if (placed == false)
								{
									openList.Insert(openListCount, adjTile);
									openListCount++;
								}
							}
						}
						
					}

					nodesProcessed++;
					closedList.Add(curNode);
				}
			}
			openList.RemoveAt(0);
			openListCount--;
		}

		if (foundToTile)
		{
			path.Add(targetNode.mPosition + new Vector3(0, 1, 0));
			bool finishPath = false;
			DuckTile curTile = targetNode;

			while (!finishPath)
			{
				curTile = curTile.mPreviousTile;
				path.Add(curTile.mPosition + new Vector3(0, 1, 0));

                if (curTile == firstNode)
				{
					finishPath = true;
				}
			}
		}

		return path;
	}
}
