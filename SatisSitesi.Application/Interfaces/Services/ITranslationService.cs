using System.Threading.Tasks;

namespace SatisSitesi.Application.Interfaces.Services
{
    public interface ITranslationService
    {
        Task<string> TranslateAsync(string text, string sourceLang, string targetLang);
    }
}
