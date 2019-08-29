using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeckGenerator
{
    public class UnitPoolInfo {
        public List<WargameUnit> AvailableUnits { get; }
        public double Weight { get; }
        public UnitPoolInfo(List<WargameUnit> units, double weight) {
            AvailableUnits = units;
            Weight = weight;
        }
    }

    public struct CategoryRestriction {
        public double Weigth { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        public int MinTransportYear { get; set; }
        public int MaxTransportYear { get; set; }

        public CategoryRestriction(double weigth, int minYear, int maxYear, int minTransportYear, int maxTransportYear) {
            Weigth = weigth;
            MinYear = minYear;
            MaxYear = maxYear;
            MinTransportYear = minTransportYear;
            MaxTransportYear = maxTransportYear;
        }
    }

    public class RandomDeckGenerator {
        private INdfDatabase database;

        private static HashSet<Deck.DeckType> bluforDecks = new HashSet<Deck.DeckType> {
            Deck.DeckType.Blufor, Deck.DeckType.Anzac, Deck.DeckType.Canada, Deck.DeckType.Denamark, Deck.DeckType.France, Deck.DeckType.Holand,
            Deck.DeckType.Israel, Deck.DeckType.Japan, Deck.DeckType.Norway, Deck.DeckType.SKorea, Deck.DeckType.Sweden, Deck.DeckType.UK, Deck.DeckType.US,
            Deck.DeckType.WGermany
        };

        private static Dictionary<Deck.DeckType, Countries> typeToCountry = new Dictionary<Deck.DeckType, Countries> {
            { Deck.DeckType.Anzac, Countries.ANZ },
            { Deck.DeckType.WGermany, Countries.RFA },
            { Deck.DeckType.Canada, Countries.CAN },
            { Deck.DeckType.Denamark, Countries.DAN },
            { Deck.DeckType.France, Countries.FR },
            { Deck.DeckType.Japan, Countries.JAP },
            { Deck.DeckType.Holand, Countries.HOL },
            { Deck.DeckType.Norway, Countries.NOR },
            { Deck.DeckType.SKorea, Countries.ROK },
            { Deck.DeckType.Sweden, Countries.SWE },
            { Deck.DeckType.UK, Countries.UK },
            { Deck.DeckType.US, Countries.US },
            { Deck.DeckType.Israel , Countries.ISR },
            { Deck.DeckType.Czech, Countries.TCH },
            { Deck.DeckType.EGermany, Countries.RDA },
            { Deck.DeckType.NKorea, Countries.NK },
            { Deck.DeckType.Poland, Countries.POL },
            { Deck.DeckType.China, Countries.CHI },
            { Deck.DeckType.Ussr, Countries.URSS },
            { Deck.DeckType.Finland, Countries.FIN },
            { Deck.DeckType.Yugoslavia, Countries.YUG }
        };

        public int UsedActivationPoints { get; private set; } = 0;

        Dictionary<Categories, CategoryRestriction> deckRestrictions = new Dictionary<Categories, CategoryRestriction>() {
            { Categories.HELICOPTER, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.INFANTRY, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.LOGISTICS, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.NAVAL, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.PLANE, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.RECON, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.SUPPORT, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.TANK, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) },
            { Categories.VEHICLE, new CategoryRestriction(1.0, 1900, 2100, 1900, 2100) }
        };

        Dictionary<Subcategories, double> subCategoryWeights = new Dictionary<Subcategories, double>() {
            { Subcategories.NONE, 100},
            { Subcategories.LOG_CMD_INF, 100},
            { Subcategories.LOG_CMD_VEH, 100},
            { Subcategories.LOG_SUP_TRUCK, 100},
            { Subcategories.LOG_CMD_ARMORED, 100},
            { Subcategories.LOG_SUP_HELO, 100},
            { Subcategories.LOG_CMD_HELO, 100},
            { Subcategories.LOG_FOB, 100},
            { Subcategories.INF_LINE, 100},
            { Subcategories.INF_ATGM, 100},
            { Subcategories.INF_FIRE_SUPPORT, 100},
            { Subcategories.INF_MANPADS, 100},
            { Subcategories.INF_ENGINEERS, 100},
            { Subcategories.INF_SF, 100},
            { Subcategories.INF_RESERVISTS, 100},
            { Subcategories.INF_LIGHT, 100},
            { Subcategories.SUP_MORTAR, 100},
            { Subcategories.SUP_HOWITZER, 100},
            { Subcategories.SUP_AA_SPAAG, 100},
            { Subcategories.SUP_AA_MISSILE, 100},
            { Subcategories.SUP_MRLS, 100},
            { Subcategories.TNK_CAVALRY, 100},
            { Subcategories.TNK_HEAVY, 100},
            { Subcategories.TNK_MEDIUM, 100},
            { Subcategories.TNK_LIGHT, 100},
            { Subcategories.REC_VEHICLE, 100},
            { Subcategories.REC_SF, 100},
            { Subcategories.REC_INFANTRY, 100},
            { Subcategories.REC_HELO, 100},
            { Subcategories.VEH_IFV_TRANSPORT, 100},
            { Subcategories.VEH_ATGM, 100},
            { Subcategories.VEH_FIRE_SUPPORT, 100},
            { Subcategories.VEH_TRANSPORT, 100},
            { Subcategories.VEH_FLAMER, 100},
            { Subcategories.HEL_GUNSHIP, 100},
            { Subcategories.HEL_ATGM, 100},
            { Subcategories.HEL_TRANSPORT, 100},
            { Subcategories.HEL_AA, 100},
            { Subcategories.PLN_ANTI_TANK, 100},
            { Subcategories.PLN_MULTIROLE, 100},
            { Subcategories.PLN_BOMBER, 100},
            { Subcategories.PLN_ASF, 100},
            { Subcategories.PLN_SEAD, 100},
            { Subcategories.PLN_INTERCEPTOR, 100},
            { Subcategories.NAV_SHORE, 100},
            { Subcategories.NAV_SUPPORT, 100},
            { Subcategories.NAV_FRIGATE, 100},
            { Subcategories.NAV_SUPPLY, 100},
            { Subcategories.NAV_AS_HELO, 100},
            { Subcategories.NAV_AS_PLANE, 100},
            { Subcategories.NAV_AS_TRUCK, 100}
        };
        
        Dictionary<Categories, UnitPoolInfo> unitPool;

        private Random rnGesus;

        public Dictionary<uint, int> AddedUnits { get; private set; }
        private List<WargameUnit> unitsAndTransports;
        Deck deck;
        Dictionary<Categories, List<int>> costMatrix;
        Regex fobRegex = new Regex(".*FOB.*", RegexOptions.IgnoreCase);

        public RandomDeckGenerator(INdfDatabase database) {
            this.database = database;
            rnGesus = new Random();
            unitPool = new Dictionary<Categories, UnitPoolInfo>() {
                { Categories.HELICOPTER, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.INFANTRY, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.LOGISTICS, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.NAVAL, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.PLANE, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.RECON, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.SUPPORT, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.TANK, new UnitPoolInfo(new List<WargameUnit>(), 1.0) },
                { Categories.VEHICLE, new UnitPoolInfo(new List<WargameUnit>(), 1.0) }
            };
        }

        public void SetWeigth(Categories category, double weigth) {
            var restriction = deckRestrictions[category];
            restriction.Weigth = weigth;
            deckRestrictions[category] = restriction;
        }

        public void SetWeigth(Subcategories category, double weigth) {
            subCategoryWeights[category] = weigth;
        }

        public void SetMinimumYear(Categories category, int year) {
            var restriction = deckRestrictions[category];
            restriction.MinYear = year;
            deckRestrictions[category] = restriction;
        }

        public void SetMaximumYear(Categories category, int year) {
            var restriction = deckRestrictions[category];
            restriction.MaxYear = year;
            deckRestrictions[category] = restriction;
        }

        public void SetMinimumTransportYear(Categories category, int year) {
            var restriction = deckRestrictions[category];
            restriction.MinTransportYear = year;
            deckRestrictions[category] = restriction;
        }

        public void SetMaximumTransportYear(Categories category, int year) {
            var restriction = deckRestrictions[category];
            restriction.MaxTransportYear = year;
            deckRestrictions[category] = restriction;
        }

        public Deck GenerateDeck(Deck.DeckType type) {
            deck = new Deck(type);
            List<WargameUnit> units = LoadUnits(type);

            units = FilterInvalidUnits(units);
            units = FilterByDeckType(units, type);
            foreach (var unit in units) {
                RemoveNavalTransports(unit);
            }
            CategorizeUnitsAndApplyRestrictions(units);

            InitAddedUnits(units);
            InitCardCountChecker(type);
            
            foreach (var unitPoolCategory in unitPool) {
                InitUnitWeights(unitPoolCategory.Value.AvailableUnits);
            }

            int activationPoints = GetActivationPoints(type);

            costMatrix = database.GetCostMatrix();
            
            UsedActivationPoints = 0;

            AddCommander();
            AddCommander();
            unitPool[Categories.LOGISTICS].AvailableUnits.RemoveAll(unit => unit.IsCommander);

            AddFob();

            while (CanDrawAnotherCard()) {
                Categories category = ChooseCategory();
                if (unitPool[category].AvailableUnits.Count == 0 || costMatrix[category].Count == 0 || costMatrix[category].First() + UsedActivationPoints > activationPoints) {
                    unitPool.Remove(category);
                } else {
                    WargameUnit selectedUnit = DrawCard(unitPool[category].AvailableUnits);
                    AddUnitToDeck(selectedUnit, deck);
                    UpdateDeckCost(category, costMatrix);
                }
            }

            List<WargameUnit> navalUnits = LoadUnits(type);
            navalUnits = FilterByDeckType(navalUnits, type);
            navalUnits = navalUnits.Where(unit => unit.Cards > 0
                                        && unit.AvailableVeterancy.Count > 0
                                        && unit.Specializations.Count > 0
                                        && unit.Year > 1900
                                        && (unit.Category == Categories.NAVAL
                                            || HasNavalTransport(unit))).ToList();

            InitUnitWeights(navalUnits);

            foreach (var unit in navalUnits) {
                unit.Transports.RemoveAll(t => t.Category != Categories.NAVAL && !HasNavalTransport(t));
            }

            while (navalUnits.Count > 0 && costMatrix[Categories.NAVAL].Count > 0) {
                WargameUnit navalUnit = DrawCard(navalUnits);
                AddUnitToDeck(navalUnit, deck);
                UpdateDeckCost(Categories.NAVAL, costMatrix);
            }

            return deck;
        }

        private void InitUnitWeights(List<WargameUnit> units) {
            Dictionary<Subcategories, int> unitsPerSubcategory = new Dictionary<Subcategories, int>();
            var allSubcategories = Enum.GetValues(typeof(Subcategories)).Cast<Subcategories>();
            foreach (var subcategory in allSubcategories) {
                unitsPerSubcategory[subcategory] = units.Count(unit => unit.Subcategory == subcategory);
                if (unitsPerSubcategory[subcategory] == 0) {
                    unitsPerSubcategory[subcategory] = 1;
                }
            }
            foreach (var unit in units) {
                unit.Weight = subCategoryWeights[unit.Subcategory] / unitsPerSubcategory[unit.Subcategory];
                foreach (var transport in unit.Transports) {
                    transport.Weight = subCategoryWeights[transport.Subcategory] / unitsPerSubcategory[transport.Subcategory];
                    foreach (var superTransport in transport.Transports) {
                        superTransport.Weight = subCategoryWeights[superTransport.Subcategory] / unitsPerSubcategory[superTransport.Subcategory];
                    }
                }
            }
        }

        private bool HasNavalTransport(WargameUnit unit) {
            foreach (var transport in unit.Transports) {
                if (transport.Category == Categories.NAVAL) {
                    return true;
                }
                if (HasNavalTransport(transport)) {
                    return true;
                }
            }
            return false;
        }

        private void RemoveNavalTransports(WargameUnit unit) {
            unit.Transports.RemoveAll(t => t.Category == Categories.NAVAL);
            foreach (var transport in unit.Transports) {
                RemoveNavalTransports(transport);
            }
        }

        private void InitAddedUnits(List<WargameUnit> units) {
            AddedUnits = new Dictionary<uint, int>();
            foreach (var unit in units) {
                AddedUnits[unit.UnitId] = 0;
                foreach (var transport in unit.Transports) {
                    AddedUnits[transport.UnitId] = 0;
                }
            }
        }

        private void InitCardCountChecker(Deck.DeckType type) {
            unitsAndTransports = LoadUnits(type);
            unitsAndTransports = FilterInvalidUnits(unitsAndTransports);
            unitsAndTransports = FilterByDeckType(unitsAndTransports, type);
            HashSet<WargameUnit> transports = new HashSet<WargameUnit>();
            foreach (var unit in unitsAndTransports) {
                foreach (var transport in unit.Transports) {
                    transports.Add(transport);
                }
            }
            unitsAndTransports.AddRange(transports);
        }

        private void AddUnitToDeck(WargameUnit selectedUnit, Deck deck) {
            byte veterancy = (byte)selectedUnit.AvailableVeterancy[rnGesus.Next(selectedUnit.AvailableVeterancy.Count)];
            if (selectedUnit.Transports.Count > 0) {
                WargameUnit transport = DrawCard(selectedUnit.Transports);
                if (transport.Transports.Count > 0) {
                    WargameUnit superTransport = DrawCard(transport.Transports);
                    deck.AddUnit(veterancy, (int)selectedUnit.UnitId, (int)transport.UnitId, (int)superTransport.UnitId);
                } else {
                    deck.AddUnit(veterancy, (int)selectedUnit.UnitId, (int)transport.UnitId);
                }
                
                //AddedUnits[transport.UnitId] = AddedUnits[transport.UnitId] + 1;
            }
            else {
                deck.AddUnit(veterancy, (int)selectedUnit.UnitId);
            }
            
            //AddedUnits[selectedUnit.UnitId] = AddedUnits[selectedUnit.UnitId] + 1;
        }

        private void AddCommander() {
            List<WargameUnit> commanders = unitPool[Categories.LOGISTICS].AvailableUnits.Where(unit => unit.IsCommander).ToList();
            if (commanders.Count == 0) {
                throw new Exception("No command unit available.");
            }
            WargameUnit commander = DrawCard(commanders);
            if (commander.Cards <= 0) {
                unitPool[Categories.LOGISTICS].AvailableUnits.RemoveAll(unit => unit.UnitId == commander.UnitId);
            }
            AddUnitToDeck(commander, deck);
            UpdateDeckCost(Categories.LOGISTICS, costMatrix);
        }

        private void AddFob() {
            List<WargameUnit> fobs = unitPool[Categories.LOGISTICS].AvailableUnits.Where(unit => fobRegex.IsMatch(unit.Name)).ToList();
            if (fobs.Count <= 0) return;
            WargameUnit fob = DrawCard(fobs);
            if (fob.Cards <= 0) {
                unitPool[Categories.LOGISTICS].AvailableUnits.RemoveAll(unit => unit.UnitId == fob.UnitId);
            }
            AddUnitToDeck(fob, deck);
            UpdateDeckCost(Categories.LOGISTICS, costMatrix);
        }

        private void UpdateDeckCost(Categories category, Dictionary<Categories, List<int>> costMatrix) {
            UsedActivationPoints += costMatrix[category].First();
            costMatrix[category].RemoveAt(0);
        }

        public List<WargameUnit> LoadUnits(Deck.DeckType type) {
            List<WargameUnit> units;
            if (bluforDecks.Contains(type)) {
                units = database.GetOtanUnits();
            }
            else {
                units = database.GetPactUnits();
            }
            return FilterOutTransports(units);
        }

        public static List<WargameUnit> FilterOutTransports(List<WargameUnit> units) {
            HashSet<uint> transports = new HashSet<uint>();
            foreach (var unit in units) {
                foreach (var transport in unit.Transports)
                    transports.Add(transport.UnitId);
            }
            return units.Where(unit => !transports.Contains(unit.UnitId)).ToList();
        }

        public List<WargameUnit> FilterInvalidUnits(List<WargameUnit> units) {
            return units.Where(unit => unit.Cards > 0 
                                        && unit.AvailableVeterancy.Count > 0 
                                        && unit.Category != Categories.NAVAL
                                        && unit.Specializations.Count > 0
                                        && (unit.Year > 1900 || fobRegex.IsMatch(unit.Name))).ToList();
        }
        

        public List<WargameUnit> FilterByDeckType(List<WargameUnit> units, Deck.DeckType type) {
            if (IsSingleCountryDeckType(type)) {
                return units.Where(unit => unit.Category == Categories.NAVAL || unit.Country == typeToCountry[type].ToString()).ToList();
            } else {
                return units.Where(unit => !unit.IsPrototype).ToList();
            }
        }

        private int GetActivationPoints(Deck.DeckType type) {
            if (IsSingleCountryDeckType(type)) {
                return database.GetActivationPoints(typeToCountry[type]);
            }
            else {
                return database.GetDefaultActivationPoints();
            }
        }

        private bool IsSingleCountryDeckType(Deck.DeckType type) {
            return type != Deck.DeckType.Blufor && type != Deck.DeckType.Redfor;
        }

        private void CategorizeUnitsAndApplyRestrictions(List<WargameUnit> units) {
            unitPool = new Dictionary<Categories, UnitPoolInfo>();
            foreach (var category in deckRestrictions.Keys) {
                List<WargameUnit> unitsInCategory = units.Where(unit => IsUnitValid(unit, category)).ToList();
                unitsInCategory = FilterTransports(unitsInCategory, category);
                if (unitsInCategory.Count > 0) {
                    unitPool[category] = new UnitPoolInfo(unitsInCategory, deckRestrictions[category].Weigth);
                }
            }
        }

        private bool IsUnitValid(WargameUnit unit, Categories category) {
            return unit.Category == category
                    && (unit.Year >= deckRestrictions[category].MinYear
                        && unit.Year <= deckRestrictions[category].MaxYear
                        || fobRegex.IsMatch(unit.Name));
        }

        private List<WargameUnit> FilterTransports(List<WargameUnit> units, Categories category) {
            List<WargameUnit> filteredUnits = new List<WargameUnit>();
            foreach (var unit in units) {
                if (unit.Transports.Count > 0) {
                    List<WargameUnit> filteredTransports = unit.Transports.Where(t => IsTransportValid(t, category)).ToList();
                    unit.Transports.RemoveAll(t => true);
                    unit.Transports.AddRange(filteredTransports);
                    if (unit.Transports.Count > 0) {
                        filteredUnits.Add(unit);
                    }
                } else {
                    filteredUnits.Add(unit);
                }
            }
            return filteredUnits;
        }

        private bool IsTransportValid(WargameUnit transport, Categories category) {
            return transport.Year >= deckRestrictions[category].MinTransportYear
                && transport.Year <= deckRestrictions[category].MaxTransportYear;
        }

        private bool CanDrawAnotherCard() {
            return unitPool.Count > 0;
        }

        private List<WargameUnit> ShuffleUnits(List<WargameUnit> units) {
            List<WargameUnit> shuffledUnits = new List<WargameUnit>(units);
            for (int i = 0; i < 20 * shuffledUnits.Count; i++) {
                int indexA = rnGesus.Next(shuffledUnits.Count);
                int indexB = rnGesus.Next(shuffledUnits.Count);
                WargameUnit unit = shuffledUnits[indexA];
                shuffledUnits[indexA] = shuffledUnits[indexB];
                shuffledUnits[indexB] = unit;
            }
            return shuffledUnits;
        }

        public Categories ChooseCategory() {
            double totalWeight = unitPool.Values.Sum(poolInfo => poolInfo.Weight);
            double chosen = rnGesus.NextDouble() * totalWeight;
            var enumerator = unitPool.GetEnumerator();
            while (chosen > 0 && enumerator.MoveNext()) {
                chosen -= enumerator.Current.Value.Weight;
            }
            return enumerator.Current.Key;
        }

        public WargameUnit DrawCard(List<WargameUnit> availableUnits) {
            double totalWeight = availableUnits.Sum(unit => unit.Weight);
            double chosen = rnGesus.NextDouble() * totalWeight;
            var enumerator = availableUnits.GetEnumerator();
            while (chosen > 0 && enumerator.MoveNext()) {
                chosen -= enumerator.Current.Weight;
            }
            WargameUnit selectedUnit = enumerator.Current;
            //CheckCardCount(selectedUnit);
            selectedUnit.Cards--;
            CleanUpUnitPool();
            return selectedUnit;
        }

        private void CleanUpUnitPool() {
            foreach (var pool in unitPool.Values) {
                CleanUpUnits(pool.AvailableUnits);
            }
        }

        private void CleanUpUnits(List<WargameUnit> units) {
            foreach (var unit in units) {
                if (unit.Transports.RemoveAll(transport => transport.Cards == 0) > 0 && unit.Transports.Count == 0) {
                    unit.Cards = 0;
                }
            }
            units.RemoveAll(unit => unit.Cards == 0);
        }

        private void CheckCardCount(WargameUnit selectedUnit) {
            WargameUnit referenceUnit = unitsAndTransports.Find(unit => unit.UnitId == selectedUnit.UnitId);
            if (AddedUnits[selectedUnit.UnitId] + 1 > referenceUnit.Cards) {
                Console.WriteLine("Exceeded.");
            }
        }
    }
}
