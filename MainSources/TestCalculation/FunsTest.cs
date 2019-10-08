using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Calculation;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestCalculation
{
    [TestClass]
    public class FunsTest
    {
        [TestMethod]
        public void Speed()
        {
            var fun = new Funs();
            fun.Begin = new DateTime(2000, 1, 1, 1, 0, 0);
            fun.End = new DateTime(2000, 1, 1, 1, 0, 25);
            var beg = fun.Begin;
            var par = new SingleValue[3];
            par[1] = new SingleValue(new Moment(3));
            par[2] = new SingleValue(new Moment(0));
            par[0] = new SingleValue(new List<Moment>
                {
                    new Moment(beg.AddSeconds(5), 1),
                    new Moment(beg.AddSeconds(6), 2),
                    new Moment(beg.AddSeconds(7), 3),
                    new Moment(beg.AddSeconds(8), 4),
                    new Moment(beg.AddSeconds(9), 2, null, 1),
                    new Moment(beg.AddSeconds(10), 4),
                    new Moment(beg.AddSeconds(20), 0)
                });
            var res = new SingleValue();
            fun.speedrir(par, res);
            Assert.AreEqual(res.Moments.Count, 7);
            Assert.AreEqual(Math.Round(res.Moments[0].Real, 4), 0.1129);
            Assert.AreEqual(res.Moments[1].Real, 1);
            Assert.AreEqual(res.Moments[2].Real, -0.5);
            Assert.AreEqual(res.Moments[3].Real, 0);
            Assert.AreEqual(Math.Round(res.Moments[4].Real, 5), -0.05714);
            Assert.AreEqual(Math.Round(res.Moments[5].Real, 4), -0.4);
            Assert.AreEqual(Math.Round(res.Moments[6].Real, 4), -0.2526);
            
            Assert.AreEqual(res.Moments[0].Nd, 0);
            Assert.AreEqual(res.Moments[1].Nd, 0);
            Assert.AreEqual(res.Moments[2].Nd, 1);
            Assert.AreEqual(res.Moments[3].Nd, 1);
            Assert.AreEqual(res.Moments[4].Nd, 1);
            Assert.AreEqual(res.Moments[5].Nd, 0);
            Assert.AreEqual(res.Moments[6].Nd, 0);

            Assert.AreEqual(res.Moments[0].Time, beg.AddSeconds(0));
            Assert.AreEqual(res.Moments[1].Time, beg.AddSeconds(5));
            Assert.AreEqual(res.Moments[2].Time, beg.AddSeconds(7));
            Assert.AreEqual(res.Moments[3].Time, beg.AddSeconds(8));
            Assert.AreEqual(res.Moments[4].Time, beg.AddSeconds(9));
            Assert.AreEqual(res.Moments[5].Time, beg.AddSeconds(10));
            Assert.AreEqual(res.Moments[6].Time, beg.AddSeconds(14));
        }
    }
}
