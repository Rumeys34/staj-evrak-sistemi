using System;
using System.IO;
using System.Linq;
using UglyToad.PdfPig;


namespace StajEvrakSistemi.Yardımcılar
{
    public class Pdfokuyucu: ITextExtractor
    {
        public bool CanHandle(string dosyauzantısı)
        {
            return dosyauzantısı.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }

        public ExtractionResult Extract(Stream dosyaIcerigi)
        {
            try
            {
                using var pdf = PdfDocument.Open(dosyaIcerigi);
                string metin = string.Join("\n",pdf.GetPages().Select(sayfa => sayfa.Text));
                return new ExtractionResult(true, metin, null);
            }
            catch (Exception hata)
            {
                return new ExtractionResult(false, "", $"PDF okuma hatası: {hata.Message}");
            }
        }
    }
}