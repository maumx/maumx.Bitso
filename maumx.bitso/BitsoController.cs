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
    public  class BitsoController
    {

        private string BitsoSecret { get; set; }

        private string BitsoKey { get; set; }


        /// <summary>
        /// Constuctor
        /// </summary>
        /// <param name="bitsoSecret"></param>
        /// <param name="bitsoKey"></param>
        public BitsoController(string bitsoSecret, string bitsoKey)
        {
            this.BitsoSecret = bitsoSecret;
            this.BitsoKey = bitsoKey;
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

            byte[] key = System.Text.Encoding.UTF8.GetBytes(BitsoSecret);
            HMACSHA256 hash = new HMACSHA256(key);
            string once = DateTime.Now.Ticks.ToString();
            string message = once + httpMethod + requestPath + jsonPayload;
            byte[] byteMensaje = System.Text.Encoding.UTF8.GetBytes(message);
            string sBuilder = BitConverter.ToString(hash.ComputeHash(byteMensaje)).Replace("-", "").ToLower();
            return  string.Format("Bitso {0}:{1}:{2}", BitsoKey, once, sBuilder);
           
        }

        /// <summary>
        /// Crete an Http Request to the specified BITSO API path.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestedPath"></param>
        /// <param name="httpMethod"></param>
        /// <param name="jsonPayload"></param>
        /// <returns></returns>
        private T MakeRequest<T>(string requestedPath, string httpMethod,string jsonPayload="")
    where T : BitsoEntity,new()
        {
            // T jsonDes = new GeneralResponse<T[]>() ;

            T jsonDes = new T();
            GeneralResponse<T> response = new GeneralResponse<T>();

            try
            {


                string signaturePath = requestedPath.Substring(ConfigurationManager.AppSettings["Bitso.BasePath"].Length);


                string signature = GenerateSignature(httpMethod, signaturePath,jsonPayload);

                WebRequest req = WebRequest.Create(requestedPath);
                WebHeaderCollection header = new WebHeaderCollection();
                header.Add("Authorization", signature);
                req.Headers = header;
                req.Method = httpMethod;
                WebResponse resp = req.GetResponse();
                System.IO.Stream resultado = resp.GetResponseStream();
                System.IO.StreamReader sr = new System.IO.StreamReader(resultado);
                string json = sr.ReadToEnd();
                response = JsonConvert.DeserializeObject<GeneralResponse<T>>(json);


            }
            catch (Exception ex)
            {
                throw ex;

            }

            return jsonDes;
        }


        /// <summary>
        /// Make a Http request to BITSO User's Account Balance
        /// </summary>
        /// <returns></returns>
        public UserBalance GetUserBalance()
        {           
            string httpMethod = "GET";
            string rutaCompleta = string.Format("{0}{1}", ConfigurationManager.AppSettings["Bitso.BasePath"], ConfigurationManager.AppSettings["Bitso.UserBalancePath"]);
            string llave = GenerateSignature(httpMethod, ConfigurationManager.AppSettings["Bitso.UserBalancePath"], "");
            var x=  MakeRequest<UserBalance>(rutaCompleta, httpMethod);
            return x;
        }




    }
}
