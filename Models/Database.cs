using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using CsQuery;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

namespace IS_Lab1.Models
{
    public class Database: BindableBase
    {
        public HttpClient client;
        public CookieContainer cookie = new CookieContainer() 
        {
            PerDomainCapacity = 100,
            Capacity = 100,
            MaxCookieSize = 16384
        };
        public List<Offer> Offers { get; private set; } = new List<Offer>();
        private CookieContainer cookieContainer = new CookieContainer();
        public int ElementsOnPage { get; private set; }
        public int CntElements { get; private set; }
        public float PercentLoad { get; private set; }
        private string CSRF_Token = "";
        private static bool completed = false;

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);
        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool InternetGetCookieEx(string lpszUrl, string lpszCookieName, StringBuilder lpszCookieData, ref int lpdwSize, int dwFlags, IntPtr lpReserved);
        //using System.Runtime.InteropServices;
        [DllImport("wininet.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);

        const int ERROR_INSUFFICIENT_BUFFER = 122;

        const int INTERNET_COOKIE_HTTPONLY = 0x00002000;

        public static string GetCookies(string uri)
        {
            StringBuilder buffer;
            string result;
            int bufferLength;
            int flags;

            bufferLength = 1024;
            buffer = new StringBuilder(bufferLength);

            flags = INTERNET_COOKIE_HTTPONLY;

            if (InternetGetCookieEx(uri, null, buffer, ref bufferLength, flags, IntPtr.Zero))
            {
                result = buffer.ToString();
            }
            else
            {
                result = null;

                if (Marshal.GetLastWin32Error() == ERROR_INSUFFICIENT_BUFFER)
                {
                    buffer.Length = bufferLength;

                    if (Database.InternetGetCookieEx(uri, null, buffer, ref bufferLength, flags, IntPtr.Zero))
                    {
                        result = buffer.ToString();
                    }
                }
            }

            return result;
        }

        public Database() 
        {
            var handler = new HttpClientHandler()
            { 
                CookieContainer = cookie,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                UseCookies = true
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            if(CSRF_Token != "")
                client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", CSRF_Token);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:70.0) Gecko/20100101 Firefox/70.0");
        }

        public void Load() 
        {
            var json = File.ReadAllText("database.json");
            Offers = JsonConvert.DeserializeObject<List<Offer>>(json);
        }

        public void Save() 
        {
            var json = JsonConvert.SerializeObject(Offers);
            File.WriteAllText("database.json", json);
        }

        static void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            completed = true;
        }

        [STAThread]
        public void GetCSRFToken() 
        {
            CSRF_Token = "";
            completed = false;
            var url = "https://www.dns-shop.ru/catalog/c01df46f39137fd7/stiralnye-mashiny/";
            client.DefaultRequestHeaders.Remove("X-Requested-With");
            client.DefaultRequestHeaders.Remove("X-CSRF-Token");

            //Костыль обманка для обхода проверки на парсер!!!!!!!!!
            WebBrowser wb = new WebBrowser();
            wb.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(wb_DocumentCompleted);
            while (CSRF_Token == "") 
            {

                //wb.Navigate(url);
                wb.ScriptErrorsSuppressed = true;
                //while (!completed)
                //{
                //    Application.DoEvents();
                //    Thread.Sleep(100);
                //}
                //Thread.Sleep(3000);
                cookie.GetCookies(new Uri("https://www.dns-shop.ru/"))
            .Cast<Cookie>()
            .ToList()
            .ForEach(c => c.Expired = true);
                var cq = new CQ(wb.DocumentText);
                var csrfTag = cq["meta[name=csrf-token]"];
                var html = GetURL(url, false);
                completed = false;
                wb.DocumentText = html;
                while (!completed)
                {
                    Application.DoEvents();
                    Thread.Sleep(100);
                }
                Thread.Sleep(15000);

                //cq = new CQ(html);
                //csrfTag = cq["meta[name=csrf-token]"];
                //if (csrfTag[0] != null)
                //    CSRF_Token = csrfTag[0].GetAttribute("content");
                var strCookie = GetCookies("https://www.dns-shop.ru/");
                foreach (var c in strCookie.Split(';'))
                {
                    var t = c.Trim().Split('=');
                    cookie.Add(new Cookie(t[0], t[1], "/", ".dns-shop.ru"));
                }
                html = GetURL(url, false);
                cq = new CQ(html);
                csrfTag = cq["meta[name=csrf-token]"];
                if (csrfTag[0] != null)
                    CSRF_Token = csrfTag[0].GetAttribute("content");
            }
            client.DefaultRequestHeaders.Add("X-CSRF-Token", CSRF_Token);
            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            wb.Dispose();
        }

        public string GetURL(string url, bool key = true)
        {
            var resp = client.GetAsync(url).GetAwaiter().GetResult();
            if (resp.IsSuccessStatusCode) 
            {
                if(key && resp.Content.Headers.ContentType.ToString().Contains("text/html")) 
                {
                    return GetURL(url, key);
                }
                return resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else 
            {
                if(resp.StatusCode == HttpStatusCode.Forbidden) 
                {
                    GetCSRFToken();
                    return GetURL(url, key);
                }
            }
            return "";
        }

        public string PostURL(string url, HttpContent content, bool key = true)
        {
            var resp = client.PostAsync(url, content).GetAwaiter().GetResult();
            if (resp.IsSuccessStatusCode)
            {
                if (key && resp.Content.Headers.ContentType.ToString().Contains("text/html"))
                {
                    Thread.Sleep(1000);
                    return PostURL(url, content, key);
                }
                return resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else 
            {
                if (resp.StatusCode == HttpStatusCode.Forbidden)
                {
                    GetCSRFToken();
                    return PostURL(url, content, key);
                }
            }
            return "";
        }

        class GuidContainer
        {
            public string id { get; set; }
        }

        class PriceRequest
        {
            public string id { get; set; }
            public GuidContainer data { get; set; }
        }

        class PriceContainer 
        {
            public string type { get; set; } = "min-price";
            public List<PriceRequest> containers { get; } = new List<PriceRequest>();
        }

        public void Parse() 
        {
            var min_price = 0;
            var max_price = 500000;
            var order = 6;

            GetCSRFToken();

            GetPage(1, min_price, max_price, order);
            Thread.Sleep(15000);

            for (int page = 2; page * ElementsOnPage <= CntElements; page++) 
            {
                GetPage(page, min_price, max_price, order);
                PercentLoad = (100 * page * ElementsOnPage) / CntElements;
                Thread.Sleep(5000);
                OnPropertyChanged("PercentLoad");
            }
        }

        public void GetPage(int page, int min_price, int max_price, int order = 0)
        {
            string url = "";
            if (page <= 1)
                url = "https://www.dns-shop.ru/catalog/c01df46f39137fd7/stiralnye-mashiny/?price=" + min_price.ToString() + "-" + max_price.ToString();
            else
                url = "https://www.dns-shop.ru/catalog/c01df46f39137fd7/stiralnye-mashiny/?p=" + page.ToString() + "&price=" + min_price.ToString() + "-" + max_price.ToString();
            
            if(order >= 1)
                url += "&order=" + order.ToString();

            var data = GetURL(url);
            var json = JObject.Parse(data);
            var html = json["html"].ToString(); 
            CQ cq = CQ.Create(html);
            var priceRequest = new PriceContainer();
            var containers = priceRequest.containers;
            string cnt = json["data"]["itemsCount"].ToString();
            cq["div .catalog-item"].Each(c =>
            {
                var guid = c.GetAttribute("data-guid");
                if (guid == "") return;
                var offer = new Offer();
                offer.guid = guid;
                var product = c.Cq().Find(".n-catalog-product__main");
                var product_info = product.Find(".product-info");
                var price = product.Find(".product-min-price");
                var link = product_info.Find(".ui-link");
                var pics = product_info.Find("picture");
                offer.Name = link.Text().Trim();
                offer.URL = link[0].GetAttribute("href").Trim();
                if(pics[0] != null)
                    offer.Image = pics[0][0].GetAttribute("data-srcset");
                offer.Rating = product_info.Find(".product-info__rating")[0].GetAttribute("data-rating").Trim();
                offer.Character = product_info.Find(".product-info__title-description").Text().Trim();
                offer.PriceId = price[0].GetAttribute("id");
                containers.Add(new PriceRequest()
                {
                    id = offer.PriceId,
                    data = new GuidContainer()
                    {
                        id = offer.guid
                    }
                }) ;
                GetCharacters(guid, offer);
                Thread.Sleep(1000);
                Offers.Add(offer);
            });

            string jsonPriceRequest = JsonConvert.SerializeObject(priceRequest);

            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                {"data", jsonPriceRequest}
            }) ;

            data = PostURL("https://www.dns-shop.ru/ajax-state/min-price/?cityId=17", content);
       
            json = JObject.Parse(data);
            foreach (var price in json["data"]["states"]) 
            {
                var pid = price["id"].ToString();
                var offer = Offers.Where(t => t.PriceId == pid).First();
                offer.Price = int.Parse(price["data"]["current"].ToString());
                if (offer.Price < 20000) offer.Familly.Budget = Budget.small;
                if (offer.Price >= 20000 && offer.Price < 35000) offer.Familly.Budget = Budget.normal;
                if (offer.Price >= 35000) offer.Familly.Budget = Budget.big;
            }

            if (page == 1)
                ElementsOnPage = Offers.Count;
            int cntElements = 0;
            int.TryParse(string.Join("", cnt.Where(c => char.IsDigit(c))), out cntElements);
            CntElements = cntElements;
        }

        public void GetCharacters(string guid, Offer offer) 
        {
            var data = GetURL("https://www.dns-shop.ru/catalog/product/get-tabs-data/?productGuid=" + guid + "&activeTab=characteristics");
            var json = JObject.Parse(data);
            var html = json["data"]["content"].ToString();
            CQ cq = CQ.Create(html);
            var parse = cq.Find("tr");
            Dictionary<string, string> characters = new Dictionary<string, string>();
            cq["tr"].Each(c =>
            {
                if (c.ChildElements.Count() != 2) return;
                var t1 = c[0].Cq().Text().Trim();
                var t2 = c[1].Cq().Text().Trim();
                if (t1 == null) return;
                characters.Add(t1, t2);
            });

            if (characters["Тип загрузки"] == "фронтальная")
                offer.Washer.LoadType = LoadType.Frontal;
            else
                offer.Washer.LoadType = LoadType.Vertical;

            offer.Washer.Drying = characters["Тип изделия"] == "стирально-сушильная машина";

            if (characters.ContainsKey("Загрузка белья для стирки и отжима"))
            {
                var loadingLaundry = float.Parse(characters["Загрузка белья для стирки и отжима"].Replace(" кг", "").Trim());
                if (loadingLaundry <= 5) offer.Client.Familly.CntPeople = 1;
                if (loadingLaundry > 5 && loadingLaundry <= 8) offer.Familly.CntPeople = 3;
                if (loadingLaundry > 8) offer.Familly.CntPeople = 6;
            }

            string programs = "";
            if (characters.ContainsKey("Программы"))
            {
                programs = characters["Программы"];
                offer.Washer.SpecialPrograms = programs.Contains("пух") || programs.Contains("спорт") || programs.Contains("деликат") || programs.Contains("антиаллегерн");
            }

            if (characters.ContainsKey("Инверторный двигатель"))
                offer.Washer.InverterMotor = characters["Инверторный двигатель"] == "есть";
            if (characters.ContainsKey("Функциональные особенности"))
                offer.Washer.AddingLaundry = characters["Функциональные особенности"].Contains("дозагрузка белья");
            if (characters.ContainsKey("Отсрочка запуска"))
                offer.Washer.DelayStart = characters["Отсрочка запуска"] == "есть";
            if (characters.ContainsKey("Защита от протечек"))
                offer.Washer.LeakageProtection = characters["Защита от протечек"] == "есть";
            if (characters.ContainsKey("Блокировка от детей"))
                offer.Familly.Children = programs.Contains("детск") && (characters["Блокировка от детей"] == "есть");

            var width = float.Parse(characters["Ширина"].Replace(" см", "").Trim());
            var depth = float.Parse(characters["Глубина"].Replace(" см", "").Trim());
            if (width >= 55.1 && depth >= 55.1)
                offer.Placement.Area = Area.big;
            else 
            {
                offer.Placement.Area = Area.normal;
                if(width <=45 || depth <= 45)
                    offer.Placement.Area = Area.small;
            }
        }
    }
}
