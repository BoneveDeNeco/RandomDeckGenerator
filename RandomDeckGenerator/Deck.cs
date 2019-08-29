using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DeckGenerator {
    public class UnitStruct {
        public byte Veterancy { get; }
        public int Unit { get; }

        public UnitStruct(byte veterancy, int unit) {
            Veterancy = veterancy;
            Unit = unit;
        }
    }

    public class TransportedUnitStruct : UnitStruct {
        public int Transport { get; }

        public TransportedUnitStruct(byte veterancy, int unit, int transport) : base(veterancy, unit) {
            Transport = transport;
        }
    }

    public class SuperTransportedUnitStruct : TransportedUnitStruct {
        public int SuperTransport { get; }

        public SuperTransportedUnitStruct(byte veterancy, int unit, int transport, int superTransport) : base(veterancy, unit, transport) {
            SuperTransport = superTransport;
        }
    }

    public class Deck {
        //00011110110011110000
        private static readonly BitArray BluforGeneral = new BitArray(new bool[] { false, false, false, true, true, true, true, false, true, true, false, false, true, true, true });
        //01010010110011110000
        private static readonly BitArray RedforGeneral = new BitArray(new bool[] { false, true, false, true, false, false, true, false, true, true, false, false, true, true, true });

        private static readonly BitArray Anzac = new BitArray(new bool[] { false, false, false, true, false, false, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray WGermany = new BitArray(new bool[] { false, false, false, false, false, true, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray Canada = new BitArray(new bool[] { false, false, false, false, true, false, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray Denamark = new BitArray(new bool[] { false, false, false, false, true, false, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray France = new BitArray(new bool[] { false, false, false, false, false, true, false, false, true, false, true, false, true, true, true });
        private static readonly BitArray Japan = new BitArray(new bool[] { false, false, false, false, false, true, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray Holand = new BitArray(new bool[] { false, false, false, true, false, true, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray Norway = new BitArray(new bool[] { false, false, false, false, true, true, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray SKorea = new BitArray(new bool[] { false, false, false, true, false, true, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray Sweden = new BitArray(new bool[] { false, false, false, false, true, true, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray UK = new BitArray(new bool[] { false, false, false, false, false, false, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray US = new BitArray(new bool[] { false, false, false, false, false, false, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray Israel = new BitArray(new bool[] { false, false, false, true, true, false, false, false, true, true, false, false, true, true, true });

        private static readonly BitArray Czech = new BitArray(new bool[] { false, true, false, false, false, true, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray EGermany = new BitArray(new bool[] { false, true, false, false, false, false, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray NKorea = new BitArray(new bool[] { false, true, false, false, true, false, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray Poland = new BitArray(new bool[] { false, true, false, false, false, true, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray China = new BitArray(new bool[] { false, true, false, false, true, false, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray Ussr = new BitArray(new bool[] { false, true, false, false, false, false, true, false, true, true, false, false, true, true, true });
        private static readonly BitArray Finland = new BitArray(new bool[] { false, true, false, false, true, true, false, false, true, true, false, false, true, true, true });
        private static readonly BitArray Yugoslavia = new BitArray(new bool[] { false, true, false, false, true, true, true, false, true, true, false, false, true, true, true });

        private static readonly BitArray CatA = new BitArray(new bool[] { true, false });
        private static readonly BitArray CatB = new BitArray(new bool[] { false, true });
        private static readonly BitArray CatC = new BitArray(new bool[] { false, false });

        public enum DeckType {
            Blufor,
            Redfor,
            Anzac,
            WGermany,
            Canada,
            Denamark,
            France,
            Japan,
            Holand,
            Norway,
            SKorea,
            Sweden,
            UK,
            US,
            Israel,
            Czech,
            EGermany,
            NKorea,
            Poland,
            China,
            Ussr,
            Finland,
            Yugoslavia
        };

        private static Dictionary<DeckType, BitArray> deckBits = new Dictionary<DeckType, BitArray> {
            { DeckType.Blufor, BluforGeneral },
            { DeckType.Redfor, RedforGeneral },
            { DeckType.Anzac, Anzac },
            { DeckType.WGermany, WGermany },
            { DeckType.Canada, Canada },
            { DeckType.Denamark, Denamark },
            { DeckType.France, France },
            { DeckType.Japan, Japan },
            { DeckType.Holand, Holand },
            { DeckType.Norway, Norway },
            { DeckType.SKorea, SKorea },
            { DeckType.Sweden, Sweden },
            { DeckType.UK, UK },
            { DeckType.US, US },
            { DeckType.Israel , Israel },
            { DeckType.Czech, Czech },
            { DeckType.EGermany, EGermany },
            { DeckType.NKorea, NKorea },
            { DeckType.Poland, Poland },
            { DeckType.China, China },
            { DeckType.Ussr, Ussr },
            { DeckType.Finland, Finland },
            { DeckType.Yugoslavia, Yugoslavia }
        };

        private BitArray deckType;

        private List<UnitStruct> units = new List<UnitStruct>();
        private List<TransportedUnitStruct> transportedUnits = new List<TransportedUnitStruct>();
        private List<SuperTransportedUnitStruct> superTransportedUnits = new List<SuperTransportedUnitStruct>();

        public Deck(DeckType type) {
            deckType = deckBits[type];
        }

        public string GetDeckCode() {
            BitArray unitWithTransportCount = new BitArray(5, false);
            BitArray bargesCount = new BitArray(4, false);

            unitWithTransportCount = ConvertToBigEndianBitArray(transportedUnits.Count, 5);
            bargesCount = ConvertToBigEndianBitArray(superTransportedUnits.Count, 4);
            BitArray deck = new BitArray(0);

            deck.Append(deckType).Append(CatA).Append(bargesCount).Append(unitWithTransportCount);

            foreach (SuperTransportedUnitStruct superTransportedUnit in superTransportedUnits) {
                BitArray vetCode = ConvertToBigEndianBitArray(superTransportedUnit.Veterancy, 3);
                BitArray unitCode = ConvertToBigEndianBitArray(superTransportedUnit.Unit, 11);
                BitArray transportCode = ConvertToBigEndianBitArray(superTransportedUnit.Transport, 11);
                BitArray superTransportCode = ConvertToBigEndianBitArray(superTransportedUnit.SuperTransport, 11);

                deck.Append(vetCode).Append(unitCode).Append(transportCode).Append(superTransportCode);
            }

            deck.Append(GetTransportedUnitsCodes());

            deck.Append(GetUnitsCodes());

            deck = AddPadding(deck);

            return Convert.ToBase64String(BitArrayToByteArray(deck));
        }

        public void AddUnit(byte veterancy, int unit) {
            units.Add(new UnitStruct(veterancy, unit));
        }

        public void AddUnit(byte veterancy, int unit, int transport) {
            transportedUnits.Add(new TransportedUnitStruct(veterancy, unit, transport));
        }

        public void AddUnit(byte veterancy, int unit, int transport, int superTransport) {
            superTransportedUnits.Add(new SuperTransportedUnitStruct(veterancy, unit, transport, superTransport));
        }

        public BitArray ConvertToBigEndianBitArray(int number, int numBits) {
            if (numBits > 32) { return null; }
            BitArray bitArray = new BitArray(new int[] { number });
            bool[] bits = new bool[numBits];
            for (int i = 0; i < numBits; i++) {
                bits[i] = bitArray[numBits - 1 - i];
            }

            return new BitArray(bits);
        }

        public int GetNumberOfUnits() {
            return units.Count + transportedUnits.Count;
        }

        private BitArray GetTransportedUnitsCodes() {
            BitArray codes = new BitArray(0);
            foreach (TransportedUnitStruct transportedUnit in transportedUnits) {
                BitArray vetCode = ConvertToBigEndianBitArray(transportedUnit.Veterancy, 3);
                BitArray unitCode = ConvertToBigEndianBitArray(transportedUnit.Unit, 11);
                BitArray transportCode = ConvertToBigEndianBitArray(transportedUnit.Transport, 11);

                codes.Append(vetCode).Append(unitCode).Append(transportCode);
            }
            return codes;
        }

        private BitArray GetUnitsCodes() {
            BitArray codes = new BitArray(0);
            foreach (UnitStruct unit in units) {
                BitArray vetCode = ConvertToBigEndianBitArray(unit.Veterancy, 3);
                BitArray unitCode = ConvertToBigEndianBitArray(unit.Unit, 11);
                codes.Append(vetCode).Append(unitCode);
            }
            return codes;
        }

        private BitArray AddPadding(BitArray unpadded) {
            int padding = 8 - (unpadded.Count % 8);
            bool[] padded = new bool[unpadded.Count + padding];
            for (int i = 0; i < padded.Length; i++) { padded[i] = false; }
            unpadded.CopyTo(padded, 0);
            return new BitArray(padded);
        }

        private byte[] BitArrayToByteArray(BitArray bits) {
            BitArray correctEndiannessArray = CorrectBitEndiannes(bits);
            byte[] ret = new byte[(correctEndiannessArray.Length - 1) / 8 + 1];
            correctEndiannessArray.CopyTo(ret, 0);
            return ret;
        }

        private BitArray CorrectBitEndiannes(BitArray bits) {
            BitArray correctEndiannessArray = new BitArray(bits.Length);
            int bytes = bits.Length / 8;
            for (int b = 0; b < bytes; b++) {
                correctEndiannessArray[(b * 8) + 7] = bits[(b * 8) + 0];
                correctEndiannessArray[(b * 8) + 6] = bits[(b * 8) + 1];
                correctEndiannessArray[(b * 8) + 5] = bits[(b * 8) + 2];
                correctEndiannessArray[(b * 8) + 4] = bits[(b * 8) + 3];
                correctEndiannessArray[(b * 8) + 3] = bits[(b * 8) + 4];
                correctEndiannessArray[(b * 8) + 2] = bits[(b * 8) + 5];
                correctEndiannessArray[(b * 8) + 1] = bits[(b * 8) + 6];
                correctEndiannessArray[(b * 8) + 0] = bits[(b * 8) + 7];
            }
            return correctEndiannessArray;
        }

        private string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private string Base64Decode(string base64EncodedData) {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public static class BitArrayExtensions {
        public static BitArray Append(this BitArray current, BitArray after) {
            int offset = current.Length;
            current.Length = current.Length + after.Length;
            for (int i = 0; i < after.Length; i++) {
                current[i + offset] = after[i];
            }
            return current;
            /*var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);*/
        }
    }
}
