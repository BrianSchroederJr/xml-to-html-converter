using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace XMLtoHTML_Converter
{
    public partial class XMLtoHTML_Viewer : System.Web.UI.Page
    {
        // String representing file name and location
        string xmlFileToLoad = @"C:\Users\bschroeder\Desktop\books.xml";

        // Case Sesitive full name of starting node tag
        string startNodeTagName = "catalog";
        
        // *Optional - Add full names of any tags to exclude from display
        string[] excludeNodeNames = new string[] { "EXCLUDE_NODE_NAME" };

        protected void Page_Load(object sender, EventArgs e)
        {
            // The XmlDocument class is an in-memory representation of an XML document.
            // It implements the W3C XML Document Object Model (DOM) Level 1 Core and the Core DOM Level 2. [docs.microsoft.com]
            XmlDocument xmlDoc = new XmlDocument();

            // Try to load to XML file into XmlDocument object
            try
            {
                xmlDoc.Load(xmlFileToLoad);
            }
            catch
            {
                // Output error message to browser console in the event of a failure (for now)
                Response.Write("<script>console.log('Failed to load file.');</script>");
            }

            // Get the nodes contained in the startNodeTagName
            XmlNodeList nodeList = xmlDoc.GetElementsByTagName(startNodeTagName);

            // Call DisplayHTMLNode() and append result to output div for all nodes in nodeList
            foreach (XmlNode node in nodeList)
            {
                div1.InnerHtml += DisplayHTMLNodes(node, 0, false);
            }
        }



        // Display each node as an HTML formatted object with special handling of leaf nodes and top parent nodes
        private string DisplayHTMLNodes(XmlNode node, int nodeLevel, bool showAttributes)
        {
            string result = "";
            string atts = "";

            // Increment nodeLevel
            nodeLevel++;

            // Put any attributes together for display
            if (showAttributes && node.Attributes != null)
            {
                foreach (XmlAttribute a in node.Attributes)
                {
                    atts += " | " + a.LocalName + "=" + a.Value;
                }
            }

            // Format element for display and recursively call this function for all child nodes
            if (node.HasChildNodes)
            {
                // If this is a leaf (final node) with just 1 child node(the data)
                if (node.ChildNodes.Count == 1 && !node.ChildNodes.Item(0).HasChildNodes)
                {
                    // Style leaf nodes in the root differently 
                    if (node.ParentNode.Name == startNodeTagName)
                    {
                        result += string.Format("<br/><fieldset style='border-color: lightgray; border-bottom: none; border-right: none; border-left: none;'><legend>&nbsp;<span style='color: darkblue; font-size: large'><b>{0}</b></span><span style='color: red'>{1}</span>: <span style='font-size: large'><b>{2}</b>&nbsp;</span></legend></fieldset>", AddSpaces(node.LocalName), atts, node.ChildNodes.Item(0).Value);
                    }
                    else
                    {
                        result += string.Format("&nbsp;&nbsp;&nbsp;&nbsp;{0}<span style='color: red'>{1}</span>: <b>{2}</b><br/>", AddSpaces(node.LocalName), atts, node.ChildNodes.Item(0).Value);
                    }
                }
                // Else parent node (recursively call function)
                else
                {
                    // If root node (start node) then style differently
                    if (node.Name == startNodeTagName)
                    {
                        result += string.Format("<fieldset style='border-color: lightgray; border-bottom: none; border-right: none; border-left: none;'>" +
                        "<legend>&nbsp;<span style='color: darkblue; font-size: x-large'><b><u>{0}</u></b></span><span style='color: red'>{1}</span>&nbsp;</legend>"
                        , AddSpaces(node.LocalName), atts);
                    }
                    // If first child after root node then style differently
                    else if (node.ParentNode.Name == startNodeTagName)
                    {
                        result += string.Format("<br/><fieldset style='border-color: lightgray;'>" +
                        "<legend>&nbsp;<span style='color: darkblue; font-size: x-large'><b>{0}</b></span><span style='color: red'>{1}</span>&nbsp;</legend>"
                        , AddSpaces(node.LocalName), atts);
                    }
                    // All other child nodes
                    else
                    {
                        result += string.Format("<fieldset style='border-color: lightgray; border-bottom: none; border-right: none; border-left: none;'>" +
                        "<legend>&nbsp;<span style='color: darkblue; font-size: large'><b>{0}</b></span><span style='color: red'>{1}</span>&nbsp;</legend>"
                        , AddSpaces(node.LocalName), atts);
                    }
                    // Process remaining nodes
                    result += ProcessChildNodes(node.ChildNodes, nodeLevel);
                    // Close up fieldset tag
                    result += ("</fieldset>");
                }
            }
            // Return final node result
            return result;
        }



        // Recall the DisplayHTMLNodes method for every node
        private string ProcessChildNodes(XmlNodeList childNodes, int nodeLevel)
        {
            string result = "";

            foreach (XmlNode childNode in childNodes)
            {
                // Recursively call this method for all children that are not in the excludes array
                if (!excludeNodeNames.Any(childNode.Name.Contains))
                {
                    result += DisplayHTMLNodes(childNode, nodeLevel, true);
                }
            }
            return result;
        }



        // This method adds white space to camel case strings
        private string AddSpaces(string camelCaseString)
        {
            // The Regular Expression below does not match if the character to the left is the beginning of the line 
            // AND using the Regex.Replace() puts a space to the left of the match for Capital Letters followed by a Lower Case Letter (this puts a space after 1 in Adam1Jane2)
            // OR in front of a Capital Letter that has a Lower Case Letter next to the left of it.
            return Regex.Replace(camelCaseString, @"((?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z]))", " $1");
        }



    }// End class
}// End namespace
