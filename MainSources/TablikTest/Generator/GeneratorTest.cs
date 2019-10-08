using BaseLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tablik.Generator;

namespace TablikTest.Generator
{
    [TestClass]
    public class GeneratorTest
    {
        [TestMethod]
        public void Generate()
        {
            var tdir = Different.GetTestInfoTaskDir() + @"Tablik\Generator\";
            var generator = new TablikGenerator();
            generator.Generate(tdir + "Project1Gen.accdb", tdir + "Project1GenData.accdb", tdir + "Project1Result.accdb");
            generator.Close();
        }

        [TestMethod]
        public void GenerateRasSurgut()
        {
            var tdir = Different.GetTestInfoTaskDir() + @"Tablik\Generator\RasSurgut\";
            var generator = new TablikGenerator();
            generator.Generate(tdir + "RasProjectGen.accdb", tdir + "RasProjectGenData.accdb", tdir + "RasProject.accdb");
            generator.Close();
        }

        [TestMethod]
        public void GenerateRasKurgan()
        {
            var tdir = Different.GetTestInfoTaskDir() + @"Tablik\Generator\RasKurgan\";
            var generator = new TablikGenerator();
            generator.Generate(tdir + "RasProjectGen.accdb", tdir + "RasProjectGenData.accdb", tdir + "RasProject.accdb");
            generator.Close();
        }
    }
}
