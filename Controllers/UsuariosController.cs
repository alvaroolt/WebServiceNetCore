using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using WebServiceApiRest.Models;
using WebServiceApiRest.Models.Response;

namespace WebServiceNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        // HTTP GET de usuario según contraseña. Devuelve una instancia de la clase Respuesta en la que
        // se almacena la información del usuario en el oRespuesta.Data. En caso de error, se almacena en
        // oRespuesta.Mensaje el mensaje de error.
        [HttpGet("{pass}")]
        public IActionResult Get(string pass)
        {
            Respuesta<Usuarios> oRespuesta = new Respuesta<Usuarios>();
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;
                    Usuarios usuario = new Usuarios();

                    cmd = new MySqlCommand("SELECT * FROM `usuarios` WHERE USER_PWD = @pass", conexion);
                    cmd.Parameters.AddWithValue("@pass", pass);

                    //se abre la conexión
                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        usuario.user_id = Convert.ToInt32(dr["user_id"].ToString());
                        usuario.user_nom = dr["user_nom"].ToString();
                        usuario.user_pwd = dr["user_pwd"].ToString();
                        usuario.user_rol = Convert.ToInt32(dr["user_admin"].ToString()) == 0 ? Usuarios.RolUsuario.Usuario : Usuarios.RolUsuario.Administrador;

                        oRespuesta.Exito = 1;
                        oRespuesta.Data = usuario;
                    }
                    else
                    {
                        oRespuesta.Mensaje = "No se encuentra el usuario";
                    }


                }
            }
            catch (Exception e)
            {
                oRespuesta.Mensaje = e.Message;
            }
            return Ok(oRespuesta);
        }
    }
}
