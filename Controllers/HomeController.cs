using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SamplePlantApp.Models;
using System.Data.SqlClient;
using System.Data;

namespace SamplePlantApp.Controllers
{
    public class HomeController : Controller
    {
        SqlConnection SQLCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlantModel"].ConnectionString);
        SqlCommand SQLCmd;
        SqlDataReader SQLDr;

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LoginAuthenticate(string UserNameInp, string PasswordInp)
        {
            var _JsonData = default(dynamic);
            string retmsg = string.Empty;
            bool isActive = false;
            PlantModelEntities DBEntity = new PlantModelEntities();
            int count = -1;
            bool ActiveStatus = false;
            try
            {
                using (SQLCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlantModel"].ConnectionString))
                {
                    if (SQLCon.State == System.Data.ConnectionState.Closed || SQLCon.State == System.Data.ConnectionState.Open)
                    {
                        SQLCon.Open();
                    }

                    string Query = "SELECT count(*) FROM GNM_User where UserName='" + UserNameInp + "' and Password='" + PasswordInp + "'";
                    SQLCmd = new SqlCommand(Query, SQLCon);
                    SQLCmd.CommandType = System.Data.CommandType.Text;
                    count = int.Parse(SQLCmd.ExecuteScalar().ToString());
                }

                if (count != 0)
                {
                    using (SQLCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlantModel"].ConnectionString))
                    {
                        if (SQLCon.State == System.Data.ConnectionState.Closed || SQLCon.State == System.Data.ConnectionState.Open)
                        {
                            SQLCon.Open();
                        }

                        string Query = "SELECT UserIsActive FROM GNM_User where UserName='" + UserNameInp + "' and Password='" + PasswordInp + "'";
                        SQLCmd = new SqlCommand(Query, SQLCon);
                        SQLCmd.CommandType = System.Data.CommandType.Text;
                        ActiveStatus = bool.Parse(SQLCmd.ExecuteScalar().ToString());
                        if (ActiveStatus != false)
                        {
                            isActive = true;
                            _JsonData = new
                            {
                                retmsg = "Success",
                                isActive
                            };
                        }
                        else
                        {
                            _JsonData = new
                            {
                                retmsg = "Failure",
                                isActive
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _JsonData = new
                {
                    retmsg = "Failure",
                    isActive
                };
            }
            return Json(_JsonData, JsonRequestBehavior.AllowGet);
        }

        #region ::: load dropdowns :::
        public JsonResult LoadDropdowns()
        {
            var _Jsonresult = default(dynamic);
            List<Dropdownlist> NameID = new List<Dropdownlist>();
            DataTable DT1 = new DataTable();
            Dropdownlist SingleRow = null;
            List<Dropdownlist> NameID2 = new List<Dropdownlist>();
            DataTable DT2 = new DataTable();
            try
            {
                using (SQLCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlantModel"].ConnectionString))
                {
                    if (SQLCon.State == System.Data.ConnectionState.Closed || SQLCon.State == System.Data.ConnectionState.Open)
                    {
                        SQLCon.Open();
                    }

                    string Query = "SELECT PlantID,PlantName FROM GNM_PlantDetails";
                    SQLCmd = new SqlCommand(Query, SQLCon);
                    SQLCmd.CommandType = System.Data.CommandType.Text;
                    SQLDr = SQLCmd.ExecuteReader();
                    if (SQLDr.Read())
                    {
                        if (SQLDr.HasRows)
                        {
                            DT1.Load(SQLDr);
                            foreach (DataRow item in DT1.Rows)
                            {
                                SingleRow = new Dropdownlist();
                                SingleRow.ID = int.Parse(item["PlantID"].ToString());
                                SingleRow.Name = item["PlantName"].ToString();
                                NameID.Add(SingleRow);
                            }
                        }
                    }
                }


                using (SQLCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlantModel"].ConnectionString))
                {
                    if (SQLCon.State == System.Data.ConnectionState.Closed || SQLCon.State == System.Data.ConnectionState.Open)
                    {
                        SQLCon.Open();
                    }

                    string Query1 = "SELECT StatusID,StatusName FROM GNM_Status";
                    SQLCmd = new SqlCommand(Query1, SQLCon);
                    SQLCmd.CommandType = System.Data.CommandType.Text;
                    SQLDr = SQLCmd.ExecuteReader();
                    if (SQLDr.Read())
                    {
                        if (SQLDr.HasRows)
                        {
                            DT2.Load(SQLDr);
                            foreach (DataRow item in DT2.Rows)
                            {
                                SingleRow = new Dropdownlist();
                                SingleRow.ID = int.Parse(item["StatusID"].ToString());
                                SingleRow.Name = item["StatusName"].ToString();
                                NameID2.Add(SingleRow);
                            }
                        }
                    }
                }

                _Jsonresult = new
                {
                    NameID,
                    NameID2
                };
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(_Jsonresult, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ::: GET DETAILS :::
        public JsonResult GetMaterialDetails(int PlantID, int StatusID)
        {
            var _JsonData = default(dynamic);
            List<MaterialsTable> _RetList = new List<MaterialsTable>();
            MaterialsTable MatTable = new MaterialsTable();
            DataTable DT = new DataTable();
            try
            {
                using (SQLCon = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["PlantModel"].ConnectionString))
                {
                    if (SQLCon.State == System.Data.ConnectionState.Closed || SQLCon.State == System.Data.ConnectionState.Open)
                    {
                        SQLCon.Open();
                    }

                    string StoredProcedure = "Up_FetchMachineDetails";
                    SQLCmd = new SqlCommand(StoredProcedure, SQLCon);
                    SQLCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SQLCmd.Parameters.AddWithValue("@PlantID", (PlantID == 0 ? 0 : PlantID));
                    SQLCmd.Parameters.AddWithValue("@StatusID", (StatusID == 0 ? 0 : StatusID));
                    SQLDr = SQLCmd.ExecuteReader();
                    if (SQLDr.Read())
                    {
                        if (SQLDr.HasRows)
                        {
                            DT.Load(SQLDr);
                            foreach (DataRow item in DT.Rows)
                            {
                                MatTable = new MaterialsTable();
                                MatTable.MaterialName = item["MaterialName"].ToString();
                                MatTable.CreatedDate = item["CreatedDate"].ToString();
                                MatTable.PlantName = item["PlantName"].ToString();
                                MatTable.StatusName = item["StatusName"].ToString();
                                MatTable.UserName = item["UserName"].ToString();
                                _RetList.Add(MatTable);
                            }
                        }
                    }
                }

                _JsonData = new
                {
                    _RetList
                };
            }
            catch (Exception ex)
            {
                throw;
            }
            return Json(_JsonData, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }

    public class MaterialsTable
    {
        public string MaterialName { get; set; }
        public string CreatedDate { get; set; }
        public string StatusName { get; set; }
        public string PlantName { get; set; }
        public string UserName { get; set; }
    }

    public class Dropdownlist
    {
        public int ID { get; set; }
        public string Name { get; set; }

    }
}