using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeckGenerator;
using System.Collections.Generic;

namespace RandomDeckGeneratorTests
{
    public class FakeNdfDatabase : INdfDatabase {

        public int GetActivationPoints(Countries country) {
            return 10;
        }

        public Dictionary<Categories, List<int>> GetCostMatrix() {
            return new Dictionary<Categories, List<int>> {
                {Categories.LOGISTICS, new List<int>()  { 1, 2, 2, 2, 3 } },
                {Categories.INFANTRY, new List<int>()   { 1, 2, 2, 2, 3 } },
                {Categories.SUPPORT, new List<int>()    { 1, 2, 2, 2, 3 } },
                {Categories.TANK, new List<int>()       { 1, 2, 2, 2, 3 } },
                {Categories.RECON, new List<int>()      { 1, 2, 2, 2, 3 } },
                {Categories.VEHICLE, new List<int>()    { 1, 2, 2, 2, 3 } },
                {Categories.HELICOPTER, new List<int>() { 1, 2, 2, 3, 3 } },
                {Categories.PLANE, new List<int>()      { 1, 2, 3, 4, 5 } },
                {Categories.NAVAL, new List<int>()      { 0, 0, 0, 0, 0 } }
            };
        }

        public int GetDefaultActivationPoints() {
            return 10;
        }

        public List<WargameUnit> GetOtanUnits() {
            var units = new List<WargameUnit>() {
                (new WargameUnit(1, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.UK.ToString(), false, true, "")),
                new WargameUnit(2, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.CAN.ToString(), false, false, ""),
                new WargameUnit(3, 1, Categories.INFANTRY, Subcategories.NONE, 1990, Countries.US.ToString(), false, false, ""),
                new WargameUnit(4, 1, Categories.TANK, Subcategories.NONE, 1990, Countries.US.ToString(), false, false, ""),
                new WargameUnit(5, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.CAN.ToString(), false, true, ""),
                new WargameUnit(6, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.CAN.ToString(), false, false, ""),
                new WargameUnit(7, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.CAN.ToString(), false, false, ""),
                new WargameUnit(8, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.CAN.ToString(), false, false, ""),
                new WargameUnit(9, 1, Categories.LOGISTICS, Subcategories.NONE, 1990, Countries.CAN.ToString(), false, false, "")
            };
            foreach (var unit in units) {
                unit.AvailableVeterancy.Add(1);
                unit.AvailableVeterancy.Add(2);
                unit.Specializations.Add(Specializations.ARMORED);
            }
            return units;
        }

        public List<WargameUnit> GetPactUnits() {
            throw new NotImplementedException();
        }

        public Dictionary<Categories, int> GetSlotsPerCategory() {
            throw new NotImplementedException();
        }

        public Dictionary<Categories, List<int>> GetUnfilteredCostMatrix() {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class RandomDeckGeneratorTest
    {
        RandomDeckGenerator generator;
        INdfDatabase fakeDatabase = new FakeNdfDatabase();

        [TestInitialize]
        public void Setup() {
            generator = new RandomDeckGenerator(fakeDatabase);
        }

        [TestMethod]
        public void BuildsDecks() {
            Deck randomDeck = generator.GenerateDeck(Deck.DeckType.Blufor);

            Assert.IsTrue(randomDeck.GetNumberOfUnits() > 0);
        }

        [TestMethod]
        public void DeckIsLimitedByActivationPoints() {
            Deck randomDeck = generator.GenerateDeck(Deck.DeckType.Blufor);

            Assert.IsTrue(generator.UsedActivationPoints > 0);
            Assert.IsTrue(generator.UsedActivationPoints < 10, "generator.UsedActivationPoints = " + generator.UsedActivationPoints);
        }

        [TestMethod]
        public void SelectsCategoriesFromCategoriesPool() {
            HashSet<Categories> categories = new HashSet<Categories>() { Categories.HELICOPTER, Categories.INFANTRY, Categories.LOGISTICS, Categories.NAVAL, Categories.PLANE,
                Categories.RECON, Categories.SUPPORT, Categories.TANK, Categories.VEHICLE };

            Categories category = generator.ChooseCategory();

            Assert.IsTrue(categories.Contains(category), "Category " + category.ToString() + " is not a valid category");
        }

        //[TestMethod]
        public void NumberOfCardsDoesNotExceedLimits() {
            RandomDeckGenerator realGenerator = new RandomDeckGenerator(new NdfDatabase("NDF_Win.dat"));
            List<WargameUnit> units = realGenerator.LoadUnits(Deck.DeckType.Ussr);
            
            units = realGenerator.FilterInvalidUnits(units);
            units = realGenerator.FilterByDeckType(units, Deck.DeckType.Ussr);
            for (int i = 0; i < 10; i++) {
                realGenerator.GenerateDeck(Deck.DeckType.Ussr);
                var usedUnits = realGenerator.AddedUnits;
                foreach (var unit in units) {
                    Assert.IsTrue(unit.Cards >= usedUnits[unit.UnitId], "Unit " + unit.Name + ": Max is " + unit.Cards + ", but used " + usedUnits[unit.UnitId]);
                    foreach (var transport in unit.Transports) {
                        Assert.IsTrue(transport.Cards >= usedUnits[transport.UnitId], "Transport " + transport.Name + ": Max is " + transport.Cards + ", but used " + usedUnits[transport.UnitId]);
                    }
                }
            }
        }
    }
}
