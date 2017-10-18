using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace NCmachine
{
    public class PolaczenieClass
    {
       public bool Otworz(ref SerialPort PolaczenieParamI, MaszynaClass Maszyna, string WybranyCOM)
        {
           try
            {
               if(!(PolaczenieParamI.IsOpen))
                PolaczenieParamI = new SerialPort(WybranyCOM, 9600, Parity.None, 8, StopBits.One);
                    PolaczenieParamI.Open();
                    return true;
           }
            catch (Exception)
            {
                MessageBox.Show("Port nie został otwarty", "Coś poszło nie tak");
                return false;
            }
        
        }



        public bool Zamknij(ref SerialPort PolaczenieParamI)
        {
            try
            {
                PolaczenieParamI.Dispose();
                PolaczenieParamI.Close();
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Port nie został zamknięty", "Coś poszło nie tak");
                return false;
            }

        }

        public void Wyslij(string Komenda, ref SerialPort PolaczenieParam, ref RichTextBox ZmienWyslane)
        {
            try
            {
                PolaczenieParam.Write(Komenda);
                if(ZmienWyslane.InvokeRequired)
                {
                    ZmienWyslane.Invoke(new ObierzTextHander(OdbierzTextExec), ZmienWyslane, Komenda + System.Environment.NewLine);
                }
                else
                    OdbierzTextExec(ZmienWyslane, Komenda + System.Environment.NewLine);
            /*    ZmienWyslane.Text += Komenda + System.Environment.NewLine;
                ZmienWyslane.SelectionStart = ZmienWyslane.Text.Length;
                ZmienWyslane.ScrollToCaret();*/
            }
            catch(Exception e)
            {
                MessageBox.Show("Połączenie nie jest otwarte\r\n" + e.Message, "Coś poszło nie tak");
            }
        }



        public void Odbierz(string wiadomosc, ref RichTextBox ZmienOdebrane)
        {
            if (ZmienOdebrane.InvokeRequired)
            {
                ZmienOdebrane.Invoke(new ObierzTextHander(OdbierzTextExec), ZmienOdebrane, wiadomosc);
            }
            else
                OdbierzTextExec(ZmienOdebrane, wiadomosc);
            
        }

        public delegate void ObierzTextHander(RichTextBox ZmienOdebrane, string wiadomosc);

        private void OdbierzTextExec(RichTextBox ZmienOdebrane, string wiadomosc)
        {
            ZmienOdebrane.Text += wiadomosc;// +System.Environment.NewLine;
            ZmienOdebrane.SelectionStart = ZmienOdebrane.Text.Length;
            ZmienOdebrane.ScrollToCaret();
        }













         public SerialPort PolaczenieParam { get; set; }

         public SerialPort PolaczenieParamI { get; set; }
    }
}
