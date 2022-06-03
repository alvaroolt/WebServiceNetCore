using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using WebServiceApiRest.Models;
using WebServiceApiRest.Models.Response;
using WebServiceNetCore.Models;

namespace WebServiceNetCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LDocumentoController : ControllerBase
    {

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
