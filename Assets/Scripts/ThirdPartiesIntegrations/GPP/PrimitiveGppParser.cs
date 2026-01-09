using System;

namespace ThirdPartiesIntegrations.GPP
{
    /// <summary>
    /// Primitive parser to get user consent from GPP string. This class works only with "usnat" section.
    /// And supports only "DBABL" and "DBABLA" hard coded headers. Also decoded section data contains only first 10
    /// entries starting from Version and ending with TargetedAdvertisingOptOut.
    /// </summary>
    /// <remarks>
    /// Note: This is a temporary solution. The comprehensive Global Privacy Protocol Encoder/Decoder is on the way
    /// and currently under development.
    /// </remarks>
    public class PrimitiveGppParser : IGppParser
    {
        private const int VersionSizeInBits = 6;

        public bool Parse(string gpp, out DecodedGppData decodedData)
        {
            decodedData = new DecodedGppData();
            string[] segments = gpp.Split('~');
            string header = segments[0];
            string section = segments[1];
            if (header != "DBABL" && header != "DBABLA")
            {
                CSDL.LogWarning($"{nameof(PrimitiveGppParser)} can't parse GPP string. Usupported header format.");
                return false;
            }
            return EnrichDecodedDataWithSection(section, ref decodedData);
        }

        private bool EnrichDecodedDataWithSection(string section, ref DecodedGppData dataToEnrich)
        {
            string binarySequence = string.Empty;
            byte[] decodedSection = Base64UrlDecode(section);
            if (decodedSection == null)
            {
                return false;
            }
            foreach (byte b in decodedSection)
            {
                binarySequence += ByteToBinaryString(b);
            }
            // The best solution to replace this would likely be to create a class of instructions that defines
            // the size of each parameter in bits, and then parse the entire GPP string using this instruction.
            dataToEnrich.Version = BinaryToInteger(binarySequence.Substring(0, VersionSizeInBits));
            dataToEnrich.SharingNotice = BinaryToInteger(binarySequence[6], binarySequence[7]);
            dataToEnrich.SaleOptOutNotice = BinaryToInteger(binarySequence[8], binarySequence[9]);
            dataToEnrich.SharingOptOutNotice = BinaryToInteger(binarySequence[10], binarySequence[11]);
            dataToEnrich.TargetedAdvertisingOptOutNotice = BinaryToInteger(binarySequence[12], binarySequence[13]);
            dataToEnrich.SensitiveDataProcessingOptOutNotice = BinaryToInteger(binarySequence[14], binarySequence[15]);
            dataToEnrich.SensitiveDataLimitUseNotice = BinaryToInteger(binarySequence[16], binarySequence[17]);
            dataToEnrich.SaleOptOut = BinaryToInteger(binarySequence[18], binarySequence[19]);
            dataToEnrich.SharingOptOut = BinaryToInteger(binarySequence[20], binarySequence[21]);
            dataToEnrich.TargetedAdvertisingOptOut = BinaryToInteger(binarySequence[22], binarySequence[23]);
            return true;
        }

        private int BinaryToInteger(char bit1, char bit2)
        {
            return Convert.ToInt32($"{bit1}{bit2}", 2);
        }

        private int BinaryToInteger(string binary)
        {
            return Convert.ToInt32(binary, 2);
        }

        private string ByteToBinaryString(byte b)
        {
            string binary = "";
            string hexadecimal = b.ToString("X2");
            foreach (char c in hexadecimal) {
                int integer = Convert.ToInt32(c.ToString(), 16);
                binary += Convert.ToString(integer, 2).PadLeft(4, '0');
            }
            return binary;
        }

        private byte[] Base64UrlDecode(string input)
        {
            string base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            byte[] convertationResult;
            try
            {
                convertationResult = Convert.FromBase64String(base64);
            }
            catch (Exception ex)
            {
                CSDL.LogError($"{nameof(PrimitiveGppParser)} can't decode segment of GPP string with the reason: " +
                    $"{ex.Message}");
                return null;
            }
            return convertationResult;
        }
    }
}
