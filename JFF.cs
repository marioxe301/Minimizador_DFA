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
        public bool[][] matriz;
        public List<Estados> estadosTemp;

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
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(this.path);

                XmlNodeList _estadostemp = doc.DocumentElement.SelectNodes("/structure/automaton/state");

                foreach (XmlNode _estados in _estadostemp)
                {
                    bool final = false, inicial = false;
                    int sizeChilds = _estados.ChildNodes.Count;

                    if (sizeChilds==6)
                    {
                        if (_estados.ChildNodes[5].Name == "initial")
                        {
                            inicial = true;
                        }
                        else if (_estados.ChildNodes[5].Name == "final")
                        {
                            final = true;
                        }
                    }


                    Estados tmp = new Estados(Convert.ToInt32(_estados.Attributes["id"].Value)
                    , _estados.Attributes["name"].Value, Convert.ToDouble(_estados.SelectSingleNode("x").InnerText)
                    , Convert.ToDouble(_estados.SelectSingleNode("y").InnerText), inicial, final);

                    this.estados.Add(tmp);
                }
            }catch(Exception e) { Console.WriteLine(e.Message); }
        }

        public void CargarTransiciones()
        {
            try
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
            }catch(Exception e) { Console.WriteLine(e.Message); }
        }

        public void CargarAlfabeto()
        {
            try
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
            }catch(Exception e) { Console.WriteLine(e.Message); }
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

        //agregar mas casos
        public bool esDFA()
        {
            bool check = false;
            for(int i = 0; i < this.estados.Count; i++)
            {
                if(this.estados[i].transiciones.Count == this.alfabeto.Count)
                {
                    check = true;
                }
                else
                {
                    check = false;
                }
            }

            return check;
        }

        public void minimizarDFA(string directory)
        {
            List<Estados> tempRef1 = this.estados;
            List<Estados> tempRef2 = this.estados;

            tempRef1.RemoveAt(0);
            tempRef2.RemoveAt(tempRef2.Count - 1);

            enumerarEstados(tempRef1);
            enumerarEstados(tempRef2);

            //eliminar los estados inalcanzables
            eliminarEstadosInalcanzables();
            //marcar los distingubles No final con Final (NF,F)

            //comparaciones con pares

            //concatenacion de estados

            //generador de nuevo objeto estados
            
            //generador de archivo XML
        }

        public void eliminarEstadosInalcanzables()
        {
            List<bool> MarcarEstados = new List<bool>();

            int size = this.estados.Count;
            
            //matriz para verificar la cantidad de estados y si son alcanzables
            for(int i = 0; i < size; i++)
            {
                MarcarEstados.Add(false);
            }

            //verificar los estados

            for(int i = 0; i < this.estados.Count; i++)
            {
                for(int j = 0; j < this.estados[i].transiciones.Count; j++)
                {
                    int pos = buscarIndexPorId(this.estados[i].transiciones[j].to);
                    MarcarEstados[pos] = true;
                }
            }
            //remueve los estados inalcanzables
            for(int i = 0; i < MarcarEstados.Count; i++)
            {
                if (!MarcarEstados[i])
                {
                    this.estados.RemoveAt(i);
                }
            }

        }

        public int buscarIndexPorId(int id)
        {
            for(int i = 0; i < this.estados.Count; i++)
            {
                if(this.estados[i].id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public void enumerarEstados(List<Estados>lista)
        {
            for(int i = 0; i < lista.Count; i++)
            {
                lista[i].position = i;
            }
        }

        
    }
}
