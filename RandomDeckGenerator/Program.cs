using moddingSuite.BL;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Edata;
using moddingSuite.Model.Ndfbin;
using moddingSuite.ViewModel.Edata;
using moddingSuite.ViewModel.Ndf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using System.Collections.ObjectModel;
using System.Reflection;

namespace DeckGenerator {
    public class Program {

        static RandomDeckGenerator generator;

        static void Main(string[] args) {
            string fileName = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\NDF_Win.dat";
            NdfDatabase ndfDatabase = new NdfDatabase(fileName);
            generator = new RandomDeckGenerator(ndfDatabase);
            /*
            var allUnits = ndfDatabase.GetOtanUnits();
            allUnits.AddRange(ndfDatabase.GetPactUnits());

            Dictionary<Subcategories, List<WargameUnit>> subcats = new Dictionary<Subcategories, List<WargameUnit>>();

            foreach (var unit in allUnits) {
                if (!subcats.ContainsKey(unit.Subcategory)) {
                    subcats[unit.Subcategory] = new List<WargameUnit>();
                }
                subcats[unit.Subcategory].Add(unit);
            }

            PrintSubcategories(subcats, Categories.LOGISTICS);
            PrintSubcategories(subcats, Categories.INFANTRY);
            PrintSubcategories(subcats, Categories.SUPPORT);
            PrintSubcategories(subcats, Categories.TANK);
            PrintSubcategories(subcats, Categories.RECON);
            PrintSubcategories(subcats, Categories.VEHICLE);
            PrintSubcategories(subcats, Categories.HELICOPTER);
            PrintSubcategories(subcats, Categories.PLANE);
            PrintSubcategories(subcats, Categories.NAVAL);
            */
            
            generator.SetMinimumYear(Categories.INFANTRY, 1975);
            generator.SetMinimumYear(Categories.SUPPORT, 1975);
            generator.SetMinimumYear(Categories.TANK, 1985);
            generator.SetMinimumYear(Categories.VEHICLE, 1975);
            generator.SetMinimumYear(Categories.HELICOPTER, 1975);
            generator.SetMinimumYear(Categories.PLANE, 1975);

            generator.SetWeigth(Categories.VEHICLE, 0.5);
            generator.SetWeigth(Categories.LOGISTICS, 0.7);

            generator.SetWeigth(Subcategories.INF_LINE, 250);
            generator.SetWeigth(Subcategories.INF_ENGINEERS, 0);
            generator.SetWeigth(Subcategories.INF_LIGHT, 150);

            for (int i = 0; i < 10; i++) {
                Deck deck = generator.GenerateDeck(Deck.DeckType.US);
                Console.WriteLine("@" + deck.GetDeckCode());
            }
            
            Console.ReadKey();
        }

        static private void PrintSubcategories(Dictionary<Subcategories, List<WargameUnit>> subcats, Categories category) {
            Console.WriteLine(category.ToString() + ":");
            foreach (var pair in subcats) {
                var units = pair.Value.Where(unit => unit.Category == category).ToList();
                if (units.Count > 0) {
                    StringBuilder line = new StringBuilder();
                    line.Append(pair.Key).Append(": ");
                    foreach (var unit in pair.Value) {
                        line.Append(unit.Name).Append(" ");
                    }
                    Console.WriteLine(line);
                }
            }
        }

        static private void PrintUnits(List<WargameUnit> units, List<uint> unitsInDeck) {
            foreach (uint unitId in unitsInDeck) {
                WargameUnit wUnit = units.Find(unit => unit.UnitId == unitId);
                if (wUnit != null) {
                    Console.WriteLine(wUnit.Name + ", " + wUnit.Category.ToString());
                }
                else {
                    Console.WriteLine(unitId + ": Not found in units...");
                }
            }
        }

        static private void TestDeck() {
            Deck deck = new Deck(Deck.DeckType.Blufor);
            //deck.AddUnit(2, 200);
            deck.AddUnit(1, 1142); //FIN unit on a blu deck... modded database.
            //deck.AddUnit(2, 357);
            //deck.AddUnit(1, 975);
            //deck.AddUnit(1, 351);
            //deck.AddUnit(1, 454, 843);
            //deck.AddUnit(1, 1204);
            //deck.AddUnit(4, 68, 857);
            //deck.AddUnit(2, 1083, 1088);
            //deck.AddUnit(1, 325);
            //deck.AddUnit(2, 734);
            //deck.AddUnit(2, 67);
            //deck.AddUnit(1, 257);
            //deck.AddUnit(1, 1196, 1130);
            //deck.AddUnit(2, 548);
            //deck.AddUnit(2, 166);
            //deck.AddUnit(1, 916, 673);
            //deck.AddUnit(1, 650);
            //deck.AddUnit(1, 20);
            //deck.AddUnit(1, 559);
            Console.WriteLine("@" + deck.GetDeckCode());
        }
    }
}
