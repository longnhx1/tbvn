using QRCoder;

namespace Lab_03.Infrastructure;

/// <summary>Tạo ảnh PNG QR (base64) cho vé điện tử — phục vụ demo đồ án.</summary>
public static class TicketQrPng
{
    public static string ToDataUrlPng(string text)
    {
        var bytes = ToPngBytes(text);
        return "data:image/png;base64," + Convert.ToBase64String(bytes);
    }

    public static byte[] ToPngBytes(string text)
    {
        using var gen = new QRCodeGenerator();
        using var data = gen.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var png = new PngByteQRCode(data);
        return png.GetGraphic(20);
    }
}
