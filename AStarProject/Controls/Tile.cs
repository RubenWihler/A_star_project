﻿namespace AStarProject.Controls
{
    public class Tile : Button
    {
        private Form1 _form;
        private TileType _type = TileType.Empty;

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
                        this.BackColor = Color.FromArgb(0x44, 0x44, 0x44);
                        break;
                    case TileType.Start:
                        this.BackColor = Color.FromArgb(0x55, 0xff, 0x7a);
                        break;
                    case TileType.End:
                        this.BackColor = Color.FromArgb(0xff, 0x55, 0x55);
                        break;

                    default:
                        break;
                }
            }
        }

        public bool Freezed { get; set; } = false;

        public int Id { get; set; }
        public string TileText { get; set; }

        public void Init(Form1 form, int id)
        {
            this.Id = id;
            this._form = form;

            //init events
            InitEvent();

            //set style
            InitStyle();

            //set default type
            this.Type = TileType.Empty;
        }

        private void InitStyle() {
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = Color.FromArgb(0x44, 0x44, 0x44);
        }

        private void InitEvent()
        {
            this.MouseDown += (object obj, MouseEventArgs args) => Interact(args);
            this.MouseMove += (object obj, MouseEventArgs args) => Interact(args);
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
