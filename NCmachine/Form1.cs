using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Data.SqlClient;//MUSI BYC
using System.Data.Sql;//TO TEZ

namespace NCmachine
{

    public partial class NC : Form
    {
        //
        //Wybierz connectionString w zależnościod tego u kogo odpalasz aplikację!
        //
        public string connectionString =
            "Data Source=(LocalDB)\\v11.0;AttachDbFilename=C:\\Users\\Piotrek\\Documents\\Visual Studio 2013\\Projects\\NC z Bazą\\NCmachine 28.11.14 - Kopia z baza danych\\NCmachine\\BazaParametrow.mdf;Integrated Security=True;Connect Timeout=30";
   //"Data Source=(LocalDB)\\v11.0;AttachDbFilename=D:\\Users\\Krzysiek\\Dysk Google\\Laserowa Dzida\\Software\\NC z Bazą 03.02.15\\NCmachine 28.11.14 - Kopia z baza danych\\NCmachine\\BazaParametrow.mdf;Integrated Security=True;Connect Timeout=30";
        public SerialPort PolaczenieParam = new SerialPort();
        public PolaczenieClass Polaczenie = new PolaczenieClass();
        public MaszynaClass Maszyna = new MaszynaClass();
        public WczytywanieObrazu Wczytaj = new WczytywanieObrazu();
        public CRC16CLASS CRC = new CRC16CLASS();
        public string odebrana_linia;
        public bool TestujFlaga = false;    //ustawiane na true gdy wysłane jest żądanie o odpowiedź testową
        public string[] GotowyPlik;
        public string Konfiguracja;


        static String path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); //tworzenie folderu specjalnego dla zapisu plików


        public NC()
        {
            //Control.CheckForIllegalCrossThreadCalls = False;
            InitializeComponent();

        }



        //Komunikacja


