// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using System.Net;

public class Program
{
    /// <summary>
    /// Use to all log collect,formating, response to customer 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="err_string"></param>
    /// <param name="ex"></param>
    /// <param name="frm"></param>
    /// <returns></returns>
    public static string? NewInternalServerError(string message, string err_string, Exception ex, Constants.Format frm)
    {
        string res;
        BError err = new BError();
        err.Message = message;
        err.Error = err_string;
        err.Status = (int)HttpStatusCode.InternalServerError;
        err.Causes = ex.Message;

        switch (frm)
        {
            case Constants.Format.JSON:
                res = JsonSerializer.Serialize<BError>(err);
                break;
            default:
                res = err.Message;
                break;
        }
        //TODO
        /// <summary>
        /// write log to cloud, or logsystems
        /// </summary>
        return res;
    }

    public class CultureSpecificQuotedDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return Convert.ToDecimal(reader.GetString(), System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }
            else
            {
                return reader.GetInt32();
            }
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

    }


    public static string url_price = "https://api.coindesk.com/v1/bpi/currentprice.json";

    /// <summary>
    /// All HTTP Get method use
    /// </summary>
    /// <param name="url"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> GetCallAPI<T>(string url)
    {
        var jsonOptions = new JsonSerializerOptions();

        jsonOptions.Converters.Add(new CultureSpecificQuotedDecimalConverter());
        T res;
        using (HttpClient client = new HttpClient())
        {
            var response = await client.GetAsync(url);
            //TODO
            //add authentication

            if (response != null)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                res = JsonSerializer.Deserialize<T>(jsonString, jsonOptions)!;
                return res;
            }
        }
        return default(T);
    }


    public static async Task CoinPrice(string coin_name = "BTC")
    {
        try
        {
            var btc_price = await GetCallAPI<BTC>(url_price);

            Console.WriteLine(String.Format("COIN -> {0}", coin_name));
            Console.WriteLine(String.Format(" Date -> {0}", btc_price!.time!.FirstOrDefault(x => x.Key == "updateduk").Value));
            string v = "";
            int line = 10;
            int j = 0;
            foreach (string i in btc_price!.disclaimer!.Split(' '))
            {
                if (line == j)
                {
                    v += "\r\n  ";
                    j = 0;
                }
                v += i + " ";
                j++;
            }
            Console.WriteLine(String.Format(" Desc -> {0}", v));

            if (btc_price.bpi != null)
            {
                foreach (var bpi in btc_price.bpi.Values)
                {
                    Console.WriteLine(String.Format("   [Currency:{0} -> price:{1:#,##0.00}{2}]", bpi.code, bpi.rate, HttpUtility.HtmlDecode(bpi.symbol!)));
                }
            }

        }
        catch (Exception ex)
        {
            var res_err = NewInternalServerError(ex.Message, "CoinPrice", ex, Constants.Format.JSON);
            Console.WriteLine(res_err);
        }
    }

    public static void Main()
    {
        CoinPrice().Wait();
    }
}