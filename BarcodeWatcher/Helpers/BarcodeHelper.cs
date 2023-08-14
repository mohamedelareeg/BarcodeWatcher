using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace BarcodeWatcher.Helper
{
    public static class BarcodeHelper
    {
        public static BarcodeFormat[] SupportedFormats { get; } = new BarcodeFormat[]
        {
            BarcodeFormat.QR_CODE,
            BarcodeFormat.CODE_128,
            BarcodeFormat.CODE_39
        };
        public static Result DecodeBarcode(string imagePath)
        {
            try
            {
                BarcodeReader reader = new BarcodeReader
                {
                    AutoRotate = true, // Enable auto-rotation of the image
                    Options = new DecodingOptions
                    {
                        TryHarder = true, // Improve performance by spending more time to detect barcode
                        PossibleFormats = SupportedFormats // Set the supported barcode formats
                    }
                };

                // Load the image from the file path
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    Result result = reader.Decode(bitmap);
                    if (result != null)
                    {
                        string barcodeType = result.BarcodeFormat.ToString();
                        Console.WriteLine("Barcode type: " + barcodeType);
                        return result;
                    }
                    else
                    {
                        Console.WriteLine("No barcode detected.");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Barcode decoding error: " + ex.Message);
                return null;
            }
        }

    }
}
