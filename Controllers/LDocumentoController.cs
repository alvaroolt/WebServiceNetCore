using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using WebServiceApiRest.Models;
using WebServiceApiRest.Models.Response;

namespace WebServiceNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LDocumentoController : ControllerBase
    {

        [HttpPost]
        public IActionResult Add(Ldocumentos model, int numMesa)
        {
            Respuesta<Ldocumentos> oRespuesta = new Respuesta<Ldocumentos>();
            MySqlTransaction transaction = default;

            try
            {
                getTarifaByMesa(numMesa);
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();

                    conexion.Open();
                    cmd.Transaction = transaction;
                    transaction = conexion.BeginTransaction();

                    Ldocumentos objldocumento = new Ldocumentos();
                    objldocumento.ldoc_doc_id = model.ldoc_doc_id;
                    objldocumento.ldoc_tipo = model.ldoc_tipo;
                    objldocumento.ldoc_linea = model.ldoc_linea;
                    objldocumento.ldoc_art_id = model.ldoc_art_id;
                    objldocumento.ldoc_descripcion = model.ldoc_descripcion;
                    objldocumento.ldoc_cantidad = model.ldoc_cantidad;
                    objldocumento.ldoc_pvp = model.ldoc_pvp;
                    objldocumento.ldoc_dto = model.ldoc_dto;
                    objldocumento.ldoc_cdto = model.ldoc_cdto;
                    objldocumento.ldoc_importe = model.ldoc_importe;
                    objldocumento.ldoc_importe_pvp = model.ldoc_importe_pvp;
                    objldocumento.ldoc_iva = model.ldoc_iva;
                    objldocumento.ldoc_civa = model.ldoc_civa;
                    objldocumento.ldoc_tipo_iva = model.ldoc_tipo_iva;
                    objldocumento.ldoc_cant_prn = model.ldoc_cant_prn;
                    objldocumento.ldoc_ter_id = model.ldoc_ter_id;
                    objldocumento.ldoc_varia_notas = model.ldoc_varia_notas;
                    objldocumento.ldoc_err_prn = model.ldoc_err_prn;
                    objldocumento.ldoc_usuario = model.ldoc_usuario;



                    cmd = new MySqlCommand("insert into ldocumentos ...", conexion);
                    //cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    conexion.Close();

                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Línea añadida con éxito";
                }
            }
            catch (Exception e)
            {
                oRespuesta.Mensaje = e.Message;
                transaction.Rollback();
            }

            return Ok(oRespuesta);
        }

        private int getTarifaByMesa(int numMesa)
        {
            MySqlTransaction transaction = default;
            int tarifa = 1;
            int numSalon = 0;
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    MySqlDataReader dr = null;

                    conexion.Open();
                    cmd.Transaction = transaction;

                    cmd = new MySqlCommand("SELECT OBJSALON_N_SALON from objetos_salon where OBJSALON_ORDEN=@numMesa", conexion);
                    cmd.Parameters.AddWithValue("@numMesa", numMesa);
                    numSalon = cmd.ExecuteNonQuery();

                    cmd = new MySqlCommand("SELECT * FROM configuracion WHERE config_id = 1", conexion);
                    cmd.ExecuteNonQuery();

                    if (dr.Read())
                    {
                        tarifa = Convert.ToInt32(dr["CONFIG_TARIFA_SALON" + numSalon].ToString());
                    }

                    transaction.Commit();
                    conexion.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }

            return tarifa;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Respuesta<Ldocumentos> oRespuesta = new Respuesta<Ldocumentos>();
            MySqlTransaction transaction = default;

            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();

                    conexion.Open();
                    cmd.Transaction = transaction;
                    transaction = conexion.BeginTransaction();

                    cmd = new MySqlCommand("DELETE FROM ldocumentos WHERE LDOC_ID = @id", conexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    conexion.Close();

                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Línea eliminada con éxito";
                }
            }
            catch (Exception e)
            {
                oRespuesta.Mensaje = e.Message;
                transaction.Rollback();
            }

            return Ok(oRespuesta);
        }
    }
}
