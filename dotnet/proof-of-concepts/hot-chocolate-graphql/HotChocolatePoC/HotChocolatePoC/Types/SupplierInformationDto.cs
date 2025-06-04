namespace HotChocolatePoC.Types;

/// <summary>
/// Represents Supplier Information.
/// </summary>
public class SupplierInformationDto
{
    /// <summary>
    /// The currency in which the amounts a calculated.
    /// </summary>
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Id of a supplier at Relaxdays (creditor number).
    /// </summary>
    public string Kto { get; set; }
}