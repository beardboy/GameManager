namespace GameManager.Maui.Models
{
    public class ReleaseDateWithCovers
    {
        public DateTimeOffset Date { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }
        
        public string Summary { get; set; }

        public string Company { get; set; }

        public string CoverUrl { get; set; } = "https://images.igdb.com/igdb/image/upload/t_cover_big/co213x.jpg";

        public string BannerUrl { get; set; } = "https://images.igdb.com/igdb/image/upload/t_cover_big/co213x.jpg";
        
        public string NiceDate => (DateTimeOffset.Now - Date).TotalDays < 1 
            ? "Today" 
            : (DateTimeOffset.Now - Date).TotalDays < 7 
                ? $"{(DateTimeOffset.Now - Date).TotalDays:F0} days ago" 
                : $"{Date:MMM dd, yyyy}";

        public string NiceDateV2 => (DateTimeOffset.Now - Date).TotalDays < 1 
            ? $"{Date:MMM dd, yyyy} (Today)"
            : (DateTimeOffset.Now - Date).TotalDays < 7 
                ? $"{Date:MMM dd, yyyy} ({(DateTimeOffset.Now - Date).TotalDays:F0} days ago)" 
                : (DateTimeOffset.Now - Date).TotalDays < 365 
                    ? $"{Date:MMM dd, yyyy} ({((DateTimeOffset.Now - Date).TotalDays / 30):F0} months ago)"
                    : $"{Date:MMM dd, yyyy}";

        public double Rating { get; set; }

    }
}
