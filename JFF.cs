using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(this.path);

                XmlNodeList _estadostemp = doc.DocumentElement.SelectNodes("/structure/automaton/state");

                foreach (XmlNode _estados in _estadostemp)
                {
                    bool final = false, inicial = false;
                    int sizeChilds = _estados.ChildNodes.Count;

                    if (sizeChilds == 7)
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
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
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
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
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
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        public bool ExisteAlfabeto(string read)
        {
            for (int i = 0; i < this.alfabeto.Count; i++)
            {
                if (this.alfabeto[i] == read)
                {
                    return true;
                }
            }

            return false;
        }

        public int ObtenerPoscicion(int id)
        {
            for (int i = 0; i < this.estados.Count; i++)
            {
                if (this.estados[i].id == id)
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
            for (int i = 0; i < this.estados.Count; i++)
            {
                if (this.estados[i].transiciones.Count == this.alfabeto.Count)
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
            try
            {
                //eliminar los estados inalcanzables
                eliminarEstadosInalcanzables();

               /* bool[][] tabla = new bool[this.estados.Count - 1][];

                for(int i = 0; i<this.estados.Count - 1; i++)
                {
                    tabla[i] = new bool[i + 1];
                }

                List<Puntos> puntos = new List<Puntos>();
                List<Pares> pares = new List<Pares>();
                List<Estados> E1 = this.estados;
                List<Estados> E2 = this.estados;

                E1.RemoveAt(0);
                E2.RemoveAt(E2.Count - 1);


                enumerarEstados(E1);
                enumerarEstados(E2);

                //marcar estados distinguibles
                marcarEstadosDistinguibles(tabla, E1, E2);

                //optener los puntos de los estados no marcados
                puntos = obtenerPares(tabla, E1, E2);

                //otener todos los pares con el alfabeto
                pares = obtenerParesconTransciciones(puntos, E1, E2);

                //marcar los que tiene consigo un par deterministico al lado


                                */   
                


            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void eliminarEstadosInalcanzables()
        {
            List<bool> MarcarEstados = new List<bool>();

            int size = this.estados.Count;

            //matriz para verificar la cantidad de estados y si son alcanzables
            for (int i = 0; i < size; i++)
            {
                MarcarEstados.Add(false);
            }

            //verificar los estados

            for (int i = 0; i < this.estados.Count; i++)
            {
                for (int j = 0; j < this.estados[i].transiciones.Count; j++)
                {
                    if (this.estados[i].id != this.estados[i].transiciones[j].to) {
                        int pos = buscarIndexPorId(this.estados[i].transiciones[j].to);
                        MarcarEstados[pos] = true;
                    }else if((this.estados[i].id == this.estados[i].transiciones[j].to) && this.estados[i].inital)
                    {
                        int pos = buscarIndexPorId(this.estados[i].transiciones[j].to);
                        MarcarEstados[pos] = true;
                    }
                    
                }
            }
            //remueve los estados inalcanzables
            for (int i = 0; i < MarcarEstados.Count; i++)
            {
                if (!MarcarEstados[i])
                {
                    this.estados.RemoveAt(i);
                }
            }

        }

        public int buscarIndexPorId(int id)
        {
            for (int i = 0; i < this.estados.Count; i++)
            {
                if (this.estados[i].id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public void enumerarEstados(List<Estados> lista)
        {
            for (int i = 0; i < lista.Count; i++)
            {
                lista[i].position = i;
            }
        }


        public void marcarEstadosDistinguibles(bool[][]matriz,List<Estados>est1, List<Estados> est2)
        {
            for(int i = 0; i < est2.Count; i++)
            {
                for(int j = 0; j < est1.Count; j++)
                {
                    if(esDistinguible(est1[j],est2[i]))
                    {
                        matriz[est1[j].position][est2[i].position] = true;
                    }
                }
            }
        }

         public bool esDistinguible(Estados est1,Estados est2)
         {
             if((est1.final && est2.final==false) || (est1.final==false && est2.final))
             {
                 return true;
             }
             else
             {
                 return false;
             }
         }

         public List<Puntos> obtenerPares(bool[][] matriz, List<Estados> est1, List<Estados> est2)
         {
             List<Puntos> listaPares = new List<Puntos>();
            

             for (int i = 0; i< this.estados.Count-1;i++)
             {

                 for(int j = 0; j< matriz[i].Length; j++)
                 {
                     if (!matriz[i][j])
                     {
                        /*listaPares.Add(obtenerPosPorId(i, est1));
                        listaPares.Add(obtenerPosPorId(j, est2));*/
                        listaPares.Add(new Puntos(obtenerPos(j, est2), obtenerPos(i, est1)));
                     }
                 }
             }

             return listaPares;
         }

         public int obtenerPos(int pos, List<Estados> est)
         {
             return est[pos].position;
         }

         public Estados getEstadoporPos(int pos, List<Estados> est)
         {
             return est[pos];
         }

         public void MarcarEstados(bool [][]matriz,List<Pares>pares,List<Estados> est1, List<Estados> est2)
         {
            for(int i = 0; i < pares.Count-1; i++)
            {
                //primero pares
                int a = obtenerPosicionPorNombre(pares[i].a, est2);
                int b = obtenerPosicionPorNombre(pares[i].b, est1);

                int c = obtenerPosicionPorNombre(pares[i+1].a, est2);
                int d = obtenerPosicionPorNombre(pares[i+1].b, est1);


                if (matriz[a].Length < b)
                {
                    
                }
            }
         }

         public List<Pares> obtenerParesconTransciciones(List<Puntos> puntos, List<Estados> est1, List<Estados> est2)
        {
            List<Pares> pares = new List<Pares>();
            Estados temp1;
            Estados temp2;
            for(int i = 0; i < puntos.Count; i++)
            {
               temp1 =  obtenerEstado(puntos[i].p2,est2); //abajo
               temp2 = obtenerEstado(puntos[i].p1, est1); // a lado

               for(int j = 0; j < this.alfabeto.Count; j++)
                {
                    agregarPares(pares, this.alfabeto[j], temp1, temp2, est1, est2);
                }
            }

            return pares;
        }

        public string obtenerNombrePorId(int id, List<Estados> est)
        {
            for (int i = 0; i < est.Count; i++)
            {
                if (est[i].id == id)
                {
                    return est[i].name;
                }
            }

            return null;
        }

        public void agregarPares(List<Pares> pares,string read,Estados est1,Estados est2, List<Estados> st1, List<Estados> st2)
        {
            string p1="",p2="";
            
            for(int i = 0; i < est1.transiciones.Count; i++)
            {
                  if(est1.transiciones[i].read == read)
                {
                    p1 = obtenerNombrePorId(est1.transiciones[i].to, st1);
                }
            }

            for (int i = 0; i < est2.transiciones.Count; i++)
            {
                if (est2.transiciones[i].read == read)
                {
                    p2 = obtenerNombrePorId(est2.transiciones[i].to, st2);
                }
            }

            pares.Add(new Pares(p2, p1));
        }

        public Estados obtenerEstado(int pos, List<Estados> est)
         {
             return est[pos];
         }

         public int obtenerPosicionPorNombre(string read, List<Estados> est)
        {
            for(int i = 0; i < est.Count; i++)
            {
                if (est[i].name == read)
                {
                    return est[i].position;
                }
            }

            return -1;
        }

        /*bool parEsDistinguible(Pares par)
        {

        }*/

        public void GenerarXML(string path,string name)
        {
            //CreateFile(name);

            //creaamos el padre
            XmlDocument document = new XmlDocument();
            XmlNode meta =  document.CreateXmlDeclaration("1.0", "UTF-8", "no");
            XmlNode root = document.CreateElement("structure");
            XmlNode type = document.CreateElement("type");
            XmlNode automata = document.CreateElement("automaton");
            type.InnerText = "fa";


            //Añadirlo

            document.AppendChild(meta);
            document.AppendChild(root);


            root.AppendChild(type);
            root.AppendChild(automata);

            
            //Añadir los estados
            for(int i = 0; i < this.estados.Count; i++)
            {
                XmlNode estado = document.CreateElement("state");
                XmlAttribute id = document.CreateAttribute("id");
                XmlAttribute nombre = document.CreateAttribute("name");
                id.Value = this.estados[i].id.ToString();
                nombre.Value = this.estados[i].name;

                estado.Attributes.Append(id);
                estado.Attributes.Append(nombre);

                XmlNode x = document.CreateElement("x");
                x.InnerText = this.estados[i].x.ToString();

                XmlNode y = document.CreateElement("y");
                y.InnerText = this.estados[i].y.ToString();

                estado.AppendChild(x);
                estado.AppendChild(y);
                if (this.estados[i].inital)
                {
                    XmlNode inicial = document.CreateElement("initial");
                    estado.AppendChild(inicial);
                }

                if (this.estados[i].final)
                {
                    XmlNode inicial = document.CreateElement("final");
                    estado.AppendChild(inicial);
                }

                automata.AppendChild(estado);

            }

            for(int i = 0; i < this.estados.Count; i++)
            {
                for(int j = 0; j < this.estados[i].transiciones.Count; j++)
                {
                    XmlNode trans = document.CreateElement("transition");
                    XmlNode from = document.CreateElement("from");
                    XmlNode to = document.CreateElement("to");
                    XmlNode read = document.CreateElement("read");

                    from.InnerText = this.estados[i].transiciones[j].from.ToString();
                    to.InnerText = this.estados[i].transiciones[j].to.ToString();
                    read.InnerText = this.estados[i].transiciones[j].read;

                    trans.AppendChild(from);
                    trans.AppendChild(to);
                    trans.AppendChild(read);

                    automata.AppendChild(trans);

                }
            }

            document.Save(path + "\\" + name);

        }

     }
        

        /*
        public void llenarTransiciones(string[,] tabla, List<Estados> estados)
        {
            for (int i = 0; i < estados.Count; i++)
            {
                for (int j = 0; j < estados[i].transiciones.Count; j++)
                {
                    tabla[i, j] = obtenerNombrePorId(estados[i].transiciones[j].to);
                }
            }
        }

        public string obtenerNombrePorId(int id)
        {
            for (int i = 0; i < this.estados.Count; i++)
            {
                if (this.estados[i].id == id)
                {
                    return this.estados[i].name;
                }
            }

            return null;
        }

        public bool esFinal(string name)
        {
            for(int i = 0; i < this.estados.Count; i++)
            {
                if (this.estados[i].name == name)
                {
                    return this.estados[i].final;
                }
            }

            return false;
        }

        public List<string> listaFinales(string[,] tabla)
        {
            List<string> lista = new List<string>();

            for(int i = 0; i < tabla.Length; i++)
            {
                for(int j = 0; j < (tabla.Length-this.alfabeto.Count); j++)
                {
                    if (esFinal(tabla[i, j]))
                    {
                        lista.Add(tabla[i, j]);
                    }
                }
            }

            return lista;
        }

        public List<string> listarNoFinales(string[,] tabla)
        {
            List<string> lista = new List<string>();
            int size = (tabla.Length - this.alfabeto.Count);

            for (int i = 0; i < tabla.Length; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!esFinal(tabla[i, j]))
                    {
                        lista.Add(tabla[i, j]);
                    }
                }
            }

            return lista;
        }

        public int obtenerPosPorNombre(string name)
        {
            for(int i = 0; i < this.estados.Count; i++)
            {
                if(this.estados[i].name == name)
                {
                    return this.estados[i].position;
                }
            }

            return -1;
        }


        public void Equivalencias(List<string>list1, List<string> list2, string[,] tabla)
        {
            if(list1.Count < list2.Count)
            {
                //lista 2 es mayor
                for(int i = 0; i < list2.Count-1; i++)
                {
                    for(int j = 0; j < this.alfabeto.Count; j++)
                    {
                        int contador = 0;
                        string temp = "";
                        int primero = obtenerPosPorNombre(list2[0]); // A,B,C,D
                        int segundo = obtenerPosPorNombre(list2[i + 1]); //
                        for(int x = 0; x < this.alfabeto.Count; x++)
                        {
                            //si tienen lo mismo en la tabla de transiciones entonces no hay problema
                            if (tabla[primero, x] == tabla[segundo, i])
                            {
                                //no pasa nada porque esta bien
                            }else if (list2.Contains(tabla[primero, x]) && list2.Contains(tabla[segundo, i]))
                            {
                                //no pasa nada porque estan dentro de la lista
                            }
                            else
                            {
                                temp = temp + tabla[segundo, i];
                                list1.Add(tabla[segundo, i]);
                                contador++;
                            }
                            
                        }

                        if (contador >= 2)
                        {
                            for (int y = 0; y < contador; y++)
                            {
                                list2.RemoveAt(list2.Count - 1);
                            }

                            list2.Add(temp);
                        }

                    }
                }
            }
            else
            {
                
                //lista 1 es mayor
                for (int i = 0; i < list1.Count - 1; i++)
                {
                    
                    for (int j = 0; j < this.alfabeto.Count; j++)
                    {
                        int contador = 0;
                        string temp = "";
                        int primero = obtenerPosPorNombre(list1[0]); // A,B,C,D
                        int segundo = obtenerPosPorNombre(list1[i + 1]); //
                        for (int x = 0; x < this.alfabeto.Count; x++)
                        {
                            //si tienen lo mismo en la tabla de transiciones entonces no hay problema
                            if (tabla[primero, x] == tabla[segundo, i])
                            {
                                //no pasa nada porque esta bien
                            }
                            else if (list1.Contains(tabla[primero, x]) && list1.Contains(tabla[segundo, i]))
                            {
                                //no pasa nada porque estan dentro de la lista
                            }
                            else
                            {
                                temp = temp + tabla[segundo, i];
                                list2.Add(tabla[segundo, i]);
                                contador++;
                            }

                        }

                        if (contador >= 2)
                        {
                            for(int y = 0; y < contador; y++)
                            {
                                list2.RemoveAt(list2.Count - 1);
                            }

                            list2.Add(temp);
                        }


                    }
                }
            }
        }
        */
    }

