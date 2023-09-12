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
        const int GRID_SIZE_Y = 20;
        const int TILE_SIZE = 85;

        const int MIN_TILE_SIZE = 5;
        const int MAX_TILE_SIZE = TILE_SIZE * 5;

        private bool _is_calculating = false;
        private int _tile_size = TILE_SIZE;

        public int TileSize
        {
            get { return _tile_size; }
            set
            {
                if (value > MAX_TILE_SIZE) value = MAX_TILE_SIZE;
                if (value < MIN_TILE_SIZE) value = MIN_TILE_SIZE;

                _tile_size = value;
                UpdateGrid(_tile_size);
            }
        }
        public Tile? StartTile { get; set; } = null;
        public Tile? EndTile { get; set; } = null;
        public Dictionary<int, Tile> Tiles { get; set; }

        public bool IsCalculating
        {
            get { return _is_calculating; }
            set
            {
                _is_calculating = value;
                btn_clear.Enabled = _is_calculating;
                btn_start.Enabled = _is_calculating;
                btn_reset.Enabled = _is_calculating;
            }
        }

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
            lbl_op_result.Text = "";
        }

        private void InitEvents()
        {
            this.btn_start.Click += (object obj, EventArgs args) => Start();
            this.btn_reset.Click += (object obj, EventArgs args) => Reset();
            btn_clear.Click += (object obj, EventArgs args) => Clear();
            this.btn_zoom_in.Click += ZoomIn;
            this.btn_zoom_out.Click += ZoomOut;
        }

        private void CreateGrid(int sizeX, int sizeY, int tileSize = TILE_SIZE)
        {
            Tiles = new Dictionary<int, Tile>();
            int id = 0;

            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    Tile tile = new Tile(id, x, y, this)
                    {
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

        private void UpdateGrid(int tileSize = TILE_SIZE)
        {
            for (int i = 0; i < Tiles.Count; i++)
            {
                var t = Tiles[i];
                t.Location = new Point(t.position.X * tileSize, t.position.Y * tileSize);
                t.Size = new Size(tileSize, tileSize);
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// Remove all calculated tile and path
        /// </summary>
        public void Clear()
        {
            foreach (Tile t in Tiles.Values)
            {
                if (t.Type == TileType.Calculated || t.Type == TileType.Path)
                {
                    t.Type = TileType.Empty;
                }
            }
        }

        /// <summary>
        /// Clear all tiles and reset the start and end tiles
        /// </summary>
        public void Reset()
        {
            if (_is_calculating) return;
            StartTile = null;
            EndTile = null;
            ClearTiles();
        }

        private void ClearTiles()
        {
            var tiles_count = Tiles.Count;

            Tiles.Values.ToList().ForEach(tile =>
            {
                tile.Reset();
            });
        }



        #endregion

        #region Start

        public void Start()
        {
            // var operation = new AstarCalculation(Tiles, StartTile, EndTile);
            // operation.CalculateSync();
            StartAsync();
        }

        private async void StartAsync()
        {
            if (_is_calculating) return;
            if (StartTile == null || EndTile == null)
            {
                MessageBox.Show("You must set a start and end tile.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _is_calculating = true;

            var time = DateTime.Now;

            var operation = new AstarCalculation(Tiles, StartTile, EndTile, GRID_SIZE_X, GRID_SIZE_Y);
            var result = await operation.CalculateAsync();

            var calculation_time = DateTime.Now - time;

            _is_calculating = false;

            if (!result.success)
            {
                MessageBox.Show("The path is impossible !", "No path found !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lbl_op_result.Text = "operation failed !";
                return;
            }

            var calculation_time_in_ms = calculation_time.TotalMilliseconds;
            lbl_op_result.Text = $"operation success in {calculation_time_in_ms}ms";

            var path = result.path.Select((id) => Tiles[id]);
            var calculed_tiles = result.calculated_tiles.Select((id) => Tiles[id]);

            foreach (var tile in calculed_tiles)
            {
                if (tile.Type == TileType.End) continue;
                tile.SetType(TileType.Calculated);
            }

            foreach (var tile in path)
            {
                if (tile.Type == TileType.End) continue;
                tile.SetType(TileType.Path);
            }
        }

        #endregion

        private void ZoomIn(object sender, EventArgs e)
        {
            TileSize += 20;
        }

        private void ZoomOut(object sender, EventArgs e)
        {
            TileSize -= 20;
        }
    }
}