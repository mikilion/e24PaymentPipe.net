using System;
using System.IO;
using System.Net;
using System.Text;

using e24PaymentPipe.Exceptions;

namespace e24PaymentPipe
{
  /// <summary>
  /// Handles the communication with the payment gateway
  /// <example>
  /// <code>
  /// var paymentServer = new PaymentServerUrlBuilder("bankserver.example.com","/context", UseSSL.on, 443);
  ///Uri paymentServerUrl = paymentServer.ToUrl();
  ///
  ///var init=new PaymentInitMessage(
  ///    id: "yourId",
  ///    password: "yourPassword",
  ///    action: RequiredAction.Authorization,
  ///    amount: 5.30,
  ///    language: AcceptedLanguage.ITA,
  ///    responseURL: new Uri("http://www.example.com/TransactionOK.htm"),
  ///    errorURL: new Uri("http://www.example.com/TransactionKO.htm"),
  ///    trackId: new Guid().ToString(),
  ///    currency: 978
  ///    );
  ///
  ///var pipe = new PaymentPipe(paymentServerUrl);
  ///PaymentDetails paymentdetails=null;
  ///try
  ///{
  ///   paymentdetails = pipe.performPaymentInitialization(init);
  ///}
  ///catch(BadResponseFromWebServiceException ex)
  ///{
  ///   Console.WriteLine("Error! AttemptedUrl:", ex.AttemptedUrl);
  ///    Console.WriteLine("Error! AttemptedParameters:", ex.AttemptedParams);
  ///}
  ///
  ///Console.WriteLine("paymentID: {0}", paymentdetails.paymentId);
  ///Console.WriteLine("paymentpage: {0}", paymentdetails.paymentPage);
  /// </code>
  /// </example>
  /// </summary>
  public sealed class PaymentPipe
  {
    /// <summary>
    /// Url for the payment gateway (should be given by the provider)
    /// </summary>
    public Uri PaymentServerUrl { get; private set; }

    /// <summary>
    /// default constructor
    /// </summary>
    /// <param name="paymentServerUrl"><see cref="PaymentServerUrl" /></param>
    /// <exception cref="ArgumentNullException">thrown if a null paymentServerUrl is
    /// provided</exception>
    public PaymentPipe(Uri paymentServerUrl)
    {
      if (paymentServerUrl == null) throw new ArgumentNullException("paymentServerUrl");
      this.PaymentServerUrl = paymentServerUrl;
    }

    public PaymentPipe()
    {
      // TODO: Complete member initialization
    }

    /// <summary>
    /// sends a payment initialization message to the payment gateway
    /// </summary>
    /// <param name="initMessage">instance of PaymentInitMessage that wraps the
    /// details for the requested operation</param>
    /// <param name="timeout">After this time (in milliseconds), the method will stop waiting for an
    /// answer from the payment gateway. It's an optional parameter.Default 5000 milliseconds.</param>
    /// <returns>PaymentDetails for the requested operation</returns>
    /// <exception cref="e24PaymentPipe.Exceptions.BadResponseFromWebServiceException"></exception>
    public PaymentDetails PerformPaymentInitialization(PaymentInitMessage initMessage, int timeout = 5000)
    {
      if (initMessage == null) throw new ArgumentNullException("initMessage");

      Uri url = BuildUrl("PaymentInitHTTPServlet");

      string
        urlParameters = initMessage.ToUrlParameters(),
        response = sendMessage(url, urlParameters, timeout);

      if (String.IsNullOrEmpty(response) || response.StartsWith("!ERROR!") || !response.Contains(":"))
      {
        throw new BadResponseFromWebServiceException(response, url.ToString(), urlParameters);
      }

      int firstColon = response.IndexOf(":");

      string
        pId = response.Substring(0, firstColon),
        pPage = response.Substring(firstColon + 1);

      return new PaymentDetails(pId, string.Format("{0}?PaymentID={1}", pPage, pId));
    }

    /// <summary>
    /// sends a payment message to the payment gateway
    /// </summary>
    /// <param name="paymentMessage">instance of PaymentMessage that wraps the
    /// details for the requested operation</param>
    /// <param name="timeout">After this time (in milliseconds), the method will stop waiting for an
    /// answer from the payment gateway. It's an optional parameter.Default 5000 milliseconds.</param>
    /// <returns>TransactionDetails for the requested operation</returns>
    /// <exception cref="e24PaymentPipe.Exceptions.BadResponseFromWebServiceException"></exception>
    public TransactionDetails PerformPaymentTransaction(PaymentMessage paymentMessage, int timeout = 5000)
    {
      if (paymentMessage == null) throw new ArgumentNullException("paymentMessage");

      Uri url = BuildUrl("PaymentTranHTTPServlet");

      string
        urlParameters = paymentMessage.ToUrlParameters(),
        response = sendMessage(url, urlParameters, timeout);

      if (String.IsNullOrEmpty(response) || response.StartsWith("!ERROR!") || !response.Contains(":"))
      {
        throw new BadResponseFromWebServiceException(response, url.ToString(), urlParameters);
      }

      System.Diagnostics.Trace.TraceInformation(response);

      TransactionDetails transactionDetail = new TransactionDetails();
      SByte i = 0;

      // Parse response
      foreach (string value in response.Split(':'))
      {
        if (!string.IsNullOrEmpty(value))
        {
          switch (i)
          {
            case 0: transactionDetail.Result = parseResultOperation(value); break;
            case 1: transactionDetail.Auth = Uri.UnescapeDataString(value); break;
            case 2: transactionDetail.Ref = Uri.UnescapeDataString(value); break;
            case 3: transactionDetail.Avr = Uri.UnescapeDataString(value); break;
            case 4: transactionDetail.PostDate = Uri.UnescapeDataString(value); break;
            case 5: transactionDetail.TransId = Uri.UnescapeDataString(value); break;
            case 6: transactionDetail.TrackId = Uri.UnescapeDataString(value); break;
            case 7: transactionDetail.Udf1 = Uri.UnescapeDataString(value); break;
            case 8: transactionDetail.Udf2 = Uri.UnescapeDataString(value); break;
            case 9: transactionDetail.Udf3 = Uri.UnescapeDataString(value); break;
            case 10: transactionDetail.Udf4 = Uri.UnescapeDataString(value); break;
            case 11: transactionDetail.Udf5 = Uri.UnescapeDataString(value); break;
          }
        }

        i++;
      }

      return transactionDetail;
    }

