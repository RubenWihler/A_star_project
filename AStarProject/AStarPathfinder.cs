using AStarProject;
using AStarProject.Controls;

internal readonly struct TileData
{
    public readonly int g_cost;
    public readonly int h_cost;
    public readonly int f_cost;
    public readonly int x;
    public readonly int y;
    public readonly int id;
    public readonly int parent_id;
    public readonly bool is_wall;

    public TileData(int gCost, int hCost, int fCost, int x, int y, int tileId, int parentId, bool isWall)
    {
        this.g_cost = gCost;
        this.h_cost = hCost;
        this.f_cost = fCost;
        this.y = y;
        this.x = x;
        this.id = tileId;
        this.parent_id = parentId;
        this.is_wall = isWall;
    }
}

public readonly struct AStarOperationResult{
    public readonly bool success;
    public readonly int[] path;
    public readonly int[] calculated_tiles;

    public AStarOperationResult(bool success, int[] path, int[] calculatedTiles)
    {
        this.success = success;
        this.path = path;
        this.calculated_tiles = calculatedTiles;
    }
}

public class AstarCalculation{
    private bool _hasFounded = false;
    private Dictionary<int, TileData> _tiles;
    private int _startTile;
    private int _endTile;
    private int _row_size;
    private int _column_size;

    public AstarCalculation(Dictionary<int, Tile> tiles, Tile startTile, Tile endTile, int rowSize, int columnSize)
    {
        _startTile = startTile.Id;
        _endTile = endTile.Id;
        _row_size = rowSize;
        _column_size = columnSize;

        //init tiles data from tiles
        _tiles = new Dictionary<int, TileData>();
        foreach (var t in tiles)
        {
            Tile tile = t.Value;

            var data = new TileData(
                0,
                0,
                0,
                (int)tile.position.X,
                (int)tile.position.Y,
                tile.Id,
                0,
                tile.Type == TileType.Wall
            );

            _tiles.Add(t.Key, data);
        }
    }

    public async Task<AStarOperationResult> CalculateAsync()
    {
        return await Task.Run(() => CalculateResult());
    }
    private AStarOperationResult CalculateResult()
    {
        TileData current_tile;
        var open = new List<TileData>();
        var closed = new List<int>();
        var calculated_tiles = new List<int>();

        //ajouter la tile de depart dans open
        var old_start_data = _tiles[_startTile];
        current_tile = CreateStartData(_startTile, old_start_data.x, old_start_data.y);
        _tiles[_startTile] = current_tile;
        open.Add(current_tile);

        while (!_hasFounded)
        {
            //le chemin est impossible
            if (open.Count == 0)
            {
                _hasFounded = false;
                return new AStarOperationResult(false, Array.Empty<int>(), calculated_tiles.ToArray());
            }

            //trouver la tile avec le plus petit fCost
            current_tile = FindLowestFCostTileData(open);

            //enlever la tile de open et l'ajouter dans closed
            open.Remove(current_tile);
            closed.Add(current_tile.id);

            //si la tile est la tile de fin, on arrete la boucle
            if (current_tile.id == _endTile)
            {
                _hasFounded = true;
                break;
            }

            //trouver les voisins de la tile
            var neighbours_id = FindNeighbourOf(current_tile.id, current_tile.x, current_tile.y);
            var neighbours = neighbours_id.Select((id) => _tiles[id]);

            foreach (var n in neighbours)
            {
                //si la tile est un mur, on l'ignore
                if (n.is_wall) continue;
                //si la tile est dans closed, on l'ignore
                if (closed.Contains(n.id)) continue;

                //calculer le gCost de la tile
                var g_cost = CalculateGCost(n.x, n.y, current_tile.x, current_tile.y, current_tile.g_cost);

                //si la tile n'est pas dans open, on cree un nouveau TileData et on l'ajoute dans open 
                if (!open.Contains(n)){
                    var data = CreateTDataFrom(n.id, current_tile.id, g_cost, CalculateHCost(n.x, n.y), n.x, n.y, false);

                    //ajouter la tile dans open
                    open.Add(n);
                    _tiles[n.id] = data;
                    calculated_tiles.Add(n.id);
                }
                //si il est deja dans open, on verifie si le gCost est plus petit que le gCost de la tile data deja existante
                else if (g_cost < _tiles[n.id].g_cost){
                    var old_data = _tiles[n.id];
                    var data = CreateTDataFrom(n.id, current_tile.id, g_cost, old_data.h_cost, n.x, n.y, false);
                    
                    //remplacer l'ancienne tile data par la nouvelle
                    open.Remove(n);
                    open.Add(data);
                    _tiles[n.id] = data;
                }
            }

        }

        //reconstruire le chemin
        var path = new List<int>();
        var current_path_tile_data = _tiles[_endTile];

        while (current_path_tile_data.id != _startTile)
        {
            path.Add(current_path_tile_data.id);
            current_path_tile_data = _tiles[current_path_tile_data.parent_id];
        }

        path.Reverse();

        return new AStarOperationResult(true, path.ToArray(), calculated_tiles.ToArray());
    }

