using System.Globalization;
using SatisSitesi.Application.Interfaces.Services;

namespace SatisSitesi.Application.Services;

public class CurrencyService : ICurrencyService
{
    // Mock exchange rates referenced to TRY (1 Unit = X TRY)
    // In a real app, this would come from a database or an external API cache.
    private readonly Dictionary<string, decimal> _exchangeRates = new(StringComparer.OrdinalIgnoreCase)
    {
        { "TRY", 1.0m },
        { "USD", 31.50m }, // 1 USD = 31.50 TRY
        { "EUR", 34.00m }, // 1 EUR = 34.00 TRY
        { "SAR", 8.40m }   // 1 SAR = 8.40 TRY
    };

    public string GetFormattedPrice(decimal basePriceTry)
    {
        var currentCulture = CultureInfo.CurrentUICulture.Name;
        var targetCurrency = GetCurrencyForCulture(currentCulture);

        var convertedPrice = ConvertTryToCurrency(basePriceTry, targetCurrency);

        // Map specific culture formats to currencies
        CultureInfo formatCulture = GetFormatCultureForCurrency(targetCurrency);
        
        return convertedPrice.ToString("C", formatCulture);
    }

    public decimal GetConvertedPrice(decimal basePriceTry)
    {
        var currentCulture = CultureInfo.CurrentUICulture.Name;
        var targetCurrency = GetCurrencyForCulture(currentCulture);
        return ConvertTryToCurrency(basePriceTry, targetCurrency);
    }

    private string GetCurrencyForCulture(string cultureName)
    {
        return cultureName.ToLowerInvariant() switch
        {
            "tr" => "TRY",
            "en" => "USD",
            "de" => "EUR",
            "fr" => "EUR",
            "ar" => "SAR",
            _ => "TRY"
        };
    }

    private decimal ConvertTryToCurrency(decimal amountTry, string targetCurrency)
    {
        if (_exchangeRates.TryGetValue(targetCurrency, out var rate) && rate > 0)
        {
            return amountTry / rate;
        }
        return amountTry;
    }

    private CultureInfo GetFormatCultureForCurrency(string targetCurrency)
    {
        return targetCurrency.ToUpperInvariant() switch
        {
            "TRY" => new CultureInfo("tr-TR"),
            "USD" => new CultureInfo("en-US"),
            "EUR" => new CultureInfo("de-DE"), // Standardizing EUR format to German loosely
            "SAR" => new CultureInfo("ar-SA"),
            _ => CultureInfo.CurrentUICulture
        };
    }
}
