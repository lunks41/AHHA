using Newtonsoft.Json;

namespace AHHA.Core.Common
{
    public class DBGetConnection
    {
        public string GetconnectionDB(string regId)
        {
            //read the company registration data from json
            string regCompanyData = File.ReadAllText("regCompany.json");

            //Convert json to object list
            var regCompany = JsonConvert.DeserializeObject<IEnumerable<CompanyRegistration>>(regCompanyData);

            // find out the regId & get the connectionstring from there
            return regCompany.Where(b => b.RegId == regId).FirstOrDefault().ConnectionStringName;
        }

        public bool ValidateRegId(string regId)
        {
            //read the company registration data from json
            string regCompanyData = File.ReadAllText("regCompany.json");

            //Convert json to object list
            var regCompany = JsonConvert.DeserializeObject<IEnumerable<CompanyRegistration>>(regCompanyData);

            // find out the regId & get the connectionstring from there
            var CheckData = regCompany.Where(b => b.RegId == regId).FirstOrDefault().ConnectionStringName;

            return (CheckData != null ? true : false);
        }
    }
}