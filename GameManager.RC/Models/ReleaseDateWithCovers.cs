namespace GameManager.Models
{
    public class ReleaseDateWithCovers
    {
        public DateTimeOffset Date { get; set; }

        public string Name { get; set; }

        public string CoverUrl { get; set; } = "https://images.igdb.com/igdb/image/upload/t_cover_big/co213x.jpg";

        public string NiceDate => (DateTimeOffset.Now - Date).TotalDays < 1 
            ? "Today" 
            : (DateTimeOffset.Now - Date).TotalDays < 7 
                ? $"{(DateTimeOffset.Now - Date).TotalDays:F0} days ago" 
                : $"{Date:MMM dd, yyyy}";

        public double Rating { get; set; }

    }
}
