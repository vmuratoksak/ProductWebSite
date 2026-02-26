namespace SatisSitesi.Application.Interfaces.Services;

public interface ICurrencyService
{
    /// <summary>
    /// Converts a base TRY price to the user's current culture currency and formats it.
    /// </summary>
    string GetFormattedPrice(decimal basePriceTry);
    
    /// <summary>
    /// Returns the converted raw decimal value for calculation purposes.
    /// </summary>
    decimal GetConvertedPrice(decimal basePriceTry);
}
