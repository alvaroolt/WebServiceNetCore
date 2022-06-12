using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebServiceApiRest.Models;
using WebServiceApiRest.Models.Response;

namespace WebServiceNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CodebarsController : ControllerBase
    {

        // HTTP GET de Codebars según el código de barras. Devuelve una instancia de la clase Respuesta en la que
        // se almacena la información del codebars en el oRespuesta.Data. En caso de error, se almacena en
        // oRespuesta.Mensaje el mensaje de error.
        [HttpGet("{codebars}")]
        public IActionResult Get(string codebars)
        {
            Respuesta<Codebars> oRespuesta = new Respuesta<Codebars>();
            try
            {
                //Conexión a la BD
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;

                    cmd = new MySqlCommand("SELECT * FROM codebars WHERE cbar_code = @codebar", conexion);
                    cmd.Parameters.AddWithValue("@codebar", codebars);

                    // se abre la conexión
                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    // si el DataReader tiene algo para leer
                    if (dr.Read())
                    {
                        Codebars objCodebar = new Codebars();

                        objCodebar.cbar_art_id = Convert.ToInt32(dr["cbar_art_id"].ToString());
                        objCodebar.cbar_code = dr["cbar_code"].ToString();

                        // hubo éxito
                        oRespuesta.Exito = 1;
                        // la instancia de respuesta obtiene la lista de artículos en Data
                        oRespuesta.Data = objCodebar;
                    }
                    else
                    {
                        // no hubo éxito porque se encuentra el articulo en la BD
                        oRespuesta.Mensaje = "No existe el código de barras";
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