    private int CalculateHCost(int x, int y)
    {
        var x_pos = Math.Abs((int)x - (int)_tiles[_endTile].x);
        var y_pos = Math.Abs((int)y - (int)_tiles[_endTile].y);

        int res = (int)Math.Ceiling(Math.Sqrt(x_pos*x_pos + y_pos*y_pos) * 10);
        return (int)res;
    }
    private int CalculateGCost(int x, int y, int parentX, int parentY, int parentGCost)
    {
        //si la tile est sur la meme ligne ou colonne que la tile parente, on ajoute 10 au gCost, sinon on ajoute 14
        var to_add = x == parentX || y == parentY ? 10 : 14;
        return (int)(parentGCost + to_add);
    }
    private int CalculateFCost(int gCost, int hCost)
    {
        return gCost + hCost;
    }

    private int[] FindNeighbourOf(int id, int x, int y)
    {
        var result = new List<int>(8);

        if (x != 0) result.Add(id - 1);
        
        if (x != _row_size - 1) result.Add(id + 1);

        if (y != 0) result.Add((int)(id - _row_size));

        if (y != _column_size - 1) result.Add((int)(id + _row_size));

        if (x != 0 && y != 0) result.Add((int)(id - _row_size - 1));

        if (x != _row_size - 1 && y != 0) result.Add((int)(id - _row_size + 1));

        if (x != 0 && y != _column_size - 1) result.Add((int)(id + _row_size - 1));

        if (x != _row_size - 1 && y != _column_size - 1) result.Add((int)(id + _row_size + 1));

        return result.ToArray();


        // var neighbours = new List<Tile>(8);
        // var tilePosition = tile.position;
        // var neighbours_tiles = from t in tiles
        //                        where t.position.X >= tilePosition.X - 1 && t.position.X <= tilePosition.X + 1
        //                        where t.position.Y >= tilePosition.Y - 1 && t.position.Y <= tilePosition.Y + 1
        //                        where t.position != tilePosition
        //                        select t;
        // return neighbours_tiles.ToArray();
    }

    private TileData CreateStartData(int id, int x, int y)
    {
        var h_cost = CalculateHCost(x, y);

        return new TileData(
            0,
            h_cost,
            CalculateFCost(0, h_cost),
            x,
            y,
            id,
            0,
            false
        );
    }
    private TileData FindLowestFCostTileData(List<TileData> tileDatas)
    {
        var lowestFCost = tileDatas.Min(t => t.f_cost);
        var lowest_f_cost_tiles = tileDatas.Where(t => t.f_cost == lowestFCost).ToArray();

        var lowestHCost = lowest_f_cost_tiles.Min(t => t.h_cost);
        return lowest_f_cost_tiles.First(t => t.h_cost == lowestHCost);
    }
    private TileData CreateTDataFrom(int tileId, int parentId, int gCost, int hCost, int x, int y, bool isWall)
    {
        return new TileData(
            gCost,
            hCost,
            CalculateFCost(gCost, hCost),
            x,
            y,
            tileId,
            parentId,
            isWall
        );
    }
}
