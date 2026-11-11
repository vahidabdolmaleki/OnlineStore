using System.Globalization;
using System.Text;

namespace Shop.Application.Helpers;

public static class PersianOrderDate
{
    private static readonly TimeZoneInfo IranTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "Iran Standard Time"
                : "Asia/Tehran");

    public static DateTime? StartOfDayUtc(string? persianDate)
    {
        if (persianDate is null)
            return null;

        var iranMidnight = ParsePersianDate(persianDate);
        return TimeZoneInfo.ConvertTimeToUtc(iranMidnight, IranTimeZone);
    }

    public static DateTime? StartOfNextDayUtc(string? persianDate)
    {
        if (persianDate is null)
            return null;

        // اول روز بعد را در تقویم/ساعت ایران می‌سازیم،
        // بعد آن را به UTC تبدیل می‌کنیم.
        var nextIranMidnight = ParsePersianDate(persianDate).AddDays(1);
        return TimeZoneInfo.ConvertTimeToUtc(nextIranMidnight, IranTimeZone);
    }

    private static DateTime ParsePersianDate(string input)
    {
        var normalized = NormalizeDigits(input.Trim());

        if (normalized.Length != 10 ||
            normalized[4] != '/' ||
            normalized[7] != '/' ||
            !int.TryParse(normalized[..4], NumberStyles.None,
                CultureInfo.InvariantCulture, out var year) ||
            !int.TryParse(normalized.Substring(5, 2), NumberStyles.None,
                CultureInfo.InvariantCulture, out var month) ||
            !int.TryParse(normalized.Substring(8, 2), NumberStyles.None,
                CultureInfo.InvariantCulture, out var day))
        {
            throw new ArgumentException(
                "تاریخ شمسی باید به شکل yyyy/MM/dd باشد؛ مثال: 1405/08/20.");
        }

        try
        {
            var date = new PersianCalendar().ToDateTime(
                year, month, day, 0, 0, 0, 0);

            // این زمان، ساعت محلی ایران است؛ نه ساعت سیستم و نه UTC.
            return DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            throw new ArgumentException(
                $"تاریخ شمسی «{input}» معتبر نیست.", ex);
        }
    }

    private static string NormalizeDigits(string value)
    {
        var result = new StringBuilder(value.Length);

        foreach (var character in value)
        {
            if (character is >= '۰' and <= '۹')
                result.Append((char)('0' + character - '۰'));
            else if (character is >= '٠' and <= '٩')
                result.Append((char)('0' + character - '٠'));
            else
                result.Append(character);
        }

        return result.ToString();
    }
}
