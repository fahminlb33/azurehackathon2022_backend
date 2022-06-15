using Evangelion01.Contracts.Models;

namespace Evangelion01.Contracts
{
    public static class EntityValueConverter
    {
        private static Dictionary<GradeCategory, GradeSubject[]> _subjectCategoryMapping = new()
        {
            {
                GradeCategory.Stem,
                new[]
                {
                    GradeSubject.Mathematics,
                    GradeSubject.Biology,
                    GradeSubject.Physics,
                    GradeSubject.Chemistry,
                    GradeSubject.AdvanceMathematics
                }
            },
            {
                GradeCategory.Literature,
                new[]
                {
                    GradeSubject.BahasaIndonesia,
                    GradeSubject.EnglishLanguage,
                    GradeSubject.IndonesianLiterature,
                    GradeSubject.EnglishLiterature,
                    GradeSubject.GermanLanguage,
                    GradeSubject.BahasaSunda,
                    GradeSubject.Anthropology
                }
            },
            {
                GradeCategory.Business,
                new[]
                {
                    GradeSubject.Mathematics,
                    GradeSubject.AdvanceMathematics,
                    GradeSubject.Economy,
                    GradeSubject.BahasaIndonesia,
                    GradeSubject.EnglishLanguage
                }
            },
            {
                GradeCategory.Arts,
                new[]
                {
                    GradeSubject.Arts,
                    GradeSubject.Crafts
                }
            },
            {
                GradeCategory.Social,
                new[]
                {
                    GradeSubject.Sociology,
                    GradeSubject.History,
                    GradeSubject.IndonesianHistory,
                    GradeSubject.Geography,
                }
            },
            {
                GradeCategory.Others,
                new[]
                {
                    GradeSubject.CivicEducation,
                    GradeSubject.PhysicalEducation,
                }
            },
        };

        public static Dictionary<GradeCategory, GradeSubject[]> GetSubjectCategoryMapping()
        {
            return _subjectCategoryMapping;
        }

        public static IEnumerable<GradeCategory> GetCategoriesFromSubject(this GradeSubject subject)
        {
            foreach (var item in _subjectCategoryMapping)
            {
                if (item.Value.Contains(subject))
                {
                    yield return item.Key;
                }
            }
        }
    }
}
