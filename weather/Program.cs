using System;
using System.Collections.Generic;
using System.Data;

namespace DivideConquer
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable table = new DataTable();

            Attribute outlook = new Attribute(Constants.Outlook, new List<Option>() { new Option(Constants.Sunny), new Option(Constants.Overcast), new Option(Constants.Rainy) });
            Attribute temperature = new Attribute(Constants.Temperature, new List<Option>() { new Option(Constants.Hot), new Option(Constants.Mild), new Option(Constants.Cool) });
            Attribute humidity = new Attribute(Constants.Humidity, new List<Option>() { new Option(Constants.High), new Option(Constants.Normal) });
            Attribute windy = new Attribute(Constants.Windy, new List<Option>() { new Option(Constants.False), new Option(Constants.True) });
            Attribute play = new Attribute(Constants.Play, new List<Option>() { new Option(Constants.Yes), new Option(Constants.No) });

            List<Attribute> attributes = new List<Attribute>() { outlook, temperature, humidity, windy };

            table.Columns.Add(outlook.attributeName, typeof(string));
            table.Columns.Add(temperature.attributeName, typeof(string));
            table.Columns.Add(humidity.attributeName, typeof(string));
            table.Columns.Add(windy.attributeName, typeof(string));
            table.Columns.Add(play.attributeName, typeof(string));

            #region Weather Data

            table.Rows.Add(Constants.Sunny, Constants.Hot, Constants.High, Constants.False, Constants.No);
            table.Rows.Add(Constants.Sunny, Constants.Hot, Constants.High, Constants.True, Constants.No);
            table.Rows.Add(Constants.Overcast, Constants.Hot, Constants.High, Constants.False, Constants.Yes);
            table.Rows.Add(Constants.Rainy, Constants.Mild, Constants.High, Constants.False, Constants.Yes);
            table.Rows.Add(Constants.Rainy, Constants.Cool, Constants.Normal, Constants.False, Constants.Yes);
            table.Rows.Add(Constants.Rainy, Constants.Cool, Constants.Normal, Constants.True, Constants.No);
            table.Rows.Add(Constants.Overcast, Constants.Cool, Constants.Normal, Constants.True, Constants.Yes);
            table.Rows.Add(Constants.Sunny, Constants.Mild, Constants.High, Constants.False, Constants.No);
            table.Rows.Add(Constants.Sunny, Constants.Cool, Constants.Normal, Constants.False, Constants.Yes);
            table.Rows.Add(Constants.Rainy, Constants.Mild, Constants.Normal, Constants.False, Constants.Yes);
            table.Rows.Add(Constants.Sunny, Constants.Mild, Constants.Normal, Constants.True, Constants.Yes);
            table.Rows.Add(Constants.Overcast, Constants.Mild, Constants.High, Constants.True, Constants.Yes);
            table.Rows.Add(Constants.Overcast, Constants.Hot, Constants.Normal, Constants.False, Constants.Yes);
            table.Rows.Add(Constants.Rainy, Constants.Mild, Constants.High, Constants.True, Constants.No);

            #endregion

            Tree decisionTree = new Tree();

            decisionTree.BuildDecisionTree(table, attributes, null);

            decisionTree.PrintDecisionTree(decisionTree.treeNodes[0]);

            Console.ReadKey();
        }
    }

    internal class Attribute
    {
        public string attributeName;

        public List<Option> options;

        public double averageInformationValue;

        public Attribute(string attributeName, List<Option> options)
        {
            this.attributeName = attributeName;

            this.options = options;
        }
        /// <summary>
        /// /////////////////////
        /// </summary>
        public void setAverageInfVal()
        {
            int overallYes = 0;
            int overallNo = 0;

            foreach (Option option in this.options)
            {
                overallYes += option.yesAmount;
                overallNo += option.noAmount;
            }


            double tempYes = (double)overallYes / (overallYes + overallNo);
            double tempNo = (double)overallNo / (overallYes + overallNo);


            double tempInformationValue = -(tempYes * Math.Log(tempYes, 2) + tempNo * Math.Log(tempNo, 2));

            int overallYesNo = 0;
            double tempResult = 0;

            foreach (Option option in this.options)
            {
                overallYesNo = option.yesAmount + option.noAmount;

                tempResult += (double)overallYesNo / (overallYes + overallNo) * option.informationValue;
            }

            this.averageInformationValue = tempInformationValue - tempResult;

            Console.WriteLine("   " + this.averageInformationValue);
        }
    }

    internal static class Constants
    {
        public const string Outlook = "Outlook";

        public const string Sunny = "Sunny";
        public const string Overcast = "Overcast";
        public const string Rainy = "Rainy";


        public const string Temperature = "Temperature";

        public const string Hot = "Hot";
        public const string Mild = "Mild";
        public const string Cool = "Cool";


        public const string Humidity = "Humidity";

        public const string High = "High";
        public const string Normal = "Normal";


        public const string Windy = "Windy";

        public const string False = "False";
        public const string True = "True";


        public const string Play = "Play";

        public const string Yes = "Yes";
        public const string No = "No";
    }


    /// <summary>

    /// </summary>
    /// 
    ///////////////////////////
    internal class Tree
    {
        public List<TreeNode> treeNodes;

        public Tree()
        {
            this.treeNodes = new List<TreeNode>();
        }

        public void BuildDecisionTree(DataTable table, List<Attribute> attributes, Relation relation)
        {


            for (int i = 0; i < table.Columns.Count - 1; i++)
            {

                foreach (Attribute attribute in attributes)//
                {

                    if (table.Columns[i].Caption == attribute.attributeName)
                    {


                        for (int j = 0; j < table.Rows.Count; j++)
                        {
                            foreach (Option option in attribute.options)
                            {
                                if (table.Rows[j][i].ToString() == option.optionName)
                                {
                                    if (table.Rows[j].Field<string>("Play") == Constants.Yes)
                                    {
                                        option.yesAmount++;
                                    }
                                    else if (table.Rows[j].Field<string>("Play") == Constants.No)
                                    {
                                        option.noAmount++;
                                    }
                                }
                            }
                        }

                        /////////////

                        foreach (Option option in attribute.options)
                        {

                            option.setInformationValue();

                            if (Double.IsNaN(option.informationValue))
                            {
                                option.informationValue = 0;
                            }

                            Console.WriteLine(option.informationValue);
                        }


                        attribute.setAverageInfVal();

                        Console.WriteLine("      " + attribute.averageInformationValue);
                    }
                }
            }

            Attribute choosenAttribute = attributes[0];


            for (int i = 1; i < attributes.Count; i++)
            {
                if (attributes[i].averageInformationValue > choosenAttribute.averageInformationValue)
                {
                    choosenAttribute = attributes[i];
                }
            }

            Console.WriteLine("         " + choosenAttribute.averageInformationValue);
            Console.WriteLine(Environment.NewLine + Environment.NewLine);


            List<Relation> relations = new List<Relation>();

            DataTable dataTable;

            foreach (Option option in choosenAttribute.options)
            {
                if (option.informationValue != 0)
                {
                    dataTable = table.Copy();

                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {


                        if (dataTable.Rows[i].Field<string>(choosenAttribute.attributeName) != option.optionName)
                        {
                            dataTable.Rows.Remove(dataTable.Rows[i]);
                            i--;
                        }
                    }


                    relations.Add(new Relation(null, option.optionName, dataTable));
                }
                else
                {
                    string nodeData = "";

                    if (option.yesAmount == 0)
                    {
                        nodeData = Constants.No;
                    }
                    else if (option.noAmount == 0)
                    {
                        nodeData = Constants.Yes;
                    }

                    relations.Add(new Relation(new TreeNode(nodeData, null, null), option.optionName, null));
                }
            }

            List<Attribute> tempAttributes = new List<Attribute>();


            foreach (Attribute attribute in attributes)
            {
                if (attribute != choosenAttribute)
                {
                    tempAttributes.Add(attribute);
                }
            }

            TreeNode tempTreeNode = new TreeNode(choosenAttribute.attributeName, relations, tempAttributes);


            this.treeNodes.Add(tempTreeNode);

            if (relation != null)
            {
                relation.childNode = tempTreeNode;
            }

            foreach (Attribute attribute in attributes)
            {
                foreach (Option option in attribute.options)
                {
                    option.yesAmount = 0;
                    option.noAmount = 0;
                }

                attribute.averageInformationValue = 0;
            }


            foreach (Relation newRelation in relations)
            {
                if (newRelation.childNode == null)
                {

                    BuildDecisionTree(newRelation.dataTable, tempTreeNode.attributes, newRelation);
                }
                else
                {
                    continue;
                }
            }
        }

        public void PrintDecisionTree(TreeNode treeNode)
        {
            if (treeNode.nodeData != Constants.Yes && treeNode.nodeData != Constants.No)
            {
                Console.WriteLine(treeNode.nodeData);
            }
            else
            {
                Console.WriteLine("                      " + treeNode.nodeData);
            }

            if (treeNode.relations != null)
            {
                foreach (Relation relation in treeNode.relations)
                {
                    Console.WriteLine("           " + relation.childEdge);

                    PrintDecisionTree(relation.childNode);
                }
            }
        }
    }

    internal class Option
    {
        public string optionName;

        public int yesAmount;
        public int noAmount;

        public double informationValue;

        public Option(string optionName)
        {
            this.optionName = optionName;
        }


        public void setInformationValue()
        {
            double tempYes = (double)this.yesAmount / (this.yesAmount + this.noAmount);
            double tempNo = (double)this.noAmount / (this.yesAmount + this.noAmount);

            this.informationValue = -(tempYes * Math.Log(tempYes, 2) + tempNo * Math.Log(tempNo, 2));
        }
    }

    internal class Relation
    {
        public TreeNode childNode;
        public string childEdge;

        public DataTable dataTable;

        public Relation(TreeNode childNode, string childEdge, DataTable dataTable)
        {
            this.childNode = childNode;
            this.childEdge = childEdge;

            this.dataTable = dataTable;
        }
    }

    internal class TreeNode
    {
        public string nodeData;

        public List<Relation> relations;

        public List<Attribute> attributes;

        public TreeNode(string nodeData, List<Relation> relations, List<Attribute> attributes)
        {
            this.nodeData = nodeData;

            this.relations = relations;

            this.attributes = attributes;
        }
    }
}
