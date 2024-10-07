using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace CrystalReportService.DAL
{
    public static class ReportGenerator
    {
        public static string dbServer;
        public static string dbName;
        public static string dbUser;
        public static string dbPassword;        

        public static HttpResponseMessage GenerateReportwithQuery<T>(            
            string query,
            List<ReportParameter> reportParameters,
            string fileName) where T : ReportClass, new()
        {
            dbServer = ConfigurationManager.AppSettings["dbServer"];
            dbName = ConfigurationManager.AppSettings["dbName"];
            dbUser = ConfigurationManager.AppSettings["dbUser"];
            dbPassword = ConfigurationManager.AppSettings["dbPassword"];
            try
            {

                T reportDocument = new T();
                // Set database logon credentials
                reportDocument.SetDatabaseLogon(dbUser, dbPassword, dbServer, dbName);

                using (DatabaseHelper db = new DatabaseHelper())
                {
                    // Execute the query to get data for the report
                    DataTable dt = db.ExecuteQuery(query, CommandType.Text);

                    // Set data source to the report
                    reportDocument.SetDataSource(dt);
                }

                // Set report parameters if any
                foreach (var param in reportParameters)
                {
                    reportDocument.SetParameterValue(param.Name, param.Value);
                }

                // Export the report to PDF and return as HttpResponseMessage
                using (var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ReadFully(stream))
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    return result;
                }
            }
            catch (Exception ex)
            {
                HttpRequestMessage request = new HttpRequestMessage();
                HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                throw new System.Web.Http.HttpResponseException(response);
            }
        }

        public static HttpResponseMessage GenerateDynamicReportwithQuery(
            string query,
            string reportPath,
            List<ReportParameter> reportParameters,
            string fileName) 
        {
            dbServer = ConfigurationManager.AppSettings["dbServer"];
            dbName = ConfigurationManager.AppSettings["dbName"];
            dbUser = ConfigurationManager.AppSettings["dbUser"];
            dbPassword = ConfigurationManager.AppSettings["dbPassword"];
            try
            {   //var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/myfile.txt");
                 
                ReportDocument reportDocument = new ReportDocument();
                reportDocument.Load(System.Web.Hosting.HostingEnvironment.MapPath("~/Reports/" + reportPath+".rpt"));
                // Set database logon credentials
                reportDocument.SetDatabaseLogon(dbUser, dbPassword, dbServer, dbName);

                using (DatabaseHelper db = new DatabaseHelper())
                {
                    // Execute the query to get data for the report
                    DataTable dt = db.ExecuteQuery(query, CommandType.Text);

                    // Set data source to the report
                    reportDocument.SetDataSource(dt);
                }

                // Set report parameters if any
                foreach (var param in reportParameters)
                {
                    reportDocument.SetParameterValue(param.Name, param.Value);
                }

                // Export the report to PDF and return as HttpResponseMessage
                using (var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ReadFully(stream))
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    return result;
                }
            }
            catch (Exception ex)
            {
                HttpRequestMessage request = new HttpRequestMessage();
                HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                throw new System.Web.Http.HttpResponseException(response);
            }
        }

        public static HttpResponseMessage GenerateReportwithProcedure<T>(
            string query,
            List<ReportParameter> reportParameters,
            string fileName, SqlParameter[] sqlParam) where T : ReportClass, new()
        {
            dbServer = ConfigurationManager.AppSettings["dbServer"];
            dbName = ConfigurationManager.AppSettings["dbName"];
            dbUser = ConfigurationManager.AppSettings["dbUser"];
            dbPassword = ConfigurationManager.AppSettings["dbPassword"];
            try
            {

                T reportDocument = new T();
                // Set database logon credentials
                reportDocument.SetDatabaseLogon(dbUser, dbPassword, dbServer, dbName);

                using (DatabaseHelper db = new DatabaseHelper())
                {
                    // Execute the query to get data for the report
                    DataTable dt = db.ExecuteQuery(query, CommandType.StoredProcedure,sqlParam);

                    // Set data source to the report
                    reportDocument.SetDataSource(dt);
                }

                // Set report parameters if any
                foreach (var param in reportParameters)
                {
                    reportDocument.SetParameterValue(param.Name, param.Value);
                }

                // Export the report to PDF and return as HttpResponseMessage
                using (var stream = reportDocument.ExportToStream(ExportFormatType.PortableDocFormat))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    var result = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(ReadFully(stream))
                    };
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                    return result;
                }
            }
            catch (Exception ex)
            {
                HttpRequestMessage request = new HttpRequestMessage();
                HttpResponseMessage response = request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                throw new System.Web.Http.HttpResponseException(response);
            }
        }

        // Helper class for report parameters
        public class ReportParameter
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        // Helper method to read the stream fully
        private static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }


    }
}