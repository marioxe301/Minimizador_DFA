using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimizador
{
    public struct Pares
    {
        public string a { get; set; }
        public string b { get; set; }

        public Pares(string a, string b)
        {
            this.a = a;
            this.b = b;
        }
    }

    public struct Puntos
    {
        public int p1 { get; set; }
        public int p2 { get; set; }

        public Puntos(int p1,int p2)
        {
            this.p1 = p1;
            this.p2 = p2;
        }
    }
    public struct Transiciones
    {
        public int to { get; set; }
        public int from { get; set; }
        public string read { get; set; }

        public Transiciones(int to,int from,string read)
        {
            this.to = to;
            this.from = from;
            this.read = read;
        }
    }

    class Estados
    {
        public int id { get; set; }
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public List<Transiciones> transiciones { get; set; }
        public int position { get; set; }
        public bool inital { get; set; }
        public bool final { get; set; }

        public Estados() { }

        public Estados(int id, string name,double x,double y,bool inital,bool final)
        {
            this.id = id;
            this.name = name;
            this.x = x;
            this.y = y;
            this.inital = inital;
            this.final = final;
            this.transiciones = new List<Transiciones>();
            this.position = 0;
        }

        public void AgregarTransiciones(int to,int from,string read)
        {
            this.transiciones.Add(new Transiciones(to, from, read));
        }



    }
}
