using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using InfoDex.Models;


namespace InfoDex.Common
{
    public static class Engine
    {
        private static int result = 0;
        public static int Result
        {
            get
            {
                return result;
            }
        }

        private static string connectionString = string.Empty;
        public static string ConnectionString
        {
            get
            {
                return connectionString;
            }

            set
            {
                connectionString = value;
            }
        }


        private static DataSet Query(string query, Dictionary<string, object> parameters, bool returnDataSet)
        {
            DataSet dataSet = new DataSet();


            if (query.Trim() == string.Empty)
                return dataSet;

            if (connectionString == string.Empty)
                return dataSet;

            AccessDatabase accessDatabase = new AccessDatabase(connectionString);


            if (!returnDataSet)
                accessDatabase.QueryExecuteNonQuery(query, null);
            else
                dataSet = accessDatabase.QueryDataSet(query, parameters);

            accessDatabase.Close();


            return dataSet;
        }

        public static DataView FetchItem(string id)
        {
            AccessDatabase accessDatabase = new AccessDatabase(connectionString);
            DataSet dataSet = accessDatabase.QueryDataSet("SELECT * FROM data WHERE [Active] = 1 AND [UID] = " + id, null);


            accessDatabase.Close();

            if (dataSet.Tables.Count <= 0)
                return null;


            accessDatabase.QueryExecuteNonQuery("UPDATE data SET [LASTVIEWED] = '" + String.Format("{0:d/M/yyyy HH:mm:ss}", DateTime.Now.ToString()) + "' WHERE [Active] = 1 AND [UID] = " + id, null);
            return new DataView(dataSet.Tables[0]);
        }


        public static void SetDatabase(string database)
        {
            //  NOTE:  Had to replace the Jet driver with the ACE driver.
            //  Download from here - https://www.microsoft.com/en-us/download/details.aspx?id=54920
            //Engine.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + database + ";User Id=admin;Password=;";
            Engine.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + database + ";User Id=admin;Password=;";
        }


        public static DataView Search(string keyWords, int length)
        {
            DataView dataView = null;


            keyWords = Engine.Clean(keyWords);
            string savedKeyWords = string.Empty;

            try {
                AccessDatabase accessDatabase = new AccessDatabase(connectionString);
                DataSet dataSet = accessDatabase.QueryDataSet("SELECT * FROM data WHERE [Active] = 1 ORDER BY [HITS] DESC", null);

                accessDatabase.Close();

                if (dataSet.Tables.Count <= 0)
                    return null;


                foreach (DataRow dataRow in dataSet.Tables[0].Rows)
                {
                    savedKeyWords = dataRow["KEYWORDS"] == null ? string.Empty : dataRow["KEYWORDS"].ToString();
                    dataRow["HITS"] = Engine.WeighWord(keyWords, savedKeyWords, length);
                }

                dataView = new DataView(dataSet.Tables[0]);
                dataView.Sort = "HITS desc";
            } catch (Exception exception) {

                Console.WriteLine(exception.Message);

            } finally {
            }



            return dataView;
        }


