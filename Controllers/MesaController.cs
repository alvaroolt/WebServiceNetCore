using Microsoft.AspNetCore.Http;
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
    public class MesaController : ControllerBase
    {
        [HttpGet("{orden}")]
        public IActionResult Get(int orden)
        {
            Respuesta oRespuesta = new Respuesta();
            try
            {
                using (MySqlConnection conexion = Conexion.getInstance().ConexionDB())
                {
                    MySqlCommand cmd = null;
                    MySqlDataReader dr = null;
                    List<Documentos> listDocumentos = new List<Documentos>();

                    cmd = new MySqlCommand("SELECT objsalon_orden FROM objetos_salon WHERE OBJSALON_ORDEN = @orden", conexion);
                    cmd.Parameters.AddWithValue("@orden", orden);

                    //se abre la conexión
                    conexion.Open();
                    dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        //instrucción a ejecutar en la bd
                        cmd = new MySqlCommand("SELECT doc.*,ldoc.*,objsalon_orden FROM (objetos_salon as os LEFT join documentos AS doc ON os.OBJSALON_ORDEN = doc.DOC_MESA) LEFT JOIN ldocumentos AS LDOC ON doc.DOC_ID = LDOC.LDOC_DOC_ID WHERE os.OBJSALON_ORDEN = @orden AND doc.DOC_SERIE = 'MESA'", conexion);
                        //se le asigna el valor a @orden
                        cmd.Parameters.AddWithValue("@orden", orden);

                        //se obtiene cada usuario con sus valores, y se les añade a la lista de usuarios
                        int cont = 0;

                        Documentos objDocumento = new Documentos();
                        dr.Close();
                        dr = cmd.ExecuteReader();

                        while (dr.Read())
                        {
                            cont++;
                            string cadena = dr["doc_id"].ToString();
                            if (cadena == "")
                            {
                                objDocumento.doc_id = 0;
                            }
                            else
                            {
                                if (cont == 1)
                                {
                                    objDocumento.doc_id = Convert.ToInt64(dr["doc_id"].ToString());
                                    objDocumento.doc_serie = dr["doc_serie"].ToString();
                                    objDocumento.doc_num = Convert.ToInt32(dr["doc_num"].ToString());
                                    objDocumento.doc_tipo = dr["doc_tipo"].ToString();
                                    objDocumento.doc_mesa = Convert.ToInt32(dr["doc_mesa"].ToString());
                                    objDocumento.doc_cliente = Convert.ToInt32(dr["doc_cliente"].ToString());
                                    objDocumento.doc_regimen_id = Convert.ToInt32(dr["doc_regimen_id"].ToString());
                                    objDocumento.doc_fecha = Convert.ToDateTime(dr["doc_fecha"].ToString());
                                    objDocumento.doc_fpago = Convert.ToInt32(dr["doc_fpago"].ToString());
                                    objDocumento.doc_iva_id = Convert.ToInt32(dr["doc_iva_id"].ToString());
                                    objDocumento.doc_retencion = Convert.ToDouble(dr["doc_retencion"].ToString());
                                    objDocumento.doc_base1 = Convert.ToDouble(dr["doc_base1"].ToString());
                                    objDocumento.doc_base2 = Convert.ToDouble(dr["doc_base2"].ToString());
                                    objDocumento.doc_base3 = Convert.ToDouble(dr["doc_base3"].ToString());
                                    objDocumento.doc_base4 = Convert.ToDouble(dr["doc_base4"].ToString());
                                    objDocumento.doc_sum_bases = Convert.ToDouble(dr["doc_sum_bases"].ToString());
                                    objDocumento.doc_cdto_pp = Convert.ToDouble(dr["doc_cdto_pp"].ToString());
                                    objDocumento.doc_civa1 = Convert.ToDouble(dr["doc_civa1"].ToString());
                                    objDocumento.doc_civa2 = Convert.ToDouble(dr["doc_civa2"].ToString());
                                    objDocumento.doc_civa3 = Convert.ToDouble(dr["doc_civa3"].ToString());
                                    objDocumento.doc_civa4 = Convert.ToDouble(dr["doc_civa4"].ToString());
                                    objDocumento.doc_sum_civas = Convert.ToDouble(dr["doc_sum_civas"].ToString());
                                    objDocumento.doc_crec1 = Convert.ToDouble(dr["doc_crec1"].ToString());
                                    objDocumento.doc_crec2 = Convert.ToDouble(dr["doc_crec2"].ToString());
                                    objDocumento.doc_crec3 = Convert.ToDouble(dr["doc_crec3"].ToString());
                                    objDocumento.doc_crec4 = Convert.ToDouble(dr["doc_crec4"].ToString());
                                    objDocumento.doc_sum_crec = Convert.ToDouble(dr["doc_sum_crec"].ToString());
                                    objDocumento.doc_cret1 = Convert.ToDouble(dr["doc_cret1"].ToString());
                                    objDocumento.doc_cret2 = Convert.ToDouble(dr["doc_cret2"].ToString());
                                    objDocumento.doc_cret3 = Convert.ToDouble(dr["doc_cret3"].ToString());
                                    objDocumento.doc_cret4 = Convert.ToDouble(dr["doc_cret4"].ToString());
                                    objDocumento.doc_sum_crets = Convert.ToDouble(dr["doc_sum_crets"].ToString());
                                    objDocumento.doc_total1 = Convert.ToDouble(dr["doc_total1"].ToString());
                                    objDocumento.doc_total2 = Convert.ToDouble(dr["doc_total2"].ToString());
                                    objDocumento.doc_total3 = Convert.ToDouble(dr["doc_total3"].ToString());
                                    objDocumento.doc_total4 = Convert.ToDouble(dr["doc_total4"].ToString());
                                    objDocumento.doc_total = Convert.ToDouble(dr["doc_total"].ToString());
                                    objDocumento.doc_terminal = Convert.ToInt32(dr["doc_terminal"].ToString());
                                    objDocumento.doc_entregado = Convert.ToDouble(dr["doc_entregado"].ToString());
                                    objDocumento.doc_bloqueado = Convert.ToInt32(dr["doc_bloqueado"].ToString());
                                    objDocumento.doc_caja_id = Convert.ToInt32(dr["doc_caja_id"].ToString());
                                    objDocumento.doc_abonado = Convert.ToInt32(dr["doc_abonado"].ToString());
                                    objDocumento.listdoc = new List<Ldocumentos>();
                                }

                                string cadena2 = dr["ldoc_id"].ToString();
                                if (cadena2 != "")
                                {
                                    Ldocumentos ldoc = new Ldocumentos();

                                    ldoc.ldoc_id = Convert.ToInt64(dr["ldoc_id"].ToString());
                                    ldoc.ldoc_doc_id = Convert.ToInt64(dr["ldoc_doc_id"].ToString());
                                    ldoc.ldoc_tipo = dr["ldoc_tipo"].ToString();
                                    ldoc.ldoc_linea = Convert.ToInt32(dr["ldoc_linea"].ToString());
                                    ldoc.ldoc_art_id = Convert.ToInt32(dr["ldoc_art_id"].ToString());
                                    ldoc.ldoc_descripcion = dr["ldoc_descripcion"].ToString();
                                    ldoc.ldoc_cantidad = Convert.ToDouble(dr["ldoc_cantidad"].ToString());
                                    ldoc.ldoc_pvp = Convert.ToDouble(dr["ldoc_pvp"].ToString());
                                    ldoc.ldoc_dto = Convert.ToDouble(dr["ldoc_dto"].ToString());
                                    ldoc.ldoc_cdto = Convert.ToDouble(dr["ldoc_cdto"].ToString());
                                    ldoc.ldoc_importe = Convert.ToDouble(dr["ldoc_importe"].ToString());
                                    ldoc.ldoc_importe_pvp = Convert.ToDouble(dr["ldoc_importe_pvp"].ToString());
                                    ldoc.ldoc_iva = Convert.ToDouble(dr["ldoc_iva"].ToString());
                                    ldoc.ldoc_civa = Convert.ToDouble(dr["ldoc_civa"].ToString());
                                    ldoc.ldoc_tipo_iva = Convert.ToInt32(dr["ldoc_tipo_iva"].ToString());
                                    ldoc.ldoc_cant_prn = Convert.ToInt32(dr["ldoc_cant_prn"].ToString());
                                    ldoc.ldoc_ter_id = Convert.ToInt32(dr["ldoc_ter_id"].ToString());
                                    if (dr["ldoc_varia_notas"].ToString() == "")
                                    {
                                        ldoc.ldoc_varia_notas = 0;
                                    }
                                    else
                                    {
                                        ldoc.ldoc_varia_notas = Convert.ToInt32(dr["ldoc_varia_notas"].ToString());
                                    }
                                    ldoc.ldoc_err_prn = Convert.ToInt32(dr["ldoc_err_prn"].ToString());
                                    ldoc.ldoc_usuario = Convert.ToInt32(dr["ldoc_usuario"].ToString());

                                    objDocumento.listdoc.Add(ldoc);
                                }
                            }
                        }
                        if (cont == 0)
                        {
                            objDocumento.doc_mesa = orden;
                            oRespuesta.Mensaje = "La mesa está vacía";
                        }
                        listDocumentos.Add(objDocumento);
                        oRespuesta.Exito = 1;
                        oRespuesta.Data = listDocumentos;
                    }
                    else
                    {
                        oRespuesta.Mensaje = "La mesa no existe";
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
