using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;//MUSI BYC
using System.Data.Sql;//TO TEZ


namespace NCmachine
{
    public partial class EdycjaBazyForm : Form
    {

        string connectionString =
       "Data Source=(LocalDB)\\v11.0;AttachDbFilename=C:\\Users\\Piotrek\\Documents\\Visual Studio 2013\\Projects\\NC z Bazą\\NCmachine 28.11.14 - Kopia z baza danych\\NCmachine\\BazaParametrow.mdf;Integrated Security=True;Connect Timeout=30";
        ObslugaBazyClass obslBaz = new ObslugaBazyClass();//nowa instancja klasy

        public EdycjaBazyForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonAktualizujMaszyne_Click(object sender, EventArgs e)
        {
            string[] sDaneZEDTxtBoxow = new string[10];
            try
            {
                sDaneZEDTxtBoxow[0] = comboBoxAktualizacjaTabeli.SelectedItem.ToString();//pobranie ID do komendy SQL z comboboxa
                sDaneZEDTxtBoxow[1] = textBoxEDRozmPlamki.Text;
                sDaneZEDTxtBoxow[2] = textBoxEDPredX.Text;
                sDaneZEDTxtBoxow[3] = textBoxEDPrzyspX.Text;
                sDaneZEDTxtBoxow[4] = textBoxEDSkokSrubyX.Text;
                sDaneZEDTxtBoxow[5] = textBoxEDLiczKrX.Text;
                sDaneZEDTxtBoxow[6] = textBoxEDPredY.Text;
                sDaneZEDTxtBoxow[7] = textBoxEDPrzyspY.Text;
                sDaneZEDTxtBoxow[8] = textBoxEDSkokSrubyY.Text;
                sDaneZEDTxtBoxow[9] = textBoxEDLiczKrY.Text;

                ObslugaBazyClass obslBazy = new ObslugaBazyClass();
                obslBazy.AktualizujTabele(sDaneZEDTxtBoxow);
            }
            catch(Exception erakt)
            {
                MessageBox.Show(erakt.ToString());
            }

        }

