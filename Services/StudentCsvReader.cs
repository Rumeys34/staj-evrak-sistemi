using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace StajEvrakSistemi.Services;

public class StudentCsvReader
{
    private static readonly string[] RequiredHeaders =
    {
        "FullName", "TcKimlikNo", "Nationality", "StudentNo", "BirthDate", "Address",
        "Department", "Ek16Year", "PreviousInternshipDays", "Email", "Phone", "HealthAidStatus"
    };

    private static readonly string[] DateFormats = { "dd.MM.yyyy", "d.M.yyyy", "yyyy-MM-dd" };

    public CsvReadResult Read(Stream csv)
    {
        var rows = new List<StudentImportRow>();
        var errors = new List<ImportError>();
        var badLines = new HashSet<int>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            DetectDelimiter = true,
            DetectDelimiterValues = new[] { ",", ";" },
            PrepareHeaderForMatch = args => args.Header.Trim(),
            BadDataFound = args => badLines.Add(args.Context.Parser?.Row ?? 0),
        };

        using var textReader = new StreamReader(csv, Encoding.UTF8, true, 1024, leaveOpen: true);
        using var parser = new CsvReader(textReader, config);

        if (!parser.Read() || !parser.ReadHeader())
        {
            errors.Add(new ImportError(1, "Dosya boş veya başlık satırı okunamadı."));
            return new CsvReadResult(rows, errors);
        }

        var headers = (parser.HeaderRecord ?? Array.Empty<string>()).Select(h => h.Trim()).ToList();
        var missing = RequiredHeaders.Where(h => !headers.Contains(h)).ToList();
        if (missing.Count > 0)
        {
            errors.Add(new ImportError(1, $"Eksik sütun(lar): {string.Join(", ", missing)}"));
            return new CsvReadResult(rows, errors);
        }

        while (parser.Read())
        {
            int lineNo = parser.Parser.Row;   // başlık satırı 1, ilk veri satırı 2

            if (badLines.Contains(lineNo))
            {
                errors.Add(new ImportError(lineNo, "Satırda bozuk tırnak veya veri var."));
                continue;
            }

            try
            {
                string fullName = Get(parser, "FullName");
                string tc = Get(parser, "TcKimlikNo");
                string studentNo = Get(parser, "StudentNo");
                string birthText = Get(parser, "BirthDate");
                string daysText = Get(parser, "PreviousInternshipDays");

                var problems = new List<string>();
                if (fullName == "") problems.Add("FullName boş");
                if (tc == "") problems.Add("TcKimlikNo boş");
                if (studentNo == "") problems.Add("StudentNo boş");
                if (!TryParseDate(birthText, out DateOnly birthDate))
                    problems.Add($"BirthDate geçersiz: '{birthText}' (beklenen: gg.aa.yyyy)");
                if (!int.TryParse(daysText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int days) || days < 0)
                    problems.Add($"PreviousInternshipDays geçersiz: '{daysText}'");

                if (problems.Count > 0)
                {
                    errors.Add(new ImportError(lineNo, string.Join("; ", problems)));
                    continue;
                }

                rows.Add(new StudentImportRow(
                    lineNo, fullName, tc, Get(parser, "Nationality"), studentNo, birthDate,
                    Get(parser, "Address"), Get(parser, "Department"), Get(parser, "Ek16Year"),
                    days, Get(parser, "Email"), Get(parser, "Phone"), Get(parser, "HealthAidStatus")));
            }
            catch (CsvHelperException )
            {
                errors.Add(new ImportError(lineNo, "Satır okunamadı: sütun sayısı başlıkla uyuşmuyor veya veri bozuk."));
            }
        }

        return new CsvReadResult(rows, errors);
    }

    private static string Get(CsvReader parser, string name) => (parser.GetField(name) ?? "").Trim();

    private static bool TryParseDate(string text, out DateOnly date) =>
        DateOnly.TryParseExact(text, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
}
