namespace HRIS.Api.Models.Enums;

public static class AssistanceRequestStatus
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Resolved = "Resolved";
    public const string Closed = "Closed";

    public static bool IsValid(string value) => value is Pending or InProgress or Resolved or Closed;
}
