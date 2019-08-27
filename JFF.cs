using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Minimizador
{
    
    class JFF
    {
        public string path { get; set; }
        public List<Estados> estados;
        public List<string> alfabeto;


        public JFF(string path)
        {
            this.path = path;
            this.estados = new List<Estados>();
            this.alfabeto = new List<string>();
            Inicializar();
        }

        public void Inicializar()
        {
            CargarEstados();
            CargarTransiciones();
            CargarAlfabeto();
        }

        public void CargarEstados()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.path);

            XmlNodeList _estadostemp = doc.DocumentElement.SelectNodes("/structure/automaton/state");

            foreach(XmlNode _estados in _estadostemp)
            {
                bool final=false, inicial=false;

                if (_estados.ChildNodes[5].Name == "initial"){
                    inicial = true;   
                }else if (_estados.ChildNodes[5].Name == "final"){
                    final = true;
                }

              
                    Estados tmp = new Estados(Convert.ToInt32(_estados.Attributes["id"].Value)
                    , _estados.Attributes["name"].Value, Convert.ToDouble(_estados.SelectSingleNode("x").InnerText)
                    , Convert.ToDouble(_estados.SelectSingleNode("y").InnerText),inicial,final);

                this.estados.Add(tmp);  
            }
        }

        public void CargarTransiciones()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.path);

            XmlNodeList _transtemp = doc.DocumentElement.SelectNodes("/structure/automaton/transition");

            foreach (XmlNode _transicion in _transtemp)
            {
                int pos = ObtenerPoscicion(Convert.ToInt32(_transicion.SelectSingleNode("from").InnerText));
                int from = Convert.ToInt32(_transicion.SelectSingleNode("from").InnerText);
                int to = Convert.ToInt32(_transicion.SelectSingleNode("to").InnerText);
                string read = _transicion.SelectSingleNode("read").InnerText;

                this.estados[pos].AgregarTransiciones(to, from, read);
            }
        }

        public void CargarAlfabeto()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this.path);

            XmlNodeList _transtemp = doc.DocumentElement.SelectNodes("/structure/automaton/transition");

            foreach (XmlNode _transicion in _transtemp)
            {
                string read = _transicion.SelectSingleNode("read").InnerText;

                if (!ExisteAlfabeto(read))
                {
                    this.alfabeto.Add(read);
                }
            }
        }

        public bool ExisteAlfabeto(string read)
        {
            for(int i = 0; i < this.alfabeto.Count; i++)
            {
                if(this.alfabeto[i] == read)
                {
                    return true;
                }
            }

            return false;
        }

        public int ObtenerPoscicion(int id)
        {
            for(int i = 0;i< this.estados.Count; i++)
            {
                if(this.estados[i].id == id)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
