namespace AStarProject.Controls
{
    public class Tile : Button
    {
        public readonly Point position;
        private Form1 _form;
        private TileType _type = TileType.Empty;
        private TileInfosMode _tile_infos_mode = TileInfosMode.None;
        
        public TileType Type
        {
            get
            {
                return _type;
            }
            set {
                _type = value;
                switch (_type)
                {
                    case TileType.Empty:
                        this.BackColor = Color.White;
                        break;
                    case TileType.Wall:
                        this.BackColor = Color.FromArgb(0x7c, 0x7c, 0x7c);
                        break;
                    case TileType.Start:
                        this.BackColor = Color.FromArgb(0xab, 0xff, 0x7d);
                        break;
                    case TileType.End:
                        this.BackColor = Color.FromArgb(0xff, 0x90, 0x7d);
                        break;

                    case TileType.Path:
                        this.BackColor = Color.FromArgb(0xff, 0x7d, 0xec);
                        break;

                    case TileType.Calculated:
                        this.BackColor = Color.FromArgb(0xff, 0xd1, 0x7d);
                        break;

                    default:
                        break;
                }
            }
        }


        public bool Freezed { get; set; } = false;

        public uint Id { get; set; }

        public Tile(uint id, int x, int y, Form1 form) : base()
        {
            this.Id = id;
            this._form = form;
            this.position = new Point(x, y);

            //init
            this.Init();

            //set default type
            this.Type = TileType.Empty;

            //set text
            this.Text = $"x:{x} y:{y}";
        }

        public TileInfosMode InfosMode
        {
            get { return _tile_infos_mode; }
            set
            {
                switch (value)
                {
                    case TileInfosMode.None:
                        Text = string.Empty;
                        break;

                    case TileInfosMode.Position:
                        Text = $"x:{position.X} y:{position.Y}";
                        break;

                    case TileInfosMode.Id:
                        Text = $"{Id}";
                        break;

                    default:
                        break;
                }

                _tile_infos_mode = value;
            }
        }

        private void Init()
        {
            //init events
            InitEvent();

            //set style
            InitStyle();
        }
        private void InitStyle() {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = Color.FromArgb(0x44, 0x44, 0x44);
        }

        private void InitEvent()
        {
            this.MouseDown += (object obj, MouseEventArgs args) => Interact(args);
        }

        private void Interact(MouseEventArgs args)
        {
            if (Freezed) return;

            switch (args.Button)
            {
                case MouseButtons.Left:
                    var target = Type == TileType.Empty ? TileType.Wall : TileType.Empty;
                    SetType(target);
                    break;

                case MouseButtons.Right:
                    // if the tile is start or end, we set it to empty
                    if (_form.StartTile == this || _form.EndTile == this) SetType(TileType.Empty);
                    else SetType(_form.SpecialInteractionType);
                    break;

                case MouseButtons.None:
                    break;

                default:
                    break;
            }
        }

        public bool SetType(TileType type)
        {
            var oldType = Type;

            if (oldType == TileType.Start) _form.SetStartTile(null);
            if (oldType == TileType.End) _form.SetEndTile(null);

            if (type == TileType.Start) _form.SetStartTile(this);
            if (type == TileType.End) _form.SetEndTile(this);

            //if old type is empty, we accept any type
            if (oldType == TileType.Empty) {
                Type = type;
                return true;
            }

            //if old type is wall, we only accept empty type
            if (oldType == TileType.Wall && (type == TileType.Empty || type == TileType.Start || type == TileType.End))
            {
                Type = type;
                return true;
            }

            if (oldType == TileType.Calculated && type == TileType.Path)
            {
                Type = type;
                return true;
            }


            return false;
        }

        public void Reset()
        {
            this.Type = TileType.Empty;
            this.Freezed = false;
            //to do reset text
        }

    }
}
