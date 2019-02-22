using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


/*
Localized Spatial Moving Flow Field
Michael Zheng 10/31/18
Generates a grid and creates a flow field toward a target point
*/


//nodeData
public  class nodeData
{
	public Vector2 position;
	public Vector2 index;
	int maxArrayIndex;

	public bool unreachable;
	int layerMask = 1 << 8;

	public bool[,] adjacent;
	nodeData[,] neighbors;
	//flow data
	public float costSoFar;
	public Vector2 dir;
	nodeData targetNodePos;

	public nodeData(bool inpassable,Vector2 newPos,Vector2 newIndex,int newMaxArrayIndex)
	{
		if(inpassable)
		{
			costSoFar = 1000;
		}
		unreachable = inpassable;
		
		position = newPos;
		index = newIndex;
		maxArrayIndex = newMaxArrayIndex - 1;
	}

	public void doCheck()
	{
		adjacent = new bool[3, 3];
		neighbors = new nodeData[3, 3];
		nodeData[,] meshList;

		meshList = CreateMesh.Instance.meshList;
		if(unreachable)
		{
			//no points
		}
		else
		{
			for (int x = 0; x < 3; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					if (y == 1 && x == 1)
					{
						adjacent[x, y] = false;
					}
					else
					{
						//combining with x and minus a 1 will look at all nearby objects surrounding it
						if ((int)index.x + x - 1 > maxArrayIndex || (int)index.x + x - 1 < 0 || (int)index.y + y - 1 > maxArrayIndex || (int)index.y + y - 1 < 0)
						{
							adjacent[x, y] = false;
						}//dont want diagonal
						else
						{
							neighbors[x, y] = meshList[(int)index.x + x - 1, (int)index.y + y - 1];
							if (neighbors[x, y].unreachable)
							{
								adjacent[x, y] = false;
							}
							else
							{
								adjacent[x, y] = true;
							}
						}
					}
				}
			}
		}
	}

	public void findSmallNeighbor()
	{
		float smallestDistance = Mathf.Infinity;
		foreach(nodeData tempNode in neighbors)
		{
			if(tempNode != null && tempNode.costSoFar < smallestDistance)
			{
				smallestDistance = tempNode.costSoFar;
				targetNodePos = tempNode;
			}
		}

		dir = (targetNodePos.position - position).normalized;
	}
}

public class CreateMesh : MonoBehaviour {

	private static CreateMesh _instance;
	public static CreateMesh Instance{  get { return _instance; } }

