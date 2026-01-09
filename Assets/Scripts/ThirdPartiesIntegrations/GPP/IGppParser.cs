namespace ThirdPartiesIntegrations.GPP
{
    public interface IGppParser
    {
        bool Parse(string gpp, out DecodedGppData decodedGpp);
    }
}
