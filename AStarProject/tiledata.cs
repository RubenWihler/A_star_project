using System.Drawing.Text;
using System.Linq;
using AStarProject;
using AStarProject.Controls;

internal readonly struct TileData
{
    public readonly int _g_cost;
    public readonly int _h_cost;
    public readonly int _f_cost;
    public readonly int _tile_id;
    public readonly int _tile_x;
    public readonly int _tile_y;
    public readonly int _parent_id;
    public readonly int _parent_x;
    public readonly int _parent_y;
    public readonly bool _has_been_visited;

    public TileData(int g_cost, int h_cost, int f_cost, int tile_id, int tile_x, int tile_y, int parent_id, int parent_x, int parent_y, bool has_been_visited = false)
    {
        _g_cost = g_cost;
        _h_cost = h_cost;
        _f_cost = f_cost;
        _tile_id = tile_id;
        _tile_x = tile_x;
        _tile_y = tile_y;
        _parent_id = parent_id;
        _parent_x = parent_x;
        _parent_y = parent_y;
        _has_been_visited = has_been_visited;
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

    public void Calculate(){
        Tile[] tiles = _tiles.Values.ToArray();
        Tile currentTile = _startTile;
        
        //calculer la tile de depart
        var start_tile_data = CreateStartData(currentTile);

        while (!_hasFounded)
        {
            //chercher les voisins de la tile courante
            var neighbours = FindNeighbourOf(currentTile, tiles);

            //calculer les tiles voisines de la tile courante
            foreach (var n in neighbours)
            {
                //si la tile est un mur on ne la calcule pas
                if (n.Type == TileType.Wall) continue;

                //la tile qui va devenir parent des tiles voisines
                Tile parent_tile = currentTile;
                
                //si la tile a deja ete calculee on la recalcule avec les nouvelles donnees
                if (_tiles_data.ContainsKey(n.Id)){
                    var old_data = _tiles_data[n.Id];
                    var new_data = TileDataFrom(n, parent_tile, _tiles_data[parent_tile.Id], old_data);
                    _tiles_data[n.Id] = new_data;   
                }
                else{
                    //sinon on la calcule normalement
                    var data = TileDataFrom(n, parent_tile, _tiles_data[parent_tile.Id]);
                    _tiles_data.Add(n.Id, data);
                }
            }



            //marquer la tile courante comme visitee
            var current_tile_data = _tiles_data[currentTile.Id];
            _tiles_data[currentTile.Id] = VisiteTile(current_tile_data);

            

            //recuperer les tiles visitees (et donc calculees)
            var calculed_tiles_datas = _tiles_data.Values.ToArray();
            var visited_tile_datas = from t in calculed_tiles_datas
                                     where t._has_been_visited
                                     select t;

            //definir la nouvelle tile courante qui sera la tile avec le plus petit fCost
            var new_current_tile_data = FindLowestFCostTileData(visited_tile_datas.ToArray());
            currentTile = _tiles[new_current_tile_data._tile_id];

            //si la tile courante est la tile de fin, on a fini
            if (currentTile == _endTile) _hasFounded = true;
        }

        //reconstruire le chemin
        var path = new List<Tile>();
        var current_path_tile_data = _tiles_data[_endTile.Id];

        while(current_path_tile_data._tile_id != _startTile.Id)
        {
            path.Add(_tiles[current_path_tile_data._tile_id]);
            current_path_tile_data = _tiles_data[current_path_tile_data._parent_id];
        }

        path.Reverse();

        //afficher le chemin
        path.ForEach(t => t.Type = TileType.Path);

    }


    private int CalculateHCost(Point tilePosition)
    {
        int x = Math.Abs(tilePosition.X - _endTile.position.X);
        int y = Math.Abs(tilePosition.Y - _endTile.position.Y);

        return (int)Math.Ceiling(Math.Sqrt(x*x + y*y) * 10);
    }
    private int CalculateGCost(Point tilePosition, Point parentPosition, int parentGCost)
    {
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
            tile.position.X,
            tile.position.Y,
            -1,
            -1,
            -1
        );
    }
    private TileData TileDataFrom(Tile tile, Tile parentTile, TileData parentTileData)
    {
        var h_cost = CalculateHCost(tile.position);
        var g_cost = CalculateGCost(tile.position, parentTile.position, parentTileData._g_cost);
        var f_cost = CalculateFCost(g_cost, h_cost);

        return new TileData(
            g_cost,
            h_cost,
            f_cost,
            tile.Id,
            tile.position.X,
            tile.position.Y,
            parentTile.Id,
            parentTile.position.X,
            parentTile.position.Y,
            false
        );
    }
    private TileData TileDataFrom(Tile tile, Tile parentTile, TileData parentTileData, TileData oldData)
    {
        var g_cost = CalculateGCost(tile.position, parentTile.position, parentTileData._g_cost);

        // ne pas modifier les données si le gCost est supérieur à l'ancien
        if (g_cost >= oldData._g_cost) return oldData;

        return new TileData(
            g_cost,
            oldData._h_cost,
            CalculateFCost(g_cost, oldData._h_cost),
            oldData._tile_id,
            oldData._tile_x,
            oldData._tile_y,
            parentTile.Id,
            parentTile.position.X,
            parentTile.position.Y,
            oldData._has_been_visited
        );
    }

    private TileData FindLowestFCostTileData(in TileData[] tileDatas)
    {
        var lowestFCost = tileDatas.Min(t => t._f_cost);
        var lowest_f_cost_tiles = tileDatas.Where(t => t._f_cost == lowestFCost).ToArray();

        var lowestHCost = lowest_f_cost_tiles.Min(t => t._h_cost);
        return lowest_f_cost_tiles.First(t => t._h_cost == lowestHCost);
    }

    private TileData VisiteTile(in TileData tileData)
    {
        return new TileData(
            tileData._g_cost,
            tileData._h_cost,
            tileData._f_cost,
            tileData._tile_id,
            tileData._tile_x,
            tileData._tile_y,
            tileData._parent_id,
            tileData._parent_x,
            tileData._parent_y,
            true
        );
    }



}

