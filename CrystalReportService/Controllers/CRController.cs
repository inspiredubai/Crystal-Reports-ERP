using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
//using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Threading.Tasks;
using System.Web.Http;
using System.Collections;
using System.Net.Http.Headers;
using CrystalReportService.DAL;
using System.Data;
using CrystalDecisions.Web;
using CrystalReportService.Reports;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CrystalReportService.Filters;

namespace CrystalReportService.Controllers
{
    [AllowCrossSiteJson]
    [Route("api/[controller]")]    
    public class CRController :  ApiController
    {
        [HttpPost]
        [Route("api/cr/DynamicReportCall")]
        public HttpResponseMessage DynamicReportCall(DynamicInput model)
        {
            var reportParams = new List<ReportGenerator.ReportParameter>();
            JsonSerializer serializer = new JsonSerializer();
            if (model.reportParam!=null)
            {
                foreach (var mod in model.reportParam)
                {
                    ReportGenerator.ReportParameter reportParameter = new ReportGenerator.ReportParameter();
                    reportParameter.Name = mod.name;
                    reportParameter.Value = mod.value;
                    reportParams.Add(reportParameter);
                }
            }
           
            return ReportGenerator.GenerateDynamicReportwithQuery(
                model.sqlQuery, model.reportName,reportParams, model.reportDisplayName
            );
        }

        [HttpPost]
        [Route("api/cr/sample")]
        public HttpResponseMessage GetSampleReport(inputmodel model)
        {
            string query = "SELECT * FROM AccountsTransactions WHERE AccountsTransactions_AccNo='01001014'";


            var reportParams = new List<ReportGenerator.ReportParameter>
            {
                new ReportGenerator.ReportParameter { Name = "test", Value = model.acNo },
                new ReportGenerator.ReportParameter { Name = "name", Value = model.name },
                new ReportGenerator.ReportParameter { Name = "address", Value = model.address },
                new ReportGenerator.ReportParameter { Name = "cell", Value = model.cell }
            };
            return ReportGenerator.GenerateReportwithQuery<TestReport1>(
                query, reportParams, "TestReport1"
            );
        }
    }
    public class DynamicInput
    {
        public string reportName { get; set; }
        public string sqlQuery { get; set; }
        public string reportDisplayName { get; set; }
        public List<objResult> reportParam { get; set; }

    }

    public class objResult
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class inputmodel
    {
        public string acNo { get; set; }
        public string name { get; set; }
        public string address { get; set;}

        public string cell { get; set; }
    }
}