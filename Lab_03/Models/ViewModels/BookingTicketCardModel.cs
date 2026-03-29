using Lab_03.Models;

namespace Lab_03.Models.ViewModels;

public class BookingTicketCardModel
{
    public required Booking Booking { get; init; }
    public string QrDataUrl { get; init; } = "";
    public string TicketUrl { get; init; } = "";
    public string MockEmailBody { get; init; } = "";
    public bool ShowEmailBlock { get; init; } = true;
}
