namespace SwiftProcessor.Models;

public class SwiftMessage
{
    public string SenderReference { get; set; } = string.Empty;
    public string ValueDate { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string SenderIban { get; set; } = string.Empty;
    public string SenderDetails { get; set; } = string.Empty;
    public string ReceiverBankBic { get; set; } = string.Empty;
    public string BeneficiaryIban { get; set; } = string.Empty;
    public string BeneficiaryDetails { get; set; } = string.Empty;
    public string RemittanceInfo { get; set; } = string.Empty;
    public string DetailsOfCharges { get; set; } = string.Empty;
    public string RawText { get; set; } = string.Empty;
}