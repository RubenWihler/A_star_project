using AStarProject.Controls;

namespace AStarProject
{
    public enum TileType
    {
        Empty,
        Wall,
        Start,
        End,
        Path,
        Calculated
    }

    public partial class Form1 : Form
    {
        const int GRID_SIZE_X = 30;
        const int GRID_SIZE_Y = 30;
        const int TILE_SIZE = 100;

        public Tile? StartTile { get; set; } = null;
        public Tile? EndTile { get; set; } = null;
        public Dictionary<int, Tile> Tiles { get; set; }

        /// <summary>
        /// Get the special interaction type (right click on tile)
        /// </summary>
        public TileType SpecialInteractionType
        {
            get
            {
                return StartTile == null ? TileType.Start : TileType.End; 
            }
        }

        public Form1()
        {
            InitializeComponent();
            Init();
        }

        /// <summary>
        /// Set the start tile
        /// </summary>
        /// <param name="tile"></param>
        public void SetStartTile(Tile? tile)
        {
            if (StartTile != null) StartTile.Type = TileType.Empty;
            StartTile = tile;
        }
        /// <summary>
        /// Set the end tile
        /// </summary>
        /// <param name="tile"></param>
        public void SetEndTile(Tile? tile)
        {
            if (EndTile != null) EndTile.Type = TileType.Empty;
            EndTile = tile;
        }


        #region Initialization

        private void Init()
        {
            InitEvents();
            CreateGrid(GRID_SIZE_X, GRID_SIZE_Y);
        }

        private void InitEvents()
        {
            this.btn_start.Click += (object obj, EventArgs args) => Start();
            this.btn_reset.Click += (object obj, EventArgs args) => Clear();

        }

        private void CreateGrid(int sizeX, int sizeY, int tileSize = TILE_SIZE)
        {
            Tiles = new Dictionary<int, Tile>();
            int id = 0;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Tile tile = new Tile(id, x, y, this){
                        Size = new Size(tileSize, tileSize),
                        Location = new Point(x * tileSize, y * tileSize),
                        Tag = new Point(x, y)
                    };

                    Tiles.Add(id, tile);
                    this.tiles_container.Controls.Add(tile);
                    id++;
                }
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// Clear all tiles and reset the start and end tiles
        /// </summary>
        public void Clear(){
            StartTile = null;
            EndTile = null;
            ClearTiles();
        }

        private void ClearTiles(){
            var tiles_count = Tiles.Count;

            Tiles.Values.ToList().ForEach(tile => {
                tile.Reset();
            });
        }

        #endregion

        #region Start

        public void Start()
        {
            var operation = new AstarCalculation(Tiles, StartTile, EndTile);
            operation.Calculate();
        }

        #endregion

    }
}