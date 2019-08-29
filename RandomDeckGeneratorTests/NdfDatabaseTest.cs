using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeckGenerator;

namespace RandomDeckGeneratorTests {
    [TestClass]
    public class NdfDatabaseTest {
        static public NdfDatabase database;
        static public List<WargameUnit> units;

        static public List<Countries> otanCountries = new List<Countries>() { Countries.US, Countries.UK, Countries.FR, Countries.RFA, Countries.CAN, Countries.DAN, Countries.SWE, Countries.NOR,
            Countries.ROK, Countries.JAP, Countries.ANZ, Countries.HOL, Countries.ISR };

        static public List<Countries> pactCountries = new List<Countries>() { Countries.URSS, Countries.RDA, Countries.POL, Countries.TCH, Countries.NK, Countries.CHI, Countries.FIN, Countries.YUG };

        [ClassInitialize]
        static public void ClassInit(TestContext context) {
            database = new NdfDatabase("NDF_Win.dat");
            units = database.GetOtanUnits();
        }

        [TestMethod]
        public void LoadsEverythingBinary() {
            Assert.IsNotNull(database);
        }

        [TestMethod]
        public void LoadsAllOtanUnits() {
            AssertNotEmptyAndWithTransports(units);
            foreach(var country in otanCountries) {
                int count = units.Count(unit => unit.Country == country.ToString());
                Assert.IsTrue(count > 0, country.ToString() + ": should have units");
            }

            foreach (var country in pactCountries) {
                int count = units.Count(unit => unit.Country == country.ToString());
                Assert.IsTrue(count == 0, country.ToString() + ": should have no units, but has " + count);
            }
        }

        [TestMethod]
        public void LoadsAllPactUnits() {
            List<WargameUnit> pactUnits = database.GetPactUnits();
            AssertNotEmptyAndWithTransports(pactUnits);
            foreach (var country in pactCountries) {
                int count = pactUnits.Count(unit => unit.Country == country.ToString());
                Assert.IsTrue(count > 0, country.ToString() + ": should have units");
            }

            foreach (var country in otanCountries) {
                int count = pactUnits.Count(unit => unit.Country == country.ToString());
                Assert.IsTrue(count == 0, country.ToString() + ": should have no units, but has " + count);
            }
        }

        private void AssertNotEmptyAndWithTransports(List<WargameUnit> units) {
            Assert.IsTrue(units.Count > 0);
            List<WargameUnit> unitsWithTransport = units.FindAll(unit => unit.Transports.Count > 0);
            Assert.IsTrue(unitsWithTransport.Count > 0);
        }

        [TestMethod]
        public void FiltersTransports() {
            HashSet<uint> transportsSet = new HashSet<uint>();
            HashSet<uint> unitsSet = new HashSet<uint>();
            foreach (var unit in RandomDeckGenerator.FilterOutTransports(units)) {
                unitsSet.Add(unit.UnitId);
                foreach (var transport in unit.Transports)
                    transportsSet.Add(transport.UnitId);
            }
            Assert.AreEqual(0, unitsSet.Intersect(transportsSet).ToList().Count);
        }

        [TestMethod]
        public void LoadsNumberOfCardsForUnit() {
            WargameUnit groupeCmd = units.Find(unit => unit.UnitId == 1);
            Assert.AreEqual(1u, groupeCmd.Cards);
            Assert.AreEqual(1u, groupeCmd.Transports[0].Cards);
            Assert.AreEqual(5u, groupeCmd.Transports[1].Cards);
            Assert.AreEqual(3u, groupeCmd.Transports[2].Cards);
            Assert.AreEqual(9u, groupeCmd.Transports[3].Cards);
            Assert.AreEqual(3u, groupeCmd.Transports[4].Cards);
        }

        [TestMethod]
        public void LoadsUnitVeterancy() {
            WargameUnit km132Rok = units.Find(unit => unit.UnitId == 200);
            Assert.AreEqual(2, km132Rok.AvailableVeterancy.Count);
            Assert.AreEqual(1u, km132Rok.AvailableVeterancy[0]);
            Assert.AreEqual(2u, km132Rok.AvailableVeterancy[1]);
        }

        [TestMethod]
        public void LoadsUnitCategory() {
            WargameUnit amx30b2 = units.Find(unit => unit.UnitId == 14);
            Assert.AreEqual(Categories.TANK, amx30b2.Category);
        }

