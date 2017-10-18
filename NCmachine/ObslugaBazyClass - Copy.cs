using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;//MUSI BYC
using System.Data.Sql;//TO TEZ


//Connection String:
//Data Source=(LocalDB)\v11.0;AttachDbFilename=|DataDirectory|\MyLocalDatabase.mdf;Integrated Security=True

namespace NCmachine
{
    public class ObslugaBazyClass
    {
        string connectionString =
            "Data Source=(LocalDB)\\v11.0;AttachDbFilename=|DataDirectory|\\MyLocalDatabase.mdf;Integrated Security=True";
       
        public string[] PobierzDaneZBazy()//pobiera dane z domyslnej bazy
        {

            SqlConnection myConnection = new SqlConnection(connectionString);

            int BaudRate, PredkoscX, PrzyspieszenieX, SkokSrubyX, 
                LiczbaKrokowX, PredkoscY, PrzyspieszenieY, SkokSrubyY, LiczbaKrokowY;
            double RozmiarPlamki;
            string[] sDanePobrane=new string[9];//tablica string przechowujaca wartosci z bazy, 
            //ktore pozniej zostana przekonwertowane na odpowiedni typ

            myConnection.Open();//Otwiera połączenie

             SqlDataReader myReader = null;
             SqlCommand selectCommand = new SqlCommand("select * from DaneMaszyny",
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

                BaudRate = Convert.ToInt16(sDanePobrane[0]);
                RozmiarPlamki = Convert.ToDouble(sDanePobrane[1]);
                PredkoscX = Convert.ToInt16(sDanePobrane[2]);
                PrzyspieszenieX = Convert.ToInt32(sDanePobrane[3]);
                SkokSrubyX = Convert.ToInt16(sDanePobrane[4]);
                LiczbaKrokowX = Convert.ToInt16(sDanePobrane[5]);
                PredkoscY = Convert.ToInt16(sDanePobrane[6]);
                PrzyspieszenieY = Convert.ToInt16(sDanePobrane[7]);
                SkokSrubyY = Convert.ToInt16(sDanePobrane[8]);
                LiczbaKrokowY = Convert.ToInt16(sDanePobrane[9]);

                return sDanePobrane;
            
        }

        public bool SprawdzPolaczenieZBaza()//funkcja zwraca true jesli polaczono z baza oraz false jesli nie
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
            catch (Exception e)
            {
                return false; // any error is considered as db connection error for now
            }
        }
        /*public string[] PobierzDaneZWYBRANEJBazy()
        {

            SqlConnection myConnection = new SqlConnection(connectionString);

            /*int BaudRate, PredkoscX, PrzyspieszenieX, SkokSrubyX,
                LiczbaKrokowX, PredkoscY, PrzyspieszenieY, SkokSrubyY, LiczbaKrokowY;
            double RozmiarPlamki;
            string[] sDanePobrane = new string[10];//tablica string przechowujaca wartosci z bazy, 
            //ktore pozniej zostana przekonwertowane na odpowiedni typ

            myConnection.Open();//Otwiera połączenie
            SqlDataReader myReader = null;
            SqlCommand selectCommand = new SqlCommand("select * from DaneMaszyny",
                                                   myConnection);
            myReader = selectCommand.ExecuteReader();
            while (myReader.Read())//wczytuje dane
            {
                sDanePobrane[0] = (myReader["BaudRate"].ToString());
                sDanePobrane[1] = (myReader["RozmiarPlamki"].ToString());
                sDanePobrane[2] = (myReader["PredkoscX"].ToString());
                sDanePobrane[3] = (myReader["PrzyspieszenieX"].ToString());
                sDanePobrane[4] = (myReader["SkokSrubyX"].ToString());
                sDanePobrane[5] = (myReader["LiczbaKrokowX"].ToString());
                sDanePobrane[6] = (myReader["PredkoscY"].ToString());
                sDanePobrane[7] = (myReader["PrzyspieszenieY"].ToString());
                sDanePobrane[8] = (myReader["SkokSrubyY"].ToString());
                sDanePobrane[9] = (myReader["LiczbaKrokowY"].ToString());

            }
            myConnection.Close();//zamyka połączenie
            return sDanePobrane;

        }*/

        public void WprowadzNowyWierszDoBazy()
        {

            SqlConnection myConnection = new SqlConnection(connectionString);

            //string[] sDanePobrane = new string[10];//tablica string przechowujaca wartosci z bazy, 
            //ktore pozniej zostana przekonwertowane na odpowiedni typ
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
              SqlCommand InstertCommand = new SqlCommand
                ("INSTERT INTO DaneMaszyny (Id,BaudRate,RozmiarPlamki,PredkoscX,PrzyspieszenieX,SkokSrubyX,LiczbaKrokowX,PredkoscY,PrzyspieszenieY,SkokSrubyY,LiczbaKrokowY) VALUES (2,19200, 0.5, 200,200, 200,200,100,100,100,100);",
                               myConnection);
            


            myConnection.Open();//Otwiera połączenie
            //SqlDataReader myReader = null;
          //myReader = selectCommand.ExecuteReader();
            myConnection.Close();//zamyka połączenie
        }
    }
}
