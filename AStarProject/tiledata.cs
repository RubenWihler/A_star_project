using AStarProject;
using AStarProject.Controls;

internal readonly struct TileData
{
    public readonly int _g_cost;
    public readonly int _h_cost;
    public readonly int _f_cost;
    public readonly int _tile_id;
    public readonly int _parent_id;

    public TileData(int g_cost, int h_cost, int f_cost, int tile_id, int parent_id)
    {
        _g_cost = g_cost;
        _h_cost = h_cost;
        _f_cost = f_cost;
        _tile_id = tile_id;
        _parent_id = parent_id;
    }
}

public readonly struct AStarOperationResult{
    public readonly bool success;
    public readonly Tile[] path;

    public AStarOperationResult(bool success, Tile[] path)
    {
        this.success = success;
        this.path = path;
    }
}

public class AstarCalculation{
    private bool _hasFounded = false;
    private Dictionary<int, Tile> _tiles;
    private Dictionary<int, TileData> _tiles_data;
    private Tile _startTile;
    private Tile _endTile;

    public AstarCalculation(Dictionary<int, Tile> tiles, Tile startTile, Tile endTile)
    {
        _tiles = tiles;
        _startTile = startTile;
        _endTile = endTile;
        this._tiles_data = new Dictionary<int, TileData>();
    }

    public void CalculateSync()
    {
        var tiles_arr = _tiles.Values.ToArray();
        var open = new List<Tile>();
        var closed = new List<Tile>();
        var open_data = new List<TileData>();
        var currentTile = _startTile;

        //ajouter la tile de depart dans open
        var start_tile_data = CreateStartData(currentTile);
        open_data.Add(start_tile_data);
        open.Add(currentTile);
        _tiles_data.Add(start_tile_data._tile_id, start_tile_data);

        while (!_hasFounded)
        {
            //trouver la tile avec le plus petit fCost
            var current_tile_data = FindLowestFCostTileData(open_data);
            var current_tile = _tiles[current_tile_data._tile_id];

            //enlever la tile de open et l'ajouter dans closed
            open.Remove(current_tile);
            open_data.Remove(current_tile_data);
            closed.Add(current_tile);

            //si la tile est la tile de fin, on arrete la boucle
            if (current_tile == _endTile)
            {
                _hasFounded = true;
                break;
            }

            //trouver les voisins de la tile
            var neighbours = FindNeighbourOf(current_tile, tiles_arr);

            foreach (var n in neighbours)
            {
                //si la tile est un mur, on l'ignore
                if (n.Type == TileType.Wall) continue;
                //si la tile est dans closed, on l'ignore
                if (closed.Contains(n)) continue;

                //calculer le gCost de la tile
                int g_cost = CalculateGCost(n.position, current_tile.position, current_tile_data._g_cost);

                //si la tile n'est pas dans open, on cree un nouveau TileData et on l'ajoute dans open 
                if (!open.Contains(n)){
                    var data = CreateTDataFrom(n.Id, current_tile.Id, g_cost, CalculateHCost(n.position));

                    //ajouter la tile dans open
                    open.Add(n);
                    open_data.Add(data);
                    _tiles_data.Add(n.Id, data);
                }
                //si il est deja dans open, on verifie si le gCost est plus petit que le gCost de la tile data deja existante
                else if (g_cost < _tiles_data[n.Id]._g_cost){
                    var old_data = _tiles_data[n.Id];
                    var data = CreateTDataFrom(n.Id, current_tile.Id, g_cost, old_data._h_cost);
                    
                    //remplacer l'ancienne tile data par la nouvelle
                    open_data.Remove(old_data);
                    open_data.Add(data);
                    _tiles_data[n.Id] = data;
                }
            }

        }

        //reconstruire le chemin
        var path = new List<Tile>();
        var current_path_tile_data = _tiles_data[_endTile.Id];

        while (current_path_tile_data._tile_id != _startTile.Id)
        {
            path.Add(_tiles[current_path_tile_data._tile_id]);
            current_path_tile_data = _tiles_data[current_path_tile_data._parent_id];
        }

        path.Reverse();

        //afficher le chemin
        path.ForEach(t => t.Type = TileType.Path);
    }

