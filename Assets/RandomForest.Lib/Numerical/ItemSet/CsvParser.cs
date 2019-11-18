using RandomForest.Lib.Numerical.ItemSet.Feature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RandomForest.Lib.Numerical.ItemSet
{
    static class CsvParser
    {
        public static ItemNumericalSet ParseItemNumericalSet(string path, int sheetNo = 1)
        {
            var fi = new FileInfo(path);
            if (!fi.Exists)
                throw new FileNotFoundException();

            String[] lines = File.ReadAllLines(path);
            var cells = lines.Select(line => line.Split(',')).ToArray();

            int cols = cells[0].Length;
            int rows = cells.Length;

            List<string> featureNames = new List<string>();
            for (int j = 0; j < cols; j++)
            {
                featureNames.Add(cells[0][j]);
            }

            ItemNumericalSet set = new ItemNumericalSet(featureNames);

            for (int i = 1; i < rows; i++)
            {
                FeatureNumericalValue[] arr = new FeatureNumericalValue[cols];
                for (int j = 0; j < cols; j++)
                {
                    var v = cells[i][j];
                    arr[j] = new FeatureNumericalValue { FeatureName = featureNames[j], FeatureValue = Convert.ToDouble(v) };
                }
                set.AddItem(arr);
            }

            return set;
        }
    }
}
