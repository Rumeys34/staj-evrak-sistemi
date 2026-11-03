namespace StajEvrakSistemi.Services;

public record ImportResult(int Total, int Inserted, int Updated, IReadOnlyList<ImportError> Errors);