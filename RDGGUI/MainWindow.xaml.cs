using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using DeckGenerator;


namespace RDGGUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private RandomDeckGenerator generator;

        public MainWindow() {
            InitializeComponent();
            Nationality.ItemsSource = new List<Deck.DeckType> {
                Deck.DeckType.Anzac, Deck.DeckType.Blufor, Deck.DeckType.Canada, Deck.DeckType.China, Deck.DeckType.Czech, Deck.DeckType.Denamark, Deck.DeckType.EGermany, Deck.DeckType.Finland
                , Deck.DeckType.France, Deck.DeckType.Holand, Deck.DeckType.Israel, Deck.DeckType.Japan, Deck.DeckType.NKorea, Deck.DeckType.Norway, Deck.DeckType.Poland, Deck.DeckType.Redfor
                , Deck.DeckType.SKorea, Deck.DeckType.Sweden, Deck.DeckType.UK, Deck.DeckType.US, Deck.DeckType.Ussr, Deck.DeckType.WGermany, Deck.DeckType.Yugoslavia
            };
            Nationality.SelectedIndex = 0;
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DAT File (*.dat)|*.dat";
            if (openFileDialog.ShowDialog() == true) {
                NdfDatabase database = new NdfDatabase(openFileDialog.FileName);
                generator = new RandomDeckGenerator(database);
                buttonGenerate.IsEnabled = true;
            }
        }

        private void Button_Generate_Click(object sender, RoutedEventArgs e) {
            dialogLabel.Text = "";

            generator.SetWeigth(Categories.LOGISTICS, Int32.Parse(LogisticsWeight.Text));
            generator.SetWeigth(Categories.SUPPORT, Int32.Parse(SupportWeight.Text));
            generator.SetWeigth(Categories.INFANTRY, Int32.Parse(InfantryWeight.Text));
            generator.SetWeigth(Categories.TANK, Int32.Parse(TankWeight.Text));
            generator.SetWeigth(Categories.RECON, Int32.Parse(ReconWeight.Text));
            generator.SetWeigth(Categories.VEHICLE, Int32.Parse(VehicleWeight.Text));
            generator.SetWeigth(Categories.HELICOPTER, Int32.Parse(HelicopterWeight.Text));
            generator.SetWeigth(Categories.PLANE, Int32.Parse(PlaneWeight.Text));
            generator.SetWeigth(Categories.NAVAL, Int32.Parse(NavalWeight.Text));

            generator.SetMinimumYear(Categories.LOGISTICS, Int32.Parse(LogisticsMinYear.Text));
            generator.SetMinimumYear(Categories.SUPPORT, Int32.Parse(SupportMinYear.Text));
            generator.SetMinimumYear(Categories.INFANTRY, Int32.Parse(InfantryMinYear.Text));
            generator.SetMinimumYear(Categories.TANK, Int32.Parse(TankMinYear.Text));
            generator.SetMinimumYear(Categories.RECON, Int32.Parse(ReconMinYear.Text));
            generator.SetMinimumYear(Categories.VEHICLE, Int32.Parse(VehicleMinYear.Text));
            generator.SetMinimumYear(Categories.HELICOPTER, Int32.Parse(HelicopterMinYear.Text));
            generator.SetMinimumYear(Categories.PLANE, Int32.Parse(PlaneMinYear.Text));
            generator.SetMinimumYear(Categories.NAVAL, Int32.Parse(NavalMinYear.Text));

            generator.SetMaximumYear(Categories.LOGISTICS, Int32.Parse(LogisticsMaxYear.Text));
            generator.SetMaximumYear(Categories.SUPPORT, Int32.Parse(SupportMaxYear.Text));
            generator.SetMaximumYear(Categories.INFANTRY, Int32.Parse(InfantryMaxYear.Text));
            generator.SetMaximumYear(Categories.TANK, Int32.Parse(TankMaxYear.Text));
            generator.SetMaximumYear(Categories.RECON, Int32.Parse(ReconMaxYear.Text));
            generator.SetMaximumYear(Categories.VEHICLE, Int32.Parse(VehicleMaxYear.Text));
            generator.SetMaximumYear(Categories.HELICOPTER, Int32.Parse(HelicopterMaxYear.Text));
            generator.SetMaximumYear(Categories.PLANE, Int32.Parse(PlaneMaxYear.Text));
            generator.SetMaximumYear(Categories.NAVAL, Int32.Parse(NavalMaxYear.Text));

            generator.SetWeigth(Subcategories.HEL_AA, Int32.Parse(aaHeloWeigth.Text));
            generator.SetWeigth(Subcategories.HEL_ATGM, Int32.Parse(atgmHeloWeigth.Text));
            generator.SetWeigth(Subcategories.HEL_GUNSHIP, Int32.Parse(gunshipWeigth.Text));
            generator.SetWeigth(Subcategories.INF_ATGM, Int32.Parse(atgmInfWeight.Text));
            generator.SetWeigth(Subcategories.INF_ENGINEERS, Int32.Parse(engineerWeight.Text));
            generator.SetWeigth(Subcategories.INF_FIRE_SUPPORT, Int32.Parse(fireSupInfWeight.Text));
            generator.SetWeigth(Subcategories.INF_LIGHT, Int32.Parse(lightInfWeight.Text));
            generator.SetWeigth(Subcategories.INF_LINE, Int32.Parse(lineInfWeight.Text));
            generator.SetWeigth(Subcategories.INF_MANPADS, Int32.Parse(manpadInfWeight.Text));
            generator.SetWeigth(Subcategories.INF_RESERVISTS, Int32.Parse(reserveInfWeight.Text));
            generator.SetWeigth(Subcategories.INF_SF, Int32.Parse(sfInfWeight.Text));
            generator.SetWeigth(Subcategories.LOG_CMD_ARMORED, Int32.Parse(CmdArm.Text));
            generator.SetWeigth(Subcategories.LOG_CMD_HELO, Int32.Parse(CmdHel.Text));
            generator.SetWeigth(Subcategories.LOG_CMD_INF, Int32.Parse(CmdInfWeight.Text));
            generator.SetWeigth(Subcategories.LOG_CMD_VEH, Int32.Parse(CmdVeh.Text));
            generator.SetWeigth(Subcategories.LOG_FOB, Int32.Parse(fobWeight.Text));
            generator.SetWeigth(Subcategories.LOG_SUP_HELO, Int32.Parse(SupHel.Text));
            generator.SetWeigth(Subcategories.LOG_SUP_TRUCK, Int32.Parse(SupVeh.Text));
            generator.SetWeigth(Subcategories.NAV_AS_HELO, Int32.Parse(asHeloWeight.Text));
            generator.SetWeigth(Subcategories.NAV_AS_PLANE, Int32.Parse(asPlaneWeight.Text));
            generator.SetWeigth(Subcategories.NAV_AS_TRUCK, Int32.Parse(asTruckWeight.Text));
            generator.SetWeigth(Subcategories.NAV_FRIGATE, Int32.Parse(frigateWeight.Text));
            generator.SetWeigth(Subcategories.NAV_SHORE, Int32.Parse(coastalWeight.Text));
            generator.SetWeigth(Subcategories.NAV_SUPPLY, Int32.Parse(supplyShipWeight.Text));
            generator.SetWeigth(Subcategories.NAV_SUPPORT, Int32.Parse(supportShipWeight.Text));
            generator.SetWeigth(Subcategories.PLN_ANTI_TANK, Int32.Parse(antitankWeight.Text));
            generator.SetWeigth(Subcategories.PLN_ASF, Int32.Parse(asfWeight.Text));
            generator.SetWeigth(Subcategories.PLN_BOMBER, Int32.Parse(bomberWeight.Text));
            generator.SetWeigth(Subcategories.PLN_INTERCEPTOR, Int32.Parse(interceptorWeight.Text));
            generator.SetWeigth(Subcategories.PLN_MULTIROLE, Int32.Parse(multiroleWeight.Text));
            generator.SetWeigth(Subcategories.PLN_SEAD, Int32.Parse(seadWeight.Text));
            generator.SetWeigth(Subcategories.REC_HELO, Int32.Parse(heloRecWeigth.Text));
            generator.SetWeigth(Subcategories.REC_INFANTRY, Int32.Parse(infantryRecWeigth.Text));
            generator.SetWeigth(Subcategories.REC_SF, Int32.Parse(sfRecWeigth.Text));
            generator.SetWeigth(Subcategories.REC_VEHICLE, Int32.Parse(vehicleRecWeigth.Text));
            generator.SetWeigth(Subcategories.SUP_AA_MISSILE, Int32.Parse(missileWeight.Text));
            generator.SetWeigth(Subcategories.SUP_AA_SPAAG, Int32.Parse(spaagWeight.Text));
            generator.SetWeigth(Subcategories.SUP_HOWITZER, Int32.Parse(howitzerWeight.Text));
            generator.SetWeigth(Subcategories.SUP_MORTAR, Int32.Parse(mortarWeight.Text));
            generator.SetWeigth(Subcategories.SUP_MRLS, Int32.Parse(mrlsWeight.Text));
            generator.SetWeigth(Subcategories.TNK_CAVALRY, Int32.Parse(cavalryTankWeigth.Text));
            generator.SetWeigth(Subcategories.TNK_HEAVY, Int32.Parse(heavyTankWeigth.Text));
            generator.SetWeigth(Subcategories.TNK_LIGHT, Int32.Parse(lightTankWeigth.Text));
            generator.SetWeigth(Subcategories.TNK_MEDIUM, Int32.Parse(mediumTankWeigth.Text));
            generator.SetWeigth(Subcategories.VEH_ATGM, Int32.Parse(atgmVehWeigth.Text));
            generator.SetWeigth(Subcategories.VEH_FIRE_SUPPORT, Int32.Parse(fireSupVehWeigth.Text));
            generator.SetWeigth(Subcategories.VEH_FLAMER, Int32.Parse(flameWeigth.Text));

            dialogLabel.Text = "@" + generator.GenerateDeck((Deck.DeckType)Nationality.SelectedItem).GetDeckCode();
            Clipboard.SetText(dialogLabel.Text);
        }

        private void NumberOnlyPreviewKeyDown(object sender, KeyEventArgs args) {
            switch (args.Key) {
                case Key.D0:
                case Key.NumPad0:
                case Key.D1:
                case Key.NumPad1:
                case Key.D2:
                case Key.NumPad2:
                case Key.D3:
                case Key.NumPad3:
                case Key.D4:
                case Key.NumPad4:
                case Key.D5:
                case Key.NumPad5:
                case Key.D6:
                case Key.NumPad6:
                case Key.D7:
                case Key.NumPad7:
                case Key.D8:
                case Key.NumPad8:
                case Key.D9:
                case Key.NumPad9:
                    args.Handled = false;
                    break;
                default:
                    args.Handled = true;
                    break;
            }
        }


    }
}