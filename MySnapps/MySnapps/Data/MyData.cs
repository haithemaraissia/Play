
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MySnapps.Data
{
    public class MyData
    {
        /// <summary>
        /// Saves items to MyData.xml file in bin folder.
        /// </summary>
        /// <param name="items"></param>
        public void Save(System.Windows.Data.CollectionView items)
        {
            XDocument xdoc = new XDocument();

            XElement xeRoot = new XElement("Data");
            XElement xeSubRoot = new XElement("Rows");

            foreach (var item in items)
            {
                ListViewData lvc = (ListViewData)item;

                XElement xRow = new XElement("Row");
                xRow.Add(new XElement("col1", lvc.Col1));

                xeSubRoot.Add(xRow);
            }
            xeRoot.Add(xeSubRoot);
            xdoc.Add(xeRoot);

            xdoc.Save("MyData.xml");
        }

        /// <summary>
        /// Gets data from MyData.xml as rows.  
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetRows()
        {
            List<ListViewData> rows = new List<ListViewData>();

            if (File.Exists("MyData.xml"))
            {
                // Create the query 
                var rowsFromFile = from c in XDocument.
                                   Load("MyData.xml")
                                        .Elements("Data")
                                        .Elements("Rows")
                                        .Elements("Row")
                                   select c;

                // Execute the query 
                foreach (var row in rowsFromFile)
                {
                    rows.Add(new ListViewData(row.Element("col1")?.Value));
                }
            }
            return rows;
        }
    }
}
