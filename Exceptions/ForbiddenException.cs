namespace HRIS.Api.Exceptions;

// Dipakai saat user login valid tapi tidak berhak atas resource tertentu
// (mis. Manager mencoba approve leave request bawahan orang lain).
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message) { }
}
