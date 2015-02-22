﻿namespace e24PaymentPipe
{
  public enum ResultOperation
  {
    /// <summary>
    /// The transaction was approved
    /// </summary>
    Approved,

    /// <summary>
    /// The transaction was not approved
    /// </summary>
    NotApproved,

    /// <summary>
    /// The transaction was captured
    /// </summary>
    Captured,

    /// <summary>
    /// The transaction was voided
    /// </summary>
    Voided,

    /// <summary>
    /// The transaction has been reversed
    /// </summary>
    Reversed,

    /// <summary>
    /// The transaction was not captured
    /// </summary>
    NotCaptured,

    /// <summary>
    /// Risk denied the transaction
    /// </summary>
    DeniedByRisk,

    /// <summary>
    /// The authorization system did not respond within the 
    /// timeout limit
    /// </summary>
    HostTimeout,
    nothing
  }

  /// <summary>
  /// Card type
  /// </summary>
  public enum Brand
  {
    /// <summary>
    /// Visa
    /// </summary>
    VISA,

    /// <summary>
    /// Master Card
    /// </summary>
    MC,

    /// <summary>
    /// American Express
    /// </summary>
    AMEX,

    /// <summary>
    /// Diners Club
    /// </summary>
    DINERS,

    /// <summary>
    /// JCB
    /// </summary>
    JCB
  }

  /// <summary>
  /// Payment Instrument
  /// </summary>
  public enum PaymentInstrument
  {
    VPAS,

    CC
  }

  public enum LiabilityShift
  {
    /// <summary>
    /// The Merchant is guaranteed against chargebacks.
    /// </summary>
    Y,

    /// <summary>
    /// The Merchant is not guaranteed against chargebacks.
    /// </summary>
    N
  }

  public class NotificationMessage
  {
    /// <summary>
    /// Unique ID generated by and used within the Payment Gateway to identify a payment.
    /// </summary>
    public string PaymentId { get; set; }

    /// <summary>
    /// Transaction ID for a single action
    /// generated by Payment Gateway
    /// </summary>
    public string TranId { get; set; }

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
    /// The host-specified posting date that will be used for the current transaction
    /// </summary>
    public string PostDate { get; set; }

    /// <summary>
    /// The Merchant assigned tracking ID
    /// </summary>
    public string TrackId { get; set; }

    /// <summary>
    /// Unique number generated and assigned by the Commerce Gateway
    /// to a transaction going to the host.
    /// </summary>
    public string Ref { get; set; }

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

    /// <summary>
    /// The host authorization response code from which the result code is calculated.
    /// </summary>
    public string ResponseCode { get; set; }

    /// <summary>
    /// Card type
    /// </summary>
    public Brand CardType { get; set; }

    public PaymentInstrument Payinst { get; set; }

    public LiabilityShift Liability { get; set; }

    public string Error { get; set; }

    public string ErrorText { get; set; }
  }
}