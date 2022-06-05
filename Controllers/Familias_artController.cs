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
    public class Familias_artController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            Respuesta<List<Familias_art>> oRespuesta = new Respuesta<List<Familias_art>>();
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;
                    List<Familias_art> listFamilias = new List<Familias_art>();

                    cmd = new MySqlCommand("SELECT * FROM familias_art", conexion);

                    //se abre la conexión
                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        Familias_art objfamilia = new Familias_art();

                        objfamilia.fam_id = Convert.ToInt32(dr["fam_id"].ToString());
                        objfamilia.fam_nom = dr["fam_nom"].ToString();
                        objfamilia.fam_nom_corto = dr["fam_nom_corto"].ToString();
                        objfamilia.fam_imagen = dr["fam_imagen"].ToString();
                        objfamilia.fam_nom_largo = dr["fam_nom_largo"].ToString();
                        objfamilia.fam_color_fondo = dr["fam_color_fondo"].ToString();
                        objfamilia.fam_color_fuente = dr["fam_color_fuente"].ToString();
                        objfamilia.fam_orden = Convert.ToInt32(dr["fam_orden"].ToString());
                        objfamilia.fam_visible = Convert.ToInt32(dr["fam_visible"].ToString());
                        listFamilias.Add(objfamilia);
                    }

                    oRespuesta.Exito = 1;
                    oRespuesta.Data = listFamilias;
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
