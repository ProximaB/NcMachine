using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCmachine
{
    public class MaszynaClass
    {
        public int BaudRate;
        public double RozmiarPlamki;
        public int PredkoscX;
        public int PrzyspieszenieX;
        public int SkokSrubyX;
        public int LiczbaKrokowX;
        public int PredkoscY;
        public int PrzyspieszenieY;
        public int SkokSrubyY;
        public int LiczbaKrokowY;

        public bool Ustaw(int PrX,int PrzX, int SSX, int LKX, int PrY,int PrzY, int SSY, int LKY, double RP)
        {
            try
            {
                PredkoscX = PrX;
                PrzyspieszenieX = PrzX;
                SkokSrubyX = SSX;
                LiczbaKrokowX = LKX;
                PredkoscY = PrY;
                PrzyspieszenieY = PrzY;
                SkokSrubyY = SSY;
                LiczbaKrokowY = LKY;
                RozmiarPlamki = RP;
                return true;
            }
            catch(Exception)
            {
                MessageBox.Show("Nie załadowano ustawień maszyny", "Coś poszło nie tak");
                return false;
            }
        }

        public MaszynaClass()
        {
            BaudRate = 9600;
            RozmiarPlamki = (float)0.5;
        }

        public MaszynaClass(int baud)
        {
            BaudRate = baud;
        }

    }
}
