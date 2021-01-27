using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cliente

{
    class Ficha //Creamos una clase para obtener todos los movimientos de las fichas
    {
        //Esto nos sirve para guardar la posición actual de la ficha
        private int pos_x; 
        private int pos_y;

        //Estas coordenadas es para establecer la posicion inicial de cada ficha
        private int pos_x_casa;
        private int pos_y_casa;

        //Esto nos servira para saber si la ficha esta en casa o muerta.
        private Boolean casa;
        private Boolean muerta;

        private int casilla;
        private bool terminado;
        private bool entrar = false;
        int casillafinal = 0;
        private String color;    //Color elegido por el jugador
        private String nombre;   //Cada color tiene 4 fichas, por lo tanto asignamos un nombre a cada una

        public int Casilla
        {
            get
            {
                return casilla;
            }

            set
            {
                casilla = value;
            }
        }
        public int Pos_x
        {
            get
            {
                return pos_x;
            }

            set
            {
                pos_x = value;
            }
        }

        public bool Terminado
        {
            get
            {
                return terminado;
            }
            set
            {
                terminado = value;
            }
        }

        public bool Muerta
        {
            get
            {
                return muerta;
            }

            set
            {
                muerta = value;
            }
        }

        public int Pos_y
        {
            get
            {
                return pos_y;
            }

            set
            {
                pos_y = value;
            }
        }

        public int Pos_x_casa
        {
            get
            {
                return pos_x_casa;
            }

            set
            {
                pos_x_casa = value;
            }
        }

        public int Pos_y_casa
        {
            get
            {
                return pos_y_casa;
            }

            set
            {
                pos_y_casa = value;
            }
        }

        public bool Casa
        {
            get
            {
                return casa;
            }

            set
            {
                casa = value;
            }
        }

        public string Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public string Nombre
        {
            get
            {
                return nombre;
            }

            set
            {
                nombre = value;
            }
        }

        public Ficha()
        {
            //Asignamos valores vacios y mas adelante ya añadiremos los que se deben según cada ficha
            Pos_x = 0;
            Pos_x_casa = 0;
            Pos_y = 0;
            Pos_y_casa = 0;
            Casa = true;
            muerta = false;
            casilla = 0;
            color = "";
            nombre = "";
        }

        public void Crear(int posicion_x, int posicion_y, String nombre, String color)
        {
            //Esto nos servirá para introducirle los valores a las fichas

            this.Pos_x = posicion_x;
            this.Pos_y = posicion_y;
            Pos_x_casa = this.Pos_x;
            Pos_y_casa = this.Pos_y;
            casa = true;
            muerta = false;
            casilla = 0;
            this.color = color;
            this.nombre = nombre;
        }

        public void Sacarcasa(string color)
        {
            // Aqui sacamos cada ficha en la posición de salida del tablero segun su respectivo color

            switch (color)
            {
                case "rojo":
                    pos_x = 924;
                    pos_y = 299;
                    casa = false;
                    break;
                case "amarillo":
                    pos_x = 1195;
                    pos_y = 192;
                    casa = false;
                    break;
                case "azul":
                    pos_x = 1006;
                    pos_y = 106;
                    casa = false;
                    break;
                case "verde":
                    pos_x = 1112;
                    pos_y = 385;
                    casa = false;
                    break;
            }
        }


        private void Entrar(string color, int dado)
        {
            // Aqui detectamos cuando una ficha ya debe entrar hacia dentro del tablero.

            switch (color)
            {
                //La posición Y en ambas es la misma:
                case "rojo":
                case "amarillo":
                    pos_y = 245; 
                    break;

                //La posición X en ambas es la misma:
                case "azul":
                case "verde":
                    pos_x = 1060;
                    break;
            }

            if (dado >= 1 && casillafinal < 8)
            {
                //Dado mayor que 1 debido a que es el mínimo y la casilla final inferior a 8 significa antes de llegar al centro.
                //En ese caso acotamos la distancia hacia la casilla final

                casillafinal++;

                if (casillafinal <= 8)
                {
                    switch (color)
                    {
                        //Debemos fijar una posición para cada color 
                        case "rojo":
                            switch (casillafinal)
                            {
                                case 1:
                                    pos_x = 856;
                                    break;
                                case 2:
                                    pos_x = 879;
                                    break;
                                case 3:
                                    pos_x = 901;
                                    break;
                                case 4:
                                    pos_x = 924;
                                    break;
                                case 5:
                                    pos_x = 947;
                                    break;
                                case 6:
                                    pos_x = 969;
                                    break;
                                case 7:
                                    pos_x = 994;
                                    break;
                                case 8:
                                    pos_x = 1023;
                                    break;
                                default:  
                                    pos_x = 1023;
                                    break;

                            }
                            break;

                        case "amarillo":
                            switch (casillafinal)
                            {
                                case 1:
                                    pos_x = 1263;
                                    break;
                                case 2:
                                    pos_x = 1239;
                                    break;
                                case 3:
                                    pos_x = 1218;
                                    break;
                                case 4:
                                    pos_x = 1195;
                                    break;
                                case 5:
                                    pos_x = 1172;
                                    break;
                                case 6:
                                    pos_x = 1149;
                                    break;
                                case 7:
                                    pos_x = 1127;
                                    break;
                                case 8:
                                    pos_x = 1094;
                                    break;
                                default:
                                    pos_x = 1094;
                                    break;

                            }
                            break;

                        case "azul":
                            switch (casillafinal)
                            {
                                case 1:
                                    pos_y = 36;
                                    break;
                                case 2:
                                    pos_y = 61;
                                    break;
                                case 3:
                                    pos_y = 83;
                                    break;
                                case 4:
                                    pos_y = 106;
                                    break;
                                case 5:
                                    pos_y = 130;
                                    break;
                                case 6:
                                    pos_y = 153;
                                    break;
                                case 7:
                                    pos_y = 176;
                                    break;
                                case 8:
                                    pos_y = 212;
                                    break;
                                default:
                                    pos_y = 212;
                                    break;

                            }
                            break;

                        case "verde":
                            switch (casillafinal)
                            {
                                case 1:
                                    pos_y = 385;
                                    break;
                                case 2:
                                    pos_y = 408;
                                    break;
                                case 3:
                                    pos_y = 432;
                                    break;
                                case 4:
                                    pos_y = 385;
                                    break;
                                case 5:
                                    pos_y = 362;
                                    break;
                                case 6:
                                    pos_y = 338;
                                    break;
                                case 7:
                                    pos_y = 314;
                                    break;
                                case 8:
                                    pos_y = 280;
                                    break;
                                default:
                                    pos_y = 280;
                                    break;

                            }
                            break;
                    }
                    if (casillafinal >= 8)
                    {
                        terminado = true; //Esa ficha ya ha terminado
                    }
                }

            }

        }


        
        public void Moverficha(int dado, int[,] tablero)
        {
            //Esto nos permitirá realizar los movimientos por el tablero.

            bool encontrar = false;

            if (!entrar)  //Suponiendo que la ficha aún no debe entrar por su color
            {
                while (!encontrar)
                {
                    for (int i = 0; i < 68; i++)
                    {
                        if (tablero[i, 0] == pos_x && tablero[i, 1] == pos_y)
                        {
                            casilla = i + 1;
                            encontrar = true;

                            

                            /*
                             * Mientras encontrar sea falso se va a recorrer el array tablero
                             * hasta que coincidan los valores con la posicion actual de la ficha
                             * 
                             * Se le suma 1 a la posicion del array donde se encuentran esas coordenadas
                             * y de esta manera se sabe el número de casilla en la que esta la ficha
                             * */
                             
                        }
                    }
                }
            }

            if ((casilla == 17 && color.Equals("azul")) || (entrar && color.Equals("azul")))
            {
                /*
                 * Si la ficha llega al punto del tablero donde debe dejar de girar y entrar
                 * se fija entrar a true y se llama a la funcion entrar
                 * */
                 
                entrar = true;
                Entrar("azul", dado);
            }
            else if ((casilla == 34 && color.Equals("rojo")) || (entrar && color.Equals("rojo")))
            {
                entrar = true;
                Entrar("rojo", dado);
            }
            else if ((casilla == 51 && color.Equals("verde")) || (entrar && color.Equals("verde")))
            {
                entrar = true;
                Entrar("verde", dado);
            }
            else if ((casilla == 68 && color.Equals("amarillo")) || (entrar && color.Equals("amarillo")))
            {
                entrar = true;
                Entrar("amarillo", dado);
            }
            else if (casilla == 68)
            {
                //Se reinicia la matriz y empieza en 0 de nuevo.
                casilla = 0;

                if (dado >= 1)
                {
                    //Cuando el dado sea mayor a 1 se fijan las nuevas posiciones de la ficha
                    casilla++;    //Probar quitarlo esto y el -1 ***************************************
                    pos_x = tablero[(casilla - 1), 0];
                    pos_y = tablero[(casilla - 1), 1];
                    casa = false;   //La ficha ya no esta en casa
                }

            }
            else if (dado >= 1)
            {
                //Cuando el dado sea mayor a 1 se fijan las nuevas posiciones de la ficha
                casilla++;
                pos_x = tablero[(casilla - 1), 0];
                pos_y = tablero[(casilla - 1), 1];
                casa = false;  //La ficha ya no esta en casa
            }
        } 


    }
}

