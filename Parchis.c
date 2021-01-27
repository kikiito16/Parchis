#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>
//#include <my_global.h>

pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;
//----------------------------------------------------------------------------//
//Vamos a definir la estructuras para los conectados y la lista de conectados
typedef struct {
	char nombre[20];
	int socket;
}Conectado;

typedef struct {
	Conectado conectados[100];
	int num;
}ListaConectados;

ListaConectados miLista;

//Creamos una lista de sockets
int sockets[100];
int i;
char invitador [50];
char invitado[50];
int socket_invitado;
int socket_invitador;
//----------------------------------------------------------------------------//

//Vamos a definir el método DameGanador para la implementación: 1
//Buscar el ganador de la partida cuyo Identificador de partida es uno dado por el usuario	

void DameGanador (char id[30], char respuesta[60])
{
	
	//Construimos la consulta SQL
	MYSQL *conn;    //Preparamos las variables para usar la base de datos
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//inicializamos la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T10_BD",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	
	strcpy (consulta,"SELECT Ganador FROM Partida WHERE Id = '"); 
	strcat (consulta, id);
	strcat (consulta,"'");
	// hacemos la consulta 
	err=mysql_query (conn, consulta); 
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//recogemos el resultado de la consulta 
	resultado = mysql_store_result (conn); 
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "1/No existe esa partida \n");
	}
	else
	{
		printf ("El ganador de la partida es: %s\n", row[0] );
		sprintf(respuesta, "1/%s \n", row[0]);
	}
}
//----------------------------------------------------------------------------//

//Vamos a definir el método DameContra para la implementación: 2
//Buscar la contraseña del usuario cuya contraseña recibimos por parametro
void DameContra(char contra[30], char respuesta[512])
{
	//Construimos la consulta SQL
	MYSQL *conn;    //Preparamos las variables para usar la base de datos
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//inicializamos la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T10_BD",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy (consulta,"SELECT Psw FROM Jugador WHERE Us = '");
	strcat (consulta, contra);
	strcat (consulta,"'");
	// hacemos la consulta
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//recogemos el resultado de la consulta
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL)
	{
		printf ("No se han obtenido datos en la consulta\n");
		sprintf(respuesta, "2/No existe este usuario \n");
	}
	else
	{
		printf ("La contraseña es: %s\n", row[0] );
		sprintf(respuesta, "2/Tu clave es: %s ", row[0]);
	}
	printf("%s \n", respuesta);
	
}
//----------------------------------------------------------------------------//

//Vamos a definir el método DimeJugadores para la implementación: 3
//Buscar los jugadores que están jugando una partida cuyo identificador lo pasamos parametro
void DimeJugadores (char id[30], char respuesta[60])
{
	//Construimos la consulta SQL
	MYSQL *conn;    //Preparamos las variables para usar la base de datos
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//inicializamos la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T10_BD",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	strcpy(consulta,"SELECT jugadores FROM Historial WHERE Id_part = '");
	strcat(consulta, id);
	strcat(consulta, "'");
	
	//Hacemos la consulta
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//Recogemos el resultado de la consulta
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	if (row == NULL)
		sprintf (respuesta,"3/Este ID de partida no existe \n");
	
	else
	{
		printf("Los jugadores de la partida son: \n");
		sprintf(respuesta, "3/Los jugadores de la partida son: %s\n",row [0]);
	}
}
//----------------------------------------------------------------------------//

//Vamos a definir el método PonUsuario para la implementación: 4
//Añadiremos usuarios a nuestra ListaConectados recibido por parametro
int PonUsuario (ListaConectados *lista,char nombre[20], int socket_pon)
{	//Añade un nuevo conectado  (0 todo ok, -1 si esta llena)
	if (lista->num==100)
		return -1;
	else{
		strcpy(lista->conectados[lista->num].nombre,nombre);
		lista->conectados[lista->num].socket=socket_pon;
		lista->num++;
		printf("Conectado %d \n", socket_pon);
		return 0;
	}
}

