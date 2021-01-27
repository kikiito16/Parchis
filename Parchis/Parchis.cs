using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.VisualBasic;


namespace Cliente
{
    public partial class Form1 : Form
    {
        Socket server;
        string usuario, invitado;
        string psw;
        Thread atender;
        string jugador_actual;
        string jugador_invitado;
        string jugador_invitador;
        int conectado = 0;
        int num_conectados = 0;
        string[] conectados;
        List<string> lista_conectados = new List<string>();

        delegate void DelegadoParaEscribir(string mensaje);
        delegate void DelegadoParaActualizar(string mensaje);

        //Creamos la variable para el dado
        int dado;

        //Creamos las variables para saber si ha terminado un color
        int terminadosamarillo = 0;
        int terminadosazul = 0;
        int terminadosverde = 0;
        int terminadosrojo = 0;

        bool comedora = false;

        //Se crea un array de 2 dimensiones donde se guardan las coordenadas de las casillas principales del tablero
        int[,] tablero = new int[68, 2] {{1287, 192}, { 1263, 192 }, { 1241, 192 }, { 1218, 192 },
            {1195,192 }, {1173,192 }, {1149,192 }, {1127,198 }, {1106,176 }, {1112,153 }, {1112,130 },
            {1112,106 }, {1112,83 }, {1112,61 }, {1112,36 }, {1112,13 }, {1060,13 }, {1005,13 },
            {1005,36 }, {1005,61 }, {1005,83 }, {1005,106 }, {1005,130 }, {1005,153 }, {1013,176 }, {992,199 },
            {969,190 }, {947,190 }, {924,190 }, {901,190 }, {879,190 }, {856,190 }, {833,190 }, {833,245 },
            {833,299 }, {856,299 }, {879,299 }, {901,299 }, {924,299 }, {947,299 }, {969,299 }, {993,293 },
            {1012,314 }, {1007,338 }, {1007,362 }, {1007,385 }, {1007,408 }, {1007,432 }, {1007,455 }, {1007,479 },
            {1060,479 }, {1112,479 }, {1112,455 }, {1112,432 }, {1112,408 }, {1112,385 }, {1112,362 }, {1112,338 },
            {1108,314 }, {1127,294 }, {1149,301 }, {1173,301 }, {1195,301 }, {1218,301 }, {1241,301 }, {1263,301 },
            {1287,300 }, {1287,245 }};

        Random aleatorio = new System.Random(); //Esto nos genera la tirada del dado

        //Creamos las 4 fichas de cada color sin asignarles valores aun.
        Ficha rojo1 = new Ficha(); Ficha rojo2 = new Ficha(); Ficha rojo3 = new Ficha(); Ficha rojo4 = new Ficha();
        Ficha verde1 = new Ficha(); Ficha verde2 = new Ficha(); Ficha verde3 = new Ficha(); Ficha verde4 = new Ficha();
        Ficha azul1 = new Ficha(); Ficha azul2 = new Ficha(); Ficha azul3 = new Ficha(); Ficha azul4 = new Ficha();
        Ficha amarillo1 = new Ficha(); Ficha amarillo2 = new Ficha(); Ficha amarillo3 = new Ficha(); Ficha amarillo4 = new Ficha();


        Ficha[] fichas = new Ficha[16];  //Creamos un vector de fichas para las 16
        String turno = ""; //Esto nos indicara a quien le toca jugar 

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false; //Necesario para que los elementos de los formularios puedan ser
            Desconectarse_Del_Servidor.Visible = false;
            label1.Visible = false;
            nombre.Visible = false;
            Contraseña_Usuario.Visible = false;
            label2.Visible = false;
            ID.Visible = false;
            Jugadores_Partidas.Visible = false;
            Ganador_Partida.Visible = false;
            Realizar_Consulta.Visible = false;
            Tablero_pb.Visible = true;
            roja_1.Visible = true;
            roja_2.Visible = true;
            roja_3.Visible = true;
            roja_4.Visible = true;
            azul_1.Visible = true;
            azul_2.Visible = true;
            azul_3.Visible = true;
            azul_4.Visible = true;
            amarilla_1.Visible = true;
            amarilla_2.Visible = true;
            amarilla_3.Visible = true;
            amarilla_4.Visible = true;
            verde_1.Visible = true;
            verde_2.Visible = true;
            verde_3.Visible = true;
            verde_4.Visible = true;
            dado_pb.Visible = true;
            Dado_But.Visible = true;
            Invitado_group.Visible = false;
            Invitacion_group.Visible = false;
            Conectados_box.Visible = false;

        }


        private void Actualiza_Grid(string mensaje)
        // actualiza la lista de conectados cada vez que se conecte un usuario
        {
            Conectados_data.ColumnCount = 1;
            Conectados_data.RowCount = num_conectados;

            conectados = mensaje.Split(',');

            for (int i = 0; i < num_conectados; i++)
                Conectados_data.Rows[i].Cells[0].Value = conectados[i];
        }

        private void Invitacion_lbl(string mensaje)
        {
            this.invitado_lbl.Text = mensaje + " te ha invitado a una partida";
        }


