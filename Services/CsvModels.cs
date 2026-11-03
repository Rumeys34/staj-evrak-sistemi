namespace StajEvrakSistemi.Services;

public record StudentImportRow(
    int LineNo, string FullName, string TcKimlikNo, string Uyruk, string StudentNo,
    DateOnly BirthDate, string Address, string Department, string Ek16Year,
    int PreviousInternshipDays, string Email, string Phone, string HealthAidStatus);

public record ImportError(int LineNo, string Message);

public record CsvReadResult(
    IReadOnlyList<StudentImportRow> Rows, IReadOnlyList<ImportError> Errors);