//A partir del metodo PonUsuario creamos el metodo LogIn en el que pediremos 
//al cliente que se registre y lo añadiremos a nuestra ListaConectados
void LogIn(ListaConectados *l,char us[60], char psw[60], int socket_login, char respuesta[512])
{
	//Construimos la consulta SQL
	MYSQL *conn;    //Preparamos las variables para usar la base de datos
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [80];
	
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//inicializamos la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T10_BD",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	strcpy(consulta,"SELECT Psw FROM Jugador WHERE Us = '");
	strcat(consulta, us);
	strcat(consulta, "'");
	
	//Hacemos la consulta
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//Recogemos el resultado de la consulta
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL)
		sprintf (respuesta,"4/0\n"); //Manda un 0 si no encuentra ningun usuario
	
	else
	{
		if(strcmp(psw,row[0])==0){
			printf("Usuario: %s, Contraseña: %s\n", us, psw);
			sprintf(respuesta, "4/%s",us);
			pthread_mutex_lock( &mutex);
			PonUsuario(l,us,socket_login);	
			//printf("Guardo en la lista %d %s\n",i,us);
			i++;
			pthread_mutex_unlock( &mutex);
		}
		else{
			sprintf(respuesta, "4/0");
		}
	}
	printf ("%s\n",respuesta);
}