        [TestMethod]
        public void LoadsUnitYear() {
            WargameUnit commandoMarine = units.Find(unit => unit.UnitId == 110);
            Assert.AreEqual(1975u, commandoMarine.Year);
        }

        [TestMethod]
        public void LoadsDefaultActivationPoints() {
            Assert.AreEqual(45, database.GetDefaultActivationPoints());
        }

        [TestMethod]
        public void LoadsCountrySpecificActivationPoints() {
            Assert.AreEqual(60, database.GetActivationPoints(Countries.UK));
        }

        [TestMethod]
        public void LoadsUnfilteredCostMatrix() {
            Dictionary<Categories, List<int>> expectedCostMatrix = new Dictionary<Categories, List<int>> {
                {Categories.LOGISTICS, new List<int>() { 1, 2, 2, 2, 3, 1, 1, 1, 1 } },
                {Categories.INFANTRY, new List<int>() { 1, 2, 2, 2, 3, 1, 1, 1, 1 } },
                {Categories.SUPPORT, new List<int>() { 1, 2, 2, 2, 3, 1, 1, 1, 1 } },
                {Categories.TANK, new List<int>() { 1, 2, 2, 2, 3, 1, 1, 1, 1 } },
                {Categories.RECON, new List<int>() { 1, 2, 2, 2, 3, 1, 1, 1, 1 } },
                {Categories.VEHICLE, new List<int>() { 1, 2, 2, 2, 3, 1, 1, 1, 1 } },
                {Categories.HELICOPTER, new List<int>() { 1, 2, 2, 3, 3, 1, 1, 1, 1 } },
                {Categories.PLANE, new List<int>() { 1, 2, 3, 4, 5, 1, 1, 1, 1 } },
                {Categories.NAVAL, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
            };

            Dictionary<Categories, List<int>> costMatrix = database.GetUnfilteredCostMatrix();

            CollectionAssert.AreEqual(expectedCostMatrix[Categories.LOGISTICS], costMatrix[Categories.LOGISTICS]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.INFANTRY], costMatrix[Categories.INFANTRY]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.SUPPORT], costMatrix[Categories.SUPPORT]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.TANK], costMatrix[Categories.TANK]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.RECON], costMatrix[Categories.RECON]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.VEHICLE], costMatrix[Categories.VEHICLE]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.HELICOPTER], costMatrix[Categories.HELICOPTER]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.PLANE], costMatrix[Categories.PLANE]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.NAVAL], costMatrix[Categories.NAVAL]);

        }

        [TestMethod]
        public void LoadsSlotsPerCategory() {
            Dictionary<Categories, int> expectedSlotsPerCategory = new Dictionary<Categories, int> {
                {Categories.LOGISTICS, 5 },
                {Categories.INFANTRY, 5 },
                {Categories.SUPPORT, 5 },
                {Categories.TANK, 5 },
                {Categories.RECON, 5 },
                {Categories.VEHICLE, 5 },
                {Categories.HELICOPTER, 5 },
                {Categories.PLANE, 5 },
                {Categories.NAVAL, 5 }
            };

            Dictionary<Categories, int> slotsPerCategory = database.GetSlotsPerCategory();

            Assert.AreEqual(expectedSlotsPerCategory[Categories.LOGISTICS], slotsPerCategory[Categories.LOGISTICS]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.INFANTRY], slotsPerCategory[Categories.INFANTRY]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.SUPPORT], slotsPerCategory[Categories.SUPPORT]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.TANK], slotsPerCategory[Categories.TANK]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.RECON], slotsPerCategory[Categories.RECON]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.VEHICLE], slotsPerCategory[Categories.VEHICLE]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.HELICOPTER], slotsPerCategory[Categories.HELICOPTER]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.PLANE], slotsPerCategory[Categories.PLANE]);
            Assert.AreEqual(expectedSlotsPerCategory[Categories.NAVAL], slotsPerCategory[Categories.NAVAL]);
        }

        [TestMethod]
        public void LoadsFilteredCostMatrix() {
            Dictionary<Categories, List<int>> expectedCostMatrix = new Dictionary<Categories, List<int>> {
                {Categories.LOGISTICS, new List<int>() { 1, 2, 2, 2, 3 } },
                {Categories.INFANTRY, new List<int>() { 1, 2, 2, 2, 3 } },
                {Categories.SUPPORT, new List<int>() { 1, 2, 2, 2, 3 } },
                {Categories.TANK, new List<int>() { 1, 2, 2, 2, 3 } },
                {Categories.RECON, new List<int>() { 1, 2, 2, 2, 3 } },
                {Categories.VEHICLE, new List<int>() { 1, 2, 2, 2, 3 } },
                {Categories.HELICOPTER, new List<int>() { 1, 2, 2, 3, 3 } },
                {Categories.PLANE, new List<int>() { 1, 2, 3, 4, 5 } },
                {Categories.NAVAL, new List<int>() { 0, 0, 0, 0, 0 } }
            };

            Dictionary<Categories, List<int>> costMatrix = database.GetCostMatrix();

            CollectionAssert.AreEqual(expectedCostMatrix[Categories.LOGISTICS], costMatrix[Categories.LOGISTICS]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.INFANTRY], costMatrix[Categories.INFANTRY]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.SUPPORT], costMatrix[Categories.SUPPORT]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.TANK], costMatrix[Categories.TANK]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.RECON], costMatrix[Categories.RECON]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.VEHICLE], costMatrix[Categories.VEHICLE]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.HELICOPTER], costMatrix[Categories.HELICOPTER]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.PLANE], costMatrix[Categories.PLANE]);
            CollectionAssert.AreEqual(expectedCostMatrix[Categories.NAVAL], costMatrix[Categories.NAVAL]);

        }

        [TestMethod]
        public void LoadsUnitsCountry() {
            HashSet<string> allCountries = new HashSet<string> { "US", "UK", "FR", "RFA", "CAN", "DAN", "SWE", "NOR", "URSS", "RDA", "POL", "TCH", "NK", "ROK", "CHI", "JAP", "ANZ", "HOL", "ISR", "FIN", "YUG" };
            List<WargameUnit> units = new List<WargameUnit>(database.GetOtanUnits());
            units.AddRange(database.GetPactUnits());

            foreach (var unit in units) {
                Assert.IsTrue(allCountries.Contains(unit.Country), unit.Country + ": Not a valid country. UnitId: " + unit.UnitId);
            }
        }

        [TestMethod]
        public void LoadsSpecializations() {
            WargameUnit anzMilan = units.Find(unit => unit.UnitId == 25);
            Assert.IsTrue(anzMilan.Specializations.Contains(Specializations.AIR));
            Assert.IsTrue(anzMilan.Specializations.Contains(Specializations.ARMORED));
            Assert.IsTrue(anzMilan.Specializations.Contains(Specializations.MARINE));
            Assert.IsTrue(anzMilan.Specializations.Contains(Specializations.MECHANIZED));
            Assert.IsTrue(anzMilan.Specializations.Contains(Specializations.MOTORIZED));
            Assert.IsTrue(anzMilan.Specializations.Contains(Specializations.SUPPORT));
        }

        [TestMethod]
        public void LoadsPrototypeUnits() {
            List<WargameUnit> protoUnits = units.Where(unit => unit.IsPrototype).ToList();
            List<WargameUnit> nonProtoUnits = units.Where(unit => !unit.IsPrototype).ToList();

            Assert.IsTrue(protoUnits.Count > 0);
            Assert.IsTrue(nonProtoUnits.Count > 0);
        }

        [TestMethod]
        public void LoadsCommandUnits() {
            List<WargameUnit> commandUnits = units.Where(unit => unit.IsCommander).ToList();
            List<WargameUnit> regularUnits = units.Where(unit => !unit.IsCommander).ToList();

            Assert.IsTrue(commandUnits.Count > 0);
            Assert.IsTrue(commandUnits.Count > 0);
        }

        [TestMethod]
        public void LoadsUpgradedTransports() {
            WargameUnit riflemen90 = units.Find(unit => unit.Name == "Unit_US_85_Riflemen");
            Assert.AreEqual(8, riflemen90.Transports.Count); 
        }

        [TestMethod]
        public void LoadsUnitSubCategory() {
            WargameUnit riflemen90 = units.Find(unit => unit.Name == "Unit_US_85_Riflemen");
            Assert.AreEqual(Subcategories.INF_LINE, riflemen90.Subcategory);
        }
    }
}