	private void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		}
		else
		{
			_instance = this;
		}
	}
	
	//size by the x or y
	public int size;
	//space between each node
	public float distance;
	public nodeData[,] meshList;

	public GameObject generators;
	public GameObject nodeSpanwers;
	public GameObject wall;
	public GameObject shootableWall;
	public Transform targetTransform;

	Vector2 pos;

	public List<string> maps;
	// Use this for initialization
	void Start () {
		meshList = new nodeData[size, size];
		StreamReader reader = new StreamReader("MapAssets/" + maps[Random.Range(0,maps.Count)] + ".txt");

		//spawn all the nodes
		for (int rows = size - 1; rows >= 0;rows--)
		{
			string readLine = reader.ReadLine();
			Debug.Log(readLine + rows);
			for (int  cols = 0; cols < size; cols++)
			{
				Vector3 newPos = new Vector3(cols * distance, rows * distance, 0);
				nodeData newNode;
				char type;

				
				if (cols == 0)
				{
					type = readLine[0];
				}
				else
				{
					type = readLine[cols * 2];

				}
				if (type == '1')
				{
					newNode = new nodeData(true, newPos, new Vector2(cols, rows), size);
					Instantiate(wall, newPos, transform.rotation, gameObject.transform);
				}
				else if(type == '4')
				{
					newNode = new nodeData(true, newPos, new Vector2(cols, rows), size);
					Instantiate(shootableWall, newPos, transform.rotation, gameObject.transform);
				}
				else
				{
					if(type == '2')
					{
						Instantiate(nodeSpanwers, newPos, transform.rotation);
					}
					if (type == '3')
					{
						Instantiate(generators, newPos, transform.rotation);
					}
					newNode = new nodeData(false, newPos, new Vector2(cols, rows), size);
				}
				meshList[cols,rows] = newNode;		
			}
		}

		//each node does a check
		foreach(nodeData node in meshList)
		{
			node.doCheck();
		}

		doFullFlow();
	}

	private void Update()
	{
		doFlow(100);
	}

	void doFullFlow()
	{
		Vector2 pos = targetTransform.position;

		List<nodeData> openList = new List<nodeData>();
		List<nodeData> closedList = new List<nodeData>();

		//addfirstnode
		nodeData firstNode = getNode(pos);
		firstNode.dir = (pos -firstNode.position).normalized;
		firstNode.costSoFar = 0;

		openList.Add(firstNode);

		while(openList.Count > 0)
		{
			nodeData curNode = openList[0];
			Vector2 curpos = curNode.position;

			//if there was a duplicate within the openlist and used list it is ignored
			if (!closedList.Contains(curNode))
			{
				//locate all adjecent nodes and check if it is traversable
				for (int x = 0; x < 3; x++)
				{
					for (int y = 0; y < 3; y++)
					{
						if (curNode.adjacent[x, y])
						{
							nodeData newNode = meshList[(int)curNode.index.x + x - 1, (int)curNode.index.y + y - 1];
							Vector2 newPos = newNode.position;
							if (!closedList.Contains(newNode))
							{
								Vector2 distance = (curpos - newPos);
								newNode.costSoFar = curNode.costSoFar + distance.magnitude;
								newNode.dir = distance.normalized;
					
								if ((pos - newNode.position).magnitude < 1.6)
								{
									newNode.dir = (pos - newPos).normalized;
								}
						
								openList.Add(newNode);
							}
						}
					}
				}

				closedList.Add(curNode);
			}

			openList.RemoveAt(0);
		}
	}


	void doFlow(float range)
	{
		pos = targetTransform.position;

		int openlistCount = 0;
		List<nodeData> openList = new List<nodeData>();
		HashSet<nodeData> closedList = new HashSet<nodeData>();

		//addfirstnode
		nodeData firstNode = getNode(pos);
		firstNode.dir = (pos - firstNode.position).normalized;
		firstNode.costSoFar = 0;

		openList.Add(firstNode);
		openlistCount++;
		bool loop = true;
		while (loop && openlistCount > 0)
		{
			nodeData curNode = openList[0];
			Vector2 curpos = curNode.position;
			if(curNode.costSoFar > range)
			{
				loop = false;
			}
			else
			{
				//if there was a duplicate within the openlist and used list it is ignored
				if (!closedList.Contains(curNode))
				{
					//locate all adjecent nodes and check if it is traversable
					for (int x = 0; x < 3; x++)
					{
						for (int y = 0; y < 3; y++)
						{
							if (curNode.adjacent[x, y])
							{
								nodeData newNode = meshList[(int)curNode.index.x + x - 1, (int)curNode.index.y + y - 1];
								Vector2 newPos = newNode.position;
								if (!closedList.Contains(newNode) || curNode == newNode)
								{
									Vector2 distance = (curpos - newPos);

									if (x == 1 || x == y)
									{
										newNode.costSoFar = curNode.costSoFar + 1;
									}
									else
									{
										newNode.costSoFar = curNode.costSoFar + 1.41f;
									}

									bool placed = false;
									for(int i = 0; i < openlistCount;i++)
									{
										if(openList[i].costSoFar > newNode.costSoFar)
										{
											placed = true;
											openList.Insert(i, newNode);
											openlistCount++;
											i = openlistCount;
										}
									}
									
									if(placed == false)
									{
										openList.Add(newNode);
										openlistCount++;
									}
								}
							}
						}
					}

					closedList.Add(curNode);
				}
			}
			openList.RemoveAt(0);
			openlistCount--;
		}
		Debug.Log("Nodes processes: " + closedList.Count);
			
		foreach (nodeData nodeTemp in closedList)
		{
			if (!nodeTemp.unreachable)
			{
				nodeTemp.findSmallNeighbor();
				Vector2	nodePos = nodeTemp.position;
				if ((pos - nodePos).magnitude < 1.6)
				{
					nodeTemp.dir = (pos - nodePos).normalized;
				}
			}
		}
	}


	public nodeData getNode(Vector2 position)
	{
		nodeData bestTarget = null;
		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = position;
		List<nodeData> nodeSpots = new List<nodeData>();

		foreach (nodeData potentialTarget in meshList)
		{
			if (potentialTarget == null)
			{
				return null;
			}
			Vector3 directionToTarget = (Vector3)potentialTarget.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestDistanceSqr)
			{
				closestDistanceSqr = dSqrToTarget;
				bestTarget = potentialTarget;
				if (potentialTarget.unreachable != true)
				{
					nodeSpots.Add(bestTarget);
				}
			}
		}

		return nodeSpots[nodeSpots.Count - 1];
	}

	public Vector2 getDirection(Vector2 position)
	{
		nodeData bestTarget = null;
		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = position;
		List<nodeData> nodeSpots = new List<nodeData>();

		foreach (nodeData potentialTarget in meshList)
		{
			if (potentialTarget == null)
			{
				return new Vector2(0, 0);
			}
			Vector3 directionToTarget = (Vector3)potentialTarget.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestDistanceSqr)
			{
				closestDistanceSqr = dSqrToTarget;
				bestTarget = potentialTarget;
				if (potentialTarget.unreachable != true)
				{
					nodeSpots.Add(bestTarget);
				}
			}
		}

		return nodeSpots[nodeSpots.Count - 1].dir;
	}

}
