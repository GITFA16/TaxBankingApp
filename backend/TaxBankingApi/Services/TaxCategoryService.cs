namespace TaxBankingApi.Services;

public class TaxCategoryService
{
    public string GetSuggestedCategory(string description)
    {
        var text = description.ToLower(); // Convert the description to lowercase for case-insensitive comparison

        if (text.Contains("krankenversicherung") ||
            text.Contains("swica") ||
            text.Contains("helsana") ||
            text.Contains("insurance") ||
            text.Contains("health") ||
            text.Contains("css"))
        {
            return "Krankenkasse";
        }

        if (text.Contains("weiterbildung") ||
            text.Contains("course") ||
            text.Contains("University") ||
            text.Contains("schule") ||
            text.Contains("bildung") ||
            text.Contains("Fachhochschule") ||
            text.Contains("abb ts"))
        {
            return "Weiterbildung";
        }

        if (text.Contains("donation") ||
            text.Contains("spende") ||
            text.Contains("help") ||
            text.Contains("wwf") ||
            text.Contains("red cross") ||
            text.Contains("charity") ||
            text.Contains("bantuan"))
        {
            return "Spenden";
        }

        if (text.Contains("3a") ||
            text.Contains("vorsorge")||
            text.Contains("pension") ||
            text.Contains("retirement") ||
            text.Contains("pensionskasse") ||
            text.Contains("pension fund"))
        {
            return "Vorsorge 3a";
        }

        if (text.Contains("mortage interest") ||
            text.Contains("hypothek") ||
            text.Contains("mortage") ||
            text.Contains("zins") ||
            text.Contains("interest"))
        {
            return "Hypothekenzinsen";
        }

        if (text.Contains("childcare") ||
            text.Contains("kinderbetreuung") ||
            text.Contains("daycare") ||
            text.Contains("kindergarten"))
        {
            return "Kinderbetreuung";
        }

        if (text.Contains("public transport") ||
            text.Contains("öffentlicher verkehr") ||
            text.Contains("bahn") ||
            text.Contains("bus") ||
            text.Contains("tram"))
        {
            return "Öffentlicher Verkehr";
        }

        if (text.Contains("professional expenses") ||
            text.Contains("homeoffice") ||
            text.Contains("Dienstreise") ||
            text.Contains("Bussiness travel") ||   
            text.Contains("business trip") ||
            text.Contains("work equipment") ||
            text.Contains("work") ||    
            text.Contains("arbeits"))
        {
            return "Professionelle Auslagen";
        }

        return "Uncategorized";
    }
}