using System.Collections.Generic;

public static partial class Dictionaries
{
    public static readonly Dictionary<Language, List<string>> Quote = new()
    {
        {
            Language.English,
            new List<string> { "There are two types of people: those who can extrapolate from incomplete data.", "" }
        },
        {
            Language.Romanian,
            new List<string>
                { "Exista doua tipuri de oameni: cei care pot sa extraga concluzii din date incomplete.", "" }
        }
    };
    // TODO: Add more inspirational quote in different language.
}