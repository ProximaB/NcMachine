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


//Connection String:
//Data Source=(LocalDB)\v11.0;AttachDbFilename="C:\Users\Piotrek\Documents\Visual Studio 2013\Projects\NC z Bazą\NCmachine 28.11.14 - Kopia z baza danych\NCmachine\BazaParametrow.mdf";Integrated Security=True;Connect Timeout=30

namespace NCmachine
{
    public class ObslugaBazyClass
    {
        public string connectionString =
       "Data Source=(LocalDB)\\v11.0;AttachDbFilename=C:\\Users\\Piotrek\\Documents\\Visual Studio 2013\\Projects\\NC z Bazą\\NCmachine 28.11.14 - Kopia z baza danych\\NCmachine\\BazaParametrow.mdf;Integrated Security=True;Connect Timeout=30";        //--------------------------------------------------------------------//
        //---------------Pobieranie danych z bazy----------------------------//
        //-------------------------------------------------------------------//
        public string[] PobierzDaneZBazy(string sWybraneId)//pobiera dane z wybranego wiersza
        {

            SqlConnection myConnection = new SqlConnection(connectionString);

            /*Dane do konwersji
             * int BaudRate, PredkoscX, PrzyspieszenieX, SkokSrubyX, 
             *   LiczbaKrokowX, PredkoscY, PrzyspieszenieY, SkokSrubyY, LiczbaKrokowY;
             * double RozmiarPlamki;*/
            string[] sDanePobrane = new string[9];//tablica string przechowujaca wartosci z bazy, 
            //ktore pozniej zostana przekonwertowane na odpowiedni typ

            myConnection.Open();//Otwiera połączenie

            SqlDataReader myReader = null;
            SqlCommand selectCommand = new SqlCommand("select * from DaneMaszyny WHERE ID=" + sWybraneId,
                                                   myConnection);
            myReader = selectCommand.ExecuteReader();
            while (myReader.Read())//wczytuje dane
            {
                sDanePobrane[0] = (myReader["RozmiarPlamki"].ToString());
                sDanePobrane[1] = (myReader["PredkoscX"].ToString());
                sDanePobrane[2] = (myReader["PrzyspieszenieX"].ToString());
                sDanePobrane[3] = (myReader["SkokSrubyX"].ToString());
                sDanePobrane[4] = (myReader["LiczbaKrokowX"].ToString());
                sDanePobrane[5] = (myReader["PredkoscY"].ToString());
                sDanePobrane[6] = (myReader["PrzyspieszenieY"].ToString());
                sDanePobrane[7] = (myReader["SkokSrubyY"].ToString());
                sDanePobrane[8] = (myReader["LiczbaKrokowY"].ToString());

            }
            myConnection.Close();//zamyka połączenie

            return sDanePobrane;

        }
        //
        //Sprawdzanie połączenia
        //
        public bool CheckDbConnection()//funkcja zwraca true jesli polaczono z baza oraz false jesli nie
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))//bierze glownego connection stringa
                {
                    connection.Open();
                    connection.Close();
                    return true;

                }
            }
            catch (Exception)
            {
                return false; // any error is considered as db connection error for now
            }
        }

        public void WprowadzNowyWierszDoBazy(string[] sArrWithDataFromTxtBoxes)//wprowadza nową maszynę do bazy (nowy wiersz)
        {
            using (SqlConnection localconnection = new SqlConnection(connectionString))
            {

                //Sposoby dodawania są równoważne, dodaje w taki sam sposób CHYBA
                //-----------1-szy sposób------------------------//

                SqlCommand cmd = new SqlCommand(
"INSERT INTO [dbo].[DaneMaszyny] ([Id], [RozmiarPlamki], [PredkoscX], [PrzyspieszenieX], [SkokSrubyX], [LiczbaKrokowX], [PredkoscY], [PrzyspieszenieY], [SkokSrubyY], [LiczbaKrokowY]) VALUES (@Id, @RozmiarPlamki, @PredkoscX, @PrzyspieszenieX, @SkokSrubyX, @LiczbaKrokowX, @PredkoscY, @PrzyspieszenieY, @SkokSrubyY, @LiczbaKrokowY)", localconnection);

                cmd.CommandType = CommandType.Text;//tekstowa komenda SQL                   

                cmd.Connection.Open();

                cmd.Parameters.AddWithValue("@Id", sArrWithDataFromTxtBoxes[0]);
                cmd.Parameters.AddWithValue("@RozmiarPlamki", sArrWithDataFromTxtBoxes[1]);
                cmd.Parameters.AddWithValue("@PredkoscX", sArrWithDataFromTxtBoxes[2]);
                cmd.Parameters.AddWithValue("@PrzyspieszenieX", sArrWithDataFromTxtBoxes[3]);
                cmd.Parameters.AddWithValue("@SkokSrubyX", sArrWithDataFromTxtBoxes[4]);
                cmd.Parameters.AddWithValue("@LiczbaKrokowX", sArrWithDataFromTxtBoxes[5]);
                cmd.Parameters.AddWithValue("@PredkoscY", sArrWithDataFromTxtBoxes[6]);
                cmd.Parameters.AddWithValue("@PrzyspieszenieY", sArrWithDataFromTxtBoxes[7]);
                cmd.Parameters.AddWithValue("@SkokSrubyY", sArrWithDataFromTxtBoxes[8]);
                cmd.Parameters.AddWithValue("@LiczbaKrokowY", sArrWithDataFromTxtBoxes[9]);

                //cmd.ExecuteNonQuery();
                MessageBox.Show(Convert.ToString(cmd.ExecuteNonQuery()), "Info z metody");

                //TU BŁĄD!!!
                //nie można użyć teog message boxa bo wykona ExecuteNonQuery 2 raz
                //MessageBox.Show(Convert.ToString(cmd.ExecuteNonQuery()), "Info z metody");//wgląd na wynik dzialania execute'a 
                cmd.Connection.Close();

                // */


                //
                //2-gi sposób
                //
                /*
SqlCommand ustaw = new SqlCommand(
"INSERT INTO [dbo].[DaneMaszyny] VALUES (@Id, @RozmiarPlamki, @PredkoscX, @PrzyspieszenieX, @SkokSrubyX, @LiczbaKrokowX, @PredkoscY, @PrzyspieszenieY, @SkokSrubyY, @LiczbaKrokowY);", localconnection); //komenda tworzenia nowego wiersza
                                  
                   
                                  
                ustaw.Connection.Open();
                     
                SqlParameter prm = new SqlParameter("@Id", sArrWithDataFromTxtBoxes[0]);
                SqlParameter prm2 = new SqlParameter("@RozmiarPlamki", sArrWithDataFromTxtBoxes[1]);
                SqlParameter prm3 = new SqlParameter("@PredkoscX", sArrWithDataFromTxtBoxes[2]);
                SqlParameter prm4 = new SqlParameter("@PrzyspieszenieX", sArrWithDataFromTxtBoxes[3]);
                SqlParameter prm5 = new SqlParameter("@SkokSrubyX", sArrWithDataFromTxtBoxes[4]);
                SqlParameter prm6 = new SqlParameter("@LiczbaKrokowX", sArrWithDataFromTxtBoxes[5]);
                SqlParameter prm7 = new SqlParameter("@PredkoscY", sArrWithDataFromTxtBoxes[6]);
                SqlParameter prm8 = new SqlParameter("@PrzyspieszenieY", sArrWithDataFromTxtBoxes[7]);
                SqlParameter prm9 = new SqlParameter("@SkokSrubyY", sArrWithDataFromTxtBoxes[8]);
                SqlParameter prm10 = new SqlParameter("@LiczbaKrokowY", sArrWithDataFromTxtBoxes[9]);

                ustaw.Parameters.Add(prm);
                ustaw.Parameters.Add(prm2);
                ustaw.Parameters.Add(prm3);
                ustaw.Parameters.Add(prm4);
                ustaw.Parameters.Add(prm5);
                ustaw.Parameters.Add(prm6);
                ustaw.Parameters.Add(prm7);
                ustaw.Parameters.Add(prm8);
                ustaw.Parameters.Add(prm9);
                ustaw.Parameters.Add(prm10);
                    
                ustaw.ExecuteNonQuery();
                
                MessageBox.Show(Convert.ToString(ustaw.ExecuteNonQuery()),"Info z metody");//wgląd na wynik dzialania execute'a 
                    
                ustaw.Connection.Close();
                 */

                MessageBox.Show("Dodano maszynę do bazy", "Info z metody");


            }
        }//end of WprowadzNowyWierszDoBazy

        //
        //Usuwanie maszyny z bazy
        //
        public void UsunMaszyneZBazy(string Id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[DaneMaszyny] WHERE Id = @IdNumber;", connection);

                cmd.Parameters.AddWithValue("@IDNumber", Id);//Id musi byc string
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
        }

        public void AktualizujTabele(string[] sArrWithDataFromTxtBoxes)//metoda aktualizuje dane w istniejacym wierszu, wrzuca dane z tabeli sDane
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //komenda uaktualniająca bazę
                    SqlCommand cmd = new SqlCommand(
    "UPDATE [dbo].[DaneMaszyny] SET RozmiarPlamki=@RozmiarPlamki, PredkoscX=@PredkoscX, PrzyspieszenieX=@PrzyspieszenieX,SkokSrubyX=@SkokSrubyX, LiczbaKrokowX=@LiczbaKrokowX, PredkoscY=@PredkoscY, PrzyspieszenieY=@PrzyspieszenieY, SkokSrubyY=@SkokSrubyY, LiczbaKrokowY=@LiczbaKrokowY WHERE Id=@Id", connection);

                    cmd.CommandType = CommandType.Text;//tekstowa komenda SQL                  

                    cmd.Connection.Open();


                    cmd.Parameters.AddWithValue("@Id", sArrWithDataFromTxtBoxes[0]);
                    cmd.Parameters.AddWithValue("@RozmiarPlamki", sArrWithDataFromTxtBoxes[1]);
                    cmd.Parameters.AddWithValue("@PredkoscX", sArrWithDataFromTxtBoxes[2]);
                    cmd.Parameters.AddWithValue("@PrzyspieszenieX", sArrWithDataFromTxtBoxes[3]);
                    cmd.Parameters.AddWithValue("@SkokSrubyX", sArrWithDataFromTxtBoxes[4]);
                    cmd.Parameters.AddWithValue("@LiczbaKrokowX", sArrWithDataFromTxtBoxes[5]);
                    cmd.Parameters.AddWithValue("@PredkoscY", sArrWithDataFromTxtBoxes[6]);
                    cmd.Parameters.AddWithValue("@PrzyspieszenieY", sArrWithDataFromTxtBoxes[7]);
                    cmd.Parameters.AddWithValue("@SkokSrubyY", sArrWithDataFromTxtBoxes[8]);
                    cmd.Parameters.AddWithValue("@LiczbaKrokowY", sArrWithDataFromTxtBoxes[9]);

                    cmd.ExecuteNonQuery();//wykonanie polecenia
                    //
                    //aktywuj poniższe jeśli chcesz wiedzieć co się dzieje w ExecuteNonQuery
                    //
                    //MessageBox.Show(Convert.ToString(cmd.ExecuteNonQuery()), "Info z metody");//tutaj wykonuje
                    cmd.Connection.Close();
                    MessageBox.Show("Zaktualizowano maszynę nr" + sArrWithDataFromTxtBoxes[0]);
                }
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.ToString());
            }





        }
        public void WprowadzPlytkeDoPlytkaPCB(string id, string opis, string zawartosc)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                
                
                SqlCommand cmd = new SqlCommand(
"INSERT INTO [dbo].[PlytkaPCB] ([Id_plyt],[Dane],[Opis]) VALUES (@Id, @Dane, @Opis);", connection);
                cmd.CommandType = CommandType.Text;//tekstowa komenda SQL                  
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Opis", opis);
                cmd.Parameters.AddWithValue("@Dane", zawartosc);
                cmd.Connection.Open();
                MessageBox.Show(Convert.ToString(cmd.ExecuteNonQuery()), "Info z metody");
                cmd.Connection.Close();
            }
        }

    }//end of ObslugaBazyClass
}

