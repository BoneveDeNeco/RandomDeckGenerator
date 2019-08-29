using moddingSuite.BL;
using moddingSuite.BL.Ndf;
using moddingSuite.Model.Edata;
using moddingSuite.Model.Ndfbin;
using moddingSuite.Model.Ndfbin.Types.AllTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeckGenerator {

    public enum Categories {
        LOGISTICS = 3,
        INFANTRY = 6,
        SUPPORT = 13,
        TANK = 9,
        RECON = 10,
        VEHICLE = 8,
        HELICOPTER = 11,
        PLANE = 7,
        NAVAL = 12,
        UNKNOWN = 0
    }

    public enum Subcategories {
        NONE,
        LOG_CMD_INF,
        LOG_CMD_VEH,
        LOG_SUP_TRUCK,
        LOG_CMD_ARMORED,
        LOG_SUP_HELO,
        LOG_CMD_HELO,
        LOG_FOB,
        INF_LINE,
        INF_ATGM,
        INF_FIRE_SUPPORT,
        INF_MANPADS,
        INF_ENGINEERS,
        INF_SF,
        INF_RESERVISTS,
        INF_LIGHT,
        SUP_MORTAR,
        SUP_HOWITZER,
        SUP_AA_SPAAG,
        SUP_AA_MISSILE,
        SUP_MRLS,
        TNK_CAVALRY,
        TNK_HEAVY,
        TNK_MEDIUM,
        TNK_LIGHT,
        REC_VEHICLE,
        REC_SF,
        REC_INFANTRY,
        REC_HELO,
        VEH_IFV_TRANSPORT,
        VEH_ATGM,
        VEH_FIRE_SUPPORT,
        VEH_TRANSPORT,
        VEH_FLAMER,
        HEL_GUNSHIP,
        HEL_ATGM,
        HEL_TRANSPORT,
        HEL_AA,
        PLN_ANTI_TANK,
        PLN_MULTIROLE,
        PLN_BOMBER,
        PLN_ASF,
        PLN_SEAD,
        PLN_INTERCEPTOR,
        NAV_SHORE,
        NAV_SUPPORT,
        NAV_FRIGATE,
        NAV_SUPPLY,
        NAV_AS_HELO,
        NAV_AS_PLANE,
        NAV_AS_TRUCK
    }

    public enum Specializations {
        MOTORIZED,
        ARMORED,
        SUPPORT,
        MARINE,
        MECHANIZED,
        AIR
    }

    public enum Countries {
        US,
        UK,
        FR,
        RFA,
        CAN,
        DAN,
        SWE,
        NOR,
        ROK,
        JAP,
        ANZ,
        HOL,
        ISR,
        URSS,
        RDA,
        POL,
        TCH,
        NK,
        CHI,
        FIN,
        YUG
    }

    public class WargameUnit {
        public uint UnitId { get; }
        public uint Cards { get; set; }
        public Categories Category { get; }
        public Subcategories Subcategory { get; }
        public uint Year { get; }
        public string Country { get; }
        public bool IsPrototype { get; }
        public bool IsCommander { get; }
        public string Name { get; }
        public List<WargameUnit> Transports { get; }
        public List<uint> AvailableVeterancy { get; }
        public HashSet<Specializations> Specializations { get; }
        public double Weight { get; set; }

        public WargameUnit(uint unitId, uint cards, Categories category, Subcategories subcategory, uint year, string country, bool isPrototype, bool isCommander, string name) {
            UnitId = unitId;
            Cards = cards;
            Category = category;
            Subcategory = subcategory;
            Year = year;
            Country = country;
            IsPrototype = isPrototype;
            IsCommander = isCommander;
            Name = name;
            Transports = new List<WargameUnit>();
            AvailableVeterancy = new List<uint>();
            Specializations = new HashSet<Specializations>();
        }
    }

    public class NdfDatabase : INdfDatabase {

        private NdfBinary everythingNdfbin;
        private NdfClass tShowRoomDeckSerializer;
        private NdfClass tUniteAuSolDescriptor;
        private NdfClass tUniteDescriptor;
        private NdfClass tTransportableModuleDescriptor;
        private NdfClass tShowRoomDeckRuleManager;
        private ObservableCollection<NdfPropertyValue> deckRulesManagerProperties;
        private Dictionary<uint, uint> instanceIdToUnitId;
        private Dictionary<uint, WargameUnit> unitIdToUnit;
        private Dictionary<uint, List<uint>> unitIdToTransports;
        private Dictionary<uint, uint> unitUpgradeTree;

        private static Dictionary<uint, Categories> categories = new Dictionary<uint, Categories> {
            { 3, Categories.LOGISTICS },
            { 6, Categories.INFANTRY },
            { 13, Categories.SUPPORT },
            { 9, Categories.TANK },
            { 10, Categories.RECON },
            { 8, Categories.VEHICLE },
            { 11, Categories.HELICOPTER },
            { 7, Categories.PLANE },
            { 12, Categories.NAVAL }
        };

        private static Dictionary<string, Specializations> specializations = new Dictionary<string, Specializations>() {
            { "5E767965E3000000", Specializations.MOTORIZED},
            { "5C76718B57360E00", Specializations.ARMORED},
            { "DAD77965E3000000", Specializations.SUPPORT},
            { "23B8605ED9380000", Specializations.MARINE},
            { "8BD43C9757360E00", Specializations.MECHANIZED},
            { "0BB7685ED9380000", Specializations.AIR}
        };

        private static Dictionary<string, Subcategories> subcategories = new Dictionary<string, Subcategories>() {
            { "CED540D854468B07", Subcategories.LOG_CMD_INF},
            { "CED594D1E2010000", Subcategories.LOG_CMD_VEH},
            { "1FE7459955468B07", Subcategories.LOG_SUP_TRUCK},
            { "CED55CDC52468B07", Subcategories.LOG_CMD_ARMORED},
            { "D623459955468B07", Subcategories.LOG_SUP_HELO},
            { "D623395753468B07", Subcategories.LOG_CMD_HELO},
            { "4C0695D1E2010000", Subcategories.LOG_FOB},
            { "D1C341D854468B07", Subcategories.INF_LINE},
            { "DE026153192D1E00", Subcategories.INF_ATGM},
            { "5E374165B4780000", Subcategories.INF_FIRE_SUPPORT},
            { "103645D853468B07", Subcategories.INF_ENGINEERS},
            { "CB026153192D1E00", Subcategories.INF_MANPADS},
            { "99733565B4780000", Subcategories.INF_SF},
            { "4F3359D355468B07", Subcategories.INF_RESERVISTS},
            { "10365965B4780000", Subcategories.INF_LIGHT},
            { "DCE271D955468B07", Subcategories.SUP_MORTAR},
            { "A3C72D65B4780000", Subcategories.SUP_HOWITZER},
            { "D1B2685D192D1E00", Subcategories.SUP_AA_SPAAG},
            { "D7D295D1E2010000", Subcategories.SUP_AA_MISSILE},
            { "1D675D65B4780000", Subcategories.SUP_MRLS},
            { "15E6814B53468B07", Subcategories.TNK_CAVALRY},
            { "42852D5E192D1E00", Subcategories.TNK_LIGHT},
            { "43852D5E192D1E00", Subcategories.TNK_MEDIUM},
            { "44852D5E192D1E00", Subcategories.TNK_HEAVY},
            { "D203360F57468B07", Subcategories.REC_VEHICLE},
            { "9D963D5A57468B07", Subcategories.REC_SF},
            { "CDC36153192D1E00", Subcategories.REC_INFANTRY},
            { "D623350F57468B07", Subcategories.REC_HELO},
            { "203495D1E2010000", Subcategories.VEH_IFV_TRANSPORT},
            { "DDE3545E192D1E00", Subcategories.VEH_ATGM},
            { "D2036A5F57468B07", Subcategories.VEH_FIRE_SUPPORT},
            { "8DB694D1E2010000", Subcategories.VEH_TRANSPORT},
            { "92772D1654468B07", Subcategories.VEH_FLAMER},
            { "8F847D51192D1E00", Subcategories.HEL_GUNSHIP},
            { "99F548DE52468B07", Subcategories.HEL_ATGM},
            { "8F842D9C57468B07", Subcategories.HEL_TRANSPORT},
            { "99F548CB52468B07", Subcategories.HEL_AA},
            { "57B494D1E2010000", Subcategories.PLN_ANTI_TANK},
            { "DCE459DF55468B07", Subcategories.PLN_MULTIROLE},
            { "CC95318F54468B07", Subcategories.PLN_BOMBER},
            { "DAD771D352468B07", Subcategories.PLN_ASF},
            { "CEF27450192D1E00", Subcategories.PLN_SEAD},
            { "0DF778D854468B07", Subcategories.PLN_INTERCEPTOR},
            { "DE9231DD53468B07", Subcategories.NAV_SHORE},
            { "DAD7795D53468B07", Subcategories.NAV_SUPPORT},
            { "1307395753468B07", Subcategories.NAV_FRIGATE},
            { "5166810B56468B07", Subcategories.NAV_SUPPLY},
            { "93D4598F54468B07", Subcategories.NAV_AS_HELO},
            { "93D471D352468B07", Subcategories.NAV_AS_PLANE},
            { "93D4490F58468B07", Subcategories.NAV_AS_TRUCK}
        };

        static public readonly HashSet<string> otanCountries = new HashSet<string>() { Countries.US.ToString(), Countries.UK.ToString(), Countries.FR.ToString(), Countries.RFA.ToString(), Countries.CAN.ToString(),
            Countries.DAN.ToString(), Countries.SWE.ToString(), Countries.NOR.ToString(), Countries.ROK.ToString(), Countries.JAP.ToString(), Countries.ANZ.ToString(), Countries.HOL.ToString(), Countries.ISR.ToString() };

        static public readonly HashSet<string> pactCountries = new HashSet<string>() { Countries.URSS.ToString(), Countries.RDA.ToString(), Countries.POL.ToString(), Countries.TCH.ToString(),
            Countries.NK.ToString(), Countries.CHI.ToString(), Countries.FIN.ToString(), Countries.YUG.ToString() };

        public NdfDatabase(string fileName) {
            string everythingNdfbinPattern = @".*everything.ndfbin";

            Regex regex = new Regex(everythingNdfbinPattern, RegexOptions.IgnoreCase);

            var ndfbinReader = new NdfbinReader();
            var edataManager = new EdataManager(fileName);
            edataManager.ParseEdataFile();

            var everythingNdfbinRaw = edataManager.Files.Where(t => regex.IsMatch(t.ToString())).First();

            everythingNdfbin = ndfbinReader.Read(edataManager.GetRawData(everythingNdfbinRaw));

            tShowRoomDeckSerializer = everythingNdfbin.Classes.Where(cls => cls.Name == "TShowRoomDeckSerializer").First();
            tUniteAuSolDescriptor = everythingNdfbin.Classes.Where(cls => cls.Name == "TUniteAuSolDescriptor").First();
            tUniteDescriptor = everythingNdfbin.Classes.Where(cls => cls.Name == "TUniteDescriptor").First();
            tTransportableModuleDescriptor = everythingNdfbin.Classes.Where(cls => cls.Name == "TTransportableModuleDescriptor").First();
            tShowRoomDeckRuleManager = everythingNdfbin.Classes.Where(cls => cls.Name == "TShowRoomDeckRuleManager").First();
            deckRulesManagerProperties = tShowRoomDeckRuleManager.Instances[0].PropertyValues;
        }

        public List<WargameUnit> GetOtanUnits() {
            var references = GetUnitReferences("OtanUnitIds").Where(reference => otanCountries.Contains(GetCountry(GetUnit((NdfMap)reference.Value)))).ToList();
            return GetUnits(references);
        }

        public List<WargameUnit> GetPactUnits() {
            var references = GetUnitReferences("PactUnitIds").Where(reference => pactCountries.Contains(GetCountry(GetUnit((NdfMap)reference.Value)))).ToList();
            return GetUnits(references);
        }

        public int GetDefaultActivationPoints() {
            var defaultActivationPoints = deckRulesManagerProperties.Where(p => p.Property.Name == "DefaultActivationPoints").First();
            return (int)(UInt32)((NdfUInt32)defaultActivationPoints.Value).Value;
        }

        public int GetActivationPoints(Countries country) {
            var modifiersForCountry = deckRulesManagerProperties.Where(p => p.Property.Name == "ModifiersForCountry").First();
            var countryMapping = ((NdfMapList)modifiersForCountry.Value).Where(v => ((NdfMap)v.Value).Key.Value.ToString() == country.ToString()).First();
            var activationPointsProperty = ((NdfObjectReference)((MapValueHolder)((NdfMap)countryMapping.Value).Value).Value).Instance.PropertyValues.Where(p => p.Property.Name == "ActivationPoints").First();
            return GetDefaultActivationPoints() + (int)((NdfInt32)activationPointsProperty.Value).Value;
        }

        public Dictionary<Categories, List<int>> GetCostMatrix() {
            Dictionary<Categories, List<int>> costMatrix = GetUnfilteredCostMatrix();
            Dictionary<Categories, int> slotsPerCategory = GetSlotsPerCategory();

            foreach (var costRowPair in costMatrix) {
                var costRow = costRowPair.Value;
                costRow.RemoveRange(slotsPerCategory[costRowPair.Key], costRow.Count - slotsPerCategory[costRowPair.Key]);
            }
            
            return costMatrix;
        }

        public Dictionary<Categories, List<int>> GetUnfilteredCostMatrix() {
            Dictionary<Categories, List<int>> unfilteredCostMatrix = new Dictionary<Categories, List<int>>();
            var defaultCostMatrixProperty = deckRulesManagerProperties.Where(p => p.Property.Name == "DefaultCostMatrix").First();

            foreach (var costMatrixRow in (NdfMapList)defaultCostMatrixProperty.Value) {
                Categories category = categories[(uint)(Int32)((NdfInt32)((NdfMap)costMatrixRow.Value).Key.Value).Value];
                List<int> costs = new List<int>();
                foreach (var row in (NdfCollection)((MapValueHolder)((NdfMap)costMatrixRow.Value).Value).Value) {
                    costs.Add((int)((NdfInt32)row.Value).Value);
                }
                unfilteredCostMatrix[category] = costs;
            }

            return unfilteredCostMatrix;
        }

        public Dictionary<Categories, int> GetSlotsPerCategory() {
            Dictionary<Categories, int> slotsPerCategory = new Dictionary<Categories, int>();
            var defaultSlotMatrix = deckRulesManagerProperties.Where(p => p.Property.Name == "DefaultSlotMatrix").First();

            foreach(var slotMatrixRow in (NdfMapList)defaultSlotMatrix.Value) {
                Categories category = categories[(uint)(Int32)((NdfInt32)((NdfMap)slotMatrixRow.Value).Key.Value).Value];
                slotsPerCategory[category] = (int)((NdfInt32)((MapValueHolder)((NdfMap)slotMatrixRow.Value).Value).Value).Value;
            }

            return slotsPerCategory;
        }

        private List<WargameUnit> GetUnits(List<CollectionItemValueHolder> references) {
            var units = new List<WargameUnit>();

            instanceIdToUnitId = GetInstanceIdToUnitIdMap(references);
            unitIdToUnit = new Dictionary<uint, WargameUnit>();
            unitIdToTransports = new Dictionary<uint, List<uint>>();

            unitUpgradeTree = new Dictionary<uint, uint>(); //<unit, next unit>
            BuildUnitUpgradeTree();

            foreach (var reference in references) {
                NdfMap map = (NdfMap)reference.Value;
                var unit = GetUnit(map);
                uint unitId = (uint)((NdfUInt32)map.Key.Value).Value;
                
                unitIdToTransports[unitId] = GetTransportsIds(unit);
                var rawCategory = GetCategory(unit);
                Categories category = categories.ContainsKey(rawCategory) ? categories[rawCategory] : Categories.UNKNOWN;

                var modulesProperty = unit.PropertyValues.Where(p => p.Property.Name == "Modules").First();
                var modules = (NdfCollection)modulesProperty.Value;
                var filteredList = modules.Where(m => ((NdfStringReference)((NdfString)((NdfMap)m.Value).Key.Value).Value).Value == "TypeUnit").First();
                var tModuleSelector = ((NdfObjectReference)((MapValueHolder)((NdfMap)filteredList.Value).Value).Value).Instance;
                var defaultProperty = tModuleSelector.PropertyValues.Where(p => p.Property.Name == "Default").First();
                var typeUnitModuleDescriptor = ((NdfObjectReference)defaultProperty.Value).Instance;
                var filtersProperty = typeUnitModuleDescriptor.PropertyValues.Where(p => p.Property.Name == "Filters").First();
                var subCategoryLocalizationHash = "None";
                if (filtersProperty.Value.GetType() == typeof(NdfMapList)) {
                    subCategoryLocalizationHash = ((NdfCollection)((MapValueHolder)((NdfMap)((NdfMapList)filtersProperty.Value)[0].Value).Value).Value)[0].Value.ToString();
                }

                Subcategories subcategory = subcategories.ContainsKey(subCategoryLocalizationHash) ? subcategories[subCategoryLocalizationHash] : Subcategories.NONE;
                
                var wUnit = new WargameUnit(unitId, GetNumberOfCards(unit), category, subcategory, GetYear(unit), GetCountry(unit), IsPrototype(unit), IsCommander(unit), GetName(unit));
                wUnit.AvailableVeterancy.AddRange(GetAvailableVeterancy(unit));

                List<Specializations> specs = GetAvailableSpecializations(unit);

                foreach (var spec in specs) {
                    wUnit.Specializations.Add(spec);
                }
                units.Add(wUnit);
                unitIdToUnit[unitId] = wUnit;
            }

            foreach (var unit in units) {
                foreach(var transportId in unitIdToTransports[unit.UnitId]) {
                    unit.Transports.Add(unitIdToUnit[transportId]);
                }
            }

            return units;
        }

        private NdfMapList GetUnitReferences(string faction) {
            NdfPropertyValue unitIds = tShowRoomDeckSerializer.Instances[0].PropertyValues.Where(prop => prop.Property.Name == faction).First();
            return (NdfMapList)unitIds.Value;
        }

        private void BuildUnitUpgradeTree() {
            var validInstances = tUniteAuSolDescriptor.Instances.Where(i => instanceIdToUnitId.ContainsKey(i.Id)).ToList();
            foreach (var tUniteAuSolInstance in validInstances) {
                uint unitId = tUniteAuSolInstance.Id;
                var upgradeRequiredProperty = tUniteAuSolInstance.PropertyValues.Where(p => p.Property.Name == "UpgradeRequire").First();
                if (upgradeRequiredProperty.Value.GetType() != typeof(NdfNull)) {
                    uint parentInstanceId = ((NdfObjectReference)upgradeRequiredProperty.Value).Instance.Id;
                    unitUpgradeTree[instanceIdToUnitId[parentInstanceId]] = instanceIdToUnitId[unitId];
                }
            }
        }

        private Dictionary<uint, uint> GetInstanceIdToUnitIdMap(List<CollectionItemValueHolder> references) {
            Dictionary<uint, uint> instanceIdToUnitId = new Dictionary<uint, uint>();

            foreach (var reference in references) {
                NdfMap map = (NdfMap)reference.Value;
                NdfObjectReference unitReference = (NdfObjectReference)((MapValueHolder)map.Value).Value;
                instanceIdToUnitId[(uint)unitReference.InstanceId] = (uint)((NdfUInt32)map.Key.Value).Value;
            }

            return instanceIdToUnitId;
        }

        private NdfObject GetUnit(NdfMap map) {
            MapValueHolder valueHolder = (MapValueHolder)map.Value;
            NdfObjectReference unitReference = (NdfObjectReference)valueHolder.Value;
            ObservableCollection<NdfObject> instances;
            if (unitReference.Class.Name == "TUniteAuSolDescriptor") {
                instances = tUniteAuSolDescriptor.Instances;
            }
            else {
                instances = tUniteDescriptor.Instances;
            }
            return instances.Where(i => i.Id == unitReference.InstanceId).First();
        }

        private List<uint> GetTransportsIds(NdfObject unit) {
            var modulesProperty = unit.PropertyValues.Where(p => p.Property.Name == "Modules").First();
            var modules = (NdfCollection)modulesProperty.Value;
            var filteredList = modules.Where(m => ((NdfStringReference)((NdfString)((NdfMap)m.Value).Key.Value).Value).Value == "Transportable").ToList();
            List<uint> transportIdList = new List<uint>();

            if (filteredList.Count > 0) {
                var transportModule = filteredList[0];
                var propertyValues = ((NdfObjectReference)((MapValueHolder)((NdfMap)transportModule.Value).Value).Value).Instance.PropertyValues;
                var propertyValue = propertyValues.Where(p => p.Property.Name == "TransportListAvailableForSpawn").First();
                if (propertyValue.Value.GetType() == typeof(NdfCollection)) {
                    var transportList = (NdfCollection)propertyValue.Value;
                    foreach (var transportReference in transportList) {
                        var transportMovingTypeProperty = ((NdfObjectReference)transportReference.Value).Instance.PropertyValues.Where(p => p.Property.Name == "UnitMovingType").First();
                        int movingType = (int)((NdfInt32)transportMovingTypeProperty.Value).Value;
                        //if (movingType != 9) {
                            uint transportId = instanceIdToUnitId[(uint)(int)((NdfObjectReference)transportReference.Value).InstanceId];
                            transportIdList.Add(transportId);
                            List<uint> transportUpgrades = new List<uint>();
                            while(unitUpgradeTree.ContainsKey(transportId)) {
                                transportUpgrades.Add(unitUpgradeTree[transportId]);
                                transportId = unitUpgradeTree[transportId];
                            }
                            transportIdList.AddRange(transportUpgrades);
                        //}
                    }
                }
            }

            return transportIdList;
        }



        private uint GetNumberOfCards(NdfObject unit) {
            return GetUnitPropertyValue(unit, "MaxPacks");
        }

        private List<uint> GetAvailableVeterancy(NdfObject unit) {
            List<uint> veterancy = new List<uint>();
            var maxDeployableAmountProperty = unit.PropertyValues.Where(p => p.Property.Name == "MaxDeployableAmount").First();
            if (maxDeployableAmountProperty.Value.GetType() == typeof(NdfCollection)) {
                var deployableAmountList = (NdfCollection)maxDeployableAmountProperty.Value;
                uint i = 0;
                foreach (var deployableAmount in deployableAmountList) {
                    int amount = (Int32)((NdfInt32)deployableAmount.Value).Value;
                    if (amount > 0) {
                        veterancy.Add(i);
                    }
                    i++;
                }
            }
            return veterancy;
        }

        private uint GetCategory(NdfObject unit) {
            return GetUnitPropertyValue(unit, "Factory");
        }

        private uint GetYear(NdfObject unit) {
            return GetUnitPropertyValue(unit, "ProductionYear");
        }

        private uint GetUnitPropertyValue(NdfObject unit, string propertyName) {
            uint value = 0;
            var propertyList = unit.PropertyValues.Where(p => p.Property.Name == propertyName);
            if (propertyList.Count() > 0) {
                var property = propertyList.First();
                if (property.Value.GetType() == typeof(NdfUInt32)) {
                    value = (UInt32)((NdfUInt32)property.Value).Value;
                } else if (property.Value.GetType() == typeof(NdfInt32)) {
                    value = (uint)(Int32)((NdfInt32)property.Value).Value;
                }
            }
            return value;
        }

        private string GetCountry(NdfObject unit) {
            var propertyList = unit.PropertyValues.Where(p => p.Property.Name == "MotherCountry");
            if (propertyList.Count() > 0) {
                return propertyList.First().Value.ToString();
            }
            return "UNKNOWN";
        }

        private string GetName(NdfObject unit) {
            var propertyList = unit.PropertyValues.Where(p => p.Property.Name == "ClassNameForDebug");
            if (propertyList.Count() > 0) {
                return propertyList.First().Value.ToString();
            }
            return "UNKNOWN";
        }

        private bool IsPrototype(NdfObject unit) {
            var isPrototypePropertyList = unit.PropertyValues.Where(p => p.Property.Name == "IsPrototype");
            if (isPrototypePropertyList.Count() > 0) {
                if (isPrototypePropertyList.First().Value.GetType() != typeof(NdfNull)) {
                    return true;
                }
            }
            return false;
        }

        private bool IsCommander(NdfObject unit) {
            var modulesProperty = unit.PropertyValues.Where(p => p.Property.Name == "Modules").First();
            var modules = (NdfCollection)modulesProperty.Value;
            var commandMagagerModule = modules.Where(m => ((NdfStringReference)((NdfString)((NdfMap)m.Value).Key.Value).Value).Value == "CommandManager").ToList();
            if (commandMagagerModule.Count > 0) {
                return true;
            }
            return false;
        }

        private List<Specializations> GetAvailableSpecializations(NdfObject unit) {
            List<Specializations> specs = new List<Specializations>();
            var specProperty = unit.PropertyValues.Where(p => p.Property.Name == "UnitTypeTokens").First();
            if (specProperty.Value.GetType() == typeof(NdfCollection)) {
                foreach (var specPropertyValue in (NdfCollection)specProperty.Value) {
                    string hash = ((NdfLocalisationHash)specPropertyValue.Value).ToString();
                    specs.Add(specializations[hash]);
                }
            }
            return specs;
        }
    }
}
