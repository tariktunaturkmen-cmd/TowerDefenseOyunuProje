using System.Drawing;

namespace TowerDefenseOyunuProje
{
    // ÝNÞA ALANI SINIFI: Sabit inþaat alanlarýný temsil eder
    // Bu alanlar forma týklandýðýnda kule eklemeyi saðlar
    public class InsaAlani
    {
        // KAPSÜLLEME: Private deðiþkenler, Property ile dýþarý açýlýyor
        private int _x;
        private int _y;
        private int _genislik = 50;
        private int _yukseklik = 50;
        private bool _doluMu = false;
        private Kule _yerlesenKule = null;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public int Genislik
        {
            get { return _genislik; }
        }

        public int Yukseklik
        {
            get { return _yukseklik; }
        }

        public bool DoluMu
        {
            get { return _doluMu; }
            set { _doluMu = value; }
        }

        public Kule YerlesenKule
        {
            get { return _yerlesenKule; }
            set { _yerlesenKule = value; }
        }

        // Constructor: X ve Y koordinatlarýyla inþa alaný oluþturur
        public InsaAlani(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Bu alana týklandý mý kontrolü
        public bool TiklanaYildimi(int mouseX, int mouseY)
        {
            return mouseX >= X && mouseX <= X + Genislik &&
                   mouseY >= Y && mouseY <= Y + Yukseklik;
        }

        // Alanýn orta noktasýný döndür (kule burada yerleþecek)
        public Point OrtaNokta()
        {
            return new Point(X + Genislik / 2, Y + Yukseklik / 2);
        }
    }
}