    public NotificationMessage GetPayementStatus(Stream inputStream)
    {
      using (StreamReader reader = new StreamReader(inputStream))
      {
        try
        {
          string postData = reader.ReadToEnd();
          NotificationMessage notificationMsg = new NotificationMessage();

          foreach (String segment in postData.Split('&'))
          {
            string[] singlePair = segment.Split('=');

            if (singlePair.Length == 2)
            {
              string value = Uri.UnescapeDataString(singlePair[1]);

              switch (singlePair[0])
              {
                case "paymentid":

                  notificationMsg.PaymentId = value;

                  break;

                case "tranid":

                  notificationMsg.TranId = value;

                  break;

                case "result":

                  notificationMsg.Result = parseResultOperation(value);

                  break;

                case "auth":

                  notificationMsg.Auth = value;

                  break;

                case "postdate":

                  notificationMsg.PostDate = value;

                  break;

                case "trackid":

                  notificationMsg.TrackId = value;

                  break;

                case "ref":

                  notificationMsg.Ref = value;

                  break;

                case "udf1":

                  notificationMsg.Udf1 = value;

                  break;

                case "udf2":

                  notificationMsg.Udf2 = value;

                  break;

                case "udf3":

                  notificationMsg.Udf3 = value;

                  break;

                case "udf4":

                  notificationMsg.Udf4 = value;

                  break;

                case "udf5":

                  notificationMsg.Udf5 = value;

                  break;

                case "responsecode":

                  notificationMsg.ResponseCode = value;

                  break;

                case "cardtype":

                  switch (value)
                  {
                    case "VISA":

                      notificationMsg.CardType = Brand.VISA;

                      break;

                    case "MC":

                      notificationMsg.CardType = Brand.MC;

                      break;

                    case "AMEX":

                      notificationMsg.CardType = Brand.AMEX;

                      break;

                    case "DINERS":

                      notificationMsg.CardType = Brand.DINERS;

                      break;

                    case "JCB":

                      notificationMsg.CardType = Brand.JCB;

                      break;
                  }

                  break;

                case "payinst":

                  switch (value)
                  {
                    case "CC":

                      notificationMsg.Payinst = PaymentInstrument.CC;

                      break;

                    case "VPAS":

                      notificationMsg.Payinst = PaymentInstrument.VPAS;

                      break;
                  }

                  break;

                case "liability":

                  switch (value)
                  {
                    case "N":

                      notificationMsg.Liability = LiabilityShift.N;

                      break;

                    case "Y":

                      notificationMsg.Liability = LiabilityShift.Y;

                      break;
                  }

                  break;

                case "Error":

                  notificationMsg.Error = value;

                  break;

                case "ErrorText":

                  notificationMsg.ErrorText = value;

                  break;
              }
            }
          }

          return notificationMsg;
        }
        catch (Exception e)
        {
          System.Diagnostics.Trace.TraceError(e.StackTrace);

          return null;
        }
      }
    }

    private Uri BuildUrl(string servletName)
    {
      var sb = new StringBuilder(this.PaymentServerUrl.AbsoluteUri);
      sb.Append("servlet/");
      sb.Append(servletName);

      return new Uri(sb.ToString());
    }

    private string sendMessage(Uri url, string data, int timeout = 5000)
    {
      WebRequest webRequest = WebRequest.Create(url);
      webRequest.Method = "POST";
      webRequest.Timeout = timeout; // wait for 5 seconds and then timeout

      webRequest.ContentType = "application/x-www-form-urlencoded";

      using (Stream reqStream = webRequest.GetRequestStream())
      {
        byte[] postArray = Encoding.ASCII.GetBytes(data);
        reqStream.Write(postArray, 0, postArray.Length);
        reqStream.Close();
      }

      using (StreamReader sr = new StreamReader(webRequest.GetResponse().GetResponseStream()))
      {
        string resp = sr.ReadToEnd();
        return resp;
      }
    }

    private string[] ParseResponse(string response)
    {
      return response.Split(':');
    }

    private ResultOperation parseResultOperation(string result)
    {
      switch (result)
      {
        case "APPROVED": return ResultOperation.Approved;
        case "NOT+APPROVED": return ResultOperation.NotCaptured;
        case "CAPTURED": return ResultOperation.Captured;
        case "NOT+CAPTURED": return ResultOperation.NotCaptured;
        case "DENIED+BY+RISK": return ResultOperation.DeniedByRisk;
        case "HOST+TIMEOUT": return ResultOperation.HostTimeout;
        case "REVERSED": return ResultOperation.Reversed;
        case "VOIDED": return ResultOperation.Voided;
        default: return ResultOperation.nothing;
      }
    }
  }
}