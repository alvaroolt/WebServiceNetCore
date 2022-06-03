using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServiceNetCore.Models
{
    public class Conexion
    {
        #region "PATRON SINGLETON"
        private static Conexion conexion = null;
        private Conexion() { }
        public static Conexion getInstance()
        {
            //si no existe una conexión, se crea una nueva
            if (conexion == null)
            {
                conexion = new Conexion();
            }
            return conexion;
        }
        #endregion

        public MySqlConnection ConexionDB()
        {
            MySqlConnection conexion = new MySqlConnection();

            //credenciales de la bd
            conexion.ConnectionString = "server=192.168.1.185;user=indusoft_factura;password=34026067mM;database=ids_tpv";
            //conexion.ConnectionString = "server=localhost;user=root;password=;database=ids_tpv";
            return conexion;
        }
    }
}
