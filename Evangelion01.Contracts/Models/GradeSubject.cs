using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Evangelion01.Contracts.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GradeSubject
    {
        // Common Subjects
        Mathematics,
        BahasaIndonesia,
        EnglishLanguage,
        Religion,
        PhysicalEducation,
        CivicEducation,
        Crafts,
        Arts,
        BahasaSunda,
        IndonesianHistory,

        // MIPA (Science)
        AdvanceMathematics,
        Biology,
        Physics,
        Chemistry,

        // IPS (Social)
        Economy,
        History,
        Geography,
        Sociology,
        
        // Bahasa (Language and Literature)
        Anthropology,
        IndonesianLiterature,
        EnglishLiterature,
        GermanLanguage,
        JapaneseLanguage,
    }
}
