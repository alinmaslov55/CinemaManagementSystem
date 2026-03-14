using QRCoder;
using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaSystem.Utility
{
    public static class QRCodeHelper
    {
        public static byte[] GenerateQRCodeBytes(string payload)
        {
            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
