using System.Threading.Tasks;

namespace SatisSitesi.Services.Interfaces
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
