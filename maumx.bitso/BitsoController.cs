using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using maumx.Bitso.Entities;
using maumx.Bitso.Entities.Response;



namespace maumx.Bitso
{
    public class BitsoController
    {
        private static BitsoController Instance = null;
        protected string BitsoSecret { get; set; }
        protected string BitsoKey { get; set; }
        protected bool IsPrivateAPIConfigured { get; set; }
        protected int MaxRequestPerMinute { get { return 30; } }
        protected int CurrentRequestAttemps { get; set; }
        protected DateTime LastPerformedRequest { get; set; }



        /// <summary>
        ///  Initialice an object for making request to BITSO PUBLIC API
        /// </summary>
        protected BitsoController()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static BitsoController GetInstance()
        {
            if (Instance == null)
            {
                Instance = new BitsoController();
            }
            return Instance;
        }

        /// <summary>
        /// Set's the specified key for making BITSO PRIVATE API requests.
        /// </summary>
        /// <param name="bitsoSecret"></param>
        /// <param name="bitsoKey"></param>
        public BitsoController ConfigureBitsoKeys (string bitsoSecret, string bitsoKey)
        {
            if (string.IsNullOrEmpty(bitsoSecret) || string.IsNullOrEmpty(bitsoKey))
            {
                throw new ArgumentNullException("Bitso Keys", "Keys must be specified.");
            }
            Instance.BitsoSecret = bitsoSecret;
            Instance.BitsoKey = bitsoKey;
            Instance.IsPrivateAPIConfigured = true;

            return Instance;
        }

        /// <summary>
        /// Generate a different Key for BITSO PRIVATE API every time this method is called.
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <param name="requestPath"></param>
        /// <param name="jsonPayload"></param>
        /// <returns>A Bitso signature string </returns>
        private string GenerateSignature(string httpMethod, string requestPath, string jsonPayload)
        {

            byte[] key = System.Text.Encoding.UTF8.GetBytes(Instance.BitsoSecret);
            HMACSHA256 hash = new HMACSHA256(key);
            string once = DateTime.Now.Ticks.ToString();
            string message = once + httpMethod + requestPath + jsonPayload;
            byte[] byteMessage = System.Text.Encoding.UTF8.GetBytes(message);
            string sBuilder = BitConverter.ToString(hash.ComputeHash(byteMessage)).Replace("-", "").ToLower();
            return string.Format("Bitso {0}:{1}:{2}", Instance.BitsoKey, once, sBuilder);

        }

        /// <summary>
        /// 
        /// </summary>
        private void IsValidPrivateApiRquest()
        {
            if (!Instance.IsPrivateAPIConfigured)
            {
                throw new InvalidOperationException("Can not perform request.Bitso Keys are not configured");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void IsValidRequest()
        {
            DateTime currentDateTimeRequest = DateTime.Now;
            if ((currentDateTimeRequest - Instance.LastPerformedRequest).Minutes == 0 )
            {
                if (Instance.CurrentRequestAttemps > Instance.MaxRequestPerMinute)
                {
                    throw new InvalidOperationException("Number of request attemps per minute has been reached");
                }
                
            }
            else
            {
                Instance.CurrentRequestAttemps = 0;
            }

        }

        #region "Public Methods"

        /// <summary>
        /// Make a Http request to BITSO User's Account Balance
        /// </summary>
        /// <returns>An  UserBalance containing an arrays of user's account balance</returns>
        /// <exception cref="WebException" />
        public UserBalance GetUserBalance()
        {
            IsValidPrivateApiRquest();
            string httpMethod = "GET";
            string fullPath = string.Format("{0}{1}", ConfigurationManager.AppSettings["Bitso.BasePath"], ConfigurationManager.AppSettings["Bitso.UserBalancePath"]);
            return MakeRequest<UserBalance>(fullPath, httpMethod);

        }
        #endregion


        #region "Private Methods"

        /// <summary>
        /// Crete an Http Request to the specified BITSO API path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestedPath"></param>
        /// <param name="httpMethod"></param>
        /// <param name="jsonPayload"></param>
        /// <returns></returns>
        private T MakeRequest<T>(string requestedPath, string httpMethod, string jsonPayload = "")
    where T : class
        {
            IsValidRequest();
            GeneralResponse<T> response = new GeneralResponse<T>();
            try
            {
                string signaturePath = requestedPath.Substring(ConfigurationManager.AppSettings["Bitso.BasePath"].Length);
                string signature = GenerateSignature(httpMethod, signaturePath, jsonPayload);
                WebRequest req = WebRequest.Create(requestedPath);
                WebHeaderCollection header = new WebHeaderCollection();
                header.Add("Authorization", signature);
                req.Headers = header;
                req.Method = httpMethod;
                WebResponse resp = req.GetResponse();
                Instance.CurrentRequestAttemps++;
                Instance.LastPerformedRequest = DateTime.Now;
                System.IO.Stream resultado = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(resultado);
                string json = sr.ReadToEnd();
                response = JsonConvert.DeserializeObject<GeneralResponse<T>>(json);
        
            }
            catch (Exception ex)
            {
                throw;
            }

            return response.PayLoad;
        }
        #endregion


    }
}
