using System;
using System.Collections.Generic;
using Main.Runtime;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace Flower
{
	/// <summary>
	/// A tower placement location made from a grid.
	/// Its origin is centered in the middle of the lower-right cell. It can be oriented in any direction
	/// </summary>
	//[RequireComponent(typeof(BoxCollider2D))]
	public class CarPlacementGrid : GameFrameworkComponent, IPlacementArea
	{
		/// <summary>
		/// Prefab used to visualise the grid
		/// </summary>
		public PlacementCell placementTilePrefab;

		/// <summary>
		/// The dimensions of the grid 
		/// </summary>
		public IntVector2 dimensions;

		/// <summary>
		/// Size of the edge of a cell
		/// </summary>
		[Tooltip("The size of the edge of one grid cell for this area. Should match the physical grid size of towers")]
		public float gridSize = 1;

		/// <summary>
		/// Inverted grid size, to multiply with
		/// </summary>
		float m_InvGridSize;

		/// <summary>
		/// Array of available cells
		/// </summary>
		bool[,] m_AvailableCells;

		/// <summary>
		/// Array of <see cref="PlacementCell"/>s
		/// </summary>
		PlacementCell[,] m_Tiles;

		public PlacementCell[,] AllCells
		{
			get { return m_Tiles; }
		}
		private Dictionary<int, Vector3> m_CellPosDict = new Dictionary<int, Vector3>();
		/// <summary>
		/// Converts a location in world space into local grid coordinates.
		/// </summary>
		/// <param name="worldLocation"><see cref="Vector3"/> indicating world space coordinates to convert.</param>
		/// <param name="sizeOffset"><see cref="IntVector2"/> indicating size of object to center.</param>
		/// <returns><see cref="IntVector2"/> containing the grid coordinates corresponding to this location.</returns>
		public IntVector2 WorldToGrid(Vector3 worldLocation, IntVector2 sizeOffset)
		{
			Vector3 localLocation = transform.InverseTransformPoint(worldLocation);

			// Scale by inverse grid size
			localLocation *= m_InvGridSize;

			// Offset by half size
			var offset = new Vector3(sizeOffset.x * 0.5f, 0.0f, sizeOffset.y * 0.5f);
			localLocation -= offset;

			int xPos = Mathf.RoundToInt(localLocation.x);
			int yPos = Mathf.RoundToInt(localLocation.z);

			return new IntVector2(xPos, yPos);
		}

		/// <summary>
		/// Returns the world coordinates corresponding to a grid location.
		/// </summary>
		/// <param name="gridPosition">The coordinate in grid space</param>
		/// <param name="sizeOffset"><see cref="IntVector2"/> indicating size of object to center.</param>
		/// <returns>Vector3 containing world coordinates for specified grid cell.</returns>
		public Vector3 GridToWorld(IntVector2 gridPosition, IntVector2 sizeOffset)
		{
			// Calculate scaled local position
			Vector3 localPos = new Vector3(gridPosition.x + (sizeOffset.x * 0.5f), gridPosition.y + (sizeOffset.y * 0.5f), 0) *
							   gridSize;

			return transform.TransformPoint(localPos);
		}

		/// <summary>
		/// Tests whether the indicated cell range represents a valid placement location.
		/// </summary>
		/// <param name="gridPos">The grid location</param>
		/// <param name="size">The size of the item</param>
		/// <returns>Whether the indicated range is valid for placement.</returns>
		public CarFitStatus Fits(IntVector2 gridPos, IntVector2 size)
		{
			// If the tile size of the tower exceeds the dimensions of the placement area, immediately decline placement.
			if ((size.x > dimensions.x) || (size.y > dimensions.y))
			{
				return CarFitStatus.OutOfBounds;
			}

			IntVector2 extents = gridPos + size;

			// Out of range of our bounds
			if ((gridPos.x < 0) || (gridPos.y < 0) ||
				(extents.x > dimensions.x) || (extents.y > dimensions.y))
			{
				return CarFitStatus.OutOfBounds;
			}

			// Ensure there are no existing towers within our tile silhuette.
			for (int y = gridPos.y; y < extents.y; y++)
			{
				for (int x = gridPos.x; x < extents.x; x++)
				{
					if (m_AvailableCells[x, y])
					{
						return CarFitStatus.Overlaps;
					}
				}
			}

			// If we've got this far, we've got a valid position.
			return CarFitStatus.Fits;
		}

		/// <summary>
		/// Sets a cell range as being occupied by a tower.
		/// </summary>
		/// <param name="gridPos">The grid location</param>
		/// <param name="size">The size of the item</param>
		public void Occupy(IntVector2 gridPos, IntVector2 size)
		{
			IntVector2 extents = gridPos + size;

			// Validate the dimensions and size
			if ((size.x > dimensions.x) || (size.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
			}

			// Out of range of our bounds
			if ((gridPos.x < 0) || (gridPos.y < 0) ||
				(extents.x > dimensions.x) || (extents.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
			}

			// Fill those positions
			for (int y = gridPos.y; y < extents.y; y++)
			{
				for (int x = gridPos.x; x < extents.x; x++)
				{
					m_AvailableCells[x, y] = true;

					// If there's a placement tile, clear it
					if (m_Tiles != null && m_Tiles[x, y] != null)
					{
						m_Tiles[x, y].SetState(PlacementCellState.Filled);
					}
				}
			}
		}

		public void SetState(IntVector2 pos,PlacementCellState tempState)
		{
			m_Tiles[pos.x, pos.y].SetState(tempState);
		}

		/// <summary>
		/// Removes a tower from a grid, setting its cells as unoccupied.
		/// </summary>
		/// <param name="gridPos">The grid location</param>
		/// <param name="size">The size of the item</param>
		public void Clear(IntVector2 gridPos, IntVector2 size)
		{
			IntVector2 extents = gridPos + size;

			// Validate the dimensions and size
			if ((size.x > dimensions.x) || (size.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("size", "Given dimensions do not fit in our grid");
			}

			// Out of range of our bounds
			if ((gridPos.x < 0) || (gridPos.y < 0) ||
				(extents.x > dimensions.x) || (extents.y > dimensions.y))
			{
				throw new ArgumentOutOfRangeException("gridPos", "Given footprint is out of range of our grid");
			}

			// Fill those positions
			for (int y = gridPos.y; y < extents.y; y++)
			{
				for (int x = gridPos.x; x < extents.x; x++)
				{
					m_AvailableCells[x, y] = false;

					// If there's a placement tile, clear it
					if (m_Tiles != null && m_Tiles[x, y] != null)
					{
						m_Tiles[x, y].SetState(PlacementCellState.Empty);
					}
				}
			}
		}

		/// <summary>
		/// Initialize values
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			//ResizeCollider();

			// Initialize empty bool array (defaults are false, which is what we want)
			m_AvailableCells = new bool[dimensions.x, dimensions.y];

			// Precalculate inverted grid size, to save a division every time we translate coords
			m_InvGridSize = 1 / gridSize;

			//SetUpGrid();
		}

		/// <summary>
		/// Set collider's size and center
		/// </summary>
		void ResizeCollider()
		{
			var myCollider = GetComponent<BoxCollider2D>();
			Vector3 size = new Vector3(dimensions.x, 0, dimensions.y) * gridSize;
			myCollider.size = size;

			// Collider origin is our bottom-left corner
			myCollider.offset = size * 0.5f;
		}

		public Transform MParent { get; set; }

		/// <summary>
		/// Instantiates Tile Objects to visualise the grid and sets up the <see cref="m_AvailableCells" />
		/// </summary>
		public void SetUpGrid()
		{
			PlacementCell tileToUse;
			tileToUse = placementTilePrefab;

			if (tileToUse != null)
			{
				// Create a container that will hold the cells.
				var tilesParent = new GameObject("Container");
				MParent = transform;
				tilesParent.transform.parent = transform;
				tilesParent.transform.localPosition = Vector3.zero;
				tilesParent.transform.localRotation = Quaternion.identity;
				tilesParent.transform.localScale = Vector3.one;
				transform.localPosition = new Vector3(-600,-350f,0f);

				m_Tiles = new PlacementCell[dimensions.x, dimensions.y];

				for (int y = 0; y < dimensions.y; y++)
				{
					for (int x = 0; x < dimensions.x; x++)
					{
						Vector3 targetPos = GridToWorld(new IntVector2(x, y), new IntVector2(1, 1));
						targetPos.y += 0.01f;
						PlacementCell newTile = Instantiate(tileToUse, transform);
						newTile.transform.SetPositionAndRotation(targetPos,Quaternion.identity);

						m_Tiles[x, y] = newTile;
						newTile.SetState(PlacementCellState.Empty);
						newTile.SetCellSize(gridSize);
						newTile.SetCellIndex(new IntVector2(x, y));
						int tempKey = IntVector2.GetKey(newTile.CellIndex);
						Vector3 tempPos = new Vector3(newTile.transform.position.x, newTile.transform.position.y, 0);
						if (m_CellPosDict.ContainsKey(tempKey))
						{
							m_CellPosDict[tempKey] = tempPos;
						}
						else
						{
							m_CellPosDict.Add(tempKey,tempPos);
						}
					}
				}

				// PlacementCell newTile1 = Instantiate(tileToUse,tilesParent.transform);
				// newTile1.transform.SetPositionAndRotation(new Vector3((m_Tiles[0,1].transform.position.x + m_Tiles[1,0].transform.position.x) / 2,
				// 	m_Tiles[0,1].transform.position.y),Quaternion.identity);
				// newTile1.SetState(PlacementCellState.Filled);
				// newTile1.SetCellSize(gridSize);
			}
		}

		public Vector3 GetCellPos(IntVector2 tempIndex)
		{
			Vector3 tempVec = Vector3.zero;
			if (m_CellPosDict.TryGetValue(IntVector2.GetKey(tempIndex),out tempVec))
			{
			}
			return tempVec;
		}
		public Vector3 GetCellLocalPos(IntVector2 tempIndex)
		{
			return m_Tiles[tempIndex.x,tempIndex.y].transform.localPosition;
		}
		public Vector3 GetLocalPos(IntVector2 firstIndex,IntVector2 lastIndex,bool isVertical)
		{
			Vector3 tempFirstVec = GetCellLocalPos(firstIndex);
			Vector3 tempLasVec = GetCellLocalPos(lastIndex);
			if (isVertical)
			{
				return new Vector3(tempFirstVec.x, (tempFirstVec.y + tempLasVec.y) / 2, 0);
			}
			else
			{
				return new Vector3((tempFirstVec.x + tempLasVec.x) / 2, tempFirstVec.y, 0);
			}
		}
		public Vector3 GetPos(IntVector2 firstIndex,IntVector2 lastIndex,bool isVertical)
		{
			Vector3 tempFirstVec = GetCellPos(firstIndex);
			Vector3 tempLasVec = GetCellPos(lastIndex);
			if (isVertical)
			{
				return new Vector3(tempFirstVec.x, (tempFirstVec.y + tempLasVec.y) / 2, 0);
			}
			else
			{
				return new Vector3((tempFirstVec.x + tempLasVec.x) / 2, tempFirstVec.y, 0);
			}
		}
#if UNITY_EDITOR
		/// <summary>
		/// On editor/inspector validation, make sure we size our collider correctly.
		/// Also make sure the collider component is hidden so nobody can mess with its settings to ensure its integrity.
		/// Also communicates the idea that the user should not need to modify those values ever.
		/// </summary>
		void OnValidate()
		{
			// Validate grid size
			if (gridSize <= 0)
			{
				Debug.LogError("Negative or zero grid size is invalid");
				gridSize = 1;
			}

			// Validate dimensions
			if (dimensions.x <= 0 ||
				dimensions.y <= 0)
			{
				Debug.LogError("Negative or zero grid dimensions are invalid");
				dimensions = new IntVector2(Mathf.Max(dimensions.x, 1), Mathf.Max(dimensions.y, 1));
			}

			// Ensure collider is the correct size
			//ResizeCollider();
			//GetComponent<BoxCollider2D>().hideFlags = HideFlags.HideInInspector;
		}

		/// <summary>
		/// Draw the grid in the scene view
		/// </summary>
		void OnDrawGizmos()
		{
			Color prevCol = Gizmos.color;
			Gizmos.color = Color.cyan;

			Matrix4x4 originalMatrix = Gizmos.matrix;
			Gizmos.matrix = transform.localToWorldMatrix;

			// Draw local space flattened cubes
			for (int y = 0; y < dimensions.y; y++)
			{
				for (int x = 0; x < dimensions.x; x++)
				{
					var position = new Vector3((x + 0.5f) * gridSize, (y + 0.5f) * gridSize, 0);
					Gizmos.DrawIcon(position, "");
				}
			}

			Gizmos.matrix = originalMatrix;
			Gizmos.color = prevCol;

			// Draw icon too, in center of position
			Vector3 center = transform.TransformPoint(new Vector3(gridSize * dimensions.x * 0.5f,
				gridSize * dimensions.y * 0.5f,
																  0));
			Gizmos.DrawIcon(center, "build_zone.png", true);
		}
#endif
	}
}