        //----------------------Ładowanie listy portów COM----------------------------//
        private void Picture_Click(object sender, EventArgs e)
        {
            LabelCOM.Text = null;
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                LabelCOM.Text = port;
            }
        }
        //---------------------------------------------------------------------------//

        //----------------------Obsługa przycisku Wybierz----------------------------//
        private void ButtonWybierzMaszyne_Click(object sender, EventArgs e)
        {
            MaszynaClass Maszyna = new MaszynaClass(Convert.ToInt16(TextBoxBaudRate.Text)); // utowrzenie obiektu maszyna

            bool a = Polaczenie.Otworz(ref PolaczenieParam, Maszyna, LabelCOM.Text);
            if (a)
            {
                CheckPolaczenie.Checked = true;  // utworzenie połącznia
                PolaczenieParam.DataReceived += new SerialDataReceivedEventHandler(PolaczenieParam_Odbierz);
                //utwórz zdarzenie odbierania danych (w klasie nie wiem jak)
            }
        }
        //---------------------------------------------------------------------------//

        //coś nie tak z zamykaniem portu
        private void ButtonRozlacz_Click(object sender, EventArgs e)
        {
            bool a = Polaczenie.Zamknij(ref PolaczenieParam);
            if (a)
                CheckPolaczenie.Checked = false; // zamknięcie połączenia
        }
        //------------------------Testowanie połączenia------------------------//
        private void ButtonTestuj_Click(object sender, EventArgs e)
        {
            Polaczenie.Wyslij(":1 testuj polaczenie #", ref PolaczenieParam, ref TextBoxWyslane);
            TestujFlaga = true;
        }
        //---------------------------------------------------------------------------//

        //------------------------Zdarzenie odebrania znaków------------------------//
        public void PolaczenieParam_Odbierz(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                odebrana_linia = PolaczenieParam.ReadLine().ToString();
                //TextBoxOdebrane.Text = linia;
                Polaczenie.Odbierz(odebrana_linia, ref TextBoxOdebrane);
            }
            catch (Exception)
            {
                Polaczenie.Odbierz("\n Zakonczono połączenie \n", ref TextBoxOdebrane);
            }
            //instrukcja switch rozdielająca odebrane komendy
            char temp = Convert.ToChar(odebrana_linia[1]);  // przechwytujemy kod komendy
            switch (temp)
            {
                case '1':       //testowanie połączenia
                    if (TestujFlaga)
                    {
                        string testString = odebrana_linia;
                        if (testString.Contains(":1 polaczenie aktywne #"))
                            SetCheckboxThread(CheckBoxTestujPolaczenie, true);
                        else
                            SetCheckboxThread(CheckBoxTestujPolaczenie, false);
                    }
                    break;
                case '2':
                    //odebrano wymiary stołu
                    break;
                case '3':
                    // coś z dojazdem
                    break;
                case '4':

                    string testString2 = odebrana_linia;
                   
                     SetCheckboxThread(CheckBoxUstawMaszyne, testString2.Contains(Konfiguracja));
                    
                    break;

                case '5':       //bloakada wysyłania na końcu pliku!

                    char[] koniec = { ' ', '#', '\r' };  //bo to trzeba usunąć z końca
                    string Snr_linii = odebrana_linia.TrimEnd(koniec);
                    Snr_linii = Snr_linii.Remove(0, 3);

                    int nr_linii = Convert.ToInt16(Snr_linii);

                    Polaczenie.Wyslij(":5 " + Snr_linii + " " + GotowyPlik[nr_linii] + " #", ref PolaczenieParam, ref TextBoxWyslane);

                    if (ProgressBarTransmisja.InvokeRequired)
                        ProgressBarTransmisja.Invoke(new SetNrLiniiHander(SetNrLiniiExec), nr_linii);
                    else
                        SetNrLiniiExec(nr_linii);
                   

                    break;


                default:
                    Polaczenie.Odbierz("odebrano nieznaną komenda", ref TextBoxOdebrane);
                    break;

            }

        }
        //---------------------------------------------------------------------------//

        public bool False { get; set; }

        private void ButtonWyslij_Click(object sender, EventArgs e)
        {


            Polaczenie.Wyslij(TextBoxWyslij.Text, ref PolaczenieParam, ref TextBoxWyslane);
            TextBoxWyslij.Clear();

        }

        public delegate void SetNrLiniiHander(int nr);

        public void SetNrLiniiExec(int nr_linii)
        {
            ProgressBarTransmisja.Value = nr_linii;
        }

        public IButtonControl myDefaultBtn { get; set; }

        private void TextBoxWyslij_TextChanged(object sender, EventArgs e)
        {

            Task.Factory.StartNew(() =>
            {

            });
            this.AcceptButton = ButtonWyslij;
        }



        //Koniec komunikajci



        //Ładowanie obrazu


        private void ButtonPrzegladajPliki_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                TextBoxWybierzPlik.Text = openFileDialog1.FileName.ToString();
                /* System.IO.StreamReader sr = new 
               System.IO.StreamReader(openFileDialog1.FileName);
            MessageBox.Show(sr.ReadToEnd());
            sr.Close();*/
            }
        }

        private void ButtonWybierzPlik_Click(object sender, EventArgs e)
        {
            picturePodglad.ImageLocation = TextBoxWybierzPlik.Text;      //załaduj adres
            TextBoxRozmiarPlamkiLadowanie.Text = Convert.ToString(Maszyna.RozmiarPlamki);    //załaduj rozmiar plamki
            TextBoxRozdzielczosc.Text = Convert.ToString(254 / Maszyna.RozmiarPlamki);   //oblicz porzebne dpi
            int szer;
            try
            {
                szer = int.Parse(TextBoxRozmiarSzer.Text);
            }
            catch (Exception)
            {
                szer = 0;
            }
            if (szer != 0)
            {
                int a = Wczytaj.PomiarDpi(TextBoxWybierzPlik.Text, szer);      //oblicz uzyskane dpi jesli podany jest wymiar
                TextBoxZaladowanaRozd.Text = a.ToString();
            }
            else
            {
                TextBoxZaladowanaRozd.Text = "Podaj wymiary";
            }

        }

        public void ButtonKonwertuj_Click(object sender, EventArgs e)
        {
            bool ak;
            try
            {
                if (System.IO.Directory.Exists(path + "gotowy_plik.txt"))
                {
                    System.IO.Directory.Delete(path + "gotowy_plik.txt");
                }
                /* Wczytaj.BmpTo01ArrWithInversion(TextBoxWybierzPlik.Text, path + "gotowe.txt");
                 Wczytaj.CountingZerosAndOnes(path + "gotowe.txt", path + "gotowe_liczby.txt", ref TextBoxWynikKonw);*/
                Wczytaj.Konwersja_obrazu(TextBoxWybierzPlik.Text, path + "gotowy_plik.txt", ref TextBoxWynikKonw);
                ak = true;
            }
            catch (Exception)
            {
                ak = false;
                MessageBox.Show("Problem z konwersją", "Coś poszło nie tak");
            }
            if (ak)//jeśli wszystko poszło ok
            {
                CheckBoxKonwertuj.Checked = true;
                GotowyPlik = System.IO.File.ReadAllLines(path + "gotowy_plik.txt");
            }
            else
            {
                CheckBoxKonwertuj.Checked = false;
            }

        }



        //--------------------------------Ładowanie ustawień maszyny-------------------------------------------//
        private void ButtonUstawMaszyne_Click(object sender, EventArgs e)
        {
            bool a;
            /*try
            {*/

            a = Maszyna.Ustaw(Convert.ToInt16(TextBoxPredkoscX.Text), Convert.ToInt16(TextBoxPrzyspieszenieX.Text),
                Convert.ToInt16(TextBoxSkokSrubyX.Text), Convert.ToInt16(TextBoxKrokiX.Text), Convert.ToInt16(TextBoxPredkoscY.Text),
                Convert.ToInt16(TextBoxPrzyspieszenieY.Text), Convert.ToInt16(TextBoxSkokSrubyY.Text), Convert.ToInt16(TextBoxKrokiY.Text),
                Convert.ToDouble(TextBoxRozmiarPlamki.Text));

            //Komputer: 	:kod  przysp X  pred_max X kroki_na_obrót_X przysp Y  pred_max Y kroki_na_obrot_Y (w impulsach na sec)  wymiar_plytki_x  wymiar_plytki_y (w pikselach) liczba_kroków_naPixel  CRC#
            //prędkość maxymalna [rad/sec]*100  [vmax/skok]*2pi*100
            int pred_max_X = (Convert.ToInt16(TextBoxPredkoscX.Text) / Convert.ToInt16(TextBoxSkokSrubyX.Text)) * 628;
            string Spred_max_X = Convert.ToString(pred_max_X);
            //przyspieszenie maxymalne [rad/sec2]*100  [accmax/skok]*2pi*100
            int przysp_X = (Convert.ToInt16(TextBoxPrzyspieszenieX.Text) / Convert.ToInt16(TextBoxSkokSrubyX.Text)) * 628;
            string Sprzysp_X = Convert.ToString(przysp_X);

            //prędkość maxymalna [rad/sec]*100  [vmax/skok]*2pi*100
            int pred_max_Y = (Convert.ToInt16(TextBoxPredkoscY.Text) / Convert.ToInt16(TextBoxSkokSrubyY.Text)) * 628;
            string Spred_max_Y = Convert.ToString(pred_max_Y);
            //przyspieszenie maxymalne [rad/sec2]*100  [accmax/skok]*2pi*100
            int przysp_Y = (Convert.ToInt16(TextBoxPrzyspieszenieY.Text) / Convert.ToInt16(TextBoxSkokSrubyY.Text)) * 628;
            string Sprzysp_Y = Convert.ToString(przysp_Y);

            int wymiar_X = Wczytaj.PomiarIlosciLini(TextBoxWybierzPlik.Text, false);
            string Swymiar_X = Convert.ToString(wymiar_X);
            string Swymiar_Y = Convert.ToString(Wczytaj.PomiarIlosciLini(TextBoxWybierzPlik.Text, true));
            int szerokosc_X = Convert.ToInt16(TextBoxRozmiarSzer.Text);
            double przemieszczenie_na_krok_X = (double)(Convert.ToDouble(TextBoxSkokSrubyX.Text) / Convert.ToDouble(TextBoxKrokiX.Text));
            int pX = (int)((double)((double)(Convert.ToDouble(szerokosc_X) / Convert.ToDouble(wymiar_X)) / przemieszczenie_na_krok_X));
            string Ilosc_krokow_na_pixel_X = Convert.ToString(pX);

            Konfiguracja = (":4 " + Sprzysp_X + " " + Spred_max_X + " " + TextBoxKrokiX.Text
                                     + " " + Sprzysp_Y + " " + Spred_max_Y + " " + TextBoxKrokiY.Text
                                     + " " + Swymiar_X + " " + Swymiar_Y + " " + "1" + " " + "67"
                                     + " #");

            Polaczenie.Wyslij(Konfiguracja, ref PolaczenieParam, ref TextBoxWyslane);
            /*   }
               catch (Exception)
               {
                   MessageBox.Show("Wprowadź prawidłowe dane", "Coś poszło nie tak");
                   a = false;
               }
               if (!a)//jeśli wszystko poszło ok
               {
                   CheckBoxUstawMaszyne.Checked = false;
               }*/
        }

        private void TextBoxWybierzPlik_TextChanged(object sender, EventArgs e)
        {
            CheckBoxKonwertuj.Checked = false;
        }
        //---------------------------------------------------------------------------//


        //-------------------------------Ładowanie obraazu głównego tabu--------------------------------------------//
        private void TabGlowneMenu_Click(object sender, EventArgs e)
        {
            CheckBoxUstawMaszyne2.Checked = CheckBoxUstawMaszyne.Checked;
            CheckBoxTestujPolaczenie2.Checked = CheckBoxTestujPolaczenie.Checked;
            CheckBoxKonwertuj2.Checked = CheckBoxKonwertuj.Checked;

            LabelGlownyRozmiar.Text = "Rozmiar płytki: ";            // uzupełnianie rozmiaru
            LabelGlownyRozmiar.Text += "  Szer " + TextBoxRozmiarSzer.Text + "mm   Wys " + TextBoxRozmiarWys.Text + "mm";

            pictureGlownyPodglad.ImageLocation = TextBoxWybierzPlik.Text;    // ładowanie obrazka

            int IloscLini = Wczytaj.PomiarIlosciLini(TextBoxWybierzPlik.Text, true); //Potrzebe do obliczenia czasu
            int Szerokosc = Convert.ToInt16(TextBoxRozmiarSzer.Text);

            LabelGlownyIloscLini.Text = "Ilość linii:   ";               // wypisywanie ilości lini
            LabelGlownyIloscLini.Text += Convert.ToString(IloscLini);

            if (Maszyna.PredkoscX != 0)
            {
                int CzasWykonania = 2 * (IloscLini * Szerokosc / Maszyna.PredkoscX);
                LabelGlownyCzasWykonania.Text = "Czas wykonania:  ";
                LabelGlownyCzasWykonania.Text += Convert.ToString(CzasWykonania) + " sekund";
            }
            else
            {
                LabelGlownyCzasWykonania.Text = "Czas wykonania:  Podaj dane maszyny ";
            }

            ProgressBarTransmisja.Maximum = Wczytaj.PomiarIlosciLini(TextBoxWybierzPlik.Text, true);

        }
        //---------------------------------------------------------------------------//

        //---------------------------------Wysyłanie danych do maszyny------------------------------------------//


        public void ButtonRozpocznijTransmisje_Click(object sender, EventArgs e)
        {
            try
            {

                Polaczenie.Wyslij(":5 0 " + GotowyPlik[0] + " #", ref PolaczenieParam, ref TextBoxWyslane);
            }
            catch (Exception)
            {
                MessageBox.Show("Wprowadź dane", "Coś poszło nie tak");
            }
            ProgressBarTransmisja.Value = 0;

        }

        //-------------------------------------------------------------------------//
        //----------------OBSLUGA BAZY DANYCH--------------------------------------//
        //-------------------------------------------------------------------------//
        private void ButtonZaladujMaszyneZDomyslnejBazy_Click(object sender, EventArgs e)
        {
            try
            {
                string sWybranaMaszyna = comboBoxMaszyny.SelectedItem.ToString();//wybiera maszyne z listy
                string[] DaneDoTxtBoxow = new string[9];

                ObslugaBazyClass DaneZBazy = new ObslugaBazyClass();
                //Przepisanie wartosci z bazy do tablicy
                DaneDoTxtBoxow = DaneZBazy.PobierzDaneZBazy(sWybranaMaszyna);//pobiera dane z wiersza wybranego z listy
                //przepisanie do Txt Boxów
                TextBoxRozmiarPlamki.Text = DaneDoTxtBoxow[0];
                TextBoxPredkoscX.Text = DaneDoTxtBoxow[1];
                TextBoxPrzyspieszenieX.Text = DaneDoTxtBoxow[2];
                TextBoxSkokSrubyX.Text = DaneDoTxtBoxow[3];
                TextBoxKrokiX.Text = DaneDoTxtBoxow[4];
                TextBoxPredkoscY.Text = DaneDoTxtBoxow[5];
                TextBoxPrzyspieszenieY.Text = DaneDoTxtBoxow[6];
                TextBoxSkokSrubyY.Text = DaneDoTxtBoxow[7];
                TextBoxKrokiY.Text = DaneDoTxtBoxow[8];
                MessageBox.Show("Udało się pobrać dane z bazy", "Ice Ice Baby", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label37_Click(object sender, EventArgs e)
        {

        }

        private void ButtonSprawdzPolaczenieZBaza_Click(object sender, EventArgs e)
        {
            ObslugaBazyClass SprPol = new ObslugaBazyClass();
            if (SprPol.CheckDbConnection())
                MessageBox.Show("Połączenie z baza jest dostępne", "Ta jeeeeeest", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Nie udało się nawiązać połączenia z bazą. Sprawdź ustawienia", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);


        }
        //-------------Wlaczenie edytora bazy danych-------------------------------//
        private void buttonWlaczEdytorBazy_Click(object sender, EventArgs e)
        {

            var oknoEdytora = new EdycjaBazyForm();
            oknoEdytora.ShowDialog();

        }


        private void comboBoxMaszyny_Click(object sender, EventArgs e)
        {
            try
            {
                comboBoxMaszyny.Items.Clear();//czysci listę
                SqlConnection myConnection = new SqlConnection(connectionString);
                myConnection.Open();//Otwiera połączenie

                SqlDataReader myReader = null;
                SqlCommand selectCommand = new SqlCommand("select * from DaneMaszyny",
                                                       myConnection);
                myReader = selectCommand.ExecuteReader();
                while (myReader.Read())//wczytuje dane
                {
                    string sId = myReader.GetValue(0).ToString();//pobiera dane z 1 kolumny potem
                    if (!comboBoxMaszyny.Items.Contains(sId))//sprawdza czy na liscie nie ma maszyny o takiej nazwie to
                        comboBoxMaszyny.Items.Add(sId);//a nastepnie wrzuca do comboboxa

                }
                myConnection.Close();//zamyka połaczenie
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //-------------------Przeniesienie wyniku konwersji (byte[]) do bazy----------------------------------//

        private void buttonPrzeniesPlikDoBazy_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                ObslugaBazyClass obs = new ObslugaBazyClass();
                //
                //Dodanie skonwertowanej plytki do tabeli PlytkaPCB
                obs.WprowadzPlytkeDoPlytkaPCB(textBox_ID_PlytkiDoTabeli_PlytkiPCB.Text, textBox_Opis_DodanejPlytkiDoTabeli.Text, TextBoxWynikKonw.Text);

            }

            catch (Exception exce)
            {
                MessageBox.Show(exce.ToString());
            }*/
        }
        private void TabUstawieniaKomunikacji_Click(object sender, EventArgs e)
        {

        }




        //-------------------------------------------Ręczne poruszanie maszyną----------------------------//
        private void ButtonGoraY_Click(object sender, EventArgs e)
        {
            Polaczenie.Wyslij(":2 0 1 " + TextBoxReczneLiczbaKrokow.Text + " #", ref PolaczenieParam, ref TextBoxWyslane);
        }

        private void ButtonDolY_Click(object sender, EventArgs e)
        {
            Polaczenie.Wyslij(":2 0 0 " + TextBoxReczneLiczbaKrokow.Text + " #", ref PolaczenieParam, ref TextBoxWyslane);
        }

        private void ButtonPrawoX_Click(object sender, EventArgs e)
        {
            Polaczenie.Wyslij(":2 1 1 " + TextBoxReczneLiczbaKrokow.Text + " #", ref PolaczenieParam, ref TextBoxWyslane);
        }

        private void ButtonLewoX_Click(object sender, EventArgs e)
        {
            Polaczenie.Wyslij(":2 1 0 " + TextBoxReczneLiczbaKrokow.Text + " #", ref PolaczenieParam, ref TextBoxWyslane);
        }

        private void TabGlowny_Click(object sender, EventArgs e)
        {

        }

        private void TabLadowanieObrazu_Click(object sender, EventArgs e)
        {

        }
        //------------------------------------------------------------------------------------------------//
        //                       Aktywacja przycisku "zaladuj do bazy"
        //--------------------------------------------------------------------------------------------------//
        private void textBox_ID_PlytkiDoTabeli_PlytkiPCB_TextChanged(object sender, EventArgs e)
        {
            if (textBox_ID_PlytkiDoTabeli_PlytkiPCB.Text == "" || textBox_ID_PlytkiDoTabeli_PlytkiPCB.Text == null || TextBoxWynikKonw.Text == null || TextBoxWynikKonw.Text == "")

                buttonPrzeniesPlikDoBazy.Enabled = false;
            else
                buttonPrzeniesPlikDoBazy.Enabled = true;

        }

        private void textBox_ID_PlytkiDoTabeli_PlytkiPCB_Click(object sender, EventArgs e)
        {
            if (textBox_ID_PlytkiDoTabeli_PlytkiPCB.Text == "" || textBox_ID_PlytkiDoTabeli_PlytkiPCB.Text == null || TextBoxWynikKonw.Text == null || TextBoxWynikKonw.Text == "")
                buttonPrzeniesPlikDoBazy.Enabled = false;
            else
                buttonPrzeniesPlikDoBazy.Enabled = true;

        }
        //---------------------------------------------------------------------------//

        public void SetCheckboxThread(CheckBox Zmien, bool val)
        {
            if (Zmien.InvokeRequired)
                Zmien.Invoke(new SetCheckboxHander(SetCheckBoxExec), Zmien, val);
            else SetCheckBoxExec(Zmien, val);

        }

        public void SetCheckBoxExec(CheckBox Zmien, bool val)
        {
            Zmien.Checked = val;
        }
    }

    public delegate void SetCheckboxHander(CheckBox Zmien, bool val);

 
}