        private void Desconectarse_Del_Servidor_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            //Vamos a desconectar el thread
            atender.Abort();
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            conectado = 0;
            Desconectarse_Del_Servidor.Visible = false;
            label1.Visible = false;
            nombre.Visible = false;
            Contraseña_Usuario.Visible = false;
            label2.Visible = false;
            ID.Visible = false;
            Jugadores_Partidas.Visible = false;
            Ganador_Partida.Visible = false;
            Realizar_Consulta.Visible = false;
            Tablero_pb.Visible = false;
            dado_pb.Visible = false;
            Invitado_group.Visible = false;
            Invitacion_group.Visible = false;
            Conectados_box.Visible = false;
            XBOX.Visible = false;
            YBOX.Visible = false;
            Xlbl.Visible = false;
            Ylbl.Visible = false;
            Iniciar_Session.Visible = true;
            UsuarioBox.Visible = true;
            ContraBox.Visible = true;
            Registrarse.Visible = true;
            Usuario1Box.Visible = true;
            Contra1Box.Visible = true;

        }

        private void Realizar_Consulta_Click(object sender, EventArgs e)
        {

            if (Ganador_Partida.Checked) //Revisamos el metodo para encontrar el ganador
            {
                string mensaje = "1/" + ID.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            if (Contraseña_Usuario.Checked) //Revisamos el metodo para encontrar la contraseña
            {
                string mensaje = "2/" + nombre.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }

            else //Revisamos el metodo para recibir los jugadores que jugaron la partida
            {
                // Para dame jugadores
                string mensaje = "3/" + ID.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }

        private void AtenderServidor()
        {
            try
            {
                while (true)
                {
                    //Operaciones para recibir el mensaje, recibimos el mensaje del servidor
                    byte[] msg2 = new byte[80];
                    server.Receive(msg2);
                    string recibido = Encoding.ASCII.GetString(msg2).TrimEnd('\0');
                    string[] trozo = recibido.Split('/'); //Partimos el mensaje por la barra "/"
                    //Tenemos dos strings
                    string mensaje;
                    int codigo = Convert.ToInt32(trozo[0]); //El numero del mensaje 1/ , 2/, 3/ hasta 7/ que es nuestro caso
                    if (codigo == 5)
                    //Recibiremos un mensaje del estilo: 5/num_conectados/conectados
                    {
                        num_conectados = Convert.ToInt32(trozo[1]);
                        mensaje = trozo[2].Split('\0')[0];
                    }
                    else
                        mensaje = trozo[1].Split('\0')[0];

                    switch (codigo)
                    {
                        case 1:  //Respuesta a Ganador
                            MessageBox.Show("El ganador fue: " + mensaje);
                            break;
                        case 2:  //Respuesta a Dame Contraseña
                            MessageBox.Show(mensaje);
                            break;
                        case 3:  //Respuesta a Dame Jugadores
                            MessageBox.Show(mensaje);
                            break;
                        case 4:  //Respuesta a Iniciar Sesión
                            if (mensaje == "Alguno de los datos es incorrecto")
                                MessageBox.Show("Alguno de los datos es incorrecto");
                            else
                            {
                                MessageBox.Show("Has iniciado sesión como: " + mensaje);

                            }
                            break;
                        case 5:  //Respuesta a la notificacion de los usuarios conectados
                            Conectados_data.Invoke(new DelegadoParaActualizar(Actualiza_Grid), new object[] { mensaje });
                            break;
                        case 6:   //Invitar a un jugador
                            mensaje = trozo[1].Split('\0')[0];
                            invitado_lbl.Invoke(new DelegadoParaEscribir(Invitacion_lbl), new object[] { mensaje });
                            /* DialogResult result;
                             result = MessageBox.Show(mensaje, " Te ha invitado a una partida el jugador:", MessageBoxButtons.YesNo);
                             if (result == System.Windows.Forms.DialogResult.Yes)
                             {
                                 string men = "7/" + mensaje + "/SI";
                                 byte[] msg = System.Text.Encoding.ASCII.GetBytes(men);
                                 server.Send(msg);
                             }
                             else
                             {
                                 string men = "7/"+mensaje+"/NO"+mensaje;
                                 byte[] msg = System.Text.Encoding.ASCII.GetBytes(men);
                                 server.Send(msg);
                             }*/
                            break;
                        case 7:  // Respuesta a la invitacion
                            int respuesta = Convert.ToInt32(mensaje);
                            if (respuesta == 0)
                                MessageBox.Show("Han rechazado tu invitación");
                            else
                            {
                                MessageBox.Show("Han aceptado tu invitación");
                                /*Invitado_group.Visible = false;
                                Invitacion_group.Visible = false;*/
                            }
                            break;

                    }
                }
            }
            catch
            {
                return;
            }
        }


        private void Iniciar_Session_Click(object sender, EventArgs e)
        {
            if ((UsuarioBox.Text != string.Empty) && (ContraBox.Text != string.Empty) && (conectado == 0))
            {
                usuario = UsuarioBox.Text;
                int Puerto1 = 50079;
                //int Puerto3 = 50080;
                //int Puerto2 = 50081;

                //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
                //al que deseamos conectarnos
                IPAddress direc = IPAddress.Parse("192.168.56.102");
                IPEndPoint ipep1 = new IPEndPoint(direc, 9050);
                //IPEndPoint ipep2 = new IPEndPoint(direc, Puerto2);
                //IPEndPoint ipep3 = new IPEndPoint(direc, Puerto3);
                //Creamos el socket 
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    server.Connect(ipep1);//Intentamos conectar el socket
                    this.BackColor = Color.Green;
                    conectado = 1;
                    MessageBox.Show("Conectado");

                }
                catch (SocketException ex)
                {
                    //Si hay excepcion imprimimos error y salimos del programa con return 
                    MessageBox.Show("No he podido conectar con el servidor");
                    return;
                }

                //Vamos a poner en marcha el thread que atenderá todos los mensajes que nos envie el servidor
                ThreadStart ts = delegate { AtenderServidor(); };
                atender = new Thread(ts);
                atender.Start();


                string mensaje = "4/" + UsuarioBox.Text + "/" + ContraBox.Text;

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                Desconectarse_Del_Servidor.Visible = true;
                label1.Visible = true;
                nombre.Visible = true;
                Contraseña_Usuario.Visible = true;
                label2.Visible = true;
                ID.Visible = true;
                Jugadores_Partidas.Visible = true;
                Ganador_Partida.Visible = true;
                Realizar_Consulta.Visible = true;
                Tablero_pb.Visible = true;
                dado_pb.Visible = true;
                Invitado_group.Visible = true;
                Invitacion_group.Visible = true;
                Conectados_box.Visible = true;
                XBOX.Visible = true;
                YBOX.Visible = true;
                Xlbl.Visible = true;
                Ylbl.Visible = true;
               /* roja_1.Visible = true;
                roja_2.Visible = true;
                roja_3.Visible = true;
                roja_4.Visible = true;*/
                Iniciar_Session.Visible = false;
                UsuarioBox.Visible = false;
                ContraBox.Visible = false;
                Registrarse.Visible = false;
                Usuario1Box.Visible = false;
                Contra1Box.Visible = false;



            }
            else
                MessageBox.Show("Rellena todos los campos");
        }

        private void Invitar_btn_Click(object sender, EventArgs e)
        {
            string mensaje = "6/" + invitar_box.Text;
            jugador_invitado = invitar_box.Text;
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }


        private void Rechazar_btn_Click(object sender, EventArgs e)
        {
            //Cuando el cliente rechaza la invitacion enviamos un NO al servidor
            string mensaje = "7/" + usuario + "/NO";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            //Invitacion_groupBox.Visible = false;
        }

        private void Aceptar_btn_Click(object sender, EventArgs e)
        {
            string mensaje = "7/" + usuario + "/SI";
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);
        }

        private void Registrarse_Click(object sender, EventArgs e)
        {
            try
            {
                string mensaje = "5/" + Usuario1Box.Text + "/" + Contra1Box.Text;
                // Enviamos al servidor el nombre
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            catch
            {
                MessageBox.Show("Error al registrar");
            }
        }

        private void Conectados_data_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int fila = e.RowIndex;
            invitado = conectados[fila];
        }

        /*
        private void dado_pb_Click(object sender, EventArgs e)
        {
            //Debemos tener una variable booleana para saber si se pueden tirar los dados o no, con tal de evitar tiradas consecutivas.

           // dado = aleatorio.Next(1, 7);  //generamos valores entre el 1 y el 6
           

            //Modificamos el valor del text box según el número que ha salido
            switch (dado)
            {
                case 1:
                    valor_dado.Text = "1";
  
                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;
                             
                case 2:
                    valor_dado.Text = "2";
  
                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;
            
                case 3:
                    valor_dado.Text = "3";
  
                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 4:
                    valor_dado.Text = "4";
  
                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 5:
                    valor_dado.Text = "5";
  
                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 6:
                   valor_dado.Text = "6";
  
                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                default:
                    //Ante cualquier problema pedimos que realice otra tirada
                    MessageBox.Show("Ha ocurrido un error, vuelve a intentarlo");
                    dado_pb.Enabled = true;
                    break;

            }
        }*/

        private void habilitar_fichas(String color)
        {
            switch (color)
            {
                //Esto lo usamos para habilitar las fichas de un color en especifico
                case "AMARILLO":
                    amarilla_1.Enabled = true;
                    amarilla_2.Enabled = true;
                    amarilla_3.Enabled = true;
                    amarilla_4.Enabled = true;
                    break;

                case "ROJO":
                    roja_1.Enabled = true;
                    roja_2.Enabled = true;
                    roja_3.Enabled = true;
                    roja_4.Enabled = true;
                    break;

                case "VERDE":
                    verde_1.Enabled = true;
                    verde_2.Enabled = true;
                    verde_3.Enabled = true;
                    verde_4.Enabled = true;
                    break;

                case "AZUL":
                    azul_1.Enabled = true;
                    azul_2.Enabled = true;
                    azul_3.Enabled = true;
                    azul_4.Enabled = true;
                    break;

                default:
                    //Determinaremos por defecto activar todas las fichas
                    amarilla_1.Enabled = true;
                    amarilla_2.Enabled = true;
                    amarilla_3.Enabled = true;
                    amarilla_4.Enabled = true;
                    roja_1.Enabled = true;
                    roja_2.Enabled = true;
                    roja_3.Enabled = true;
                    roja_4.Enabled = true;
                    verde_1.Enabled = true;
                    verde_2.Enabled = true;
                    verde_3.Enabled = true;
                    verde_4.Enabled = true;
                    azul_1.Enabled = true;
                    azul_2.Enabled = true;
                    azul_3.Enabled = true;
                    azul_4.Enabled = true;
                    break;
            }
        }

        private void deshabilitar_fichas(String color)
        {
            //Esto lo usamos para deshabilitar las fichas de un color en especifico
            switch (color)
            {
                case "AMARILLO":
                    amarilla_1.Enabled = false;
                    amarilla_2.Enabled = false;
                    amarilla_3.Enabled = false;
                    amarilla_4.Enabled = false;
                    break;
                case "ROJO":
                    roja_1.Enabled = false;
                    roja_2.Enabled = false;
                    roja_3.Enabled = false;
                    roja_4.Enabled = false;
                    break;
                case "VERDE":
                    verde_1.Enabled = false;
                    verde_2.Enabled = false;
                    verde_4.Enabled = false;
                    verde_3.Enabled = false;
                    break;
                case "AZUL":
                    azul_1.Enabled = false;
                    azul_2.Enabled = false;
                    azul_3.Enabled = false;
                    azul_4.Enabled = false;
                    break;
                default:
                    ////Determinaremos por defecto desactivar todas las fichas
                    amarilla_1.Enabled = false;
                    amarilla_2.Enabled = false;
                    amarilla_3.Enabled = false;
                    amarilla_4.Enabled = false;
                    roja_1.Enabled = false;
                    roja_2.Enabled = false;
                    roja_3.Enabled = false;
                    roja_4.Enabled = false;
                    verde_1.Enabled = false;
                    verde_2.Enabled = false;
                    verde_3.Enabled = false;
                    verde_4.Enabled = false;
                    azul_1.Enabled = false;
                    azul_2.Enabled = false;
                    azul_3.Enabled = false;
                    azul_4.Enabled = false;
                    break;

            }
        }

        private void PasarTurno()
        {
            switch (turno)
            {
                //En este switch se cambia el turno al color siguiente
                case "ROJO":
                    turno = "VERDE";
                    TurnoBox.Text = turno;
                    break;
                case "VERDE":
                    turno = "AMARILLO";
                    TurnoBox.Text = turno;
                    break;
                case "AMARILLO":
                    turno = "AZUL";
                    //turno = "ROJO";
                    TurnoBox.Text = turno;
                    break;
                case "AZUL":
                    turno = "ROJO";
                    TurnoBox.Text = turno;
                    break;
            }
        }

        private void Comer(int[,] tablero, int j, Ficha actual)
        {
            //Este metodo nos servira para saber si una ficha ha sido comida
            if (j == 1)
            //j es el valor de un for que se utiliza para mover las fichas
            {
                for (int i = 0; i < fichas.Length; i++)
                { //Recorremos la matriz de fichas
                    if (actual.Casilla == fichas[i].Casilla)
                    { //Si coinciden ambas fichas en la misma casilla y son de distinto color:
                        switch (fichas[i].Nombre)
                        {
                            case "rojo1":
                                if (actual.Color == "rojo")      
                                    break;  //No pasa nada

                                else
                                { //Si son de distinto color, la que ya estaba en esa casilla es comida y vuelve a casa
                                    roja_1.Location = new Point(rojo1.Pos_x_casa, rojo1.Pos_y_casa);
                                    rojo1.Casa = true; //se fija casa como true
                                    rojo1.Casilla = 0; //se fija casilla como 0
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            //Repetimos el proceso para todas las fichas de cada color
                            case "rojo2":
                                if (actual.Color == "rojo")  
                                    break; 
                                else
                                {
                                    roja_2.Location = new Point(rojo2.Pos_x_casa, rojo2.Pos_y_casa);
                                    rojo2.Casa = true;
                                    rojo2.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }

                            case "rojo3":
                                if (actual.Color == "rojo") 
                                    break;

                                else
                                {
                                    roja_3.Location = new Point(rojo3.Pos_x_casa, rojo3.Pos_y_casa);
                                    rojo3.Casa = true;
                                    rojo3.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }

                            case "rojo4":
                                if (actual.Color == "rojo") 
                                    break; 
                                else
                                {
                                    roja_4.Location = new Point(rojo4.Pos_x_casa, rojo4.Pos_y_casa);
                                    rojo4.Casa = true;
                                    rojo4.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }

                            case "azul1":
                                if (actual.Color == "azul") 
                                    break; 
                                else
                                {
                                    azul_1.Location = new Point(azul1.Pos_x_casa, azul1.Pos_y_casa);
                                    azul1.Casa = true;
                                    azul1.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }

                            case "azul2":
                                if (actual.Color == "azul") 
                                    break; 
                                else
                                {
                                    azul_2.Location = new Point(azul2.Pos_x_casa, azul2.Pos_y_casa);
                                    azul2.Casa = true;
                                    azul2.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }

                            case "azul3":
                                if (actual.Color == "azul") 
                                    break; 
                                else
                                {
                                    azul_3.Location = new Point(azul3.Pos_x_casa, azul3.Pos_y_casa);
                                    azul3.Casa = true;
                                    azul3.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "azul4":
                                if (actual.Color == "azul") 
                                    break; 
                                else
                                {
                                    azul_4.Location = new Point(azul4.Pos_x_casa, azul4.Pos_y_casa);
                                    azul4.Casa = true;
                                    azul4.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "verde1":
                                if (actual.Color == "verde") 
                                    break; 
                                else
                                {
                                    verde_1.Location = new Point(verde1.Pos_x_casa, verde1.Pos_y_casa);
                                    verde1.Casa = true;
                                    verde1.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "verde2":
                                if (actual.Color == "verde") 
                                    break; 
                                else
                                {
                                    verde_2.Location = new Point(verde2.Pos_x_casa, verde2.Pos_y_casa);
                                    verde2.Casa = true;
                                    verde2.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "verde3":
                                if (actual.Color == "verde")
                                    break; 
                                else
                                {
                                    verde_3.Location = new Point(verde3.Pos_x_casa, verde3.Pos_y_casa);
                                    verde3.Casa = true;
                                    verde3.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "verde4":
                                if (actual.Color == "verde") 
                                    break; 
                                else
                                {
                                    verde_4.Location = new Point(verde4.Pos_x_casa, verde4.Pos_y_casa);
                                    verde4.Casilla = 0;
                                    verde4.Casa = true;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "amarillo1":
                                if (actual.Color == "amarillo") 
                                    break; 
                                else
                                {
                                    amarilla_1.Location = new Point(amarillo1.Pos_x_casa, amarillo1.Pos_y_casa);
                                    amarillo1.Casa = true;
                                    amarillo1.Casilla = 0;
                                    comedora = true;
                                    dado = 20;

                                    break;
                                }
                            case "amarillo2":
                                if (actual.Color == "amarillo") 
                                    break; 
                                else
                                {
                                    amarilla_2.Location = new Point(amarillo2.Pos_x_casa, amarillo2.Pos_y_casa);
                                    amarillo2.Casilla = 0;
                                    amarillo2.Casa = true;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "amarillo3":
                                if (actual.Color == "amarillo") 
                                    break; 
                                else
                                {
                                    amarilla_3.Location = new Point(amarillo3.Pos_x_casa, amarillo3.Pos_y_casa);
                                    amarillo3.Casa = true;
                                    amarillo3.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                            case "amarillo4":
                                if (actual.Color == "amarillo") 
                                    break; 
                                else
                                {
                                    amarilla_4.Location = new Point(amarillo4.Pos_x_casa, amarillo4.Pos_y_casa);
                                    amarillo4.Casa = true;
                                    amarillo4.Casilla = 0;
                                    comedora = true;
                                    dado = 20;
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Creamos cada ficha con el metodo crear y asi ya determinamos la posicion de casa de cada ficha

            rojo1.Crear(roja_1.Location.X, roja_1.Location.Y, "rojo1", "rojo");
            rojo2.Crear(roja_2.Location.X, roja_2.Location.Y, "rojo2", "rojo");
            rojo3.Crear(roja_3.Location.X, roja_3.Location.Y, "rojo3", "rojo");
            rojo4.Crear(roja_4.Location.X, roja_4.Location.Y, "rojo4", "rojo");
            verde1.Crear(verde_1.Location.X, verde_1.Location.Y, "verde1", "verde");
            verde2.Crear(verde_2.Location.X, verde_2.Location.Y, "verde2", "verde");
            verde3.Crear(verde_3.Location.X, verde_3.Location.Y, "verde3", "verde");
            verde4.Crear(verde_4.Location.X, verde_4.Location.Y, "verde4", "verde");
            azul1.Crear(azul_1.Location.X, azul_1.Location.Y, "azul1", "azul");
            azul2.Crear(azul_2.Location.X, azul_2.Location.Y, "azul2", "azul");
            azul3.Crear(azul_3.Location.X, azul_3.Location.Y, "azul3", "azul");
            azul4.Crear(azul_4.Location.X, azul_4.Location.Y, "azul4", "azul");
            amarillo1.Crear(amarilla_1.Location.X, amarilla_1.Location.Y, "amarillo1", "amarillo");
            amarillo2.Crear(amarilla_2.Location.X, amarilla_2.Location.Y, "amarillo2", "amarillo");
            amarillo3.Crear(amarilla_3.Location.X, amarilla_3.Location.Y, "amarillo3", "amarillo");
            amarillo4.Crear(amarilla_4.Location.X, amarilla_4.Location.Y, "amarillo4", "amarillo");

            //Aqui introducimos los valores de la matriz de fichas creada anteriormente

            fichas[0] = rojo1; fichas[1] = rojo2; fichas[2] = rojo3; fichas[3] = rojo4;
            fichas[4] = verde1; fichas[5] = verde2; fichas[6] = verde3; fichas[7] = verde4;
            fichas[8] = azul1; fichas[9] = azul2; fichas[10] = azul3; fichas[11] = azul4;
            fichas[12] = amarillo1; fichas[13] = amarillo2; fichas[14] = amarillo3; fichas[15] = amarillo4;

            deshabilitar_fichas("todas");//Se deshabilitan todas las fichas
            Boolean error = true;
            while (error)
            {
                //sale un popup preguntando que color va a empezar;
                turno = Interaction.InputBox("¿Que color va a empezar?", "Color", "ROJO");
                if (turno.ToUpper().Equals("ROJO"))
                {
                    turno = "ROJO";
                    TurnoBox.Text = turno;
                    error = false;
                }else if (turno.ToUpper().Equals("AZUL"))
                {
                    turno = "AZUL";
                     TurnoBox.Text = turno;
                    error = false;
                }else if (turno.ToUpper().Equals("AMARILLO"))
                {
                    turno = "AMARILLO";
                     TurnoBox.Text = turno;
                    error = false;
                }else if (turno.ToUpper().Equals("VERDE"))
                {
                    turno = "VERDE";
                     TurnoBox.Text = turno;
                    error = false;
                }
                else
                {
                    error = true;
                }
            }
        }

        private void roja_1_Click(object sender, EventArgs e)
        {
            if (fichas[0].Casa && dado == 5)
            {

                fichas[0].Sacarcasa("rojo");   //Sale la ficha de casa
                roja_1.Location = new Point(rojo1.Pos_x, rojo1.Pos_y);  //Va a la nueva posición que es la salida de casa
                deshabilitar_fichas("todas");   //ESto hara que vaya al caso default y por lo tanto se dehabilitan todas.
                dado_pb.Enabled = true;    //Volvemos  aactivar el dado para realizar otra tirada

            }
            else if (fichas[0].Casa && dado != 5)  //Si la ficha esta en casa y el dado es distinto a 5 se pasa el turno
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)  //Si no esta en casa entonces se mueve según el valor del dado
                {
                    rojo1.Moverficha(dado, tablero);
                    roja_1.Location = new Point(rojo1.Pos_x, rojo1.Pos_y);
                    Comer(tablero, i, rojo1);   //en caso necesario se come la ficha que haya
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            rojo1.Moverficha(dado, tablero);
                            roja_1.Location = new Point(rojo1.Pos_x, rojo1.Pos_y);
                            comedora = false;
                            Comer(tablero, i, rojo1);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }

            if (fichas[0].Terminado)   //Si llega al final
            {
                roja_1.Visible = false;
                terminadosrojo++;
                if (terminadosrojo >= 4)   //Cuando han entrado las 4 fichas se considera ganada la partida.
                {
                    MessageBox.Show("Ha ganado el equipo rojo");
                    Application.Exit();
                }
            }
        }

        //Repetimos el proceso para las 16 fichas
        private void roja_2_Click(object sender, EventArgs e)
        {
            if (fichas[1].Casa && dado == 5)
            {

                fichas[1].Sacarcasa("rojo");
                roja_2.Location = new Point(rojo2.Pos_x, rojo2.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[1].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    rojo2.Moverficha(dado, tablero);
                    roja_2.Location = new Point(rojo2.Pos_x, rojo2.Pos_y);
                    Comer(tablero, i, rojo2);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            rojo2.Moverficha(dado, tablero);
                            roja_2.Location = new Point(rojo2.Pos_x, rojo2.Pos_y);
                            comedora = false;
                            Comer(tablero, i, rojo2);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[1].Terminado)
            {
                roja_2.Visible = false;
                terminadosrojo++;
                if (terminadosrojo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo rojo");
                    Application.Exit();
                }
            }
        }

        private void roja_3_Click(object sender, EventArgs e)
        {
            if (fichas[2].Casa && dado == 5)
            {

                fichas[2].Sacarcasa("rojo");
                roja_3.Location = new Point(rojo3.Pos_x, rojo3.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[2].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    rojo3.Moverficha(dado, tablero);
                    roja_3.Location = new Point(rojo3.Pos_x, rojo3.Pos_y);
                    Comer(tablero, i, rojo3);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            rojo3.Moverficha(dado, tablero);
                            roja_3.Location = new Point(rojo3.Pos_x, rojo3.Pos_y);
                            Comer(tablero, i, rojo3);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[2].Terminado)
            {
                roja_3.Visible = false;
                terminadosrojo++;
                if (terminadosrojo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo rojo");
                    Application.Exit();
                }
            }
        }

        private void roja_4_Click(object sender, EventArgs e)
        {
            if (fichas[3].Casa && dado == 5)
            {

                fichas[3].Sacarcasa("rojo");
                roja_4.Location = new Point(rojo4.Pos_x, rojo4.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[3].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    rojo4.Moverficha(dado, tablero);
                    roja_4.Location = new Point(rojo4.Pos_x, rojo4.Pos_y);
                    Comer(tablero, i, rojo4);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            rojo4.Moverficha(dado, tablero);
                            roja_4.Location = new Point(rojo4.Pos_x, rojo4.Pos_y);
                            comedora = false;
                            Comer(tablero, i, rojo4);
                        }

                    }
                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[3].Terminado)
            {
                roja_4.Visible = false;
                terminadosrojo++;
                if (terminadosrojo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo rojo");
                    Application.Exit();
                }
            }
        }

        private void verde_1_Click(object sender, EventArgs e)
        {
            if (fichas[4].Casa && dado == 5)
            {

                fichas[4].Sacarcasa("verde");
                verde_1.Location = new Point(verde1.Pos_x, verde1.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[4].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    verde1.Moverficha(dado, tablero);
                    verde_1.Location = new Point(verde1.Pos_x, verde1.Pos_y);
                    Comer(tablero, i, verde1);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            verde1.Moverficha(dado, tablero);
                            verde_1.Location = new Point(verde1.Pos_x, verde1.Pos_y);
                            comedora = false;
                            Comer(tablero, i, verde1);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[4].Terminado)
            {
                verde_1.Visible = false;
                terminadosverde++;
                if (terminadosverde >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo verde");
                    Application.Exit();
                }
            }
        }

        private void verde_2_Click(object sender, EventArgs e)
        {
            if (fichas[5].Casa && dado == 5)
            {

                fichas[5].Sacarcasa("verde");
                verde_2.Location = new Point(verde2.Pos_x, verde2.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }

            else if (fichas[5].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }

            else
            {
                for (int i = dado; i > 0; i--)
                {
                    verde2.Moverficha(dado, tablero);
                    verde_2.Location = new Point(verde2.Pos_x, verde2.Pos_y);
                    Comer(tablero, i, verde2);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            verde2.Moverficha(dado, tablero);
                            verde_2.Location = new Point(verde2.Pos_x, verde2.Pos_y);
                            comedora = false;
                            Comer(tablero, j, verde2);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[5].Terminado)
            {
                verde_2.Visible = false;
                terminadosverde++;
                if (terminadosverde >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo verde");
                    Application.Exit();
                }
            }
        }

        private void verde_3_Click(object sender, EventArgs e)
        {
            if (fichas[6].Casa && dado == 5)
            {

                fichas[6].Sacarcasa("verde");
                verde_3.Location = new Point(verde3.Pos_x, verde3.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[6].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    verde3.Moverficha(dado, tablero);
                    verde_3.Location = new Point(verde3.Pos_x, verde3.Pos_y);
                    Comer(tablero, i, verde3);
                     while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            verde2.Moverficha(dado, tablero);
                            verde_2.Location = new Point(verde2.Pos_x, verde2.Pos_y);
                            comedora = false;
                            Comer(tablero, i, verde2);
                        }
                    }
                }

                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true; //se activa el dado
            }

            if (fichas[6].Terminado)//Si la ficha en cuestion ha terminado
            {
                verde_3.Visible = false; 
                terminadosverde++;
                if (terminadosverde >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo verde");
                    Application.Exit();
                }
            }
        }

        private void verde_4_Click(object sender, EventArgs e)
        {
            if (fichas[7].Casa && dado == 5)
            {

                fichas[7].Sacarcasa("verde");
                verde_4.Location = new Point(verde4.Pos_x, verde4.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[7].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    verde4.Moverficha(dado, tablero);
                    verde_4.Location = new Point(verde4.Pos_x, verde4.Pos_y);
                    Comer(tablero, i, verde4);
                     while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            verde4.Moverficha(dado, tablero);
                            verde_4.Location = new Point(verde4.Pos_x, verde4.Pos_y);
                            comedora = false;
                            Comer(tablero, i, verde4);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[7].Terminado)
            {
                verde_4.Visible = false;
                terminadosverde++;
                if (terminadosverde >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo verde");
                    Application.Exit();
                }
            }
        }

       private void azul_1_Click(object sender, EventArgs e)
        {
            if (fichas[8].Casa && dado == 5)
            {

                fichas[8].Sacarcasa("azul");
                azul_1.Location = new Point(azul1.Pos_x, azul1.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[8].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    azul1.Moverficha(dado, tablero);
                    azul_1.Location = new Point(azul1.Pos_x, azul1.Pos_y);
                    Comer(tablero, i, azul1);
                     while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            azul1.Moverficha(dado, tablero);
                            azul_1.Location = new Point(azul1.Pos_x, azul1.Pos_y);
                            comedora = false;
                            Comer(tablero, i, azul1);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[8].Terminado)
            {
                azul_1.Visible = false;
                terminadosazul++;
                if (terminadosazul >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo azul");
                    Application.Exit();
                }
            }
        }

        private void azul_2_Click(object sender, EventArgs e)
        {
            if (fichas[9].Casa && dado == 5)
            {

                fichas[9].Sacarcasa("azul");
                azul_2.Location = new Point(azul2.Pos_x, azul2.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[9].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    azul2.Moverficha(dado, tablero);
                    azul_2.Location = new Point(azul2.Pos_x, azul2.Pos_y);
                    Comer(tablero, i, azul2);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            azul2.Moverficha(dado, tablero);
                            azul_2.Location = new Point(azul2.Pos_x, azul2.Pos_y);
                            comedora = false;
                            Comer(tablero, i, azul2);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[9].Terminado)
            {
                azul_2.Visible = false;
                terminadosazul++;
                if (terminadosazul >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo azul");
                    Application.Exit();
                }
            }
        }

        private void azul_3_Click(object sender, EventArgs e)
        {
            if (fichas[10].Casa && dado == 5)
            {

                fichas[10].Sacarcasa("azul");
                azul_3.Location = new Point(azul3.Pos_x, azul3.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[10].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    azul3.Moverficha(dado, tablero);
                    azul_3.Location = new Point(azul3.Pos_x, azul3.Pos_y);
                    Comer(tablero, i, azul3);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            azul3.Moverficha(dado, tablero);
                            azul_3.Location = new Point(azul3.Pos_x, azul3.Pos_y);
                            comedora = false;
                            Comer(tablero, i, azul3);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[10].Terminado)
            {
                azul_3.Visible = false;
                terminadosazul++;
                if (terminadosazul >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo azul");
                    Application.Exit();
                }
            }
        }

        private void azul_4_Click(object sender, EventArgs e)
        {
            if (fichas[11].Casa && dado == 5)
            {

                fichas[11].Sacarcasa("azul");
                azul_4.Location = new Point(azul4.Pos_x, azul4.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[11].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    azul4.Moverficha(dado, tablero);
                    azul_4.Location = new Point(azul4.Pos_x, azul4.Pos_y);
                    Comer(tablero, i, azul4);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            azul4.Moverficha(dado, tablero);
                            azul_4.Location = new Point(azul4.Pos_x, azul4.Pos_y);
                            comedora = false;
                            Comer(tablero, i, azul4);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[11].Terminado)
            {
                azul_4.Visible = false;
                terminadosazul++;
                if (terminadosazul >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo azul");
                    Application.Exit();
                }
            }
        }

        private void amarilla_1_Click(object sender, EventArgs e)
        {
            if (fichas[12].Casa && dado == 5)
            {

                fichas[12].Sacarcasa("amarillo");
                amarilla_1.Location = new Point(amarillo1.Pos_x, amarillo1.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[12].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    amarillo1.Moverficha(dado, tablero);
                    amarilla_1.Location = new Point(amarillo1.Pos_x, amarillo1.Pos_y);
                    Comer(tablero, i, amarillo1);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            amarillo1.Moverficha(dado, tablero);
                            amarilla_1.Location = new Point(amarillo1.Pos_x, amarillo1.Pos_y);
                            comedora = false;
                            Comer(tablero, i, amarillo1);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[12].Terminado)
            {
                amarilla_1.Visible = false;
                terminadosamarillo++;
                if (terminadosamarillo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo amarillo");
                    Application.Exit();
                }
            }
        }

        private void amarilla_2_Click(object sender, EventArgs e)
        {
            if (fichas[13].Casa && dado == 5)
            {

                fichas[13].Sacarcasa("amarillo");
                amarilla_2.Location = new Point(amarillo2.Pos_x, amarillo2.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[13].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    amarillo2.Moverficha(dado, tablero);
                    amarilla_2.Location = new Point(amarillo2.Pos_x, amarillo2.Pos_y);
                    Comer(tablero, i, amarillo2);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            amarillo2.Moverficha(dado, tablero);
                            amarilla_2.Location = new Point(amarillo2.Pos_x, amarillo2.Pos_y);
                            comedora = false;
                            Comer(tablero, i, amarillo2);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[13].Terminado)
            {
                amarilla_2.Visible = false;
                terminadosamarillo++;
                if (terminadosamarillo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo amarillo");
                    Application.Exit();
                }
            }
        }

        private void amarilla_3_Click(object sender, EventArgs e)
        {
            if (fichas[14].Casa && dado == 5)
            {

                fichas[14].Sacarcasa("amarillo");
                amarilla_3.Location = new Point(amarillo3.Pos_x, amarillo3.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[14].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    amarillo3.Moverficha(dado, tablero);
                    amarilla_3.Location = new Point(amarillo3.Pos_x, amarillo3.Pos_y);
                    Comer(tablero, i, amarillo3);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            amarillo3.Moverficha(dado, tablero);
                            amarilla_3.Location = new Point(amarillo3.Pos_x, amarillo3.Pos_y);
                            comedora = false;
                            Comer(tablero, i, amarillo3);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }

            if (fichas[14].Terminado)
            {
                amarilla_3.Visible = false;
                terminadosamarillo++;
                if (terminadosamarillo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo amarillo");
                    Application.Exit();
                }
            }
        }

        private void amarilla_4_Click(object sender, EventArgs e)
        {
            if (fichas[15].Casa && dado == 5)
            {

                fichas[15].Sacarcasa("amarillo");
                amarilla_4.Location = new Point(amarillo4.Pos_x, amarillo4.Pos_y);
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;

            }
            else if (fichas[15].Casa && dado != 5)
            {
                deshabilitar_fichas("todas");
                PasarTurno();
                dado_pb.Enabled = true;
            }
            else
            {
                for (int i = dado; i > 0; i--)
                {
                    amarillo4.Moverficha(dado, tablero);
                    amarilla_4.Location = new Point(amarillo4.Pos_x, amarillo4.Pos_y);
                    Comer(tablero, i, amarillo4);
                    while (comedora)
                    {
                        for (int j = dado; j > 0; j--)
                        {
                            amarillo4.Moverficha(dado, tablero);
                            amarilla_4.Location = new Point(amarillo4.Pos_x, amarillo4.Pos_y);
                            comedora = false;
                            Comer(tablero, i, amarillo4);
                        }
                    }

                }
                PasarTurno();
                deshabilitar_fichas("todas");
                dado_pb.Enabled = true;
            }
            if (fichas[15].Terminado)
            {
                amarilla_4.Visible = false;
                terminadosamarillo++; ;
                if (terminadosamarillo >= 4)
                {
                    MessageBox.Show("Ha ganado el equipo amarillo");
                    Application.Exit();
                }
            }
        }

        private void Dado_But_Click(object sender, EventArgs e)
        {
            dado = Convert.ToInt32(valor_dado.Text);
            switch (dado)
            {
                case 1:
                    valor_dado.Text = "1";

                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 2:
                    valor_dado.Text = "2";

                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 3:
                    valor_dado.Text = "3";

                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 4:
                    valor_dado.Text = "4";

                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 5:
                    valor_dado.Text = "5";

                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                case 6:
                    valor_dado.Text = "6";

                    //Evitamos que el jugador realice otra tirada 
                    dado_pb.Enabled = false;

                    habilitar_fichas(turno);//Activamos las fichas del color que le toca tirar
                    break;

                default:
                    //Ante cualquier problema pedimos que realice otra tirada
                    MessageBox.Show("Ha ocurrido un error, vuelve a intentarlo");
                    dado_pb.Enabled = true;
                    break;

            }

        }




        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            XBOX.Text = e.X.ToString();
            YBOX.Text = e.Y.ToString();
        }

        /*private void Tablero_pb_MouseClick(object sender, MouseEventArgs e)
        {
            XBOX.Text = Tablero_pb.PointToClient(Cursor.Position).X.ToString();
            YBOX.Text = Tablero_pb.PointToClient(Cursor.Position).Y.ToString();

        }*/


        private void Tablero_pb_MouseClick_1(object sender, MouseEventArgs e)
        {
            XBOX.Text = Tablero_pb.PointToClient(Cursor.Position).X.ToString();
            YBOX.Text = Tablero_pb.PointToClient(Cursor.Position).Y.ToString();
        }

      





  }
}
