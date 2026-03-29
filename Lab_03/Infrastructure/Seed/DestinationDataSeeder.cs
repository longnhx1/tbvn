using Lab_03.DataAccess;
using Lab_03.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Lab_03.Infrastructure.Seed;

public static class DestinationDataSeeder
{
    /// <summary>Tỉnh/thành nổi bật Việt Nam (tên ngắn, ≤ 50 ký tự theo model).</summary>
    private static readonly string[] DefaultNames =
    [
        "Buôn Ma Thuột",
        "Cần Thơ",
        "Đà Lạt",
        "Đà Nẵng",
        "Hạ Long",
        "Hà Giang",
        "Hà Nội",
        "Hải Phòng",
        "Hội An",
        "Huế",
        "Lào Cai",
        "Mỹ Tho",
        "Nha Trang",
        "Ninh Bình",
        "Phú Quốc",
        "Quảng Bình",
        "Quy Nhơn",
        "Sa Pa",
        "TP. Hồ Chí Minh",
        "Vũng Tàu"
    ];

    public static async Task SeedAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var existing = await context.Destinations
            .Select(d => d.Name)
            .ToListAsync();

        var existingSet = new HashSet<string>(existing, StringComparer.OrdinalIgnoreCase);

        var added = 0;
        foreach (var name in DefaultNames)
        {
            if (existingSet.Contains(name))
                continue;

            context.Destinations.Add(new Destination { Name = name });
            existingSet.Add(name);
            added++;
        }

        if (added > 0)
        {
            await context.SaveChangesAsync();
            logger.LogInformation("Đã seed {Count} điểm đến mặc định.", added);
        }
    }
}
