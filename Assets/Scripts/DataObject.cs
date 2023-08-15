using System.Collections.Generic;

public class DataObject
{
    // Define your object's properties
    // ...

    public List<string> HeaderData { get; set; }
    public List<string[]> RowData { get; set; }
    //public void GetDataSetInfo()
    //{
    //    var data = CSV_data;
    //    // Clear the existing headers
    //    //csvHeadersText.text = "";

    //    int count = data.Count() - 1; //count of all rows minus header row to get total entries
    //    // Display the CSV headers
    //    if (count > 0)
    //    {
    //        string[] headers = data[0];

    //        //HeaderData = headers.ToList();
    //        string headersText = string.Join(", ", headers);
    //        HeaderData = headers.ToList();

    //        //csvHeadersText.text = headersText;
    //    }

    //    //csvInfoText.text = "";

    //    //csvInfoText.text = "Datasetcount " + count;


    //    // Group data by a specific column
    //    //int columnToGroupByIndex = 0; // Replace with the index of the column you want to group by

    //    for (int i = 0; i < HeaderData.Count; i++)
    //    {

    //        var groupedData = data.GroupBy(row => row[i]);

    //        // Display the grouped categories and their counts
    //        //csvInfoText.text += "Grouped Categories:\n";
    //        foreach (var group in groupedData)
    //        {
    //            string category = group.Key.ToString();
    //            int groupcount = group.Count();

    //            //csvInfoText.text += "Category: " + category + " | Count: " + groupcount + "\n";
    //        }
    //    }
    //}
}