        private static string Clean(string data)
        {
            StringBuilder result = new StringBuilder(data);


            result.Replace("'", "");
            result.Replace(@"""", "");
            result.Replace(";", "");
            result.Replace(":", "");
            result.Replace("+", "");
            result.Replace("`", "");
            result.Replace("~", "");
            result.Replace("#", "");
            result.Replace("$", "");
            result.Replace("%", "");
            result.Replace("^", "");
            result.Replace("&", "");
            result.Replace("*", "");
            result.Replace("(", "");
            result.Replace(")", "");
            result.Replace("-", "");
            result.Replace("_", "");
            result.Replace("+", "");
            result.Replace("=", "");
            result.Replace("{", "");
            result.Replace("[", "");
            result.Replace("}", "");
            result.Replace("]", "");
            result.Replace("|", "");
            result.Replace(@"\", "");
            result.Replace("<", "");
            result.Replace(",", "");
            result.Replace(">", "");
            result.Replace(".", "");
            result.Replace("?", "");
            result.Replace("/", "");

            return result.ToString();
        }



        private static int HitWeight(string source, string target, int length)
        {
            int result = 0;


            source = Engine.RemoveExclusions(source.Trim()).ToLower();
            target = target.Trim().ToLower();

            string[] arrSource = source.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] arrTarget = target.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);


            foreach (string sourceItem in arrSource)
            {
                foreach (string targetItem in arrTarget)
                {
                    switch (length)
                    {
                        case 0:     //  One-to-one compare.  Each match is worth one point.
                            if (sourceItem == targetItem)
                                result += 1;
                            break;
                        default:
                            result += Engine.WeighWord(sourceItem, targetItem, length);
                            break;
                    }
                }
            }

            return result;
        }



        private static string RemoveExclusions(string data)
        {
            string result = data;
            string find = string.Empty;
            AccessDatabase accessDatabase = new AccessDatabase(connectionString);
            DataSet dataSet = accessDatabase.QueryDataSet("SELECT * FROM [exclusion]", null);


            accessDatabase.Close();


            if (dataSet.Tables.Count == 0)
                return string.Empty;

            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {

                if (Convert.ToInt32(dataRow["ACTIVE"].ToString()) != 1)
                    continue;

                find = " " + dataRow["WORD"].ToString() + " ";

                result = result.Replace(find, " ");

            }

            return result.ToUpper();
        }


        private static int WeighWord(string source, string target, int length)
        {
            int result = 0;

            string padding = string.Empty.PadLeft(length);
            string searchSource = padding + source.ToUpper() + padding;
            string searchTarget = padding + target.ToUpper() + padding;
            string bufferSource = string.Empty;
            string bufferTarget = string.Empty;


            for (int s = 1; s < searchSource.Length - length; s++)
            {
                bufferSource = searchSource.Substring(s, length);    //  DO NOT TRIM!

                for (int t = 1; t < searchTarget.Length - length; t++)
                {
                    bufferTarget = searchTarget.Substring(t, length);    //  DO NOT TRIM!

                    if (bufferTarget == bufferSource)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        public static int AddItem(string summary)
        {
            int result = 0;


            if (summary.Trim() == string.Empty)
                return result;

            string keyWords = Engine.RemoveExclusions(Engine.Clean(summary));

            AccessDatabase accessDatabase = new AccessDatabase(connectionString);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SUMMARY", summary);
            parameters.Add("@BODY", summary);
            parameters.Add("@KEYWORDS", keyWords);
            result = accessDatabase.QueryExecuteNonQuery("INSERT INTO data (BODY,SUMMARY,KEYWORDS,CREATEDDATE,ACTIVE,FLAG) VALUES (@BODY,@SUMMARY,@KEYWORDS,'" + DateTime.Now + "',1,0)", parameters);
            accessDatabase.Close();

            return result;
        }

        public static void DeleteItem(string id)
        {
            if (id == string.Empty)
                return;

            AccessDatabase accessDatabase = new AccessDatabase(connectionString);
            accessDatabase.QueryExecuteNonQuery("DELETE * FROM data WHERE [UID] = " + id, null);
            accessDatabase.Close();
        }

        public static void UpdateItem(string id, string summary, string body)
        {
            if (id == string.Empty || summary == string.Empty || body == string.Empty)
                return;

            summary = Engine.Clean(summary);

            string keyWords = Engine.RemoveExclusions(Engine.Clean(summary));

            AccessDatabase accessDatabase = new AccessDatabase(connectionString);
            accessDatabase.TransStart();
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SUMMARY", summary);
            parameters.Add("@KEYWORDS", keyWords);
            parameters.Add("@BODY", body);
            accessDatabase.TransExecute("UPDATE data SET [SUMMARY]=@SUMMARY, [KEYWORDS]=@KEYWORDS, [BODY]=@BODY WHERE [UID]=" + id, parameters);
            accessDatabase.TransCommit();
            accessDatabase.Close();
        }


        public static void Recalculate()
        {
            Engine.RecalcKeyWords();
        }

        private static void RecalcKeyWords()
        {
            AccessDatabase accessDatabase = new AccessDatabase(connectionString);
            DataSet dataSet = accessDatabase.QueryDataSet("SELECT * FROM [data] WHERE [Active]=1", null);

            accessDatabase.Close();


            if (dataSet.Tables.Count == 0)
                return;

            string id = string.Empty;
            string body = string.Empty;
            string summary = string.Empty;


            foreach (DataRow dataRow in dataSet.Tables[0].Rows)
            {
                id = dataRow["UID"].ToString();
                summary = dataRow["SUMMARY"].ToString();
                body = dataRow["BODY"].ToString();

                Engine.UpdateItem(id, summary, body);
            }
        }
    }
}
