﻿namespace e24PaymentPipe
{
  public class TransactionDetails
  {
    /// <summary>
    /// Representing the result code, based
    /// on the host response code
    /// </summary>
    public ResultOperation Result { get; set; }

    /// <summary>
    /// The authentication code value returned from the host
    /// </summary>
    public string Auth { get; set; }

    /// <summary>
    /// Unique number generated and assigned by the Commerce Gateway
    /// to a transaction going to the host.
    /// </summary>
    public string Ref { get; set; }

    public string Avr { get; set; }

    /// <summary>
    /// The host-specified posting date that will be used for the current transaction
    /// </summary>
    public string PostDate { get; set; }

    /// <summary>
    /// Transaction ID for a single action
    /// generated by Payment Gateway
    /// </summary>
    public string TransId { get; set; }

    /// <summary>
    /// The Merchant assigned tracking ID
    /// </summary>
    public string TrackId { get; set; }

    /// <summary>
    /// User defined field. See the specifications from your payment gateway
    /// to see if this is used and how
    /// </summary>
    public string Udf1 { get; set; }

    /// <summary>
    /// User defined field. See the specifications from your payment gateway
    /// to see if this is used and how
    /// </summary>
    public string Udf2 { get; set; }

    /// <summary>
    /// User defined field. See the specifications from your payment gateway
    /// to see if this is used and how
    /// </summary>
    public string Udf3 { get; set; }

    /// <summary>
    /// User defined field. See the specifications from your payment gateway
    /// to see if this is used and how
    /// </summary>
    public string Udf4 { get; set; }

    /// <summary>
    /// User defined field. See the specifications from your payment gateway
    /// to see if this is used and how
    /// </summary>
    public string Udf5 { get; set; }
  }
}