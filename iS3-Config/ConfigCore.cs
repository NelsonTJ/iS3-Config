using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.Xml.Linq;
using IS3.Core;

namespace iS3.Config
{
    public class ConfigCore
    {
        public static ProjectList LoadProjectList(string fileName)
        {
            try
            {
                // Load ProjectList.xml using XamlReader
                StreamReader reader = new StreamReader(fileName);
                object obj = XamlReader.Load(reader.BaseStream);
                return obj as ProjectList;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return null;
            }
        }

        public static bool WriteProjectList(string fileName, ProjectList projList)
        {
            try
            {
                // write xml to memory stream at first
                Stream memStream = new MemoryStream();
                XmlAttributeOverrides overide = CreateOverrides();  // overide some attributes
                XmlSerializer s = new XmlSerializer(typeof(ProjectList), overide);
                s.Serialize(memStream, projList);  // write to memory

                // replace "Locations" with "ProjectList.Locations" so XamlReader would be happy.
                memStream.Position = 0;
                StreamReader reader = new StreamReader(memStream);
                string xml = reader.ReadToEnd();
                string xaml = xml.Replace("Locations", "ProjectList.Locations");

                // overide ProjectList.xml
                FileStream fs = new FileStream(fileName, FileMode.Create);
                StreamWriter writer = new StreamWriter(fs);
                writer.Write(xaml);
                writer.Close();

                return true;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return false;
            }
        }

        // Write ProjectList member variables (XMin, XMax, YMin, Ymax, UseGeographicMap)
        // and ProjectLocation member variables (ID, DefinitionFile, X, Y, Description, Default)
        // as attributes into XML, so XamlReader can work happily.
        static XmlAttributeOverrides CreateOverrides()
        {
            XmlAttributeOverrides attrOverrides = new XmlAttributeOverrides();

            // add root element namespace
            attrOverrides.Add(typeof(ProjectList), new XmlAttributes()
            {
                XmlRoot = new XmlRootAttribute()
                {
                    ElementName = "ProjectList",
                    Namespace = "clr-namespace:IS3.Core;assembly=IS3.Core"
                }
            });

            // write ProjectList member variables as attributes
            attrOverrides.Add(typeof(ProjectList), "XMax", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("XMax") });
            attrOverrides.Add(typeof(ProjectList), "XMin", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("XMin") });
            attrOverrides.Add(typeof(ProjectList), "YMax", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("YMax") });
            attrOverrides.Add(typeof(ProjectList), "YMin", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("YMin") });
            attrOverrides.Add(typeof(ProjectList), "UseGeographicMap", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("UseGeographicMap") });

            // write ProjectLocation member variables as attributes
            attrOverrides.Add(typeof(ProjectLocation), "ID", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("ID") });
            attrOverrides.Add(typeof(ProjectLocation), "DefinitionFile", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("DefinitionFile") });
            attrOverrides.Add(typeof(ProjectLocation), "X", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("X") });
            attrOverrides.Add(typeof(ProjectLocation), "Y", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("Y") });
            attrOverrides.Add(typeof(ProjectLocation), "Description", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("Description") });
            attrOverrides.Add(typeof(ProjectLocation), "Default", new XmlAttributes()
            { XmlAttribute = new XmlAttributeAttribute("Default") });

            return attrOverrides;
        }

        public static ProjectDefinition LoadProjectDefinition(string projPath, string projID)
        {
            string fileName = projPath + "\\" + projID + ".xml";
            ProjectDefinition projDef = null;
            if (!File.Exists(fileName))
                return null;

            try
            {
                StreamReader reader = new StreamReader(fileName);
                XElement root = XElement.Load(reader);

                if (root == null || root.Name != "Project")
                    return null;

                XNamespace is3 = "clr-namespace:IS3.Core;assembly=IS3.Core";
                XElement node = root.Element(is3 + "ProjectDefinition");
                if (node != null)
                {
                    object obj = XamlReader.Parse(node.ToString());
                    projDef = (ProjectDefinition)obj;

                    if (projDef.LocalFilePath == null || projDef.LocalFilePath.Length == 0)
                        projDef.LocalFilePath = projPath + "\\" + projID;
                    if (projDef.LocalTilePath == null || projDef.LocalTilePath.Length == 0)
                        projDef.LocalTilePath = projPath + "\\" + "TPKs";

                    string path = System.IO.Path.GetDirectoryName(projDef.LocalDatabaseName);
                    if (path.Length == 0)
                        projDef.LocalDatabaseName = projPath + "\\" + projDef.LocalDatabaseName;
                }

                reader.Close();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButton.OK);
                return null;
            }

            return projDef;
        }

        public static ProjectDefinition CreateProjectDefinition(string projPath, string projID)
        {
            ProjectDefinition projDef = new ProjectDefinition();

            projDef.ID = projID;
            projDef.ProjectTitle = projID;
            projDef.LocalFilePath = projPath + "\\" + projID;
            projDef.LocalTilePath = projPath + "\\" + "TPKs";
            projDef.LocalDatabaseName = projPath + "\\" + projID + "\\" + projID + ".MDB";

            return projDef;
        }
    }
}