void Registro(ListaConectados *l,char us[60], char psw[60], int socket_registro, char respuesta[512])
{
	//Construimos la consulta SQL
	MYSQL *conn;    //Preparamos las variables para usar la base de datos
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [500];
	
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//inicializamos la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T10_BD",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn,"SELECT Us from Jugador");
	if (err!=0)
	{
		printf("Error al consultar datos de la base %u %s\n",
			   mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	
	resultado=mysql_store_result(conn);
	//Estructura matricial en memoria
	//cada fila contiene los datos de una partida
	//obtenemos los datos de una fila
	row=mysql_fetch_row(resultado);
	int encontrado=0;
	if (row==NULL){
		printf("No se han obtenido datos en la consulta\n");
		sprintf(respuesta,"9/1");
	}
	
	else
	{
		while ((row !=NULL)&&(encontrado==0))
		{
			//miramos si ya existe un jugador en la base de datos con el mismo nombre y contraseÃ±a
			if((strcmp(us,row[0])==0))
				//el jugador ya existe
				//envia un 0 al cliente para informar de que este jugador ya existe
			{
				sprintf(respuesta,"9/0");
				printf("%s \n",respuesta);
				encontrado=1;
			}
			row=mysql_fetch_row(resultado); //recorre toda la tabla
		}
		
		if (encontrado==0) 
			//Debido a que no ha encontrado ningun jugador así, lo añado a la base de datos
		{
			//creamos la consulta
			strcpy (consulta, "INSERT INTO Jugador VALUES ('");
			//añadimos el usuario
			strcat (consulta, us); 
			strcat (consulta, "','");
			//añadimos la contraseña
			strcat (consulta, psw); 
			strcat (consulta, "');");
			
			// Añadimos el usuario a la lista de conectados
			PonUsuario(l,us,socket_registro);
			
			
			err = mysql_query(conn, consulta);
			if (err!=0) 
				printf ("Error al introducir datos la base %u %s\n", mysql_errno(conn), mysql_error(conn));
			else
				//se han añadido los valores con exito
				//informamos al cliente 
				sprintf(respuesta,"9/%s",us);
			
			
		}
	}
}

//Vamos a eliminar a un usuario de la base de datos
void Eliminar_datos(ListaConectados *l,char us[60], char psw[60], int socket_eliminado, char respuesta[512])
{
	//Construimos la consulta SQL
	MYSQL *conn;    //Preparamos las variables para usar la base de datos
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	char consulta [500];
	
	
	//Creamos una conexion al servidor MYSQL
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	//inicializamos la conexion
	conn = mysql_real_connect (conn, "shiva2.upc.es","root", "mysql", "T10_BD",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit (1);
	}
	
	err=mysql_query(conn,"SELECT Us from Jugador");
	if (err!=0)
	{
		printf("Error al consultar datos de la base %u %s\n",
			   mysql_errno(conn),mysql_error(conn));
		exit(1);
	}
	
	resultado=mysql_store_result(conn);
	//Estructura matricial en memoria
	//cada fila contiene los datos de una partida
	//obtenemos los datos de una fila
	row=mysql_fetch_row(resultado);
	int encontrado=0;
	if (row==NULL){
		printf("No se han obtenido datos en la consulta\n");
		sprintf(respuesta,"15/0");  //error al obtener los datos
	}
	
	else
	{
		while ((row !=NULL)&&(encontrado==0))
		{
			//miramos si existe un jugador en la base de datos con el mismo nombre
			if((strcmp(us,row[0])==0))
				encontrado=1; //el jugador existe, vamos a comprobar si coincide la contraseña
			
			row=mysql_fetch_row(resultado); //recorre toda la tabla
		}
		
		if (encontrado==1) 
			//Debido a que no ha encontrado ningun jugador así, lo añado a la base de datos
		{
			//creamos la consulta
			strcpy(consulta,"SELECT Psw FROM Jugador WHERE Us = '");
			strcat(consulta, us);
			strcat(consulta, "'");
			
			err=mysql_query (conn, consulta);
			
			if (err!=0) {
				printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				exit (1);
			}
			
			//Recogemos el resultado de la consulta
			resultado = mysql_store_result (conn);
			row = mysql_fetch_row (resultado);
			
			if (row == NULL)
				sprintf (respuesta,"15/0 \n"); //error al obtener los datos
			
			else //En caso de encontrar la contraseña del usuario, compararla con la que nos ha pasado el cliente
			{
				if(strcmp(psw,row[0])==0){
					printf("Usuario: %s, Contraseña: %s\n", us, psw);
					//sprintf(respuesta, "15/%s",us);
					strcpy (consulta, "DELETE FROM Jugador WHERE Us ='");
					strcat (consulta, us); 
					strcat (consulta, "';");
					//añadimos la contraseña
					err = mysql_query(conn, consulta);
					if (err!=0) 
						printf ("Error al introducir datos la base %u %s\n", mysql_errno(conn), mysql_error(conn));
					else
						//se han añadido los valores con exito
						//informamos al cliente 
						sprintf(respuesta,"15/%s",us);
					
					printf("%s \n",consulta);
					
					
				}
				else{
					sprintf(respuesta, "15/1"); //Manda un 1 si no coincide la contraseña
				}
			}
			printf ("%s\n",respuesta);
			
		}
		
		if(encontrado==0)
			sprintf(respuesta,"15/-1"); //Manda un -1 si no encuentra el usuario
	}
}

//Devuelve la posicion 0 si está en la lista el usuario o -1 si no esta en la lista el usuario.
int DamePos (ListaConectados *lista,char nombre[20])
{
	int i=0;
	int encontrado=0;
	while ((i<lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre,nombre)==0)
			encontrado=1;
		if(!encontrado)
			i++;		
	}
	if (encontrado)
		return i;
	else 
		return -1;
}

int DameSocket (ListaConectados *lista, char nombre [20])
{ //Devuelve el socket o -1 si no esta en la lista
	int i = 0;
	int encontrado =0;
	while ((i<lista->num)&&(encontrado == 0))
	{
		if (strcmp(lista->conectados[i].nombre, nombre) == 0)
		{
			encontrado = 1;
			return lista->conectados[i].socket;
		}
		i++;
	}
	if (!encontrado)
		return -1;
}




int EliminaUsuario (ListaConectados *lista,char nombre[20])
{	//retorna 0 si se elimina correctamente o -1 si el usuario no está en la lista
	int pos= DamePos(lista,nombre);
	if (pos==-1)
		return -1;
	else{
		int i;
		for (i=pos;i<lista->num-1;i++)
		{
			lista->conectados[i]=lista->conectados[i+1];
		}
		lista->num--;
		return 0;
	}
	
}


//Nos devuelve todos los conectados separados por /. Primero pone el número de conectados
// Ejemplo: 3/Enric/Juan/Pep
void DameConectados (ListaConectados *lista,char conectados[300])
{    
	sprintf(conectados,"%d/", lista->num);
	
	for (int i=0;  i<lista->num; i++)
	{
		sprintf(conectados, "%s%s,", conectados, lista->conectados[i].nombre);
	}
	
	if (lista->num==0)
		sprintf(conectados,"%d", lista->num);
}
//----------------------------------------------------------------------------//

/*Crearemos el siguiente metodo en el que hemos declarado la solución al error 
de conexión de múltles clientes y atenderlos adecuadamente que lo llamaremos AtenderCliente
y sirve para llamar a las diversas funciones*/


//Método para saber que hacer en cada caso
void *AtenderCliente (void *socket)
{
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
	int ret;
	
	
	char consulta [80];
	char peticion[512];
	char respuesta[512];
	char notificacion[512];
	
	
	
	int terminar =0;
	// Entramos en un bucle para atender todas las peticiones de este cliente
	//hasta que se desconecte
	while (terminar ==0)
	{
		// Ahora recibimos la peticion
		ret=read(sock_conn,peticion, sizeof(peticion));
		printf ("Recibido,socket: %d\n",sock_conn);
		
		// Tenemos que aÃ±adirle la marca de fin de string
		// para que no escriba lo que hay despues en el buffer
		peticion[ret]='\0';
		
		
		printf ("Peticion: %s\n",peticion);
		
		// vamos a ver que quieren
		char *p = strtok( peticion, "/");
		int codigo =  atoi (p);
		// Ya tenemos el codigo de la peticion
		char nombre[20];
		char us[60];
		
		if (codigo !=0)
		{
			
			p = strtok( NULL, "/");
			strcpy (nombre, p);
			// Ya tenemos el nombre
			printf ("Codigo: %d, Nombre: %s\n", codigo, nombre);
		}
		
		if (codigo ==0) //Peticion para la desconexion
		{
			terminar=1;
			pthread_mutex_lock( &mutex);
			EliminaUsuario(&miLista,nombre);
			char conectados [512];
			DameConectados(&miLista, conectados);
			sprintf(notificacion,"5/%s ",conectados);			
			for(int j =0; j<miLista.num;j++)
			{
				write(miLista.conectados[i].socket,notificacion,strlen(notificacion));
				printf("%s\n",notificacion);
			}
			strcpy(notificacion,"");
			pthread_mutex_unlock( &mutex);
		}
		
		else if (codigo ==1) //Peticion de la implementacion: 1 (DameGanador)
		{
			DameGanador(nombre,respuesta);
		}
		
		else if (codigo ==2){ //Peticion de la implementacion: 2 (DameContra)
			DameContra(nombre,respuesta);
		}
		
		else if(codigo==3) //Peticion de la implementacion: 3 (DimeJugadores)
		{
			DimeJugadores(nombre,respuesta);
		}
		
		else if(codigo==4) //Peticion de la implementacion: 4 (LogIn)
		{						
			char psw[60];
			strcpy(us,nombre);
			p = strtok( NULL, "/");
			strcpy(psw,p);
			//printf("Socket: %d \n",i);
			LogIn(&miLista,us,psw,sock_conn,respuesta);
			
			if (strcmp(respuesta,us)==0){
				char conectados [512];
				pthread_mutex_lock( &mutex);
				DameConectados(&miLista, conectados);
				sprintf(notificacion,"5/%s", conectados);			
				for(int j =0; j<=miLista.num;j++)
				{
					write(miLista.conectados[j].socket,notificacion,strlen(notificacion));
					printf("%s\n",notificacion);
				}
				strcpy(notificacion,"");
				pthread_mutex_unlock( &mutex);
			}
			//write (sock_conn,respuesta, strlen(respuesta));
		}
		
		else if (codigo==5) //Petición para registrarse
		{
			char psw[60];
			strcpy(us,nombre);
			p = strtok( NULL, "/");
			strcpy(psw,p);
			pthread_mutex_lock( &mutex);
			Registro(&miLista,us,psw,sock_conn,respuesta);
			pthread_mutex_unlock( &mutex);
			
			if (strcmp(respuesta,us)==0){
				
				char conectados [512];
				pthread_mutex_lock( &mutex);
				DameConectados(&miLista, conectados);
				sprintf(notificacion,"5/%s", conectados);			
				for(int j =0; j<=miLista.num;j++)
				{
					write(miLista.conectados[j].socket,notificacion,strlen(notificacion));
					printf("%s\n",notificacion);
				}
				strcpy(notificacion,"");
				pthread_mutex_unlock( &mutex);
			}
			
		}
		
		else if (codigo == 6)
			// cuando el usuario invita a otra persona
		{
			strcpy(invitador,nombre);
			p = strtok( NULL, "/");
			strcpy (invitado, p);
			//int socket_invitado;
			pthread_mutex_lock( &mutex);
			socket_invitado = DameSocket(&miLista,invitado);  //obtenemos el socket del invitado
			pthread_mutex_unlock( &mutex);
			printf ("Voy a invitar a %d, %s \n", socket_invitado, invitado);
			strcpy(respuesta,"");
			sprintf (respuesta,"6/%s/%s",invitador, invitado);
			write (socket_invitado,respuesta, strlen(respuesta));  //le enviamos la solicitud solo al invitado
			strcpy(respuesta,"");
			printf("Este es el socket invitado: %d \n", socket_invitado);
			printf("Invitador: %s, invitado: %s \n", invitador, invitado);
			//prueba
			sprintf(respuesta,"11/%s/%s",invitador,invitado);
			for (i=0; i<miLista.num; i++)
			{  //enviamos este mensaje a todos los clientes
				write (miLista.conectados[i].socket,respuesta, strlen(respuesta)); 
			}
			
		}
		else if (codigo == 7)
			// cuando la persona invitada responde
		{
			char decision [10];
			//int socket_invitador;
			p = strtok( NULL, "/");
			strcpy (decision, p);
			
			if (strcmp(decision, "SI")==0)
			{
				pthread_mutex_lock( &mutex);
				socket_invitador = DameSocket(&miLista,invitador);  //buscamos el socket de la persona que nos ha invitado
				sprintf (respuesta,"7/%d",1);
				pthread_mutex_unlock( &mutex);
			}
			if (strcmp(decision, "NO")==0)
			{
				printf("invitador: %s \n",invitador);
				pthread_mutex_lock( &mutex);
				socket_invitador = DameSocket(&miLista,invitador);  //buscamos el socket de la persona que nos ha invitado
				sprintf (respuesta,"7/%d",0);
				pthread_mutex_unlock( &mutex);
			}
			write (socket_invitador,respuesta, strlen(respuesta));  //respondemos solo a quien nos ha invitado
			printf("%s\n",respuesta);
			strcpy(respuesta,"");
		}
		
		else if (codigo == 8) //chat
		{
			char mensaje[200];
			strcpy(mensaje, p);
			sprintf(respuesta,"8/%s: %s",us,mensaje);
			pthread_mutex_lock( &mutex);
			for (i=0; i<miLista.num; i++){  //enviamos este mensaje a todos los clientes
				write (miLista.conectados[i].socket,respuesta, strlen(respuesta)); 
			}
			pthread_mutex_unlock( &mutex);
			strcpy(respuesta,"");
		}	
		
		else if (codigo == 9)  //Codigo para pasar los movimientos de las fichas
		{
			char pb [10] ;
			int posx;
			int posy;
			char turno[20];
			strcpy(pb,nombre);
			p=strtok(NULL,"/");
			posx=atoi(p);
			p=strtok(NULL,"/");
			posy=atoi(p);
			p=strtok(NULL,"/");
			strcpy(turno,p);
			sprintf(respuesta, "10/%s/%d/%d",pb,posx,posy);
			
			if (strcmp(turno,invitador)==0)
				write (socket_invitado,respuesta, strlen(respuesta));
			else
				write (socket_invitador,respuesta, strlen(respuesta));
			strcpy(respuesta,"");
		}
		
		else if (codigo==10)  //Marca el inicio de partida
		{
			char jugador_inicio[20];
			char color_inicio[20];
			strcpy(jugador_inicio,nombre);
			p=strtok(NULL,"/");
			strcpy(color_inicio,p);
			sprintf(respuesta,"12/%s/%s",jugador_inicio, color_inicio);
			printf("%s \n", respuesta);
			write (socket_invitado,respuesta, strlen(respuesta));
			strcpy(respuesta,"");
		}
		
		else if(codigo==11)  //Codigo para pasar el turno entre jugadores
		{
			char turno_color[20];
			strcpy(turno_color,nombre);
			char jugador_turno[20];
			char jugador_actual[20];
			p=strtok(NULL,"/");
			strcpy(jugador_turno,p);
			strcpy(respuesta,"");
			pthread_mutex_lock( &mutex);
			if (strcmp(jugador_turno,invitador)==0)
			{
				strcpy(jugador_actual,invitado);
				sprintf(respuesta,"13/%s/%s",turno_color,jugador_actual);
				write (socket_invitado,respuesta, strlen(respuesta));
				if (strcmp(jugador_actual,invitado)==0)
				{
					char respuesta2[500];
					sprintf(respuesta2,"14/%s",jugador_actual);
					write (socket_invitador,respuesta2, strlen(respuesta2));
				}
			}
			else
			{
				strcpy(jugador_actual,invitador);
				sprintf(respuesta,"13/%s/%s",turno_color,jugador_actual);
				write (socket_invitador,respuesta, strlen(respuesta));
				if (strcmp(jugador_actual,invitador)==0)
				{
					char respuesta2[500];
					sprintf(respuesta2,"14/%s",jugador_actual);
					write (socket_invitado,respuesta2, strlen(respuesta2));
				}
			}
			strcpy(respuesta,"");
			pthread_mutex_unlock( &mutex);
		}
		
		else if (codigo==12)
		{
			char usu_eliminado[20];
			char psw_eliminado [20];
			
			strcpy(usu_eliminado,nombre);
			p=strtok(NULL,"/");
			strcpy(psw_eliminado,p);
			pthread_mutex_lock( &mutex);
			Eliminar_datos(&miLista,usu_eliminado,psw_eliminado,sock_conn,respuesta);
			pthread_mutex_unlock( &mutex);
			char comprobacion[20];
			strcpy(comprobacion,"15/");
			strcat(comprobacion,usu_eliminado);
			if (strcmp(comprobacion,respuesta)==0)
			{
				terminar=1;
				pthread_mutex_lock( &mutex);
				EliminaUsuario(&miLista,nombre);
				char conectados [512];
				DameConectados(&miLista, conectados);
				sprintf(notificacion,"5/%s ",conectados);			
				for(int j =0; j<miLista.num;j++)
				{
					write(miLista.conectados[i].socket,notificacion,strlen(notificacion));
					printf("%s\n",notificacion);
				}
				strcpy(notificacion,"");
				pthread_mutex_unlock( &mutex);
			}
		}
		
		else if (codigo==13)  //Codigo para determinar que fichas han terminado
		{
			char color[20];
			char jugador[20];
			int fichas_terminadas;
			strcpy(color,nombre);
			p=strtok(NULL,"/");
			strcpy(jugador,p);
			p=strtok(NULL,"/");
			fichas_terminadas=atoi(p);
			sprintf(respuesta,"16/%s/%d",color,fichas_terminadas);
			printf("%s\n",respuesta);
			if (strcmp(jugador,invitador)==0)
				write (socket_invitador,respuesta, strlen(respuesta));
			else
				write (socket_invitado,respuesta, strlen(respuesta));
			strcpy(respuesta,"");
			
		}
		
		if ((codigo !=0) && (codigo!=6) && (codigo!= 7) && (codigo!= 8) && (codigo!=10) && (codigo!=11) &&(codigo!=13))
		{
			
			printf ("Respuesta: %s\n", respuesta);
			// Enviamos respuesta
			write (sock_conn,respuesta, strlen(respuesta));
		}
		if ((codigo==0)||(codigo ==1)||(codigo ==2)||(codigo ==3)||(codigo ==4) ||(codigo ==5) || (codigo==12)) //Mientras se están procesando todas las acciones
		{
			
			pthread_mutex_lock( &mutex);
			char conectados[512];
			DameConectados (&miLista, conectados);
			
			sprintf(notificacion,"5/%s",conectados);
			
			printf("%s\n",notificacion);
			
			//notificar a todos los clientes conectados
			for(int j =0; j<miLista.num;j++)
			{
				write(miLista.conectados[j].socket,notificacion,strlen(notificacion));
			}
			strcpy(notificacion,"");
			pthread_mutex_unlock( &mutex); //ya puedes interrumpirme
		}
		
		
	}
	close(sock_conn);
}

//----------------------------------------------------------------------------//

int main(int argc, char *argv[])
{
	miLista.num=0;
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	int puerto = 50079;
	//int puerto = 9010;
	
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	
	// Fem el bind al port
	memset(&serv_adr, 0, sizeof(serv_adr)); // inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina.
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// establecemos el puerto de escucha
	serv_adr.sin_port = htons(puerto);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	
	if (listen(sock_listen, 3) < 0)
		printf("Error en el Listen");
	
	
	i=0;
	pthread_t thread[100];
	
	// Infinito
	for (;;){
		
		printf ("Escuchando\n");		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion: %d\n",sock_conn);
		//sock_conn es el socket que usaremos para este cliente		
		
		sockets[i] = sock_conn;
		
		pthread_create (&thread[i],NULL,AtenderCliente,&sockets[i]);
		i=i+1;
		
	}
}
