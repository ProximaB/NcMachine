using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace NCmachine
{
    public class WczytywanieObrazu
    {
        public void BmpTo01Arr(string loadedFileDirectory, string savedFileDirectory)//method loads bmp file from directory and converts it into array of 0s and 1s and saves it into specified directory txt file
        {
            Bitmap bmp = new Bitmap(loadedFileDirectory); //loads bitmap
            //short[,] array = new short[bmp.Width, bmp.Height];//creates array with the same size as bmp file CHYBA NIEPOTRZEBNE
            Color color;//used to check pixel color
            int binaryVal = 0;//used to identify pixel 
            string str01ArrayFromBmp = string.Empty;//creates empty string
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {

                    color = bmp.GetPixel(x, y);
                    binaryVal = Color.White.ToArgb() == color.ToArgb() ? (short)0 : (short)1; // 0 - WHITE 1 - BLACK
                    str01ArrayFromBmp += binaryVal.ToString();//saves to string

                }

                str01ArrayFromBmp += Environment.NewLine;//after finishing reading raw go to the next line of txt file

            }

            File.WriteAllText(savedFileDirectory, str01ArrayFromBmp);//saves file into direcotry specified in patchToFile
        }

        public int PomiarDpi(string loadedFileDirectory, int szer)
        {
            double RozmiarInch = (double)(Convert.ToDouble(szer) / 25.4);
            Bitmap bmp = new Bitmap(loadedFileDirectory);
            //System.Windows.Forms.TextBox TextBoxRozmiarSzer;
            return (Convert.ToInt16(bmp.Width / RozmiarInch));
        }
        //dla a=FALSE rozmiar X - szerokość, dla a=TRUE rozmiar Y - wysokość
        public int PomiarIlosciLini(string loadedFileDirectory, bool a)
        {
            Bitmap bmp = new Bitmap(loadedFileDirectory);
            if (a)
                return bmp.Height;
            else
                return bmp.Width;
        }


        //----------------------------------Tworzy tablice 0 i 1 i odwraca kolejność co drugą linijkę-------------------//
        public void BmpTo01ArrWithInversion(string loadedFileDirectory, string savedFileDirectory)//metoda laduje plik bmp z miejsca loadedFileDirectory 
        {
            bool flag = true;
            Bitmap bmp = new Bitmap(loadedFileDirectory); //loads bitmap

            Color color;//used to check pixel color
            int binaryVal = 0;//used to identify pixel 
            string str01ArrayFromBmp = string.Empty;//creates empty string
            for (int y = 0; y < bmp.Height; y++)
            {
                if (flag)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {

                        color = bmp.GetPixel(x, y);
                        binaryVal = Color.White.ToArgb() == color.ToArgb() ? (short)0 : (short)1; // 0 - WHITE 1 - BLACK
                        str01ArrayFromBmp += binaryVal.ToString();//saves to string
                        flag = false; //(flag == false) ? true : false;//inverting flag
                    }
                }

                else
                {
                    for (int x = bmp.Width - 1; x >= 0; x--)
                    {

                        color = bmp.GetPixel(x, y);
                        binaryVal = Color.White.ToArgb() == color.ToArgb() ? (short)0 : (short)1; // 0 - WHITE 1 - BLACK
                        str01ArrayFromBmp += binaryVal.ToString();//saves to string
                        flag = true; // (flag == false) ? true : false;//inverting flag
                    }
                }//end of IF


                str01ArrayFromBmp += Environment.NewLine;//after finishing reading raw go to the next line of txt file

            }//end of FORS

            File.WriteAllText(savedFileDirectory, str01ArrayFromBmp);//saves file into direcotry specified in patchToFile

        }
        //--------------------------------------------------------------------------------------------------------------//

        //----------------------------------------Zamienia 0 i 1 na liczby całkowite (zlicza)----------------------------------------------------------------------//
        public void CountingZerosAndOnes(string loadedFileDirectory, string savedFileDirectory, ref RichTextBox zmien)
        {
            using (StreamReader sr = new StreamReader(@loadedFileDirectory))//loading txt file to stream
            {
                char[] suma = new char[0];
                int LicznikJedynek = 0;
                int LicznikZer = 0;
                using (StreamWriter sw = new StreamWriter(@savedFileDirectory))  //stream zapisywania, zapisujemy co linijkę
                {
                    while (!(sr.EndOfStream))
                    {
                        String strFileFromTxt = sr.ReadLine();
                        string LiniaGotowa;
                        LiniaGotowa = null;                                                 // zmienna string przechowująca kolejne linie
                        char[] c = strFileFromTxt.ToCharArray();
                        if (c[0] == '0') LiniaGotowa += "0 ";
                        for (int i = 0; i < strFileFromTxt.Length; i++)
                        {
                            //dostajemy jedynke
                            if (c[i] == '1')
                            {
                                LicznikJedynek++;
                            }
                            //dostajemy zero
                            if (c[i] == '0')
                            {
                                LicznikZer++;
                            }
                            if (((i < strFileFromTxt.Length - 1) && (c[i] != c[i + 1])) || (i == strFileFromTxt.Length - 1))  //instrukcja aktualizująca zmienną Liniagotowa w razie zmiany cyfry lub końca linii
                            {
                                if (c[i] == '1') LiniaGotowa += LicznikJedynek.ToString() + " ";
                                else if (c[i] == '0') LiniaGotowa += LicznikZer.ToString() + " ";
                                LicznikJedynek = 0;
                                LicznikZer = 0;
                            }
                        }
                        sw.WriteLine(LiniaGotowa);                                  //sam zapis do pliku
                        zmien.Text += LiniaGotowa + System.Environment.NewLine;     //wyśiwetlanie w boxie
                    }
                    sw.Close();
                }
                sr.Close();
            }
        }
        //--------------------------------------------------------------------------------------------------------------//

        /*  public char[] ZaladujDoTablicy(string loadedFileDirectory)
          {
              char[,] TablicaZnaków = new char[0,0];
              using (StreamReader sr = new StreamReader(loadedFileDirectory))
              {
                  while (!(sr.EndOfStream))
                  { 

                  }
                  sr.Close();
              }

              return TablicaZnaków;
          }
          */

        public void Konwersja_obrazu(string loadedFileDirectory, string savedFileDirectory, ref RichTextBox zmien) //metoda laduje plik bmp z miejsca loadedFileDirectory 
        {
            bool flag = true;
            Bitmap bmp = new Bitmap(loadedFileDirectory); //loads bitmap

            using (StreamWriter sw = new StreamWriter(@savedFileDirectory))  //stream zapisywania, zapisujemy co linijkę
            {
                Color color;//used to check pixel color
                int binaryVal = 0;//used to identify pixel 
                string str01ArrayFromBmp = string.Empty;//creates empty string
                for (int y = 0; y < bmp.Height; y++)
                {
                    str01ArrayFromBmp = string.Empty;
                    if (flag)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            color = bmp.GetPixel(x, y);
                            binaryVal = Color.White.ToArgb() == color.ToArgb() ? (short)0 : (short)1; // 0 - WHITE 1 - BLACK
                            str01ArrayFromBmp += binaryVal.ToString();//saves to string
                            flag = false; //(flag == false) ? true : false;//inverting flag
                        }
                    }
                    else
                    {
                        for (int x = bmp.Width - 1; x >= 0; x--)
                        {

                            color = bmp.GetPixel(x, y);
                            binaryVal = Color.White.ToArgb() == color.ToArgb() ? (short)0 : (short)1; // 0 - WHITE 1 - BLACK
                            str01ArrayFromBmp += binaryVal.ToString();//saves to string
                            flag = true; // (flag == false) ? true : false;//inverting flag
                        }
                    }//end of IF

                    char[] suma = new char[0];
                    int LicznikJedynek = 0;
                    int LicznikZer = 0;
                    string strFileFromTxt = str01ArrayFromBmp;
                    string LiniaGotowa;
                    LiniaGotowa = null;                                                 // zmienna string przechowująca kolejne linie
                    char[] c = strFileFromTxt.ToCharArray();
                    if (c[0] == '0') LiniaGotowa += "0 ";
                    for (int i = 0; i < strFileFromTxt.Length; i++)
                    {
                        //dostajemy jedynke
                        if (c[i] == '1')
                        {
                            LicznikJedynek++;
                        }
                        //dostajemy zero
                        if (c[i] == '0')
                        {
                            LicznikZer++;
                        }
                        if (((i < strFileFromTxt.Length - 1) && (c[i] != c[i + 1])) || (i == strFileFromTxt.Length - 1))  //instrukcja aktualizująca zmienną Liniagotowa w razie zmiany cyfry lub końca linii
                        {
                            if (c[i] == '1') LiniaGotowa += LicznikJedynek.ToString() + " ";
                            else if (c[i] == '0') LiniaGotowa += LicznikZer.ToString() + " ";
                            LicznikJedynek = 0;
                            LicznikZer = 0;
                        }
                    }
                    sw.WriteLine(LiniaGotowa);                                  //sam zapis do pliku
                    zmien.Text += LiniaGotowa + System.Environment.NewLine;     //wyśiwetlanie w boxie


                }//end of FORS
                sw.Close();
            }   //end of streamwriter


        }









    }
}
