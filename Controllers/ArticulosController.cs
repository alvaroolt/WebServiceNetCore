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
    public class ArticulosController : ControllerBase
    {
        // SOLICITUD GET PARA OBTENER LOS ARTÍCULOS SEGÚN EL ID DE CATEGORÍA/FAMILIA
        [HttpGet("{idFam}")]
        public IActionResult Get(int idFam)
        {
            // Get devuelve una instancia de Respuesta que trabaja con Lista de articulos,
            // en la que se incluye si hubo éxito, mensaje de error, y datos
            Respuesta<List<Articulos>> oRespuesta = new Respuesta<List<Articulos>>();
            try
            {
                //Conexión a la BD
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;

                    // Lista de artículos que recibirá oRespuesta en Data, para enviarse al cliente
                    List<Articulos> listArticulos = new List<Articulos>();

                    // la consulta obtiene todos los datos de artículos en los que coincida con el id de familia solicitado
                    cmd = new MySqlCommand("SELECT * FROM articulos WHERE art_fam = @idFam", conexion);
                    cmd.Parameters.AddWithValue("@idFam", idFam);

                    // se abre la conexión
                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    // si el DataReader tiene algo para leer
                    if (dr.Read())
                    {
                        while (dr.Read())
                        {
                            // instancia de articulo que se añade a la instancia de lista de articulos
                            Articulos objarticulo = new Articulos();

                            // campos de articulo
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

                            // se añade la instancia de articulo a la instancia de lista de articulos
                            listArticulos.Add(objarticulo);
                        }

                        // hubo éxito
                        oRespuesta.Exito = 1;
                        // la instancia de respuesta obtiene la lista de artículos en Data
                        oRespuesta.Data = listArticulos;
                    }
                    else
                    {
                        // no hubo éxito porque se encuentra el articulo en la BD
                        oRespuesta.Mensaje = "No existe el artículo";
                    }
                }
            }
            catch (Exception e)
            {
                // la instancia de respuesta obtiene 
                oRespuesta.Mensaje = e.Message;
            }
            return Ok(oRespuesta);
        }
    }
}
