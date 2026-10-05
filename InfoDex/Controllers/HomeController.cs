using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InfoDex.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Configure()
        {
            ViewBag.Message = "Configuration page.";

            return View();
        }

        public ActionResult Search()
        {
            ViewBag.Message = "Your search page.";
            return View();
        }
        public ActionResult RecalcDatabase(string database)
        {
            var response = this.recalcDatabase(database);

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AddDetail(string database, string keywords)
        {
            var response = this.addItem(database, keywords);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteItem(string database, string uid)
        {
            var response = this.deleteItem(database, uid);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDetail(string database, string uid, string summary, string body)
        {
            var response = this.updateItem(database, uid, summary, body);

            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public ActionResult KeywordSearch(string database, string keywords, string keywordweight)
        {
            var response = this.fetchSearchResults(database, keywords, Int32.Parse(keywordweight));

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListDatabases()
        {
            var response = this.listDatabasePaths();

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDatabase(string database)
        {
            var response = false;

            string databaseContainer = Request.PhysicalApplicationPath + @"\Databases";

            if (Directory.Exists(databaseContainer))
            {
                System.IO.File.Delete(System.IO.Path.Combine(databaseContainer, database + ".mdb"));
                response = true;
            }
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateDatabase(string database)
        {
            var response = this.createDatabase(database);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetail(string database, string id)
        {
            var response = this.fetchDetail(database, id);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDetail(string database, string uid)
        {
            var response = this.deleteItem(database, uid);

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        private int recalcDatabase(string database)
        {
            int result = -1;

            try
            {
                Common.Engine.SetDatabase(database);
                Common.Engine.Recalculate();
                result = 0;
            }
            catch (Exception exception)
            {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
            }
            finally
            {

            }

            return result;
        }


        private List<DatabaseInfo> listDatabasePaths()
        {
            List<DatabaseInfo> result = new List<DatabaseInfo>();        

            string databaseContainer = Request.PhysicalApplicationPath + @"\Databases";


            if (Directory.Exists(databaseContainer))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(databaseContainer);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*.mdb");


                foreach (FileInfo fileInfo in fileInfos)
                {
                    if (fileInfo.Name == "blank_database.mdb")
                        continue;

                    result.Add(new DatabaseInfo(fileInfo.FullName));
                }
            }
            return result;
        }


        private bool deleteDatabase(string database)
        {
            bool result = false;

            try
            {
                string databaseContainer = Request.PhysicalApplicationPath + @"\Databases";

                if (Directory.Exists(databaseContainer))
                {
                    string target = System.IO.Path.Combine(databaseContainer, database + ".mdb");

                    System.IO.File.Delete(target);
                    result = true;
                }
            }
            catch (Exception exception)
            {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
            }
            finally
            {

            }

            return result;

        }
        private bool createDatabase(string database)
        {
            bool result = false;

            try
            {
                string databaseContainer = Request.PhysicalApplicationPath + @"\Databases";

                if (Directory.Exists(databaseContainer))
                {
                    string source = System.IO.Path.Combine(databaseContainer, "blank_database.mdb");
                    string target = System.IO.Path.Combine(databaseContainer, database + ".mdb");

                    System.IO.File.Copy(source, target);
                    result = true;
                }
            }
            catch (Exception exception)
            {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
            }
            finally
            {

            }

            return result;
        }


        private int addItem(string database, string keywords)
        {
            int result = -1;

            try
            {
                Common.Engine.SetDatabase(database);
                result = Common.Engine.AddItem(keywords);
            }
            catch (Exception exception)
            {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
            }
            finally
            {

            }

            return result;
        }

        private int updateItem(string database, string uid, string summary, string body)
        {
            int result = -1;

            try
            {
                Common.Engine.SetDatabase(database);
                Common.Engine.UpdateItem(uid, summary, body);
                result = 0;
            }
            catch (Exception exception)
            {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
            }
            finally
            {

            }

            return result;
        }

        private int deleteItem(string database, string uid)
        {
            int result = -1;

            try
            {
                Common.Engine.SetDatabase(database);
                Common.Engine.DeleteItem(uid);
                result = 0;
            }
            catch (Exception exception)
            {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
            }
            finally
            {

            }

            return result;
        }


        private List<Hit> fetchSearchResults(string database, string keywords, int keywordweight)
        {
            List<Hit> result = new List<Hit>();

            try {
                Common.Engine.SetDatabase(database);
                DataView dataview = Common.Engine.Search(keywords, keywordweight);
                dataview.RowFilter = "HITS > 0";

                foreach (DataRowView rowView in dataview)
                {
                    DataRow row = rowView.Row;
                    Int32 hits = Int32.Parse(row["HITS"].ToString());
                    Int32 uid = Int32.Parse(row["UID"].ToString());
                    string kwds = row["KEYWORDS"].ToString();
                    string summary = row["SUMMARY"].ToString();
                    string lastviewed = row["LASTVIEWED"] == null ? string.Empty : row["LASTVIEWED"].ToString();
                    string createddate = row["CREATEDDATE"] == null ? string.Empty : row["CREATEDDATE"].ToString();
                    result.Add(new Hit(hits, uid, kwds, summary, lastviewed, createddate));
                }
            } catch (Exception exception) {
                //  TODO:  Devize a good response wrapper ... pretty much for anything that is being returned.
                Console.WriteLine(exception.Message);
            } finally {
                Console.WriteLine("finally");

            }

            return result;
        }
        private Detail fetchDetail(string database, string id)
        {
            Common.Engine.SetDatabase(database);

            DataView dataview = Common.Engine.FetchItem(id);
            Detail result = null;

            if (dataview.Count != 0)
            {
                try
                {
                    string uid = dataview[0]["UID"] == DBNull.Value ? "{null}" : dataview[0]["UID"].ToString();
                    string keywords = dataview[0]["KEYWORDS"] == DBNull.Value ? "{null}" : dataview[0]["KEYWORDS"].ToString();
                    string summary = dataview[0]["SUMMARY"] == DBNull.Value ? "{null}" : dataview[0]["SUMMARY"].ToString();
                    string lastviewed = dataview[0]["LASTVIEWED"] == DBNull.Value ? "{null}" : dataview[0]["LASTVIEWED"].ToString();
                    string body = dataview[0]["BODY"] == DBNull.Value ? "{null}" : dataview[0]["BODY"].ToString();

                    result = new Detail(uid, keywords, summary, lastviewed, body);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
                finally
                {
                }
            }

            return result;
        }
    }

}



public class DatabaseInfo
{
    public string JUSTNAME { get; set; }
    public string FULLPATH { get; set; }
    public DatabaseInfo() { }
    public DatabaseInfo(string fullPath) {
        this.FULLPATH = fullPath;
        this.JUSTNAME = Path.GetFileNameWithoutExtension(fullPath);
    }
}

public class Detail
{
    public string UID { get; set; }
    public string KEYWORDS { get; set; }
    public string SUMMARY { get; set; }
    public string LASTVIEWED { get; set; }
    public string BODY { get; set; }

    public Detail() { }
    public Detail(string uid, string keywords, string summary, string lastviewed, string body) {
        this.UID = uid;
        this.KEYWORDS = keywords;
        this.SUMMARY = summary;
        this.LASTVIEWED = lastviewed == null ? string.Empty : lastviewed;
        this.BODY = body;
    }
}

public class Hit
{
    public int HITS { get; set; }
    public int ID { get; set; }
    public string SUMMARY { get; set; }
    public string KEYWORDS { get; set; }
    public string LASTVIEWED { get; set; }
    public string CREATEDDATE { get; set; }
    public Hit() { }

     public Hit(int hits, int id,  string keywords, string summary, string lastviewed, string createddate) {
        this.HITS = hits;
        this.ID = id;
        this.KEYWORDS = keywords;
        this.LASTVIEWED = lastviewed;
        this.SUMMARY = summary;
        this.CREATEDDATE = createddate;
    }
}