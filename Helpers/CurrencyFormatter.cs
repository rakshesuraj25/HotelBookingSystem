using System.Globalization;

namespace HotelBookingSystem.Helpers;

/// <summary>
/// Centralized Indian Rupee (INR) currency formatting helper using en-IN grouping.
/// </summary>
public static class CurrencyFormatter
{
    private static readonly CultureInfo InrCulture = CreateInrCulture();

    private static CultureInfo CreateInrCulture()
    {
        var culture = (CultureInfo)CultureInfo.GetCultureInfo("en-IN").Clone();
        culture.NumberFormat.CurrencySymbol = "₹";
        return culture;
    }

    /// <summary>
    /// Formats an amount in INR with two decimal places (e.g. ₹10,000.00).
    /// </summary>
    public static string FormatInr(decimal amount)
    {
        return string.Format(InrCulture, "{0:C2}", amount);
    }

    /// <summary>
    /// Formats a nightly rate cleanly (e.g. ₹2,000 when whole rupees, or ₹2,000.50 when fractional).
    /// </summary>
    public static string FormatNightlyRate(decimal amount)
    {
        return amount == decimal.Truncate(amount)
            ? string.Format(InrCulture, "{0:C0}", amount)
            : string.Format(InrCulture, "{0:C2}", amount);
    }
}
