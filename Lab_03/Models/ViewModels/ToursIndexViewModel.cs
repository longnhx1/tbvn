using Lab_03.Models;

namespace Lab_03.Models.ViewModels;

public class ToursIndexViewModel
{
    public IReadOnlyList<Tour> Tours { get; set; } = [];
    public IReadOnlyList<Destination> Destinations { get; set; } = [];

    public string? SearchQuery { get; set; }
    public IReadOnlyList<int> SelectedDestinationIds { get; set; } = [];
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? DepartFrom { get; set; }
    public DateTime? DepartTo { get; set; }
    public string? DepartureLocationFilter { get; set; }
}
