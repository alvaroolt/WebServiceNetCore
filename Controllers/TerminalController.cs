using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServiceApiRest.Models;
using WebServiceApiRest.Models.Response;
using WebServiceNetCore.Models;

namespace WebServiceNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TerminalController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            Respuesta<List<Terminal>> oRespuesta = new Respuesta<List<Terminal>>();
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;
                    List<Terminal> listTerminales = new List<Terminal>();

                    cmd = new MySqlCommand("SELECT ter_id, ter_nom, ter_bloqueado FROM terminal", conexion);

                    // se abre la conexión
                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Terminal objterminal = new Terminal();

                        objterminal.ter_id = Convert.ToInt32(dr["ter_id"].ToString());
                        objterminal.ter_nom = dr["ter_nom"].ToString();
                        objterminal.ter_bloqueado = Convert.ToInt32(dr["ter_bloqueado"].ToString());

                        listTerminales.Add(objterminal);
                    }

                    oRespuesta.Exito = 1;
                    oRespuesta.Data = listTerminales;
                }
            }
            catch (Exception e)
            {
                oRespuesta.Mensaje = e.Message;
            }
            return Ok(oRespuesta);
        }

        [HttpPut]
        public IActionResult Update(Terminal terminal)
        {
            Respuesta<Terminal> oRespuesta = new Respuesta<Terminal>();

            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;

                    cmd = new MySqlCommand("update terminal set ter_bloqueado = @bloqueado where ter_id = @id;", conexion);
                    cmd.Parameters.AddWithValue("@bloqueado", terminal.ter_bloqueado);
                    cmd.Parameters.AddWithValue("@id", terminal.ter_id);

                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    oRespuesta.Exito = 1;
                    oRespuesta.Data = terminal;
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
