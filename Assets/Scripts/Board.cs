using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Board : MonoBehaviour {

	public int width;
	public int height;

	public int borderSize;

	public GameObject tilePrefab;
	public GameObject[] gamePiecePrefabs;

	public float swapTime = 0.5f;

	Tile[,] m_allTiles;
	GamePiece[,] m_allGamePieces;

	Tile m_clickedTile;
	Tile m_targetTile;     
	public bool [,] crossPaths = new bool[3,3]; 

	public Text textPath;

	public Text textMsg;

	void Start () 
	{
		m_allTiles = new Tile[width,height];
		m_allGamePieces = new GamePiece[width,height];
       
		SetupTiles();
		SetupCamera();
		FillRandom();
	}
	///Imprime el cuadrante de acuerdo al width y al heaght especificado en el inspector.
	void SetupTiles()
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				GameObject tile = Instantiate (tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;

				tile.name = "Tile (" + i + "," + j + ")";

				m_allTiles[i,j] = tile.GetComponent<Tile>();

				tile.transform.parent = transform;

				m_allTiles[i,j].Init(i,j,this);

			}
		}
	}
///Configuraciones de la cámara para su correcta visualización
	void SetupCamera()
	{
		Camera.main.transform.position = new Vector3((float)(width - 1)/2f, (float) (height-1) /2f, -10f);

		float aspectRatio = (float) Screen.width / (float) Screen.height;

		float verticalSize = (float) height / 2f + (float) borderSize;

		float horizontalSize = ((float) width / 2f + (float) borderSize ) / aspectRatio;

		Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize: horizontalSize;

	}
		
	GameObject GetRandomGamePiece()
	{
		int randomIdx = Random.Range(0, gamePiecePrefabs.Length);

		if (gamePiecePrefabs[randomIdx] == null)
		{
			Debug.LogWarning("BOARD:  " + randomIdx + "does not contain a valid GamePiece prefab!");
		}

		return gamePiecePrefabs[randomIdx];
	}

	public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
	{
		if (gamePiece == null)
		{
			Debug.LogWarning("BOARD:  Invalid GamePiece!");
			return;
		}

		gamePiece.transform.position = new Vector3(x, y, 0);
		gamePiece.transform.rotation = Quaternion.identity;

		if (IsWithinBounds(x,y))
		{
			m_allGamePieces[x,y] = gamePiece;
		}

		gamePiece.SetCoord(x,y);
	}

	bool IsWithinBounds(int x, int y)
	{
		return (x >= 0 && x < width && y>= 0 && y<height);
	}
///Imprime aleatoriamente en la pieza,en este acso el cuadro de color negro.
	void FillRandom()
	{
		
				GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;
         int i = Random.Range(0, height);
			   int j = Random.Range(0, width);
			
				if (randomPiece !=null)
				{
					randomPiece.GetComponent<GamePiece>().Init(this);
					PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
					crossPaths[i,j] = true;
					textPath.text= ("Estás en ("+ i+ ","+ j + ")");
					randomPiece.transform.parent = transform;			

		}	
	
	}
	///Obtiene la posición en la que ya cruzó el cuadro.
void  GetCrossPathSquareWalker(int x, int y,bool crossStatus = false){   	

	 crossPaths[x,y] = crossStatus;
  
		 for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Debug.Log( i + "-" + j  + "-" + crossPaths[i,j]);						
             
		    }
		}			
}
  


	public void ClickTile(Tile tile)
	{
		if (m_clickedTile == null)
		{
            crossPaths[tile.xIndex, tile.yIndex] = true;
            textMsg.text = "Has estado en (" + tile.xIndex + "," + tile.yIndex + ")";
            m_clickedTile = tile;

        }
			 
		
	}

	public void DragToTile(Tile tile)
	{
		if (m_clickedTile !=null && IsNextTo(tile,m_clickedTile))
		{
            crossPaths[tile.xIndex, tile.yIndex] = true;
            m_targetTile = tile;
		}
	}

	public void ReleaseTile()
	{
		if (m_clickedTile !=null && m_targetTile !=null)
		{
			SwitchTiles(m_clickedTile, m_targetTile);
		}

		m_clickedTile = null;
		m_targetTile = null;

	}

	void SwitchTiles(Tile clickedTile, Tile targetTile)
	{
		
		GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex,clickedTile.yIndex];
		GamePiece targetPiece = m_allGamePieces[targetTile.xIndex,targetTile.yIndex];

		clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
	  textPath.text= ("Éstas en ("+ targetTile.xIndex + ","+ targetTile.yIndex + ")");
    GetCrossPathSquareWalker(targetTile.xIndex,targetTile.yIndex);

     
      
        

    }

	bool IsNextTo(Tile start, Tile end)
	{
		if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
		{
			return true;
		}

		if (Mathf.Abs(start.yIndex - end.yIndex) == 1 && start.xIndex == end.xIndex)
		{
			return true;
		}

		return false;
	}

}
