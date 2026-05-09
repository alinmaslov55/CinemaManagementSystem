using QRCoder;
using System;

namespace CinemaSystem.Utility
{
    public static class QRCodeHelper
    {
        public static byte[] GenerateQRCodeBytes(string payload)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new ArgumentException("Payload cannot be null, empty, or whitespace.", nameof(payload));
            }

            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}