using System.Text.RegularExpressions;
using System.Globalization;
using SwiftProcessor.Models;
using System.Linq;

namespace SwiftProcessor.Services;

public class SwiftParser
{
    public SwiftMessage Parse(string rawText)
    {
        var message = new SwiftMessage { RawText = rawText };

        // :20: Sender's reference
        message.SenderReference = ExtractTagValue(rawText, "20");

        // :32A: Value date (YYMMDD), Currency (3 chars), Amount (rest)
        var f32A = ExtractTagValue(rawText, "32A");
        if (f32A.Length >= 9)
        {
            message.ValueDate = f32A.Substring(0, 6);
            message.Currency = f32A.Substring(6, 3);
            if (decimal.TryParse(f32A.Substring(9).Replace(',', '.'), CultureInfo.InvariantCulture, out decimal val))
                message.Amount = val;
        }

        // :50K: Ordering customer - first line is treated as IBAN, remaining lines as details
        var f50K = ExtractTagValue(rawText, "50K");
        var sLines = f50K.Split('\n');
        message.SenderIban = sLines[0].Trim();
        message.SenderDetails = string.Join(", ", sLines.Skip(1)).Trim();

        // :57A: Receiver bank BIC
        message.ReceiverBankBic = ExtractTagValue(rawText, "57A");

        // :59: Beneficiary - first line IBAN, remaining lines details
        var f59 = ExtractTagValue(rawText, "59");
        var bLines = f59.Split('\n');
        message.BeneficiaryIban = bLines[0].Trim();
        message.BeneficiaryDetails = string.Join(", ", bLines.Skip(1)).Trim();

        // :70: Remittance information (single-line preference)
        message.RemittanceInfo = ExtractTagValue(rawText, "70").Replace("\n", " ");

        // :71A: Details of charges
        message.DetailsOfCharges = ExtractTagValue(rawText, "71A");

        return message;
    }

    private string ExtractTagValue(string text, string tag)
    {
        var pattern = $@"(?<=:{tag}:)(.*?)(?=\r?\n:|(?:\r?\n)?-}})";
        var match = Regex.Match(text, pattern, RegexOptions.Singleline);
        return match.Success ? match.Value.Trim() : string.Empty;
    }
}