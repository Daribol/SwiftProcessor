using Microsoft.Data.Sqlite;
using SwiftProcessor.Models;

namespace SwiftProcessor.Data;

public class DatabaseService
{
    private const string DbPath = "Data Source=swift_messages.db";

    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection("Data Source=swift_messages.db");
        connection.Open();
        var cmd = connection.CreateCommand();

        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS SwiftData (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Reference TEXT,
            ValueDate TEXT,
            Currency TEXT,
            Amount REAL,
            SenderIban TEXT,
            SenderDetails TEXT,
            ReceiverBankBic TEXT,
            BeneficiaryIban TEXT,
            BeneficiaryDetails TEXT,
            RemittanceInfo TEXT,
            DetailsOfCharges TEXT,
            RawContent TEXT
        )";
        cmd.ExecuteNonQuery();
    }

    public void Save(SwiftMessage msg)
    {
        using var connection = new SqliteConnection("Data Source=swift_messages.db");
        connection.Open();
        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO SwiftData (Reference, ValueDate, Currency, Amount, SenderIban, SenderDetails, ReceiverBankBic, BeneficiaryIban, BeneficiaryDetails, RemittanceInfo, DetailsOfCharges, RawContent)
        VALUES ($ref, $vdate, $cur, $amt, $siban, $sdet, $bic, $biban, $bdet, $rem, $chg, $raw)";

        cmd.Parameters.AddWithValue("$ref", msg.SenderReference);
        cmd.Parameters.AddWithValue("$vdate", msg.ValueDate);
        cmd.Parameters.AddWithValue("$cur", msg.Currency);
        cmd.Parameters.AddWithValue("$amt", msg.Amount);
        cmd.Parameters.AddWithValue("$siban", msg.SenderIban);
        cmd.Parameters.AddWithValue("$sdet", msg.SenderDetails);
        cmd.Parameters.AddWithValue("$bic", msg.ReceiverBankBic);
        cmd.Parameters.AddWithValue("$biban", msg.BeneficiaryIban);
        cmd.Parameters.AddWithValue("$bdet", msg.BeneficiaryDetails);
        cmd.Parameters.AddWithValue("$rem", msg.RemittanceInfo);
        cmd.Parameters.AddWithValue("$chg", msg.DetailsOfCharges);
        cmd.Parameters.AddWithValue("$raw", msg.RawText);

        cmd.ExecuteNonQuery();
    }
}