using Newtonsoft.Json;

namespace UniversalKeylogger.MVVM.Model
{
    class BarcodeJSON
    {
    [JsonProperty("barcode")]
    public string Barcode { get; set; }

    public void ClearData()
    {
        Barcode = null;
    }

        public BarcodeJSON(string barcode)
        {
            Barcode = barcode;
        }
    }

}
