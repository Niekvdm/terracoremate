namespace TerracoreMate.Models.Terracore;

public class PaginatedPlayersResponse
{
    public Pagination Pagination { get; set; }
    public List<Player> Players { get; set; }
}