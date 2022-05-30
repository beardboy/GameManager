using System.Globalization;
using IGDB.Models;

namespace GameManager.Converters
{
    internal class CoverMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return string.Empty;
            }

            if (values.Length < 2)
            {
                return string.Empty;
            }

            var gameId = (long?)values[0] ?? 0;
            var covers = (List<Cover>)values[1] ?? new List<Cover>();

            if (!covers.Any())
            {
                return string.Empty;
            }

            var coverUrl = covers.FirstOrDefault(c => c.Game.Id == gameId).Url;
            if (string.IsNullOrEmpty(coverUrl))
            {
                return string.Empty;
            }

            return $"https:{coverUrl}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
