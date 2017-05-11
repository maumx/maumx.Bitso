using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using maumx.Bitso;

namespace maumx.Bitso.Test
{
    [TestClass]
    public class UserBalanceTest
    {
        [TestMethod]
        public void HappyPath()
        {
            try
            {

                BitsoController ctr = BitsoController.GetInstance();
                ctr.ConfigureBitsoKeys(bitsoSecret, bitsoKey);
                while (true)
                {
                    
                    var x = ctr.GetUserBalance();
                }
            }
            catch (WebException ex)
            {
                System.IO.Stream errorStream = ex.Response.GetResponseStream();
                System.IO.StreamReader errorStreamReader = new System.IO.StreamReader(errorStream);
                string jsonError = errorStreamReader.ReadToEnd();
                Assert.Fail(jsonError);
            }
            catch (Exception ex)
            {

                Assert.Fail();
            }



        }
    }
}