        private void buttonStworz_Click(object sender, EventArgs e)
        {

            bool emptinessChecker = false;//sprawdza czy ktoryś z elementów nie jest pusty
            string[] sArrWprowadzoneDane = new string[10];//tablica na dane wprowadzone z txtBoxów
            //TU ZMIENIONO
            //sArrWprowadzoneDane[0] = null;
            sArrWprowadzoneDane[0] = textBoxDODId.Text;
            sArrWprowadzoneDane[1] = textBoxDODRozmPlam.Text;
            sArrWprowadzoneDane[2] = textBoxDODPredX.Text;
            sArrWprowadzoneDane[3] = textBoxDODPrzyspX.Text;
            sArrWprowadzoneDane[4] = textBoxDODSkokSrubyX.Text;
            sArrWprowadzoneDane[5] = textBoxDODLiczKrX.Text;
            sArrWprowadzoneDane[6] = textBoxDODPredY.Text;
            sArrWprowadzoneDane[7] = textBoxDODPrzyspY.Text;
            sArrWprowadzoneDane[8] = textBoxDODSkokSrubyY.Text;
            sArrWprowadzoneDane[9] = textBoxDODLiczKrY.Text;

            foreach (string d in sArrWprowadzoneDane)//sprawdzenie czy wpisano wszystkie wartosci
                if ((d == "") || (d == null))
                    emptinessChecker = true;

            if (!emptinessChecker)//jeśli wszystkie pola tekstowe mają jakąś wartość
            {
                try
                {
                    obslBaz.WprowadzNowyWierszDoBazy(sArrWprowadzoneDane);

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString(), "Błąd w formie", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                MessageBox.Show("Wprowadź wszystkie potrzebne wartości", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);


        }

        private void buttonUsunMaszyneZBazy_Click(object sender, EventArgs e)//usuwanie maszyny o danym ID
        {
            ObslugaBazyClass ed = new ObslugaBazyClass();
            try
            {
                string sId = comboBox1.SelectedItem.ToString();
                ed.UsunMaszyneZBazy(sId);//pobiera ID usuwanej bazy z comboboxa
                comboBox1.Items.Clear();//usuwa wszystko z listy, po kliknieciu w nia wpisane zostana istniejace wartosci
                MessageBox.Show("Usunięto z bazy maszynę nr " + sId + ".", "Komunikat");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void EdycjaBazyForm_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'bazaParametrowDataSet.DaneMaszyny' table. You can move, or remove it, as needed.
            this.daneMaszynyTableAdapter.Fill(this.bazaParametrowDataSet.DaneMaszyny);

        }

        
        private void comboBox1_Click(object sender, EventArgs e)
        {
            try
            {
                comboBox1.Items.Clear();//czysci obecna zawartość comboboxa
                SqlConnection myConnection = new SqlConnection(connectionString);
                myConnection.Open();//Otwiera połączenie
                SqlDataReader myReader = null;
                SqlCommand selectCommand = new SqlCommand("select * from DaneMaszyny",
                                                       myConnection);
                myReader = selectCommand.ExecuteReader();
                while (myReader.Read())//wczytuje dane
                {
                    string sId = myReader.GetValue(0).ToString();//pobiera dane z 1 kolumny potem
                    if (!comboBox1.Items.Contains(sId))//sprawdza czy na liscie nie ma maszyny o takiej nazwie to
                        comboBox1.Items.Add(sId);//a nastepnie wrzuca do comboboxa

                }
                myConnection.Close();//zamyka połaczenie
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Błąd");
            }
        }
        //
        //Aktualizacja danych z tabeli
        //
        

        private void comboBoxAktualizacjaTabeli_Click(object sender, EventArgs e)//odswiezenie zawartosci combobox po kliknieciu
        {
            try
            {
                comboBoxAktualizacjaTabeli.Items.Clear();//czysci obecna zawartość comboboxa
                SqlConnection myConnection = new SqlConnection(connectionString);
                myConnection.Open();//Otwiera połączenie
                SqlDataReader myReader = null;
                SqlCommand selectCommand = new SqlCommand("select * from DaneMaszyny",
                                                       myConnection);
                myReader = selectCommand.ExecuteReader();
                while (myReader.Read())//wczytuje dane
                {
                    string sId = myReader.GetValue(0).ToString();//pobiera dane z 1 kolumny potem
                    if (!comboBoxAktualizacjaTabeli.Items.Contains(sId))//sprawdza czy na liscie nie ma maszyny o takiej nazwie to
                        comboBoxAktualizacjaTabeli.Items.Add(sId);//a nastepnie wrzuca do comboboxa

                }
                myConnection.Close();//zamyka połaczenie
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Błąd");
            }
        }

        private void buttonAktualizujPobierz_Click(object sender, EventArgs e)
        {
             try
           { 
           string sWybranaMaszyna = comboBoxAktualizacjaTabeli.SelectedItem.ToString();//wybiera maszyne z listy
           string[] DaneDoTxtBoxow = new string[9];
           
               ObslugaBazyClass DaneZBazy = new ObslugaBazyClass();
               //Przepisanie wartosci z bazy do tablicy
               DaneDoTxtBoxow = DaneZBazy.PobierzDaneZBazy(sWybranaMaszyna);//pobiera dane z wiersza wybranego z listy
               //przepisanie do Txt Boxów
               textBoxEDRozmPlamki.Text = DaneDoTxtBoxow[0];
               textBoxEDPredX.Text = DaneDoTxtBoxow[1];
               textBoxEDPrzyspX.Text = DaneDoTxtBoxow[2];
               textBoxEDSkokSrubyX.Text = DaneDoTxtBoxow[3];
               textBoxEDLiczKrX.Text = DaneDoTxtBoxow[4];
               textBoxEDPredY.Text = DaneDoTxtBoxow[5];
               textBoxEDPrzyspY.Text = DaneDoTxtBoxow[6];
               textBoxEDSkokSrubyY.Text = DaneDoTxtBoxow[7];
               textBoxEDLiczKrY.Text = DaneDoTxtBoxow[8];
               MessageBox.Show("Udało się pobrać dane z bazy", "Ice Ice Baby", MessageBoxButtons.OK, MessageBoxIcon.Information);
           }
           catch (Exception exc)
           {
               MessageBox.Show(exc.ToString(), "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
           }
       }
        

      
    }
}
