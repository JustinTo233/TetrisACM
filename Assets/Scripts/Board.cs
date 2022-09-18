using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] private Transform nextPiecePos;
    [SerializeField] private AudioSource Single;
    [SerializeField] private AudioSource Double;
    [SerializeField] private AudioSource Triple;
    [SerializeField] private AudioSource Tetris;
    [SerializeField] private AudioSource BackgroundMusic;

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public Piece nextPiece { get; private set; }

    public GameOverScreen GameOverScreen;
    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Vector3Int previewPosition;

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    private int numberOfRowsThisTurn = 0;
    private int currentScore = 0;

    public Text hudScore;


    public RectInt Bounds {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();
        this.nextPiece = this.gameObject.AddComponent<Piece>();
        this.nextPiece.enabled = false;

        previewPosition = new Vector3Int((int)nextPiecePos.position.x, (int)nextPiecePos.position.y, 0);

        for (int i = 0; i < this.tetrominoes.Length; i++)
        {
            this.tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SetNextPiece();
        SpawnPiece();
        PlayBackgroundMusic();
    }

    private void Update()
    {
        UpdateScore();
        UpdateUI();
    }

    private void SetNextPiece()
    {
        if (this.nextPiece.cells != null)
        {
            Clear(this.nextPiece);
        }

        int random = Random.Range(0, this.tetrominoes.Length);
        TetrominoData data = this.tetrominoes[random];

        this.nextPiece.Initialize(this, this.previewPosition, data);
        Set(this.nextPiece);
    }

    public void SpawnPiece()
    {
        this.activePiece.Initialize(this, this.spawnPosition, this.nextPiece.data);

        if (!IsValidPosition(this.activePiece, this.spawnPosition))
        {
            GameOver();
        }
        else
        {
            Set(this.activePiece);
        }

        SetNextPiece();
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        // Do anything else you want on game over here..

        GameOverScreen.Setup(currentScore);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        numberOfRowsThisTurn++;
        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void UpdateUI()
    {
        hudScore.text = currentScore.ToString();
    }

    public void UpdateScore()
    {
        if(numberOfRowsThisTurn > 0)
        {
            if(numberOfRowsThisTurn == 1)
            {
                ClearedOneLine();
            }
            else if(numberOfRowsThisTurn == 2)
            {
                ClearedTwoLines();
            }
            else if(numberOfRowsThisTurn == 3)
            {
                ClearedThreeLines();
            }
            else if(numberOfRowsThisTurn == 4)
            {
                ClearedFourLines();
            }

            numberOfRowsThisTurn = 0;
        }
    }

    public void ClearedOneLine()
    {
        currentScore += scoreOneLine;
        Single.Play();
    }

    public void ClearedTwoLines()
    {
        currentScore += scoreTwoLine;
        Double.Play();
    }

    public void ClearedThreeLines()
    {
        currentScore += scoreThreeLine;
        Triple.Play();
    }

    public void ClearedFourLines()
    {
        currentScore += scoreFourLine;
        Tetris.Play();   
    }

    public void PlayBackgroundMusic()
    {
        BackgroundMusic.Play();
    }
}