using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeckGenerator;
using System.Collections;

namespace RandomDeckGeneratorTests {
    [TestClass]
    public class DeckTests {
        Deck deck;
        [TestInitialize]
        public void Setup() {
            deck = new Deck(Deck.DeckType.Blufor);
        }

        [TestMethod]
        public void DeckReturnsEmptyDeckCodeWhenEmpty() {
            string deckCode = deck.GetDeckCode();
            Assert.AreEqual("Hs8AAA==", deckCode);
        }

        [TestMethod]
        public void ConvertsIntToBitArray() {
            BitArray bitArray = deck.ConvertToBigEndianBitArray(1, 6);
            CollectionAssert.AreEqual(new BitArray(new bool[] { false, false, false, false, false, true }), bitArray);
        }

        [TestMethod]
        public void ReturnsCodeForUnitWithTransport() {
            deck.AddUnit(1, 1, 539);
            deck.AddUnit(1, 57, 551);
            deck.AddUnit(1, 164, 501);
            string deckCode = deck.GetDeckCode();
            Assert.AreEqual("Hs8AyAFDZByicikPqA==", deckCode);
        }

        [TestMethod]
        public void ReturnsCodeForUnitsWithoutTransport() {
            deck.AddUnit(1, 46);
            deck.AddUnit(1, 62);
            deck.AddUnit(0, 834);
            string deckCode = deck.GetDeckCode();
            Assert.AreEqual("Hs8ACC4g+DQg", deckCode);
        }

        [TestMethod]
        public void ReturnsCodeForNavalUnitsInBarges() {
            Deck usDeck = new Deck(Deck.DeckType.US);
            usDeck.AddUnit(2, 80, 573); //Tank in a barge
            usDeck.AddUnit(1, 42, 332, 573); //Inf in truck in barge
            usDeck.AddUnit(1, 42, 332, 573);

            Assert.AreEqual("AM8QSCopiPSCopiPUFBHoA==", usDeck.GetDeckCode());
        }

        [TestMethod]
        public void ReturnsCodeForJapanDeck() {
            Deck jpnDeck = new Deck(Deck.DeckType.Japan);
            jpnDeck.AddUnit(1, 164, 501);

            Assert.AreEqual("BM8ASKQ+oA==", jpnDeck.GetDeckCode());
        }
    }
}
