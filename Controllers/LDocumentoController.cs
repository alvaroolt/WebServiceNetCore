using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public IActionResult Add(ArrayList paramList) // int idArticulo, int numMesa
        {
            int idArticulo = 0;
            int numMesa = 0;
            int idTerminal = 0;

            if (paramList.Count > 0)
            {
                idArticulo = Convert.ToInt32(Newtonsoft.Json.JsonConvert.DeserializeObject(paramList[0].ToString()));
                numMesa = Convert.ToInt32(Newtonsoft.Json.JsonConvert.DeserializeObject(paramList[1].ToString()));
                idTerminal = Convert.ToInt32(Newtonsoft.Json.JsonConvert.DeserializeObject(paramList[2].ToString()));
            }

            Respuesta<Ldocumentos> oRespuesta = new Respuesta<Ldocumentos>();
            MySqlTransaction transaction = default;

            try
            {
                int tarifa = getTarifaByMesa(numMesa);
                Articulos objarticulo = getArticuloParaAdd(idArticulo, tarifa);
                double ivaArticulo = getIvaArticulo(objarticulo);
                int idDocumento = getIdDocumentoByMesa(numMesa);

                Documentos objdocumento = new Documentos();
                objdocumento = objdocumento.getDocumento(idDocumento);

                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();

                    conexion.Open();
                    cmd.Transaction = transaction;
                    transaction = conexion.BeginTransaction();

                    Ldocumentos objldocumento = new Ldocumentos();
                    objldocumento.ldoc_doc_id = idDocumento;
                    objldocumento.ldoc_tipo = "A";
                    objldocumento.ldoc_linea = LDocumentoController.getUltimaLinea(idDocumento) + 10;
                    objldocumento.ldoc_art_id = idArticulo;
                    objldocumento.ldoc_descripcion = objarticulo.art_nom;
                    objldocumento.ldoc_cantidad = 1;
                    objldocumento.ldoc_pvp = Math.Round(objarticulo.art_pvp, 5);
                    objldocumento.ldoc_dto = 0;
                    objldocumento.ldoc_cdto = 0;
                    objldocumento.ldoc_iva = ivaArticulo;
                    objldocumento.ldoc_tipo_iva = objarticulo.art_tipo_iva;
                    objldocumento.ldoc_importe_pvp = Math.Round(objarticulo.art_pvp * 1, 5);
                    objldocumento.ldoc_importe = Math.Round(objldocumento.ldoc_importe_pvp / (1 + objldocumento.ldoc_iva / 100), 5);
                    objldocumento.ldoc_civa = Math.Round(objldocumento.ldoc_importe_pvp - objldocumento.ldoc_importe, 5);
                    objldocumento.ldoc_ter_id = 1;
                    objldocumento.ldoc_cant_prn = 1;
                    objldocumento.ldoc_err_prn = 1;
                    objldocumento.ldoc_usuario = 0;

                    string sCommand = String.Format("INSERT INTO LDOCUMENTOS (LDOC_DOC_ID, LDOC_TIPO, LDOC_LINEA, LDOC_ART_ID, LDOC_DESCRIPCION, " +
                        "LDOC_CANTIDAD, LDOC_PVP, LDOC_DTO, LDOC_CDTO, LDOC_IMPORTE, LDOC_IMPORTE_PVP, LDOC_IVA, LDOC_CIVA, LDOC_TIPO_IVA,LDOC_CANT_PRN," +
                        "LDOC_TER_ID,LDOC_ERR_PRN,LDOC_USUARIO)  VALUES({0}, '{1}', {2}, {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13},{14}," +
                        "{15},{16},{17})" +
                        "", objldocumento.ldoc_doc_id, objldocumento.ldoc_tipo, objldocumento.ldoc_linea, objldocumento.ldoc_art_id, objldocumento.ldoc_descripcion, objldocumento.ldoc_cantidad, objldocumento.ldoc_pvp.ToString().Replace(",", "."), objldocumento.ldoc_dto, objldocumento.ldoc_cdto, objldocumento.ldoc_importe.ToString().Replace(",", "."), objldocumento.ldoc_importe_pvp.ToString().Replace(",", "."), objldocumento.ldoc_iva, objldocumento.ldoc_civa.ToString().Replace(",", "."), objldocumento.ldoc_tipo_iva, objldocumento.ldoc_cant_prn, objldocumento.ldoc_ter_id, objldocumento.ldoc_err_prn, objldocumento.ldoc_usuario);

                    cmd = new MySqlCommand(sCommand, conexion);
                    cmd.ExecuteNonQuery();

                    if (idDocumento == 0)
                    {
                        //objdocumento.listdoc = new List<Ldocumentos>();
                        //objdocumento.listdoc.Add(objldocumento);
                        //objdocumento.
                    }
                    else
                    {
                        objdocumento.recalcularPrecio(idDocumento, objldocumento.ldoc_importe_pvp, objarticulo.art_tipo_iva, ivaArticulo, cmd);
                    }

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
            int tarifa = 1;
            int numSalon = 0;
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    MySqlDataReader dr = null;

                    conexion.Open();

                    cmd = new MySqlCommand("SELECT OBJSALON_N_SALON from objetos_salon where OBJSALON_ORDEN=@numMesa", conexion);
                    cmd.Parameters.AddWithValue("@numMesa", numMesa);
                    //numSalon = cmd.ExecuteNonQuery();

                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        numSalon = Convert.ToInt32(dr["OBJSALON_N_SALON"].ToString());
                    }

                    cmd = new MySqlCommand("SELECT * FROM configuracion WHERE config_id = 1", conexion);
                    dr.Close();

                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        tarifa = Convert.ToInt32(dr["CONFIG_TARIFA_SALON" + numSalon].ToString());
                        if (tarifa == 0)
                        {
                            tarifa = 1;
                        }
                    }

                    conexion.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }

            return tarifa;
        }

        private Articulos getArticuloParaAdd(int idArticulo, int idTarifa)
        {
            Articulos objarticulo = new Articulos();

            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    MySqlDataReader dr = null;

                    conexion.Open();

                    cmd = new MySqlCommand("SELECT articulos.ART_ID, articulos.ART_NOM, articulos.ART_TIPO_IVA ,ltarifas.LTARIFA_PVP FROM articulos INNER JOIN ltarifas ON articulos.ART_ID = ltarifas.LTARIFA_ART WHERE articulos.ART_ID = @idArticulo AND ltarifas.LTARIFA_ID = @idTarifa", conexion);
                    cmd.Parameters.AddWithValue("@idArticulo", idArticulo);
                    cmd.Parameters.AddWithValue("@idTarifa", idTarifa);
                    //cmd.Parameters.AddWithValue("@idTarifa", idTarifa);

                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        //pvp = Convert.ToDouble(dr["LTARIFA_PVP"].ToString());
                        objarticulo.art_id = idArticulo;
                        objarticulo.art_nom = dr["art_nom"].ToString();
                        objarticulo.art_tipo_iva = Convert.ToInt32(dr["art_tipo_iva"].ToString());
                        objarticulo.art_pvp = Convert.ToDouble(dr["ltarifa_pvp"].ToString());

                    }

                    conexion.Close();
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }

            return objarticulo;
        }

        private double getIvaArticulo(Articulos articulo)
        {
            double ivaArticulo = 0;
            int tipoIvaArticulo = articulo.art_tipo_iva;

            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {

                    MySqlCommand cmd = new MySqlCommand();
                    MySqlDataReader dr = null;

                    conexion.Open();

                    cmd = new MySqlCommand("SELECT empresa.EMP_IVA, tipo_iva.TIVA_ID, tipo_iva.TIVA_IVA1, tipo_iva.TIVA_IVA2, tipo_iva.TIVA_IVA3, tipo_iva.TIVA_IVA4 FROM empresa INNER JOIN tipo_iva ON empresa.EMP_IVA = tipo_iva.TIVA_ID WHERE empresa.EMP_ID = 1; ", conexion);

                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        switch (tipoIvaArticulo)
                        {
                            case 0:
                                ivaArticulo = Convert.ToInt32(dr["TIVA_IVA1"].ToString());
                                break;
                            case 1:
                                ivaArticulo = Convert.ToInt32(dr["TIVA_IVA2"].ToString());
                                break;
                            case 2:
                                ivaArticulo = Convert.ToInt32(dr["TIVA_IVA3"].ToString());
                                break;
                            case 3:
                                ivaArticulo = Convert.ToInt32(dr["TIVA_IVA4"].ToString());
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }

            return ivaArticulo;
        }

        private int getIdDocumentoByMesa(int numMesa)
        {
            int idDocumento = 0;

            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {

                    MySqlCommand cmd = new MySqlCommand();
                    MySqlDataReader dr = null;

                    conexion.Open();

                    cmd = new MySqlCommand("SELECT * FROM documentos WHERE doc_mesa = @numMesa AND doc_serie = 'MESA'", conexion);
                    cmd.Parameters.AddWithValue("@numMesa", numMesa);

                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        idDocumento = Convert.ToInt32(dr["DOC_ID"].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }

            return idDocumento;
        }

        public static int getUltimaLinea(int idDocumento)
        {
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {

                    conexion.Open();

                    string sqlMax = string.Format("SELECT coalesce(max(LDOC_LINEA),0) FROM ldocumentos where LDOC_DOC_ID=@idDocumento");
                    cmd = new MySqlCommand(sqlMax, conexion);
                    cmd.Parameters.AddWithValue("@idDocumento", idDocumento);

                    return (int)cmd.ExecuteScalar();

                }
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        [HttpDelete("{idLDocumento:int}/{idArticulo:int}/{numMesa:int}")]
        public IActionResult Delete(int idLDocumento, int idArticulo, int numMesa)
        {

            Respuesta<Ldocumentos> oRespuesta = new Respuesta<Ldocumentos>();
            MySqlTransaction transaction = default;

            int tarifa = getTarifaByMesa(numMesa);
            Articulos objarticulo = getArticuloParaAdd(idArticulo, tarifa);
            double ivaArticulo = getIvaArticulo(objarticulo);
            int idDocumento = getIdDocumentoByMesa(numMesa);

            Ldocumentos objldocumento = new Ldocumentos();
            objldocumento = objldocumento.getLDocumento(idLDocumento);

            Documentos objdocumento = new Documentos();
            objdocumento = objdocumento.getDocumento(idDocumento);

            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = new MySqlCommand();

                    conexion.Open();
                    cmd.Transaction = transaction;
                    transaction = conexion.BeginTransaction();

                    cmd = new MySqlCommand("DELETE FROM ldocumentos WHERE LDOC_ID = @id", conexion);
                    cmd.Parameters.AddWithValue("@id", idLDocumento);
                    cmd.ExecuteNonQuery();

                    objdocumento.recalcularPrecio(idDocumento, -objldocumento.ldoc_importe_pvp, objarticulo.art_tipo_iva, ivaArticulo, cmd);

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

        [HttpPut]
        public IActionResult Update(Ldocumentos ldocumento)
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

                    Documentos objdocumento = new Documentos();
                    objdocumento = objdocumento.getDocumento((int)ldocumento.ldoc_doc_id);

                    objdocumento.recalcularPrecio(Convert.ToInt32(ldocumento.ldoc_doc_id), -ldocumento.ldoc_importe_pvp, ldocumento.ldoc_tipo_iva, ldocumento.ldoc_civa, cmd);

                    double importePvp = Math.Round(ldocumento.ldoc_importe_pvp * ldocumento.ldoc_cantidad);
                    double importe = Math.Round(importePvp / (1 + ldocumento.ldoc_iva / 100), 5);
                    double civa = Math.Round(importePvp - importe, 5);
                    double cantidad = ldocumento.ldoc_cantidad;

                    cmd = new MySqlCommand("update ldocumentos set ldoc_pvp = @pvp, ldoc_importe_pvp = @importePvp, ldoc_importe = @importe, ldoc_civa = @civa, ldoc_cantidad = @cantidad, ldoc_ter_id = @terminal where ldoc_id = @id;", conexion);
                    cmd.Parameters.AddWithValue("@pvp", Math.Round(ldocumento.ldoc_importe_pvp, 5));
                    cmd.Parameters.AddWithValue("@importePvp", importePvp);
                    cmd.Parameters.AddWithValue("@importe", importe);
                    cmd.Parameters.AddWithValue("@civa", civa);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("terminal", ldocumento.ldoc_ter_id);
                    cmd.Parameters.AddWithValue("@id", ldocumento.ldoc_id);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();

                    objdocumento.recalcularPrecio(Convert.ToInt32(ldocumento.ldoc_doc_id), importePvp, ldocumento.ldoc_tipo_iva, civa, cmd);

                    conexion.Close();

                    oRespuesta.Exito = 1;
                    oRespuesta.Mensaje = "Línea actualizada con éxito";
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