    public async Task<AStarOperationResult> CalculateAsync()
    {
        return await Task.Run(() => CalculateResult());
    }
    private AStarOperationResult CalculateResult()
    {
        var tiles_arr = _tiles.Values.ToArray();
        var open = new List<Tile>();
        var closed = new List<Tile>();
        var open_data = new List<TileData>();
        var currentTile = _startTile;

        //ajouter la tile de depart dans open
        var start_tile_data = CreateStartData(currentTile);
        open_data.Add(start_tile_data);
        open.Add(currentTile);
        _tiles_data.Add(start_tile_data._tile_id, start_tile_data);

        while (!_hasFounded)
        {
            //trouver la tile avec le plus petit fCost
            var current_tile_data = FindLowestFCostTileData(open_data);
            var current_tile = _tiles[current_tile_data._tile_id];

            //enlever la tile de open et l'ajouter dans closed
            open.Remove(current_tile);
            open_data.Remove(current_tile_data);
            closed.Add(current_tile);

            //si la tile est la tile de fin, on arrete la boucle
            if (current_tile == _endTile)
            {
                _hasFounded = true;
                break;
            }

            //trouver les voisins de la tile
            var neighbours = FindNeighbourOf(current_tile, tiles_arr);

            foreach (var n in neighbours)
            {
                //si la tile est un mur, on l'ignore
                if (n.Type == TileType.Wall) continue;
                //si la tile est dans closed, on l'ignore
                if (closed.Contains(n)) continue;

                //calculer le gCost de la tile
                int g_cost = CalculateGCost(n.position, current_tile.position, current_tile_data._g_cost);

                //si la tile n'est pas dans open, on cree un nouveau TileData et on l'ajoute dans open 
                if (!open.Contains(n)){
                    var data = CreateTDataFrom(n.Id, current_tile.Id, g_cost, CalculateHCost(n.position));

                    //ajouter la tile dans open
                    open.Add(n);
                    open_data.Add(data);
                    _tiles_data.Add(n.Id, data);
                }
                //si il est deja dans open, on verifie si le gCost est plus petit que le gCost de la tile data deja existante
                else if (g_cost < _tiles_data[n.Id]._g_cost){
                    var old_data = _tiles_data[n.Id];
                    var data = CreateTDataFrom(n.Id, current_tile.Id, g_cost, old_data._h_cost);
                    
                    //remplacer l'ancienne tile data par la nouvelle
                    open_data.Remove(old_data);
                    open_data.Add(data);
                    _tiles_data[n.Id] = data;
                }
            }

        }

        //reconstruire le chemin
        var path = new List<Tile>();
        var current_path_tile_data = _tiles_data[_endTile.Id];

        while (current_path_tile_data._tile_id != _startTile.Id)
        {
            path.Add(_tiles[current_path_tile_data._tile_id]);
            current_path_tile_data = _tiles_data[current_path_tile_data._parent_id];
        }

        path.Reverse();

        return new AStarOperationResult(true, path.ToArray());
    }

    private int CalculateHCost(Point tilePosition)
    {
        int x = Math.Abs(tilePosition.X - _endTile.position.X);
        int y = Math.Abs(tilePosition.Y - _endTile.position.Y);

        return (int)Math.Ceiling(Math.Sqrt(x*x + y*y) * 10);
    }
    private int CalculateGCost(Point tilePosition, Point parentPosition, int parentGCost)
    {
        //si la tile est sur la meme ligne ou colonne que la tile parente, on ajoute 10 au gCost, sinon on ajoute 14
        var to_add = tilePosition.X == parentPosition.X || tilePosition.Y == parentPosition.Y ? 10 : 14;
        return parentGCost + to_add;
    }
    private int CalculateFCost(int gCost, int hCost)
    {
        return gCost + hCost;
    }

    private Tile[] FindNeighbourOf(Tile tile, Tile[] tiles)
    {
        var neighbours = new List<Tile>();

        var tilePosition = tile.position;

        var neighbours_tiles = from t in tiles
                               where t.position.X >= tilePosition.X - 1 && t.position.X <= tilePosition.X + 1
                               where t.position.Y >= tilePosition.Y - 1 && t.position.Y <= tilePosition.Y + 1
                               where t.position != tilePosition
                               select t;
        
        return neighbours_tiles.ToArray();
    }

    private TileData CreateStartData(Tile tile){
        var h_cost = CalculateHCost(tile.position);

        return new TileData(
            0,
            h_cost,
            CalculateFCost(0, h_cost),
            tile.Id,
            -1
        );
    }
    private TileData FindLowestFCostTileData(List<TileData> tileDatas)
    {
        var lowestFCost = tileDatas.Min(t => t._f_cost);
        var lowest_f_cost_tiles = tileDatas.Where(t => t._f_cost == lowestFCost).ToArray();

        var lowestHCost = lowest_f_cost_tiles.Min(t => t._h_cost);
        return lowest_f_cost_tiles.First(t => t._h_cost == lowestHCost);
    }
    private TileData CreateTDataFrom(int tileId, int parentId, int gCost, int hCost)
    {
        return new TileData(
            gCost,
            hCost,
            CalculateFCost(gCost, hCost),
            tileId,
            parentId
        );
    }
}
