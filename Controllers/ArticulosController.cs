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
    public class ArticulosController : ControllerBase
    {
        [HttpGet("{idFam}")]
        public IActionResult Get(int idFam)
        {
            Respuesta<List<Articulos>> oRespuesta = new Respuesta<List<Articulos>>();
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;
                    List<Articulos> listArticulos = new List<Articulos>();

                    cmd = new MySqlCommand("SELECT * FROM articulos WHERE art_fam = @idFam", conexion);
                    cmd.Parameters.AddWithValue("@idFam", idFam);

                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        while (dr.Read())
                        {
                            Articulos objarticulo = new Articulos();

                            objarticulo.art_id = Convert.ToInt32(dr["art_id"].ToString());
                            objarticulo.art_nom = dr["art_nom"].ToString();
                            objarticulo.art_orden = Convert.ToInt32(dr["art_orden"].ToString());
                            objarticulo.art_nom_corto = dr["art_nom_corto"].ToString();
                            objarticulo.art_imagen = dr["art_imagen"].ToString();
                            objarticulo.art_nom_largo = dr["art_nom_largo"].ToString();
                            objarticulo.art_color_fondo = dr["art_color_fondo"].ToString();
                            objarticulo.art_color_fuente = dr["art_color_fuente"].ToString();
                            objarticulo.art_prn_comanda = Convert.ToInt32(dr["art_prn_comanda"].ToString());
                            objarticulo.art_prn_auxiliar = Convert.ToInt32(dr["art_prn_auxiliar"].ToString());
                            objarticulo.art_fam = Convert.ToInt32(dr["art_fam"].ToString());
                            objarticulo.art_fam_comb = Convert.ToInt32(dr["art_fam_comb"].ToString());
                            objarticulo.art_fam_nota = Convert.ToInt32(dr["art_fam_nota"].ToString());
                            objarticulo.art_fam_suple = Convert.ToInt32(dr["art_fam_suple"].ToString());
                            objarticulo.art_tipo_iva = Convert.ToInt32(dr["art_tipo_iva"].ToString());
                            objarticulo.art_fav1 = Convert.ToInt32(dr["art_fav1"].ToString());
                            objarticulo.art_fav2 = Convert.ToInt32(dr["art_fav2"].ToString());
                            objarticulo.art_fav3 = Convert.ToInt32(dr["art_fav3"].ToString());
                            objarticulo.art_inc_comb = Convert.ToDouble(dr["art_inc_comb"].ToString());
                            objarticulo.art_prn_auxiliar2 = dr["art_prn_auxiliar2"].ToString();

                            listArticulos.Add(objarticulo);
                        }

                        oRespuesta.Exito = 1;
                        oRespuesta.Data = listArticulos;
                    }
                    else
                    {
                        oRespuesta.Mensaje = "No existe el artículo";
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
