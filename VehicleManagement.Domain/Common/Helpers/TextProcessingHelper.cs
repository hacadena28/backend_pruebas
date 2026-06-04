using System.Globalization;
using System.Text;

namespace VehicleManagement.Domain.Common.Helpers;

/// <summary>
/// Proporciona métodos de utilidad para el manejo y transformación de cadenas de texto.
/// </summary>
public static class TextProcessingHelper
{
    /// <summary>
    /// Elimina los acentos y diacríticos de una cadena, dejando solo caracteres ASCII básicos.
    /// </summary>
    /// <param name="text">Texto de entrada con posibles caracteres acentuados.</param>
    /// <returns>Cadena sin acentos ni diacríticos.</returns>
    public static string RemoveDiacritics(string text)
    {
        return string.Concat(text
            .Normalize(NormalizationForm.FormD)
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
            .Normalize(NormalizationForm.FormC);
    }
}
