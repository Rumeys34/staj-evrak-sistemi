namespace StajEvrakSistemi.Yardımcılar;


public interface ITextExtractor
{
    bool CanHandle(string dosyauzantısı);

    ExtractionResult Extract(Stream dosyaIcerigi);
}
 


public record ExtractionResult(
    bool Başarı,
    string Metin,
    string? Hata,
    bool DesteklenmeyenDosyaFormatı=false
);






public class TextExtractionService
{
    private readonly IEnumerable<ITextExtractor> _metinanla;

    public TextExtractionService(IEnumerable<ITextExtractor> metinanla)
    {
        _metinanla = metinanla;
    }
    public ExtractionResult Extract(string dosya, Stream dosyaIcerigi)

    {
        string uzantı = Path.GetExtension(dosya);
        var metincıkarıcı = _metinanla.FirstOrDefault(e => e.CanHandle(uzantı));
    
    if (metincıkarıcı == null){
        return new ExtractionResult (
            false,
            "",
            "Dosya uzantısı desteklenmiyor.",
            true
        );
    }
    return metincıkarıcı.Extract(dosyaIcerigi);
    }
}