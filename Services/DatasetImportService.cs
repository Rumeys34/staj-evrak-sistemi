using Microsoft.EntityFrameworkCore;
using StajEvrakSistemi.Models;

namespace StajEvrakSistemi.Services;

public class DatasetImportService
{
    private readonly StudentCsvReader _reader;
    private readonly AppDbContext _db;

    public DatasetImportService(StudentCsvReader reader, AppDbContext db)
    {
        _reader = reader;
        _db = db;
    }

    public async Task<ImportResult> ImportAsync(Stream csv)
    {
        CsvReadResult read = _reader.Read(csv);
        var errors = new List<ImportError>(read.Errors);

        // Aynı dosyada tekrar eden StudentNo: ilk satır kalır, sonrakiler hata olur.
        var firstLineOf = new Dictionary<string, int>();
        var uniqueRows = new List<StudentImportRow>();
        foreach (var row in read.Rows)
        {
            if (firstLineOf.TryGetValue(row.StudentNo, out int firstLine))
            {
                errors.Add(new ImportError(row.LineNo,
                    $"StudentNo '{row.StudentNo}' dosyada tekrar ediyor (ilk satır: {firstLine})."));
                continue;
            }
            firstLineOf[row.StudentNo] = row.LineNo;
            uniqueRows.Add(row);
        }

        // Veri tabanında zaten olan öğrencileri tek sorguda getir.
        var numbers = uniqueRows.Select(r => r.StudentNo).ToList();
        var existing = await _db.DatasetStudents
            .Where(s => numbers.Contains(s.StudentNo))
            .ToDictionaryAsync(s => s.StudentNo);

        int inserted = 0, updated = 0;
        foreach (var row in uniqueRows)
        {
            if (existing.TryGetValue(row.StudentNo, out var found))
            {
                Fill(found, row);
                updated++;
            }
            else
            {
                var created = new DatasetStudent();
                Fill(created, row);
                _db.DatasetStudents.Add(created);
                inserted++;
            }
        }

        await _db.SaveChangesAsync();

        // Başlık düzeyindeki hatalar (satır 1) veri satırı sayılmaz.
        int total = read.Rows.Count + read.Errors.Count(e => e.LineNo > 1);
        return new ImportResult(total, inserted, updated, errors.OrderBy(e => e.LineNo).ToList());
    }

    private static void Fill(DatasetStudent s, StudentImportRow r)
    {
        s.FullName = r.FullName;
        s.TcKimlikNo = r.TcKimlikNo;
        s.Nationality = r.Uyruk;
        s.StudentNo = r.StudentNo;
        s.BirthDate = r.BirthDate;
        s.Address = r.Address;
        s.Department = r.Department;
        s.Ek16Year = r.Ek16Year;
        s.PreviousInternshipDays = r.PreviousInternshipDays;
        s.Email = r.Email;
        s.Phone = r.Phone;
        s.HealthAidStatus = r.HealthAidStatus;
    }
}