using System;
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
            string bitsoSecret = "";
            string bitsoKey = "";

            BitsoController ctr = new BitsoController(bitsoSecret, bitsoKey);

            ctr.GetUserBalance();



        }
    }
}
